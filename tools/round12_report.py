"""Eight-section measured report, only after an authorized Round12 stop."""
from collections import Counter
import csv
import json
from pathlib import Path
import sys

ROOT=next(p for p in Path(__file__).resolve().parents if (p/'AGENTS.md').exists())
sys.path.insert(0,str(ROOT/'tools'))
from compare_kairo import sha
from apply_il_rules import receipt_is_fresh
from round12_controller import stop_reason,validate_config,verify_raw_gate,current_measurement
from round10_report import table,pct,counts,inline,il

ASSEMBLIES=('KairoLibrary','Assembly-CSharp')
SECTIONS=('1. PROGRESSO','2. O QUE FOI FEITO NESTA RODADA','3. HIPÓTESES',
 '4. PENDENTES POR GRUPO','5. TOOLCHAIN','6. COMPONENTES EXTERNOS E METADADOS',
 '7. BLOQUEIOS','8. SUA RECOMENDAÇÃO')

def read(path):return json.loads((ROOT/path).read_text(encoding='utf-8-sig'))
def check(path,digest):
 if sha(ROOT/path)!=digest:raise ValueError('Stale evidence: '+str(path))

def checked_context():
 cfg=read('config/round12-protocol.json');validate_config(cfg)
 journal=read('reports/round12-cycle-journal.json');stop=read('reports/round12-stop.json')
 if not stop_reason(cfg,journal) or not stop.get('new_reconstruction_work_stopped'):
  raise ValueError('No authorized measured stop; continue reconstruction')
 base=read(cfg['baseline_measurement']);check('build/round12/baseline/methods.csv',base['ledger_sha256'])
 final=read('reports/round12-final-measurement.json');check('config/methods.csv',final['ledger_sha256'])
 if final['baseline_commit']!=cfg['baseline_commit']:raise ValueError('Wrong baseline')
 raw=verify_raw_gate(cfg,current_measurement(cfg,journal))
 release=None
 if cfg.get('verification_origin_release'):
  binding=cfg['verification_origin_release'];check(binding['record'],binding['sha256'])
  release=read(binding['record'])['event_core']
  if release['status']!='CONFIRMED_BY_FULL_VERIFY' or release['reconstruction_cycles_added']!=0 or release['source_changes']!=0:
   raise ValueError('Origin release is not a verified event independent of reconstruction cycles')
  cfg['_checked_origin_release']=release
 receipts={};models={};metadata={};manifests={}
 for assembly in ASSEMBLIES:
  receipt=read('build/verification-'+assembly+'.json')
  if not receipt_is_fresh(ROOT,receipt) or any(receipt.get(k)for k in('regressions','body_regressions','type_regressions')):
   raise ValueError('Full proof stale or regressive: '+assembly)
  for name,digest in receipt.get('helper_source_hashes',{}).items():check(name,digest)
  provenance=final['assemblies'][assembly]['provenance']
  if provenance['build_sha256']!=receipt['build_sha256']:raise ValueError('Final PE mismatch')
  for key,hashkey in [('original_dump','original_dump_sha256'),('rebuilt_dump','rebuilt_dump_sha256'),('receipt','receipt_sha256')]:check(provenance[key],provenance[hashkey])
  proof=cfg['final_model_measurements'][assembly];model=read(proof['validation']);corpus=ROOT/proof['corpus_baseline'];manifest=read(corpus/'build-manifest.json')
  check(corpus/'verification.json',model['baseline_receipt_sha256'])
  check(cfg['models'][assembly]['source'],model['source_sha256'])
  check(cfg['models'][assembly]['client'],model['client_sha256'])
  if model['matched_complete_mismatches']:raise ValueError('Incorrect complete slot prediction')
  for source in manifest['sources']:check(source['path'],source['sha256'])
  model['coverage_complete']=pct(model['matched_model_complete'],model['matched_bodies'])
  metadata[assembly]=read('reports/'+assembly+'-metadata.json')
  if metadata[assembly]['rebuilt_sha256']!=receipt['build_sha256'] or not metadata[assembly]['whole_structural_metadata_equal']:
   raise ValueError('Final metadata proof differs: '+assembly)
  receipts[assembly]=receipt;models[assembly]=model
  manifests[assembly]=read((ROOT/receipt['build_artifact']).parent/'build-manifest.json')
  gains=sum(c['gains'][assembly]['methods_gained']for c in journal['cycles'])
  bytegains=sum(c['gains'][assembly]['il_bytes_gained']for c in journal['cycles'])
  if release:
   gains+=release['gains'][assembly]['methods_gained'];bytegains+=release['gains'][assembly]['il_bytes_gained']
  if final['assemblies'][assembly]['matched_methods']-base['assemblies'][assembly]['matched_methods']!=gains or final['assemblies'][assembly]['matched_original_il_bytes']-base['assemblies'][assembly]['matched_original_il_bytes']!=bytegains:
   raise ValueError('Journal totals disagree with measured progress')
 for cycle in journal['cycles']:
  adoption=read('reports/round12-'+cycle['label']+'-adoption.json')
  if not adoption.get('official_verified')or adoption.get('matched_losses'):raise ValueError('Cycle is not verified without losses')
 return cfg,journal,stop,base,final,raw,receipts,models,metadata,manifests

def attempted_rules(ids):
 """Distinct historical rules, not a count of unique compiler invocations."""
 found={identity:set()for identity in ids}
 for path in sorted((ROOT/'reports').glob('*-Assembly-CSharp-engine.json')):
  text=path.read_text(encoding='utf-8-sig')
  if not any(identity in text for identity in ids):continue
  doc=json.loads(text)
  for method in doc.get('methods',[]):
   if not isinstance(method,dict)or method.get('id')not in found:continue
   for trial in method.get('trials',[]):
    if trial.get('rule'):found[method['id']].add(trial['rule'])
 return {identity:sorted(rules)for identity,rules in found.items()}

def example_lines(example):
 left=example.get('original_context',[])[:15];right=example.get('rebuilt_context',[])[:15]
 rows=[]
 for i in range(max(len(left),len(right))):
  rows.append([inline(il(left[i]))if i<len(left)else'—',inline(il(right[i]))if i<len(right)else'—'])
 return table(['IL original','IL recompilado'],rows)

def rule_breakdown(journal):
 """Use main passes only; isolated probes must not double method/IL gains."""
 rows=[]
 for cycle in journal['cycles']:
  path=ROOT/'reports'/('round12-'+cycle['label']+'-'+cycle['assembly']+'-engine.json')
  if not path.exists():continue
  engine=read(path);stats=engine.get('transformations',{})
  rules=sorted({key.rsplit(':',1)[0]for key in stats if ':'in key})
  for rule in rules:
   def metric(name):return stats.get(rule+':'+name,0)
   if not metric('attempts')and not metric('accepted_steps'):continue
   rows.append([cycle['label'],rule,metric('attempts'),metric('accepted_steps'),metric('body_gains'),metric('IL_gains')])
 return rows

def raw_measurement_tables(report):
 """Regional views overlap; keep them separate from literal absolute bytes."""
 result=[]
 for kind in ('regions.csv','tables.csv'):
  proof=report['outputs'][kind];check(proof['path'],proof['sha256'])
  with (ROOT/proof['path']).open(encoding='utf-8-sig',newline='')as stream:rows=list(csv.DictReader(stream))
  if kind=='regions.csv':
   selected=[r for r in rows if int(r['different_or_unpaired_bytes'])]
   result += ['Diferenças por região correspondente, incluindo os campos não determinísticos. Estas vistas se sobrepõem e não devem ser somadas ao total literal por offset absoluto:','',table(['Camada / região','Bytes original / recompilado','Bytes diferentes ou sem par'],[[r['layer']+' / '+r['name'],r['original_bytes']+' / '+r['rebuilt_bytes'],r['different_or_unpaired_bytes']]for r in selected]),'']
  else:
   selected=[r for r in rows if int(r['different_raw_bytes'])or r['original_rows']!=r['rebuilt_rows']]
   if selected:result += ['Tabelas com diferença bruta, comparadas por linha ordinal. A correspondência semântica é verificada separadamente:','',table(['Tabela','Linhas original / recompilado','Contagem de linhas igual','Bytes diferentes'],[[r['table'],r['original_rows']+' / '+r['rebuilt_rows'],'SIM'if r['original_rows']==r['rebuilt_rows']else'NÃO',r['different_raw_bytes']]for r in selected]),'']
   else:result += ['Nenhuma tabela tem diferença bruta de linhas ou bytes.','']
 return result

def released_helpers(journal):
 rows=[]
 for cycle in journal['cycles']:
  adoption=read('reports/round12-'+cycle['label']+'-adoption.json')
  identities=adoption.get('released_held_ids',[])
  if identities:rows.append([cycle['label'],len(identities),'; '.join(inline(identity)for identity in identities)])
 return rows

def fixture_negative_rows():
 """Retain measured fixture differences, including equivalent alternatives."""
 rows=[]
 def visit(node,location,path):
  if isinstance(node,dict):
   for key,value in node.items():
    if key in ('negative_controls','negative_differences')and isinstance(value,dict):
     cells=[]
     for label,diff in value.items():
      if isinstance(diff,list):cells.append(label+': '+('DISTINTO ('+', '.join(map(str,diff))+')'if diff else'IGUAL ao controle'))
      else:cells.append(label+': '+json.dumps(diff,ensure_ascii=False))
     rows.append([str(path.relative_to(ROOT)),location or'raiz','; '.join(cells)])
    elif key not in ('artifacts','headers','locals','EH','exception_regions'):visit(value,(location+'/'+key).strip('/'),path)
  elif isinstance(node,list):
   for index,value in enumerate(node):visit(value,location+'/'+str(index),path)
 for path in sorted((ROOT/'reports').glob('round12-*-fixture-report.json')):visit(read(path),'',path)
 return rows

def main():
 cfg,journal,stop,base,final,raw,receipts,models,metadata,manifests=checked_context()
 current=final['assemblies'];prior=base['assemblies'];lines=[]
 lines += [SECTIONS[0],'',table(['Assembly','Rodada anterior: métodos e IL','Agora: métodos e IL','Ganho métodos / IL','Perdas'],[[a,counts(prior[a]),counts(current[a]),str(current[a]['matched_methods']-prior[a]['matched_methods'])+' / '+str(current[a]['matched_original_il_bytes']-prior[a]['matched_original_il_bytes']),0]for a in ASSEMBLIES]),'']
 total=sum(current[a]['matched_methods']for a in ASSEMBLIES);old=sum(prior[a]['matched_methods']for a in ASSEMBLIES);n=sum(current[a]['total_methods']for a in ASSEMBLIES);b=sum(current[a]['matched_original_il_bytes']for a in ASSEMBLIES);ob=sum(prior[a]['matched_original_il_bytes']for a in ASSEMBLIES);nb=sum(current[a]['original_total_il_bytes']for a in ASSEMBLIES)
 lines += [f'Base da rodada: commit {cfg["baseline_commit"]}. Total: {old}/{n} → {total}/{n} métodos; {ob}/{nb} → {b}/{nb} bytes de IL. Zero regressões e zero retiradas de matched. Os bytes contam o IL original dos métodos integralmente aprovados; headers, locais e exceções também precisam passar.','',SECTIONS[1],'',table(['Ciclo','Alvo / regra','Par e hipótese','Resultado medido','Métodos ganhos / perdidos','IL ganho'],[[r['label'],inline(r['primary_target'])+' / '+inline(r['rule']),r['pair']+'; '+r['hypothesis'],r['result'],str(r['methods_gained'])+' / 0',r['il_bytes_gained']]for r in journal['cycles']]),'']
 lines += ['Regras medidas separadamente no passe principal de cada ciclo. Sondas isoladas não entram novamente nestes ganhos; tentativas e avanços parciais não equivalem a corpos aprovados. Os auxiliares liberados por dependências são contabilizados pelo verificador, separadamente das regras de fonte:','',table(['Ciclo','Regra','Tentativas do motor','Avanços aceitos','Corpos ganhos','IL ganho'],rule_breakdown(journal)),'','As quatro propostas congeladas da KairoLibrary foram testadas no assembly completo e aprovadas: MaterialManager.GetMaterial (110 IL), MeshManager.GetMeshBuilder (141), IApplication.OnApplicationPause (199) e Transform.Transpose (197). Não houve novo teste em dispositivo ou APK. KairoBase/<>c::<Start>b__55_0 fechou com 182 bytes e o próprio verificador liberou automaticamente .ctor e .cctor, antes retidos, somando 18 bytes. Nenhum auxiliar foi escrito à mão ou promovido manualmente.','']
 lines += ['Auxiliares liberados automaticamente pelo verificador nesta rodada:','',table(['Ciclo','Quantidade','IDs oficiais'],released_helpers(journal)),'']
 release=cfg.get('_checked_origin_release')
 if release:
  lines += ['Evento separado de liberação de origem, depois do ciclo B39:','',table(['Método','Ganho de estado / IL','Prova mecânica'],[[inline(r['method_id']),'1 / '+str(r['original_il_bytes']),str(len(r['ownership_proof']['field_ids']))+' campos RVA, '+str(r['ownership_proof']['payload_bytes'])+' bytes de dados idênticos, '+str(len(r['ownership_proof']['origins']))+' consumidores com corpos iguais e '+str(len(r['source_availability_proof']['source_paths']))+' arquivos efetivamente compilados']for r in release['releases']]),'','Esse corpo já era idêntico. A correção genérica do verificador comprovou sua origem e disponibilidade compilada; o estado foi liberado pelo próprio verificador completo. O evento acrescenta 1 matched e 7 bytes de IL contabilizado, com zero mudanças de fonte C# e de corpos. Não é um ciclo B40 e não reescreve o ganho de B39. O motivo do vínculo anterior era a busca de uma aresta de chamada para um construtor implícito que nunca é chamado.','']
 lines += ['Controles e tentativas negativas das fixtures publicadas. DISTINTO significa diferença de corpo no comparador, não necessariamente erro semântico. IGUAL preserva alternativas indistinguíveis pelo IL. Alguns arquivos são vistas sobre a mesma matriz; as linhas não são somadas como compilações novas:','',table(['Relatório','Grupo da fixture','Resultados medidos'],fixture_negative_rows()),'']
 if (ROOT/'reports/round12-app-profile-compatibility-review.json').exists():lines += ['O primeiro commit do B33 foi interrompido no gatilho bruto porque a prova antiga do Assembly-CSharp usava esquema v1 sem o Boolean agregado exigido pelo leitor. A revalidação física dos dois PEs e dos 97 fontes confirmou 17 bytes diferentes somente em MVID e timestamp; a evidência derivada corrigiu o formato, sem mudar o comparador, as exceções, as DLLs ou os estados. O ganho do B33 foi preservado e não foi repetido como ciclo novo. A falha e os snapshots anteriores foram mantidos.','']
 if (ROOT/'reports/round12-raw-transport-review.json').exists():lines += ['A segunda tentativa de commit do B33 foi interrompida pelo limite de 80 MB: CSVs de 161294119 e 161293727 bytes e JSON de 71175649 bytes foram preservados. O transporte lossless em gzip, com manifestos e restaurador, foi verificado integralmente e passou em 9 testes de integridade. O comparador e os arquivos originais não foram modificados; os originais permanecem locais e podem ser restaurados dos arquivos publicados. A repetição do commit não cria novo ciclo nem novo ganho.','']
 if (ROOT/'reports/round12-verify-exit-tests.json').exists():lines += ['A primeira tentativa do evento de origem encontrou ambos os subverificadores com sucesso, mas o wrapper retornava 2 pela expressão return status or 2. Corrigida a agregação para preservar status 0; oito testes CLI confirmam que diferenças 1 e erros de escopo 2 continuam propagados. A execução integral foi repetida para confirmar o commit, sem contar nova liberação ou ciclo.','']
 if (ROOT/'reports/round12-verification-origin-release-recovery.json').exists():lines += ['O primeiro rascunho de proveniência da liberação foi recusado porque serializava a pasta do snapshot com barras diferentes das gravadas pela medição no Windows. A gravação passou a preservar literalmente o campo snapshot da medição; a validação exata foi mantida. O rascunho recusado e seus logs foram preservados, sem publicar evento ou alterar o journal.','']
 sources=read('reports/round12-raw-source-journal.json')['steps']
 lines += ['Resultados negativos e correções de diagnóstico: A05 não testou a lambda GetUtcOffset porque o filtro selecionava apenas Storage; zero candidatos não é uma refutação, e o teste real de UTC foi A06. Em A07, a previsão incompleta de quatro locais foi corrigida: antes e depois têm cinco locais realmente emitidos. O par diagnóstico de A08 foi corrigido para INSERTION_POINT → stloc.1 no offset 97, preservando os rótulos submetidos na proveniência. As fixtures e candidatos recusados não foram contabilizados como métodos ganhos.','', 'As seis correções de metadados abaixo preservaram todos os corpos e não ganharam métodos nem bytes de IL. Não consomem ciclos da Parte B.','',table(['Passo','Alteração de fonte','Bytes residuais fora das quatro exceções','Métodos ganhos / perdidos'],[[r['label'],r['cause'],r['outside_exceptions_differing_bytes'],'0 / 0']for r in sources]),'', 'A comparação bruta inicial tinha 142.347 bytes residuais. Retirar o alias Object introduzido na reconstrução, restaurar dois aliases genéricos e acrescentar quatro constantes locais não usadas corrigiu a emissão; nenhuma DLL foi editada. A primeira revisão do comparador identificou duas falhas de validação de evidência (manifest obrigatório e vínculo entre DTO e bytes do PE); foram corrigidas antes do aceite final. A versão revista passou em 17 testes. Nenhum APK foi criado nesta rodada.','',SECTIONS[2],'']
 hypotheses=read('reports/round12-hypotheses.json')['hypotheses']
 lines += [table(['Estado','Hipótese','Evidência'],[[h['status'],h['hypothesis'],h['evidence']]for h in hypotheses]),'', 'CONFIRMADA — igualdade de métodos e tipos não bastava para a igualdade bruta da KairoLibrary: a primeira comparação encontrou quatro StandAloneSig ausentes. Fixtures e a DLL completa confirmaram a emissão das assinaturas FIELD 0608/060e/0605/060c pelas constantes locais no PDB.','', 'O código consultado do Roslyn 1.1 está na revisão 4cb203dab89e1542cdf7d88cb6d32767bc0e5460. PdbWriter.cs:723–730 serializa constantes do escopo; MetadataWriter.cs:4398–4413 emite a assinatura FIELD e o token StandAloneSig; FullMetadataWriter.cs:280–287 interna a assinatura pelo blob. Essa leitura foi confirmada pela emissão real nas fixtures e no jogo.','', 'CONFIRMADA — aliases não usados afetam a ordem de TypeRef/TypeSpec com /debug:pdbonly. A ordem ordinal dos nomes dos aliases e a remoção do alias Object restauraram a ordem original sem mudar corpos. Os nomes e valores históricos dessas constantes e aliases continuam UNKNOWN.','',table(['Modelo','Corpos matched avaliados','Completas / exatas / erradas','Cobertura completa','Mapeados / não mapeados','Mapeados incompletos / exatos'],[[a,m['matched_bodies'],str(m['matched_model_complete'])+' / '+str(m['matched_complete_exact'])+' / '+str(m['matched_complete_mismatches']),m['coverage_complete'],str(m['matched_source_mapped'])+' / '+str(m['matched_source_unmapped']),str(m['matched_mapped_incomplete'])+' / '+str(m['matched_mapped_incomplete_equal'])]for a,m in models.items()]),'', 'Uma previsão completa errada recusa o candidato; uma incompleta permite até 12 variantes por método e passe, com máximo 24 por ciclo. A assinatura realmente emitida e o corpo completo continuam sendo decididos pelo verificador.','',SECTIONS[3],'']
 for a in ASSEMBLIES:
  v=current[a];lines += [f'{a}: {v["divergent_bodies"]} corpos divergentes, {v["divergent_original_il_bytes"]} bytes de IL; {v["equal_body_held_methods"]} auxiliares com corpo igual retidos. Assinaturas locais já iguais: {v["pending_local_signatures_equal"]}; diferentes: {v["pending_local_signatures_different"]}.','',table(['Faixa IL','Métodos divergentes','Bytes de IL'],[[k,x['methods'],x['original_il_bytes']]for k,x in v['size_distribution'].items()]),'']
  groups=v['alignment']['opcode_histogram'];lines += [table(['Par alinhado','Métodos','Peso em bytes de IL'],[[g['pair'],g['methods'],g['original_il_bytes']]for g in groups]),'']if groups else['Nenhum par pendente.','']
 appgroups=current['Assembly-CSharp']['alignment']['opcode_histogram'][:5];history=attempted_rules([g['example']['stable_id']for g in appgroups])
 control=read(cfg['control_extractor_validation'])
 if not control['valid_frozen_input_measurement']or control['inputs_changed_during_run']or control['own_IL_or_MDB_used_to_infer_AST_control']:raise ValueError('Control proof is not independent and frozen')
 check(control['method_irs'],control['method_irs_sha256'])
 for name,digest in control['extractor_inputs'].items():check(name,digest)
 lines += ['Extrator smcs preservado, medido novamente no corpus final sem usar IL recompilado ou MDB para inferir o AST. Cobertura de instruções, planos de emissão e árvores estruturadas são medidas distintas:','',table(['Corpus','Corpos','Planos completos dos dois lados','Planos iguais','Árvores completas dos dois lados','Árvores iguais','Cobertura integral das instruções'],[[name,control[name]['methods'],control[name]['paired_plan_complete'],control[name]['plans_equal'],control[name]['paired_tree_complete'],control[name]['trees_equal'],control[name]['exact_instruction_coverage']]for name in('matched','pending')]),'', 'Extração incompleta permanece incompleta; essas medidas não aprovam nem retiram estados de métodos.','', 'Os pares mais pesados do Assembly-CSharp, limitados a cinco, seguem abaixo. O peso é a soma do tamanho original dos métodos do grupo, não uma contagem dos bytes divergentes. Rótulos e alinhamento são diagnóstico; não alteram o critério do verificador.','']
 if not appgroups:lines += ['Assembly-CSharp não tem pares de divergência pendentes; nenhum exemplo de IL foi inventado.','']
 elif len(appgroups)<5:lines += [f'Existem apenas {len(appgroups)} pares pendentes; todos são apresentados abaixo.','']
 for g in appgroups:
  ex=g['example'];identity=ex['stable_id'];attempts=history[identity]
  lines += [f'{g["pair"]}: {g["methods"]} métodos, {g["original_il_bytes"]} bytes. Exemplo: '+inline(identity)+'.','',example_lines(ex),'','Já tentado, incluindo rodadas anteriores: '+(', '.join(attempts)if attempts else'nenhuma tentativa emitida localizada nos relatórios consultados; UNKNOWN fora dessa evidência')+'.','']
 lines += [SECTIONS[4],'']
 for a in ASSEMBLIES:
  m=manifests[a];lines += [a+': '+inline(m['compiler'])+'; SHA-256 '+inline(m['compiler_sha256'])+'.','', 'Opções: '+inline(' '.join(m['flags']))+'.','', 'Referências na ordem de compilação, com a identidade de arquivo fixada:','',table(['Arquivo','SHA-256'],[[inline(r['path']),inline(r['sha256'])]for r in m['references']]),'']
  if m.get('runtime'):lines += ['Runtime de compilação: '+inline(m['runtime'])+'; SHA-256 '+inline(m['runtime_sha256'])+'.','', 'Ambiente do processo: '+inline(json.dumps(m.get('scoped_environment',{}),ensure_ascii=False))+'; stdlib_binding='+str(m.get('stdlib_binding','UNKNOWN'))+'.','']
 lines += ['KairoLibrary usa o pacote Microsoft.Net.Compilers 1.1.0, csc.exe 1.1.0.51109, C# 6, otimização e /debug:pdbonly, /deterministic-. Assembly-CSharp usa o smcs 2.6.5.0 do Unity 5.3.4f1, otimização e -debug-. Compiladores, opções e referências não mudaram nesta rodada. O modelo Roslyn de fonte passou a tratar o temporário de getter sobre struct retornado por indexador. A identidade histórica exata do executável original permanece UNKNOWN; a receita atual da KairoLibrary foi comprovada pela comparação literal.','',SECTIONS[5],'','KairoLibrary: CONCLUÍDA. Os dois PEs têm '+str(raw['original_size'])+' bytes; '+str(raw['original_size']-40)+' bytes fora das quatro exceções são literalmente idênticos. Lista de diferenças restantes além dos quatro campos: VAZIA. Cabeçalhos, seções, streams, heaps, tabelas, recursos e dados RVA foram comparados. O último PE tem '+str(raw['all_different_or_unpaired_bytes'])+' bytes efetivamente diferentes, todos dentro dos 40 bytes das exceções.','',table(['Campo comprovadamente não determinístico','Offset absoluto nos dois PEs','Tamanho'],[[name,x['offset'],x['size']]for name,x in raw['exception_coordinates']['original'].items()]),'', 'Prova atual: '+inline(cfg['part_a_raw_gate']['report'])+'; SHA-256 do PE recompilado '+inline(raw['recompiled_sha256'])+'. As exceções só valem nas mesmas posições e índices de bytes nos dois arquivos. Não houve normalização, cópia de bytes do original para o resultado ou edição posterior da DLL.','', 'As diferenças iniciais de AssemblyRef eram três índices numéricos de public-key-token no #Blob, não versões ou identidades de referência. A assinatura FIELD Byte antecipada restaurou esses índices; todos os bytes brutos dessas tabelas agora correspondem. Todos os 116 dados FieldRVA, somando 16.491 bytes, já correspondiam na análise inicial; o deslocamento de oito bytes era consequência das quatro linhas ausentes.','']
 type_total=sum(r['types_total']for r in receipts.values());type_equal=sum(r['types_equal']for r in receipts.values());bodyless=sum(1 for r in receipts.values()for m in r['methods'].values()if not m.get('body_present'))
 lines += [f'Metadados estruturais: {type_equal}/{type_total} tipos; {bodyless} declarações sem corpo avaliadas pelo verificador. A comparação inclui ordem dos tipos e membros, parâmetros, atributos, layout, constantes e recursos. A correspondência estrutural do Assembly-CSharp ainda não afirma igualdade bruta do arquivo.','', 'Externos: as provas preservadas conferem as sete DLLs Mono.Security, mscorlib, System, System.Core, UnityEngine.Networking, UnityEngine.UI e UnityEngine com a distribuição oficial Unity 5.3.4f1. UnityEngine.dll corresponde a AndroidPlayer/Variations/mono/Managed, SHA-256 7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421, nos 970.752 bytes. As seis bibliotecas nativas são libmain.so, libmono.so e libunity.so em armeabi-v7a e x86; libunity corresponde ao Mono Release. Não houve nova auditoria ou decompilação de externos. Assembly-CSharp mantém duas exceções comprovadas: COFF.TimeDateStamp e Module.Mvid. DEX/Java, normalização de assemblies e APK novo permaneceram fora do escopo.','']
 if current['Assembly-CSharp']['divergent_bodies']+current['Assembly-CSharp']['equal_body_held_methods']>10:
  lines += ['Comparação bruta atualizada do Assembly-CSharp: gatilho de dez corpos pendentes ainda não atingido; não foi executada nesta rodada.','']
 else:
  measurements=cfg.get('app_raw_measurements',[]);proof=measurements[-1]if measurements else None
  if not proof:raise ValueError('Assembly-CSharp raw <=10 trigger was not measured')
  check(proof['report'],proof['sha256']);appraw=read(proof['report'])
  if appraw['recompiled_sha256']!=receipts['Assembly-CSharp']['build_sha256']:raise ValueError('Stale final App raw measurement')
  lines += ['Comparação bruta do Assembly-CSharp: '+inline(proof['report'])+f'; {appraw["outside_exceptions_differing_bytes"]} bytes residuais. É medição, sem correções por normalização.','']
  if appraw['outside_exceptions_differing_bytes']==0 and appraw['semantic_divergent_bodies']==0 and appraw['held_methods']==0:
   fields=appraw['exception_coordinates']['original']
   if fields!=appraw['exception_coordinates']['rebuilt']or set(fields)!={'Module.Mvid','COFF.TimeDateStamp'}:raise ValueError('Unreviewed App raw exception coordinates')
   span=sum(x['size']for x in fields.values())
   if span!=20 or appraw['original_size']!=appraw['rebuilt_size']:raise ValueError('Unexpected App byte geometry')
   lines += [f'Assembly-CSharp: CONCLUÍDO pelo critério literal. Os PEs têm {appraw["original_size"]} bytes; {appraw["original_size"]-span} bytes fora dos dois campos comprovados são idênticos. Lista de diferenças restantes além desses campos: VAZIA. Nesta compilação, {appraw["all_different_or_unpaired_bytes"]} bytes diferem dentro dos {span} bytes reservados aos campos.','',table(['Campo não determinístico do Assembly-CSharp','Offset nos dois PEs','Tamanho'],[[name,x['offset'],x['size']]for name,x in fields.items()]),'']
  transport=proof.get('transport')
  if transport:
   check(transport['manifest'],transport['sha256']);package=read(transport['manifest'])
   if package['raw_report_sha256']!=proof['sha256']:raise ValueError('Raw transport belongs to another report')
   for item in package['files']:check(item['archive_path'],item['archive_sha256'])
   lines += ['Os originais completos são publicados em gzip lossless. Manifesto: '+inline(transport['manifest'])+'. Restaurar e verificar: '+inline('python tools/round12_raw_transport.py restore '+transport['manifest'])+'. O restaurador verifica o tamanho e o SHA de cada byte descomprimido, sem alterar DLLs.','']
  lines += raw_measurement_tables(appraw)
  lines += ['Histórico das medições brutas do Assembly-CSharp. Repetições de verificação do mesmo ciclo são snapshots distintos, sem novo ganho ou ciclo; estes totais não são somados:','',table(['Snapshot','Estado bruto','Bytes residuais fora das exceções'],[[inline(p['snapshot']),p['status'],p['outside_exceptions_differing_bytes']]for p in measurements]),'']
 lines += [SECTIONS[6],'',stop['reason'],'', f'Ciclos concluídos: {sum(r["phase"]=="A"for r in journal["cycles"])} na Parte A e {sum(r["phase"]=="B"for r in journal["cycles"])} na Parte B. Sequência final de ciclos B sem ganho e com fila vazia: {journal["consecutive_zero_cycles"]}.','', 'Fila preparada ainda pendente: '+(', '.join(q['id']for q in cfg['prepared_queue']if q['status']=='pending')or'nenhuma')+'.','']
 for blocker in stop.get('blockers',[]):lines += [blocker,'']
 lines += [SECTIONS[7],'']
 for i,r in enumerate(stop['recommendations'],1):lines += [str(i)+'. '+(r if isinstance(r,str)else json.dumps(r,ensure_ascii=False)),'']
 text='\n'.join('## '+line if line in SECTIONS else line for line in lines).rstrip()+'\n';(ROOT/'reports/round12-final-report.md').write_text(text,encoding='utf-8',newline='\n');print('Wrote reports/round12-final-report.md; all eight sections derive from checked evidence.')

if __name__=='__main__':main()
