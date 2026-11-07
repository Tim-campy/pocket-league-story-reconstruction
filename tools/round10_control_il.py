"""Conservative IL-only control IR. This is never the method equality oracle.

Every instruction/control transfer is retained; partial structure is explicit.
No source AST, decompiler output, MDB, or expected shape is accepted as input.
"""
from collections import defaultdict,deque
import hashlib,json,re
from pathlib import Path
from round9_control_cfg import build_cfg,targets,unconditional,terminal
from round10_control_owners import resolve_transfer

WIP=Path(__file__).resolve().parent
CONTROL_STAGE=1
OPCODES=json.loads((WIP/'round9_control_opcodes.json').read_text(encoding='utf-8-sig'))
PREDICATES={'brfalse':'zero-or-null','brtrue':'nonzero-or-nonnull','beq':'equal','bne.un':'not-equal-or-unordered',
    'blt':'less-signed-ordered','blt.un':'less-unsigned-or-unordered','ble':'less-equal-signed-ordered',
    'ble.un':'less-equal-unsigned-or-unordered','bgt':'greater-signed-ordered','bgt.un':'greater-unsigned-or-unordered',
    'bge':'greater-equal-signed-ordered','bge.un':'greater-equal-unsigned-or-unordered'}
PRIMITIVES={'System.Boolean':'int32','System.Byte':'int32','System.SByte':'int32','System.Char':'int32',
    'System.Int16':'int32','System.UInt16':'int32','System.Int32':'int32','System.UInt32':'int32',
    'System.Int64':'int64','System.UInt64':'int64','System.IntPtr':'native-int','System.UIntPtr':'native-int',
    'System.Single':'float','System.Double':'float','System.Void':'void'}


def _type(raw):
    if not raw:return 'UNKNOWN'
    text=re.sub(r'^(class|valuetype)\s+','',raw)
    return text.split(' @ ',1)[0].strip()


def _parameters(text):
    if not text:return []
    result=[];start=0;depth=0
    for n,char in enumerate(text):
        if char in '<[(':depth+=1
        elif char in '>])':depth-=1
        elif char==',' and depth==0 and re.match(r'\s*(?:class |valuetype |!!?\d)',text[n+1:]):
            result.append(text[start:n]);start=n+1
    result.append(text[start:]);return [_type(part.strip()) for part in result]


def _signature(text):
    if not isinstance(text,str) or '->' not in text:return None
    left,right=text.split('->',1);begin=left.find('(',left.find('::')+2)
    if begin<0 or not left.endswith(')'):return None
    return {'parameters':_parameters(left[begin+1:-1]),'return':_type(right.split(';',1)[0]),
        'this':';this=True;' in text,'declaring_type':_type(left.split('::',1)[0])}


def _slot(instruction,family):
    op=instruction['opcode'];operand=instruction.get('operand')
    if op.startswith(family+'.') and op[len(family)+1:].isdigit():return int(op[len(family)+1:])
    if op in (family,family+'.s') and isinstance(operand,dict):return operand.get('local',operand.get('parameter'))
    return None


def _operand_width(info,instruction):
    form=info['operand_type']
    if form=='InlineNone':return 0
    if form in ('ShortInlineBrTarget','ShortInlineI','ShortInlineVar'):return 1
    if form=='InlineVar':return 2
    if form in ('InlineBrTarget','InlineField','InlineI','InlineMethod','InlineSig','InlineString','InlineTok','InlineType','ShortInlineR'):return 4
    if form in ('InlineI8','InlineR'):return 8
    if form=='InlineSwitch':return 4+4*len(targets(instruction))
    return None


def _stack_count(behaviour):
    if behaviour in ('Pop0','Push0'):return 0
    if behaviour in ('Varpop','Varpush'):return None
    return len(behaviour.split('_'))


def _effect(instruction,method_signature):
    info=OPCODES.get(instruction['opcode'])
    if info is None:return None
    pop,push=_stack_count(info['pop']),_stack_count(info['push'])
    op=instruction['opcode'];signature=None
    if op in ('call','callvirt','newobj','calli'):
        operand=instruction.get('operand');text=operand.get('method',operand.get('signature')) if isinstance(operand,dict) else None
        signature=_signature(text)
        if signature is None:return None
        pop=len(signature['parameters'])+(int(signature['this']) if op!='newobj' else 0)+(1 if op=='calli' else 0)
        push=1 if op=='newobj' or signature['return']!='System.Void' else 0
    elif op=='ret':
        if method_signature is None:return None
        pop=int(method_signature['return']!='System.Void');push=0
    if pop is None or push is None:return None
    return pop,push,signature


def _pushed_type(instruction,signature,method_signature,locals_,popped):
    op=instruction['opcode'];operand=instruction.get('operand')
    if op in ('call','callvirt','calli'):return signature['return'] if signature else 'UNKNOWN'
    if op=='newobj':return signature['declaring_type'] if signature else 'UNKNOWN'
    if op=='ldnull':return 'null-reference'
    if op=='ldstr':return 'System.String'
    if op.startswith('ldc.i4'):return 'System.Int32'
    if op=='ldc.i8':return 'System.Int64'
    if op=='ldc.r4':return 'System.Single'
    if op=='ldc.r8':return 'System.Double'
    if op.startswith('ldloca') or op.startswith('ldarga') or op in ('ldflda','ldsflda','ldelema','unbox','refanyval'):return 'managed-pointer'
    local=_slot(instruction,'ldloc')
    if isinstance(local,int) and 0<=local<len(locals_):return _type(locals_[local])
    arg=_slot(instruction,'ldarg')
    # Cecil ParameterDefinition.Index excludes the implicit this argument; macros do not.
    if isinstance(operand,dict) and 'parameter' in operand and method_signature and method_signature['this'] and isinstance(arg,int):arg+=1
    if isinstance(arg,int) and method_signature:
        if method_signature['this']:
            if arg==0:return method_signature['declaring_type']
            arg-=1
        if 0<=arg<len(method_signature['parameters']):return method_signature['parameters'][arg]
    if op in ('ldfld','ldsfld') and isinstance(operand,dict):return _type(operand.get('field','').rsplit(':',1)[-1])
    if op in ('castclass','isinst','unbox.any','ldobj') and isinstance(operand,dict):return _type(operand.get('type'))
    if op=='newarr' and isinstance(operand,dict):return _type(operand.get('type'))+'[]'
    if op=='ldlen':return 'System.UIntPtr'
    if op.startswith(('ceq','clt','cgt')):return 'System.Boolean'
    if op.startswith('conv.'):
        token=op.split('.')[1:];token=[t for t in token if t not in ('ovf','un')][0]
        return {'i':'System.IntPtr','u':'System.UIntPtr','i1':'System.SByte','u1':'System.Byte','i2':'System.Int16','u2':'System.UInt16',
            'i4':'System.Int32','u4':'System.UInt32','i8':'System.Int64','u8':'System.UInt64','r4':'System.Single','r8':'System.Double','r':'System.Double'}.get(token,'UNKNOWN')
    if op.startswith(('ldelem.','ldind.')):
        suffix=op.split('.')[-1]
        return {'i1':'System.SByte','u1':'System.Byte','i2':'System.Int16','u2':'System.UInt16','i4':'System.Int32','u4':'System.UInt32',
            'i8':'System.Int64','i':'System.IntPtr','r4':'System.Single','r8':'System.Double','ref':'object-reference'}.get(suffix,'UNKNOWN')
    if op in ('ldftn','ldvirtftn','localloc'):return 'native-pointer'
    if op in ('sizeof',):return 'System.Int32'
    if op=='box':return 'object-reference'
    if op in ('neg','not') or op.startswith(('add','sub','mul','div','rem','and','or','xor','shl','shr')):
        return popped[0]['type'] if popped else 'UNKNOWN'
    return 'UNKNOWN'


def _value(type_,offset,inputs=()):return {'type':type_,'producers':sorted({offset}|{n for item in inputs for n in item['producers']})}


def _stack_analysis(method,instructions):
    signature=_signature(method.get('signature'));by={i['offset']:n for n,i in enumerate(instructions)}
    before={instructions[0]['offset']:[]};work=deque([instructions[0]['offset']]);reasons=[];after={}
    for clause in method.get('exception_regions',[]):
        start=clause.get('handler_start');kind=clause.get('kind')
        if start in by:
            state=[_value(_type(clause.get('catch_type')) if clause.get('catch_type') else 'System.Object',start)] if kind in ('Catch','Filter') else []
            before[start]=state;work.append(start)
        filt=clause.get('filter_start',-1)
        if filt in by:before[filt]=[_value('System.Object',filt)];work.append(filt)
    maximum_steps=max(100,len(instructions)*30);steps=0
    while work and steps<maximum_steps:
        offset=work.popleft();steps+=1;index=by[offset];ins=instructions[index];state=before[offset]
        effect=_effect(ins,signature)
        if effect is None:
            reasons.append({'code':'stack-effect-unmodeled','node_id':'I'+str(offset),'detail':ins['opcode']});continue
        pop,push,callee=effect
        if pop>len(state):
            reasons.append({'code':'stack-underflow','node_id':'I'+str(offset),'detail':str((pop,len(state)))});continue
        popped=state[len(state)-pop:] if pop else [];remaining=state[:len(state)-pop] if pop else state[:]
        if ins['opcode']=='dup':output=remaining+[dict(popped[0]),dict(popped[0])]
        else:
            output=remaining+[_value(_pushed_type(ins,callee,signature,method.get('locals',[]),popped),offset,popped) for _ in range(push)]
        if ins['opcode'] in ('leave','leave.s'):output=[]
        after[offset]=output
        destinations=targets(ins)[:]
        info=OPCODES.get(ins['opcode'],{})
        if info.get('flow') not in ('Branch','Return','Throw') and index+1<len(instructions):destinations.append(instructions[index+1]['offset'])
        for destination in destinations:
            if destination not in by:continue
            if destination not in before:before[destination]=output;work.append(destination);continue
            old=before[destination]
            if len(old)!=len(output):
                reasons.append({'code':'stack-join-height-mismatch','node_id':'I'+str(destination),'detail':str((len(old),len(output)))});continue
            merged=[]
            for a,b in zip(old,output):
                type_=a['type'] if a['type']==b['type'] else ('System.Int32' if PRIMITIVES.get(a['type'])==PRIMITIVES.get(b['type'])=='int32' else 'UNKNOWN')
                merged.append({'type':type_,'producers':sorted(set(a['producers'])|set(b['producers']))})
            if merged!=old:before[destination]=merged;work.append(destination)
    if work:reasons.append({'code':'stack-fixedpoint-budget','node_id':None,'detail':str(maximum_steps)})
    unique={json.dumps(r,sort_keys=True):r for r in reasons}
    return {'before':before,'after':after,'reasons':list(unique.values()),'steps':steps}


def _predicate(ins,stack):
    base=ins['opcode'][:-2] if ins['opcode'].endswith('.s') else ins['opcode']
    count=1 if base in ('brfalse','brtrue') else 2
    values=stack.get('before',{}).get(ins['offset'])
    operands=values[-count:] if values is not None and len(values)>=count else None
    return {'opcode':ins['opcode'],'relation':PREDICATES.get(base,'UNKNOWN'),'operand_count':count,
        'types':[v['type'] for v in operands] if operands is not None else [],
        'producer_offsets':sorted({o for v in operands for o in v['producers']}) if operands is not None else [],
        'stack_inputs_known':operands is not None,'unsigned_or_unordered':'.un' in base,
        'branch_on_true':True if base=='brtrue' else False if base=='brfalse' else None,
        'logical_source_operator':'UNKNOWN: retain exact opcode; do not invert floating-point comparisons'}


def canonical_plan(ir):
    """Shared event ABI; strips coordinates only, never any transfer/opcode."""
    events=ir['emission_plan'] if isinstance(ir,dict) else ir
    targets_=set()
    for event in events:
        if event.get('target_label') is not None:targets_.add(event['target_label'])
        targets_.update(event.get('target_labels',[]))
    positions={};position=0
    for event in events:
        if event['kind']=='label':positions[event.get('label',event.get('id'))]=position
        else:position+=1
    missing=targets_-set(positions)
    if missing:raise ValueError('unbound_control_labels:'+repr(sorted(missing)))
    canonical={p:'L'+str(n) for n,p in enumerate(sorted({positions[t] for t in targets_}))}
    names={label:canonical[p] for label,p in positions.items() if label in targets_}
    result=[];position=0
    for event in events:
        if event['kind']=='label':continue
        if position in canonical:result.append({'kind':'label','label':canonical[position]})
        kind=event['kind']
        if kind=='leaf':
            if not result or result[-1]['kind']!='leaf':result.append({'kind':'leaf'})
        else:
            row={'kind':kind}
            if 'opcode' in event:row['opcode']=event['opcode'].lower()
            if 'target_label' in event:row['target_label']=names[event['target_label']]
            if 'target_labels' in event:row['target_labels']=[names[t] for t in event['target_labels']]
            if kind.startswith('eh_'):
                for key in ('handler_kind','catch_type','region_role'):
                    if key in event:row[key]=event[key]
            result.append(row)
        position+=1
    if position in canonical:result.append({'kind':'label','label':canonical[position]})
    return result


class Extractor:
    def __init__(self,method):
        self.method=method;self.instructions=method.get('instructions',[]);self.size=method.get('header',{}).get('code_size',0)
        self.by={i['offset']:n for n,i in enumerate(self.instructions)};self.reasons=[];self.leaves=[];self.plan=[];self.roles=[];self.equivalences=[]
        self.widths={};self.stack={};self.cfg={};self.serial=0
    def reason(self,code,detail,node=None):
        item={'code':code,'node_id':node['id'] if node else None,'detail':str(detail)}
        self.reasons.append(item)
        if node:node['complete']=False;node['reasons'].append(item)
    def node(self,kind,start,end,children=(),data=None):
        self.serial+=1
        return {'id':'N'+str(self.serial),'kind':kind,'complete':True,'reasons':[],
            'span':{'start':start,'end':end},'children':list(children),'data':data or {}}
    def validate(self):
        if not self.instructions:self.reason('no-CIL-body','No instructions');return False
        if len(self.by)!=len(self.instructions):self.reason('duplicate-instruction-offset','DTO offsets are not unique')
        expected=0
        for ins in self.instructions:
            offset=ins['offset'];info=OPCODES.get(ins['opcode'])
            if offset!=expected:self.reason('instruction-gap-or-overlap',(expected,offset))
            if not info:self.reason('unknown-opcode',ins['opcode']);continue
            operand_width=_operand_width(info,ins)
            if operand_width is None:self.reason('unknown-operand-width',ins['opcode']);continue
            width=info['size']+operand_width;self.widths[offset]=width;expected=offset+width
            for destination in targets(ins):
                if destination not in self.by:self.reason('invalid-branch-target',(offset,destination))
        if expected!=self.size:self.reason('body-size-coverage-mismatch',(expected,self.size))
        for clause in self.method.get('exception_regions',[]):
            for key in ('try_start','try_end','handler_start','handler_end'):
                if clause.get(key) not in self.by and clause.get(key)!=self.size:self.reason('invalid-EH-boundary',(key,clause.get(key)))
        self.validate_sections()
        return not self.reasons
    def validate_sections(self):
        sections=self.method.get('extra_sections',[]);clauses=self.method.get('exception_regions',[])
        evidence={'complete':False,'section_count':len(sections),'clause_count':len(clauses),'parsed_clause_count':0,'all_raw_bytes_accounted':False}
        self.section_validation=evidence
        if not sections:
            if clauses:self.reason('EH-clauses-without-section-layout',len(clauses))
            else:evidence.update(complete=True,all_raw_bytes_accounted=True)
            return
        try:
            raw=bytes.fromhex(self.method['raw_body_hex'].replace('-',''))
            position=self.method['header']['size']+self.size;clause_index=0
            identities={row['body_offset']:row['identity'] for row in self.method.get('metadata_operands',[])}
            for section_index,section in enumerate(sections):
                padding=bytes.fromhex(section.get('padding_before','').replace('-',''))
                if raw[position:position+len(padding)]!=padding:raise ValueError('padding mismatch')
                position+=len(padding);kind=section['kind'];fat=bool(kind&0x40);size=section['size']
                if kind&0x3f!=1:raise ValueError('non-EH section kind '+str(kind))
                header=raw[position:position+4]
                if len(header)!=4 or header[0]!=kind:raise ValueError('section kind mismatch')
                actual_size=int.from_bytes(header[1:4],'little') if fat else header[1]
                if actual_size!=size:raise ValueError('section size mismatch')
                if bool(kind&0x80)!=(section_index<len(sections)-1):raise ValueError('section chaining flag mismatch')
                stride=24 if fat else 12
                if size<4 or (size-4)%stride:raise ValueError('invalid EH clause size')
                for at in range(position+4,position+size,stride):
                    row=raw[at:at+stride]
                    if len(row)!=stride or clause_index>=len(clauses):raise ValueError('EH clause coverage mismatch')
                    if fat:
                        values=[int.from_bytes(row[n:n+4],'little') for n in range(0,24,4)]
                    else:
                        values=[int.from_bytes(row[:2],'little'),int.from_bytes(row[2:4],'little'),row[4],int.from_bytes(row[5:7],'little'),row[7],int.from_bytes(row[8:12],'little')]
                    flags,start,length,handler,handler_length,token_or_filter=values;clause=clauses[clause_index]
                    if flags not in (0,1,2,4):raise ValueError('unknown EH flags '+str(flags))
                    if clause['kind']!={0:'Catch',1:'Filter',2:'Finally',4:'Fault'}[flags]:raise ValueError('EH kind mismatch')
                    if (start,start+length,handler,handler+handler_length)!=(clause['try_start'],clause['try_end'],clause['handler_start'],clause['handler_end']):raise ValueError('EH interval mismatch')
                    if flags==1 and token_or_filter!=clause['filter_start']:raise ValueError('EH filter interval mismatch')
                    if flags==0:
                        identity=identities.get(at+(20 if fat else 8),{})
                        if identity.get('catch_type')!=clause.get('catch_type'):raise ValueError('EH catch identity mismatch')
                    clause_index+=1
                position+=size
            if clause_index!=len(clauses) or position!=len(raw):raise ValueError('unaccounted method/EH bytes')
            evidence.update(complete=True,parsed_clause_count=clause_index,all_raw_bytes_accounted=True,
                raw_method_bytes=len(raw),raw_EH_end=position,layout_retained=True)
        except (KeyError,ValueError,IndexError,TypeError) as error:
            self.reason('extra-section-validation-failed',str(error));evidence['reason']=str(error)
    def branch_role(self,ins):
        destinations=targets(ins);source=self.cfg['owner'].get(ins['offset']);op=ins['opcode']
        if op in ('leave','leave.s'):return 'eh-leave'
        if op=='switch':return 'switch-dispatch'
        if len(destinations)!=1:return 'UNKNOWN'
        destination=destinations[0]
        if destination==ins['offset']+self.widths.get(ins['offset'],0):return 'explicit-transfer-to-next'
        if (source,destination) in self.cfg['backedges']:return 'dominator-backedge'
        containing=[l for l in self.cfg['loops'] if source in l['nodes']]
        if any(destination not in l['nodes'] for l in containing):return 'loop-exit'
        if destination<ins['offset']:return 'backward-transfer-not-proved-backedge'
        return 'conditional-forward' if OPCODES[op]['flow']=='Cond_Branch' else 'forward-transfer'
    def emission(self):
        label_offsets={t for ins in self.instructions for t in targets(ins)}
        boundaries=defaultdict(list)
        for n,clause in enumerate(self.method.get('exception_regions',[])):
            kind=clause['kind'];typ=_type(clause.get('catch_type')) if kind=='Catch' else None
            for key,event in (('try_start','eh_begin'),('try_end','eh_try_end'),('handler_start','eh_handler'),('handler_end','eh_end')):
                extra={'kind':event,'region_id':'EH'+str(n),'handler_kind':kind.lower(),'region_role':key}
                if typ and typ!='UNKNOWN':extra['catch_type']=typ
                boundaries[clause[key]].append(extra)
            if kind not in ('Catch','Finally'):self.reason('EH-kind-unmodeled',kind)
        run=[]
        def flush():
            if not run:return
            first,last=run[0],run[-1];leaf_id='L'+str(len(self.leaves));start=first['offset'];end=last['offset']+self.widths.get(last['offset'],0)
            self.leaves.append({'id':leaf_id,'il_start':start,'il_end':end,'instruction_offsets':[i['offset'] for i in run],
                'instructions':[dict(i) for i in run],'complete':all(i['opcode'] in OPCODES for i in run),
                'stack_entry_height':len(self.stack['before'][start]) if start in self.stack['before'] else None,
                'stack_exit_height':len(self.stack['after'][last['offset']]) if last['offset'] in self.stack['after'] else None})
            self.plan.append({'kind':'leaf','leaf_id':leaf_id,'il_start':start,'il_end':end});run.clear()
        for ins in self.instructions:
            offset=ins['offset'];info=OPCODES.get(ins['opcode'],{});flow=info.get('flow');op=ins['opcode']
            if offset in label_offsets or offset in boundaries:flush()
            # EH events at one position are kept distinct, but deterministic.
            for event in sorted(boundaries.get(offset,[]),key=lambda e:({'eh_try_end':0,'eh_end':1,'eh_begin':2,'eh_handler':3}[e['kind']],e['region_id'])):
                self.plan.append(dict(event,il_offset=offset))
            if offset in label_offsets:self.plan.append({'kind':'label','label':'IL_'+str(offset),'il_offset':offset})
            if flow in ('Branch','Cond_Branch'):
                flush();event={'kind':'branch','opcode':op,'il_offset':offset,'il_end':offset+self.widths.get(offset,0),
                    'role':self.branch_role(ins),'instruction':dict(ins)}
                dest=targets(ins)
                if op=='switch':event['target_labels']=['IL_'+str(t) for t in dest]
                elif len(dest)==1:event['target_label']='IL_'+str(dest[0])
                else:self.reason('branch-targets-unmodeled',ins)
                if flow=='Cond_Branch':event['predicate']=_predicate(ins,self.stack) if op!='switch' else {'kind':'switch-index'}
                self.roles.append(event);self.plan.append(event)
            elif flow in ('Return','Throw'):
                flush();kind='return' if op=='ret' else 'throw' if op in ('throw','rethrow') else 'terminal'
                self.plan.append({'kind':kind,'opcode':op,'il_offset':offset,'il_end':offset+self.widths.get(offset,0)})
            else:run.append(ins)
        flush()
        for event in boundaries.get(self.size,[]):self.plan.append(dict(event,il_offset=self.size))
        if self.size in label_offsets:self.plan.append({'kind':'label','label':'IL_'+str(self.size),'il_offset':self.size})
    def tree(self):
        return RegionParser(self).parse_root()
    def run(self):
        valid=self.validate()
        if not self.instructions:
            root=self.node('unknown',0,self.size);root['complete']=False
            return self.result(root,False)
        self.cfg=build_cfg(self.method)
        self.stack=_stack_analysis(self.method,self.instructions)
        self.reasons.extend(self.stack['reasons'])
        if self.cfg['irreducible_sccs']:self.reason('irreducible-normal-CFG',[sorted(x) for x in self.cfg['irreducible_sccs']])
        self.emission();root=self.tree()
        plan_complete=valid and not any(r['code'] in ('branch-targets-unmodeled','EH-kind-unmodeled') for r in self.reasons)
        return self.result(root,plan_complete)
    def result(self,root,plan_complete):
        controlled=[ins['offset'] for ins in self.instructions if OPCODES.get(ins['opcode'],{}).get('flow') in ('Branch','Cond_Branch','Return','Throw')]
        leaf_offsets=[offset for leaf in self.leaves for offset in leaf['instruction_offsets']]
        counted=controlled+leaf_offsets
        coverage={'instruction_count':len(self.instructions),'covered_instruction_count':len(set(counted)),
            'duplicate_instruction_offsets':sorted(o for o in set(counted) if counted.count(o)>1),
            'uncovered_instruction_offsets':sorted(set(self.by)-set(counted)),
            'control_instruction_count':len(controlled),'branch_instruction_count':len(self.roles),
            'leaf_instruction_count':len(leaf_offsets),'leaf_count':len(self.leaves),
            'il_code_size':self.size,'covered_instruction_bytes':sum(self.widths.get(o,0) for o in set(counted)),
            'own_source_or_MDB_used':False}
        exact=(not coverage['duplicate_instruction_offsets'] and not coverage['uncovered_instruction_offsets']
            and coverage['covered_instruction_bytes']==self.size)
        coverage['exact_instruction_coverage']=exact
        tree_complete=bool(root.get('complete')) and not self.reasons
        report={'schema':'smcs-control-tree-v1','side':'il','method_id':self.method.get('stable_id',self.method.get('key')),
            'complete':bool(plan_complete and tree_complete and exact),'emission_plan_complete':bool(plan_complete and exact),
            'tree_complete':tree_complete,'reasons':self.reasons,'tree':root,'leaves':self.leaves,'branch_roles':self.roles,
            'coverage':coverage,'extra_section_validation':getattr(self,'section_validation',None),'emission_plan':self.plan,'emission_equivalences':self.equivalences,
            'input_method_token':self.method.get('token'),'input_unchanged':True}
        report['canonical_tree']=canonical_tree(report)
        if report['emission_plan_complete']:
            try:report['canonical_plan']=canonical_plan(report)
            except ValueError as error:
                report['complete']=report['emission_plan_complete']=False
                report['reasons'].append({'code':'canonical-unbound-label','node_id':None,'detail':str(error)})
        return report


def canonical_tree(ir):
    """Structural comparison only. Never reads the linear emission plan."""
    if not ir.get('tree_complete',ir.get('complete',False)):return None
    def sequence(children):
        flat=[]
        for child in children:
            if child['kind']=='sequence':flat.extend(child['children'])
            else:flat.append(child)
        out=[]
        for child in flat:
            if child['kind']=='leaf' and out and out[-1]['kind']=='leaf':continue
            out.append(child)
        return {'kind':'sequence','children':out}
    def visit(node):
        kind=node['kind'];data=node.get('data',{});children=[visit(c) for c in node.get('children',[])]
        if any(c is None for c in children):return None
        if kind=='sequence':return sequence(children)
        if kind=='leaf':return {'kind':'leaf'}
        if kind=='return':return sequence(children+[{'kind':'return','opcode':data.get('opcode','ret')}])
        if kind=='throw':return sequence(children+[{'kind':'throw','opcode':data.get('opcode','throw')}])
        if kind in ('break','continue'):
            row={'kind':kind,'opcode':data.get('opcode','br'),'target_role':data.get('target_role',kind),'target_depth':data.get('lexical_loop_depth',0)}
            if CONTROL_STAGE>=4:
                if 'target_owner_kind' not in data:return None
                row.update(target_owner_kind=data['target_owner_kind'],target_breakable_depth=data['target_breakable_depth'])
            return row
        if kind=='logical':
            if data.get('condition_chain') or data.get('r10_common_loop_target_chain'):return sequence(children)
            if (data.get('value_mode') or (data.get('operator') in ('&&','||') and 'branch_on_true' not in data)) and len(children)==3:
                return {'kind':'logical-value','condition':sequence([children[0]]),'right':sequence([children[1]]),'short_result':sequence([children[2]]),'join_opcode':data.get('join_opcode','br.s')}
            if data.get('opcode'):return sequence(children+[{'kind':'test','opcode':data['opcode']}])
            # Unary ! and zero comparisons can be wrappers around the emitted predicate.
            if len(children)==1:return children[0]
            return None
        if kind=='switch':
            if 'dense_table' not in data:return None
            return {'kind':'switch','dispatch':{'normalization_opcode':data.get('dense_normalization_opcode'),
                'normalization_value':data.get('dense_normalization_value',0),'table':data['dense_table'],
                'default_branch_opcode':data.get('dense_default_branch')},
                'sections':[{'keys':child.get('data',{}).get('case_keys',[]),'body':sequence([value])}
                            for child,value in zip(node.get('children',[]),children)]}
        if kind=='try':
            if 'eh_kind' not in data:return None
            return {'kind':'try','handler_kind':data['eh_kind'],'catch_types':data.get('eh_catch_types',[]),
                'try_epilogue_opcode':data.get('eh_try_epilogue'),
                'handler_epilogue_opcodes':data.get('eh_handler_epilogues',[]),
                'body':sequence([children[0]]),'handlers':[sequence([c]) for c in children[1:]]}
        if kind in ('if','ternary'):
            if data.get('constant_elided'):return sequence(children)
            if len(children) not in (2,3):return None
            join=data.get('join_opcode','br' if kind=='ternary' else None)
            if 'join_opcode' not in data and data.get('has_else') and data.get('then_can_fallthrough'):join='br'
            condition=sequence([children[0]]);then=sequence([children[1]]);otherwise=sequence([children[2]]) if len(children)==3 else None
            result={'kind':kind,'condition':condition,'then':then,'else':otherwise,'join_opcode':join}
            # Literal emit equivalence, proved by the Guard/GuardElse fixture:
            # Mono emits no else-join after a nonfallthrough true arm.
            # Moving that lexical else to the following sequence retains every
            # emitted transfer; this is not branch elimination.
            if kind=='if' and otherwise is not None and data.get('then_can_fallthrough') is False and join is None:
                result['else']=None
                return sequence([result,otherwise])
            return result
        if kind=='loop':
            if not data.get('emission_form') or len(children)!=2:return None
            def common(node):return node.get('data',{}).get('condition_chain') or node.get('data',{}).get('r10_common_loop_target_chain') or any(common(c) for c in node.get('children',[]))
            condition={'kind':'test-chain','taken_target':'loop-body','predicates':children[0]} if CONTROL_STAGE>=3 and common(node['children'][0]) else children[0]
            return {'kind':'loop','emission_form':data['emission_form'],'entry_opcode':data.get('entry_opcode'),
                'condition':condition,'body':children[1],'tail_opcode':data.get('tail_opcode')}
        return None
    return visit(ir['tree'])


class RegionParser:
    """Conservative physical-region parser, independent of source and event plan.

    Supported templates are literal Mono emission intervals, not arbitrary CFG
    canonicalization. Every join/backedge remains explicit in the node data.
    Unknown region/ownership makes the entire method tree incomplete.
    """
    def __init__(self,extractor):
        self.e=extractor;self.ins=extractor.instructions;self.n=len(self.ins);self.offsets=[i['offset'] for i in self.ins]+[extractor.size]
        self.index={v:k for k,v in enumerate(self.offsets)};self.bad=set();self.depth=0
    def node(self,kind,lo,hi,children=(),data=None):return self.e.node(kind,self.offsets[lo],self.offsets[hi],children,data)
    def unknown(self,code,lo,hi,detail):
        node=self.node('unknown',lo,hi,data={'raw_instruction_offsets':self.offsets[lo:hi]})
        self.e.reason(code,detail,node);return node
    def height(self,index,after=False):
        if index>=self.n:return None
        state=self.e.stack.get('after' if after else 'before',{}).get(self.offsets[index])
        return len(state) if state is not None else None
    def target(self,index):
        dest=targets(self.ins[index]);return self.index.get(dest[0]) if len(dest)==1 else None
    def cond(self,index):return OPCODES.get(self.ins[index]['opcode'],{}).get('flow')=='Cond_Branch' and self.ins[index]['opcode']!='switch'
    def expr_start(self,index,lo):
        pred=_predicate(self.ins[index],self.e.stack)
        start=min(pred['producer_offsets'],default=self.offsets[index])
        k=self.index.get(start)
        if k is None or k<lo or k>=index:return None
        if any(OPCODES.get(i['opcode'],{}).get('flow') in ('Branch','Cond_Branch','Return','Throw') for i in self.ins[k:index]):return None
        return k
    def condition(self,start,branch):
        node=self.node('logical',start,branch+1,[self.node('leaf',start,branch)],
            {'opcode':self.ins[branch]['opcode'],'predicate':_predicate(self.ins[branch],self.e.stack),'branch_offset':self.offsets[branch]})
        if not node['data']['predicate']['stack_inputs_known']:self.e.reason('condition-stack-unmodeled',self.offsets[branch],node)
        return node
    def dense_switch(self,lo,hi,frames):
        """One exact Mono Int32 table, no default body, outside lexical loops.

        A recognized region retains source-form ambiguity among literal
        emission-equivalent spellings. No branch or table hole is deleted.
        Sparse/hybrid dispatch and coincident loop exits remain UNKNOWN.
        """
        if frames or lo==0 or _slot(self.ins[lo],'ldloc') is None:return None
        slot=_slot(self.ins[lo],'ldloc')
        if _slot(self.ins[lo-1],'stloc')!=slot or self.height(lo)!=0:return None
        j=lo+1;normalization=None;constant=0;first=0
        if j+2<hi and self.ins[j+1]['opcode'] in ('add','sub'):
            op=self.ins[j]['opcode'];value=self.ins[j].get('operand')
            if op=='ldc.i4.m1':value=-1
            elif op.startswith('ldc.i4.') and op[-1:].isdigit():value=int(op[-1])
            elif op not in ('ldc.i4','ldc.i4.s'):return None
            if type(value) is not int:return None
            normalization=self.ins[j+1]['opcode'];constant=value
            first=value if normalization=='sub' else ((-value+(1<<31))%(1<<32))-(1<<31)
            j+=2
        if j+1>=hi or self.ins[j]['opcode']!='switch' or self.ins[j+1]['opcode']!='br':return None
        state=self.e.stack.get('before',{}).get(self.offsets[j])
        if not state or len(state)!=1 or state[0]['type']!='System.Int32':return None
        raw=targets(self.ins[j]);exit_=self.target(j+1)
        if not raw or exit_ is None or not j+1<exit_<=hi:return None
        starts=sorted({self.index.get(t,-1) for t in raw if t!=self.offsets[exit_]})
        if not starts or starts[0]!=j+2 or starts[-1]>=exit_:return None
        if any(k<=j+1 for k in starts) or any(t not in self.index for t in raw):return None
        # At least one concrete case break establishes the shared switch exit.
        if not any(self.ins[k]['opcode']=='br' and self.target(k)==exit_ for k in range(starts[0],exit_)):return None
        for ins in self.ins:
            if self.offsets[lo]<=ins['offset']<self.offsets[exit_]:continue
            if any(self.offsets[starts[0]]<=t<self.offsets[exit_] for t in targets(ins)):return None
        keys={start:[] for start in starts}
        for delta,target in enumerate(raw):
            if target==self.offsets[exit_]:continue
            keys[self.index[target]].append(first+delta)
        parts=[]
        frame={'test':-1,'exit':exit_,'body':starts[0],'kind':'switch'}
        for n,start in enumerate(starts):
            end=starts[n+1] if n+1<len(starts) else exit_
            section=self.parse(start,end,[frame],suppress_loop_at=None)
            section['data']['case_keys']=keys[start]
            parts.append(section)
        data={'selector_type':'System.Int32','dense_first':first,'dense_normalization_opcode':normalization,
              'dense_normalization_value':constant,'dense_table':[starts.index(self.index[t]) if t!=self.offsets[exit_] else -1 for t in raw],
              'dense_default_branch':'br','source_form':'UNKNOWN among literal table-switch emission equivalents'}
        result=self.node('switch',lo,exit_,parts,data)
        result['complete']=all(p['complete'] for p in parts)
        return result
    def physical_loop(self,lo,hi):
        """Return exact contiguous single-test loop interval starting at lo."""
        op=self.ins[lo]['opcode'];dest=self.target(lo)
        # While/for: br TEST; BODY; TEST: expression; brtrue BODY.
        if op in ('br','br.s') and dest is not None and lo<dest<hi:
            for k in range(dest,hi):
                if self.cond(k) and self.target(k)==lo+1:
                    start=self.expr_start(k,dest)
                    if start==dest and any(l['header']==self.offsets[dest] and l['dominated'] for l in self.e.cfg['loops']):
                        chain=[(dest,k)];next_start=k+1
                        if CONTROL_STAGE>=3:
                            while next_start<hi:
                                found=None
                                for q in range(next_start,min(hi,next_start+201)):
                                    flow=OPCODES.get(self.ins[q]['opcode'],{}).get('flow')
                                    if self.cond(q):
                                        if self.target(q)==lo+1 and self.expr_start(q,next_start)==next_start:found=q
                                        break
                                    if flow in ('Branch','Return','Throw'):break
                                if found is None:break
                                chain.append((next_start,found));next_start=found+1
                        return {'start':lo,'end':chain[-1][1]+1,'body_start':lo+1,'body_end':dest,'test':dest,'branch':chain[-1][1],
                            'test_chain':chain,'emission_form':'pretested','entry_opcode':op,'tail_opcode':None}
                    break
                if OPCODES.get(self.ins[k]['opcode'],{}).get('flow') in ('Branch','Cond_Branch','Return','Throw'):break
                if k-dest>200:break
            # for(;;) still emits an entry branch, even if it points at a br.
            if self.ins[dest]['opcode'] in ('br','br.s') and self.target(dest)==lo+1 and any(l['header']==self.offsets[dest] and l['dominated'] for l in self.e.cfg['loops']):
                return {'start':lo,'end':dest+1,'body_start':lo+1,'body_end':dest,'test':dest,'branch':None,
                    'emission_form':'infinite','entry_opcode':op,'tail_opcode':self.ins[dest]['opcode']}
        # Do: BODY; expression; conditional back to BODY. Distinguish from the
        # pretested interval above by requiring the target dominate the latch.
        possibilities=[]
        for k in range(lo+1,hi):
            if self.target(k)!=lo:continue
            owner=self.e.cfg['owner'].get(self.offsets[k]);target=self.offsets[lo]
            if (owner,target) not in self.e.cfg['backedges']:continue
            if self.cond(k):
                start=self.expr_start(k,lo)
                if start is not None and start>lo:possibilities.append({'start':lo,'end':k+1,'body_start':lo,'body_end':start,
                    'test':start,'branch':k,'emission_form':'posttested','entry_opcode':None,'tail_opcode':None})
            elif self.ins[k]['opcode'] in ('br','br.s'):
                possibilities.append({'start':lo,'end':k+1,'body_start':lo,'body_end':k,'test':lo,'branch':None,
                    'emission_form':'infinite','entry_opcode':None,'tail_opcode':self.ins[k]['opcode']})
        return max(possibilities,key=lambda l:l['end']) if possibilities else None
    def loop(self,shape,frames):
        lo,hi=shape['start'],shape['end'];branch=shape['branch']
        data={key:shape[key] for key in ('emission_form','entry_opcode','tail_opcode')}
        data.update({'test_offset':self.offsets[shape['test']],'exit_offset':self.offsets[hi],
            'body_offset':self.offsets[shape['body_start']],'source_form':'UNKNOWN: emission-equivalent lowered forms'})
        if shape['emission_form']=='pretested':
            eq={'equivalence_id':'while-vs-for-empty-init-increment','forms':['while','for with equivalent moved initializer/increment'],
                'proof':'statement.cs421-461;568-619; toy While/For raw bodies identical; iteration operations retained as leaves'}
            data['emission_equivalence']=eq
            self.e.equivalences.append(eq)
        frame={'test':shape['test'],'exit':hi,'body':shape['body_start']}
        body=self.parse(shape['body_start'],shape['body_end'],frames+[frame],suppress_loop_at=shape['body_start'])
        if len(shape.get('test_chain',[]))>1:
            condition=self.node('logical',shape['test'],branch+1,[self.condition(a,b) for a,b in shape['test_chain']],{'condition_chain':True,'taken_target_role':'loop-body'})
        else:condition=self.condition(shape['test'],branch) if branch is not None else self.node('sequence',shape['test'],shape['test'],data={'constant_true':True})
        return self.node('loop',lo,hi,[condition,body],data)
    def parse(self,lo,hi,frames=(),suppress_loop_at=None):
        root=self.node('sequence',lo,hi);i=lo;run=lo;self.depth+=1
        if self.depth>100:
            self.depth-=1;root['children'].append(self.unknown('region-depth-budget',lo,hi,100));root['complete']=False;return root
        def flush(end):
            nonlocal run
            if run<end:root['children'].append(self.node('leaf',run,end))
            run=end
        while i<hi:
            if CONTROL_STAGE>=1:
                switch=self.dense_switch(i,hi,frames)
                if switch is not None:
                    flush(i);root['children'].append(switch);i=self.index[switch['span']['end']];run=i;continue
            if i!=suppress_loop_at:
                shape=self.physical_loop(i,hi)
                if shape:
                    flush(i);root['children'].append(self.loop(shape,list(frames)));i=shape['end'];run=i;continue
            ins=self.ins[i];op=ins['opcode'];flow=OPCODES.get(op,{}).get('flow')
            if op=='switch':
                flush(i);root['children'].append(self.unknown('switch-lexical-regions-unmodeled',i,i+1,
                    {'ordered_raw_targets':targets(ins),'reason':'dense table is exact; original switch/if labels not guessed'}));i+=1;run=i;continue
            if self.cond(i):
                start=self.expr_start(i,run);target=self.target(i)
                if start is None or target is None or target<=i or target>hi:
                    flush(i);root['children'].append(self.unknown('conditional-region-unmodeled',i,i+1,
                        {'opcode':op,'target':target,'known_expression_start':start}));i+=1;run=i;continue
                flush(start);join_index=target-1;join=self.target(join_index) if join_index>i and self.ins[join_index]['opcode'] in ('br','br.s') else None
                # A branch into a known loop target is a transfer in the true
                # arm, not proof of an if/else join.
                if join is not None and any(join in (f['test'],f['exit']) for f in frames):join=None
                has_else=join is not None and target<=join<=hi
                end=join if has_else else target;then_end=join_index if has_else else target
                value_join=has_else and self.height(end) is not None and self.height(start) is not None and self.height(end)>self.height(start)
                kind='ternary' if value_join and self.ins[join_index]['opcode']=='br' else 'if'
                if value_join and self.ins[join_index]['opcode']!='br':
                    short_op=op[:-2] if op.endswith('.s') else op
                    expected='ldc.i4.0' if short_op=='brfalse' else 'ldc.i4.1' if short_op=='brtrue' else None
                    if self.ins[join_index]['opcode']=='br.s' and end==target+1 and self.ins[target]['opcode']==expected:kind='logical'
                    else:
                        root['children'].append(self.unknown('logical-value-region-unmodeled',start,end,
                            'Short join branch not one of the exact logical-value templates'));i=end;run=i;continue
                cond=self.condition(start,i);then=self.parse(i+1,then_end,frames)
                children=[cond,then]
                if has_else:children.append(self.parse(target,end,frames))
                node=self.node(kind,start,end,children,{'has_else':has_else,'join_opcode':self.ins[join_index]['opcode'] if has_else else None,
                    'conditional_opcode':op,'value_mode':kind=='logical','false_target_offset':self.offsets[target],
                    'retained_join_offset':self.offsets[join_index] if has_else else None})
                if has_else and frames and join<frames[-1]['test'] and not any(OPCODES.get(i['opcode'],{}).get('flow') in ('Branch','Cond_Branch','Return','Throw') for i in self.ins[join:frames[-1]['test']]):
                    self.e.reason('for-increment-versus-if-join-ambiguous','True-arm br targets a control-free suffix before the loop test: possible continue to for iterator or if/else join; ownership UNKNOWN',node)
                if not has_else and then_end>i+1 and (terminal(self.ins[then_end-1]['opcode']) or unconditional(self.ins[then_end-1]['opcode'])) and target<hi:
                    node['data']['emission_equivalence']={'equivalence_id':'terminal-then-else-vs-guard-tail','source_form':'UNKNOWN: both literal syntax forms emit identical control instructions','proof':'statement.cs247-264 omits join after nonfallthrough true arm; fixture Guard/GuardElse raw bodies identical'}
                    self.e.equivalences.append(dict(node['data']['emission_equivalence'],node_id=node['id']))
                if not has_else and then['children'] and then['children'][-1]['kind']=='if' and then['children'][-1]['span']['end']==self.offsets[end]:
                    eq={'equivalence_id':'and-short-circuit-vs-nested-if','source_form':'UNKNOWN among emission-equivalent condition grouping','proof':'expression.cs3450-3471; toy AndIf/NestedAndIf raw bodies identical'}
                    node['data']['emission_equivalence']=eq;self.e.equivalences.append(dict(eq,node_id=node['id']))
                if any(not c['complete'] for c in children):node['complete']=False
                root['children'].append(node);i=end;run=i;continue
            if flow=='Branch':
                flush(i);target=self.target(i);kind=None;role=None
                if CONTROL_STAGE>=4:
                    evidence=resolve_transfer(frames,target,op)
                    if evidence['complete']:
                        chosen=evidence['chosen'];kind=chosen['kind']
                        data=dict(chosen,opcode=op,target_role=kind,target_offset=self.offsets[target],owner_candidates=evidence['owner_candidates'])
                        root['children'].append(self.node(kind,i,i+1,data=data))
                    else:
                        node=self.unknown(evidence['reason'],i,i+1,{'opcode':op,'target':target,'owner_candidates':evidence['owner_candidates']})
                        node['data']['owner_candidates']=evidence['owner_candidates'];root['children'].append(node)
                    i+=1;run=i;continue
                if CONTROL_STAGE>=2 and op in ('leave','leave.s') and any(f.get('kind')=='EH' and target in f.get('return_targets',[]) for f in frames):
                    root['children'].append(self.node('return',i,i+1,data={'opcode':op,'protected_epilogue_target':self.offsets[target]}));i+=1;run=i;continue
                for depth,frame in enumerate(reversed(frames)):
                    if target==frame['exit']:kind='break';role='break';break
                    if target==frame['test']:kind='continue';role='continue';break
                if kind:
                    root['children'].append(self.node(kind,i,i+1,data={'opcode':op,'target_role':role,'lexical_loop_depth':depth,'target_offset':self.offsets[target]}))
                else:root['children'].append(self.unknown('unowned-transfer-unmodeled',i,i+1,{'opcode':op,'target':target}))
                i+=1;run=i;continue
            if flow in ('Return','Throw'):
                flush(i);kind='return' if op=='ret' else 'throw' if op in ('throw','rethrow') else 'unknown'
                node=self.node(kind,i,i+1,data={'opcode':op})
                if kind=='unknown':self.e.reason('terminal-template-unmodeled',op,node)
                root['children'].append(node);i+=1;run=i;continue
            i+=1
        flush(hi);self.depth-=1
        root['complete']=all(c['complete'] for c in root['children'])
        return root
    def parse_root(self):
        if self.e.method.get('exception_regions'):
            if CONTROL_STAGE>=2:
                simple=self.simple_eh_root()
                if simple is not None:return simple
            root=self.parse(0,self.n)
            self.e.reason('EH-lexical-lowering-unmodeled','All EH boundaries/clauses retained in plan; using/lock/foreach/manual try cannot be uniquely named from IL',root)
            return root
        return self.parse(0,self.n)
    def simple_eh_root(self):
        """Literal flat catch-group or one finally; no overlapping EH guess."""
        clauses=self.e.method.get('exception_regions',[])
        if not clauses or any(c['kind'] not in ('Catch','Finally') for c in clauses):return None
        ranges={(c['try_start'],c['try_end']) for c in clauses}
        if len(ranges)!=1:return None
        if any(c['kind']=='Finally' for c in clauses) and (len(clauses)!=1 or clauses[0]['kind']!='Finally'):return None
        if [c['handler_start'] for c in clauses]!=sorted(c['handler_start'] for c in clauses):return None
        try_start,try_end=next(iter(ranges));end=clauses[-1]['handler_end']
        if any(p not in self.index for p in [try_start,try_end,end]+[v for c in clauses for v in (c['handler_start'],c['handler_end'])]):return None
        start,try_finish,finish=[self.index[p] for p in (try_start,try_end,end)]
        if clauses[0]['handler_start']!=try_end or any(a['handler_end']!=b['handler_start'] for a,b in zip(clauses,clauses[1:])):return None
        if try_finish<=start or self.ins[try_finish-1]['opcode']!='leave' or self.target(try_finish-1)!=finish:return None
        final=clauses[0]['kind']=='Finally';handlers=[]
        for clause in clauses:
            lo,hi=self.index[clause['handler_start']],self.index[clause['handler_end']]
            if hi<=lo:return None
            if final:
                if self.ins[hi-1]['opcode']!='endfinally':return None
            elif self.ins[hi-1]['opcode']!='leave' or self.target(hi-1)!=finish:return None
        if not self.ins or self.ins[-1]['opcode']!='ret':return None
        footer=self.n-2 if self.n>1 and _slot(self.ins[-2],'ldloc') is not None else self.n-1
        frame={'test':-1,'exit':-2,'body':start,'kind':'EH','return_targets':[footer]}
        body=self.parse(start,try_finish-1,[frame])
        for clause in clauses:
            lo,hi=self.index[clause['handler_start']],self.index[clause['handler_end']]
            handlers.append(self.parse(lo,hi-1,[frame]))
        data={'eh_kind':'finally' if final else 'catch','eh_catch_types':[] if final else [_type(c['catch_type']) for c in clauses],
              'eh_try_epilogue':'leave','eh_handler_epilogues':['endfinally' if final else 'leave' for c in clauses],
              'source_form':'UNKNOWN among exact EH-lowering equivalents; clauses and implicit edges retained'}
        node=self.node('try',start,finish,[body]+handlers,data);node['complete']=all(c['complete'] for c in node['children'])
        prefix=self.parse(0,start);suffix=self.parse(finish,self.n)
        root=self.node('sequence',0,self.n,[prefix,node,suffix]);root['complete']=all(c['complete'] for c in root['children'])
        return root


def extract_il(methodDTO):
    """Extract independent control evidence without modifying the caller's DTO."""
    before=json.dumps(methodDTO,sort_keys=True,separators=(',',':'))
    result=Extractor(methodDTO).run()
    result['control_stage']=CONTROL_STAGE
    if json.dumps(methodDTO,sort_keys=True,separators=(',',':'))!=before:raise AssertionError('Read-only DTO mutated')
    return result
