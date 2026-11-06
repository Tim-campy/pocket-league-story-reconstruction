# Rodada 9: ?rvore de controle independente do fonte smcs

A AST ? derivada exclusivamente do C# por Roslyn em modo de an?lise; nenhum assembly do jogo ? emitido pelo extrator e nenhum IL, MDB ou PDB entra na constru??o estrutural. Fonte e refer?ncias s?o lidos do manifesto congelado. IR: smcs-control-tree-v1, side=source; ?rvore, folhas, plano de emiss?o, alvos lexicais e raz?es de incompletude separados. O plano ? calculado pelas regras do Mono Unity pin b6587926c0f146c8b12985744f33a13b39a22d57; o resultado continua diagn?stico e n?o substitui compara??o byte a byte.

## API

`from round9_control_ast import Client` e `with Client(manifest) as client: client.extract(path, source=None, method_id=None)` retornam uma lista de IRs. `tools/Round9ControlAst.cs` ? o helper de an?lise, SHA1f4bed7a4a6603310645f23a20f55251ff93150f7050349dc1d9c8f075dbbf6c. O cliente compila somente esse utilit?rio em cache imut?vel pelo SHA; nunca chama Emit sobre o C# alvo. Serializa??o suporta profundidade2048 para os m?todos grandes.

`emission_plan_complete` e `tree_complete` s?o diferentes: um plano conhecido pode ter uma ?rvore ainda amb?gua. `complete` exige ambos. O comparador compartilhado round9_control_il.canonical_plan mant?m todos os opcodes/transfer?ncias, inclusive br para a instru??o seguinte e largura .s, e s? alia labels sem bytes entre eles/condensa folhas cont?guas. canonical_tree opera sobre a ?rvore, n?o l? o plano.

## Regras lidas

- statement.cs225?264: if testa o ramo falso e emite br de join somente se o bra?o true pode concluir.
- statement.cs322?346,421?461,569?619: do/while/for, entrada, teste e alvo de continue; for ret?m inicializa??o e incremento na ordem de emiss?o. O incremento ? folha de corpo no controle abaixado, n?o descartado.
- statement.cs1117/1148: break/continue usam br ou leave conforme regi?o protegida. Alvo lexical e profundidade s?o preservados.
- statement.cs770?782 e2920?2950: retorno direto, retorno protegido e ret impl?cito no final. A ?rvore cont?m o retorno sint?tico, sem inferi-lo do IL.
- expression.cs3418?3560: polaridade das compara??es; variantes .un de floats preservadas; byte/short usam tipo promovido.
- expression.cs3580?3597: &&/|| em contexto de valor ret?m join br.s; Conditional.Emit4195?4218 ret?m join br longo.
- expression.cs3235?3295,3370?3379: string/delegate equality pode chamar operador; compara??o com null pode ser reduzida. Predicado registra o mesmo opcode na ?rvore e no plano.

Equival?ncias s? s?o usadas quando demonstradas por emissor e fixture integral: while/for de mesma emiss?o, guarda com retorno versus else sem join, condi??o && versus ifs aninhados. Elas n?o apagam transfer?ncias. A posse amb?gua de br entre continue de for e join if/else fica incompleta.

## Unifica??o de identidade para an?lise

DesktopAssemblyIdentityComparer.Default resolve a refer?ncia Microsoft mscorlib2.0.0 da KairoLibrary diante do perfil Unity2.0.5 do consumidor. As refer?ncias e seus bytes de produ??o n?o s?o trocados; CS0012 n?o ? ignorado. A fixture cria um Producer com Roslyn1.1 e Microsoft mscorlib e um Consumer com o smcs/perfil Unity: o comparer Default acusa CS0012, Desktop resolve, e o mesmo fonte do Consumer compila com smcs. O recibo inclui as duas identidades e hashes, al?m da chamada retargetada. Uma tentativa anterior de produzir a biblioteca Microsoft com o pr?prio smcs j? a retargetou para Unity e, portanto, foi rejeitada como fixture de identidade inadequada.

## Valida??o

Fixtures independentes em test_round9_control_ast.py capturam as previs?es AST ANTES de compilar o assembly de teste:34 planos completos/34 iguais/0errados;30 pares de ?rvore completos/30iguais.33 IRs s?o totalmente completos; OR de condi??o tem plano conhecido, mas ?rvore incompleta. A fixture de identidade usa dois assemblies de teste. Nenhum desses builds compila o jogo.

Corpus congelado de prototype07 (recibo do orquestrador):1788 m?todos smcs matched;1422 pares de plano completos/1422iguais;1253 pares de ?rvore completos/1253iguais;0 erros completos restantes. Os n?meros de cobertura incluem somente a parte comprovada, sem completar casos por consulta ao IL original. O source model n?o l? o DTO utilizado na compara??o.

Tentativas negativas foram preservadas: MaxDepth64 interrompeu as primeiras medi??es; string/delegate operator equality e const/local-loca??o expuseram defeitos corrigidos por fixture+fonte do compilador. Chamadas com delegate an?nimo antes ocultas numa folha foram identificadas genericamente e passaram a INCOMPLETE; a emiss?o de cache descrita em anonymous.cs1498?1587 ainda n?o est? modelada aqui. N?o se fez retirada de estado nem altera??o do verificador.

## Limites e pr?xima aplica??o

Construtores com inicializadores, iteradores/gerados n?o mapeados, switch, EH/using/lock, foreach de cole??o e redu??es l?gicas constantes que exigem efeitos laterais t?m raz?es expl?citas de incompletude. Uma ?rvore gigante incompleta n?o ? liberada por ser grande nem por ter plano linear ?ntegro. A reconstru??o de fonte s? poder? usar ?rvores completas e folhas ligadas sem ambiguidade; MDB pr?prio com corpo id?ntico pode mapear intervalos/folhas, nunca definir a estrutura AST. Binder/rewriter ? um componente separado em desenvolvimento; este snapshot n?o promete candidatos de jogo nem ganhos em matched.
