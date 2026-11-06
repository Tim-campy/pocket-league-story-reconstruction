"""Normal-edge CFG/dominators; no exceptional edges are invented."""
def targets(instruction):
    operand=instruction.get('operand')
    if not isinstance(operand,dict):return []
    if 'branch' in operand:return [operand['branch']]
    return operand.get('switch',operand.get('branches',[]))


def unconditional(opcode):return opcode in ('br','br.s','leave','leave.s')
def terminal(opcode):return opcode in ('ret','throw','rethrow','endfinally','endfilter','jmp')


def build_cfg(method):
    ins=method.get('instructions',[])
    if not ins:return {'error':'no-body'}
    starts={ins[0]['offset']};known={i['offset'] for i in ins};invalid=[]
    for n,i in enumerate(ins):
        for destination in targets(i):
            if destination in known:starts.add(destination)
            else:invalid.append((i['offset'],destination))
        if (targets(i) or terminal(i['opcode'])) and n+1<len(ins):starts.add(ins[n+1]['offset'])
    # EH entries are block boundaries, but exceptional transfers are deliberately
    # not invented. Unreachable handler-only edges will be UNKNOWN.
    for region in method.get('exception_regions',[]):
        for field in ('try_start','try_end','handler_start','handler_end','filter_start','try_offset','handler_offset','filter_offset'):
            if region.get(field) in known:starts.add(region[field])
    starts=sorted(starts);blocks={s:[] for s in starts};owner={};position=0
    for i in ins:
        while position+1<len(starts) and i['offset']>=starts[position+1]:position+=1
        blocks[starts[position]].append(i);owner[i['offset']]=starts[position]
    succ={s:set() for s in starts}
    for n,s in enumerate(starts):
        last=blocks[s][-1];op=last['opcode']
        succ[s].update(t for t in targets(last) if t in blocks)
        if not unconditional(op) and not terminal(op) and n+1<len(starts):succ[s].add(starts[n+1])
    pred={s:set() for s in starts}
    for s,edges in succ.items():
        for d in edges:pred[d].add(s)
    reachable=set();pending=[starts[0]]
    while pending:
        s=pending.pop()
        if s in reachable:continue
        reachable.add(s);pending.extend(succ[s]-reachable)
    dom={s:({s} if s==starts[0] else set(reachable)) for s in reachable}
    changed=True
    while changed:
        changed=False
        for s in starts[1:]:
            if s not in reachable:continue
            parents=[dom[p] for p in pred[s] if p in reachable]
            now={s}|set.intersection(*parents) if parents else {s}
            if dom[s]!=now:dom[s]=now;changed=True
    backedges=[(s,d) for s in reachable for d in succ[s] if d in dom[s]]
    # Detect irreducible SCCs explicitly; natural loops alone cannot establish
    # that a branch outside their union lies outside every possible cycle.
    visited=set();order=[]
    for root in sorted(reachable):
        if root in visited:continue
        pending=[(root,False)]
        while pending:
            node,done=pending.pop()
            if done:order.append(node);continue
            if node in visited:continue
            visited.add(node);pending.append((node,True))
            pending.extend((child,False) for child in succ[node] if child in reachable and child not in visited)
    assigned=set();irreducible=[]
    for root in reversed(order):
        if root in assigned:continue
        component=set();pending=[root]
        while pending:
            node=pending.pop()
            if node in assigned:continue
            assigned.add(node);component.add(node)
            pending.extend(parent for parent in pred[node] if parent in reachable and parent not in assigned)
        cyclic=len(component)>1 or root in succ[root]
        headers=[n for n in component if all(n in dom[s] for s in component)]
        if cyclic and not headers:irreducible.append(component)
    loops={}
    for latch,header in backedges:
        natural={header,latch};work=[] if latch==header else [latch]
        while work:
            n=work.pop()
            for p in pred[n]:
                if p in reachable and p not in natural:natural.add(p);work.append(p)
        # For a natural backedge, every discovered node should be dominated by
        # the header. Preserve this explicit check instead of assuming it.
        valid=all(header in dom[n] for n in natural)
        entry=loops.setdefault(header,{'header':header,'latches':set(),'nodes':set(),'dominated':True})
        entry['latches'].add(latch);entry['nodes'].update(natural);entry['dominated'] &= valid
    return {'blocks':blocks,'owner':owner,'succ':succ,'pred':pred,'reachable':reachable,'dom':dom,
        'backedges':set(backedges),'loops':list(loops.values()),'invalid_targets':invalid,
        'eh_regions':len(method.get('exception_regions',[])),'irreducible_sccs':irreducible}

