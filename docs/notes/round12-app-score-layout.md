# R12 — par stloc.s → stloc.0, dois casos diferentes

Preparação privada finita, dois pacotes independentes. **Zero builds de alvo, verificador, alteração pública ou estado. Resultado dos dois métodos UNKNOWN.** Base oficial B2: `build/round12/runs/b02/commit/postverify01/Assembly-CSharp`; Main e snapshot dos arquivos foram iguais byte a byte no empacotamento. Não se abriu outra família.

## SoccerForm.UpdateOffscreenTopBar — 1.091 IL

Método `form.SoccerForm::UpdateOffscreenTopBar(kairo.unity.ui.Graphics,System.Boolean)->System.Void`, original0x060001ca. Original: fat12/maxstack58/11locais/EH0/1.091IL. Atual: fat12/maxstack58/12locais/EH0/1.054IL. A primeira diferença real está em165: original `stloc.s5`, atual `stloc.0`, que guarda o segundo TeamData.

Os primeiros quatro slots originais são Int32 sem inicialização antecipada, correspondendo às âncoras dos quatro segmentos desenhados. Os usos identificam **ordem distinta da ordem das atribuições**:

- slot0: store251, load425 — âncora `num2`.
- slot1: store388, load490 — âncora `num5`.
- slot2: store283, load578 — âncora `num3`.
- slot3: store350, load607 — âncora `num4`.

Depois vêm TeamData4/5, GameView6, topBarOffN7 e cursor num8. Os dois últimos slots9/10 são x/y compartilhados pelos dois blocos de placar: ambos são inicializados em cadeia (`ldc.i4.0; dup; stloc.s10; stloc.s9` em628–632); x recebe os valores `/2-13` e `/2+13`, y recebe4 em cada bloco. O atual tem apenas o y inicializado e dois x distintos locais a esses blocos.

Composição calculada, preservando todas as folhas/inicializadores em seus pontos:

1. Declarar `int num2, num5, num3, num4;` antes do primeiro TeamData, retirando somente os tipos das quatro atribuições posteriores.
2. Desfazer o grupo recuperado que declara teamData2/instance/topBarOffN/num antes da inicialização de teamData; declarar cada um no seu inicializador já existente, na sequência TeamData4/5, GameView6, Int32 top7/num8. Não se altera nem duplica chamada.
3. No ponto de `int num6 = 0;`, declarar `int x, num6; x = num6 = 0;`. Retirar `int` dos dois x posteriores. Isso recupera a cadeia observada e o mesmo slot x9 nos dois blocos.

Mapa atual→original `{0:5,1:6,2:7,3:8,4:4,5:0,6:2,7:3,8:1,9:10,10:9,11:9}`. A cadeia acrescenta apenas o zero/store exigido no IL original. A realocação abstrata dos objetos atuais, com as larguras dos opcodes locais ajustadas, resulta em **431instruções,1.091IL,zero objetos diferentes**. É diagnóstico, não emissão/oráculo.

## UserData.DrawGraph_menuElement — 709 IL

Método `game.UserData::DrawGraph_menuElement(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32[],System.Int32[],System.Int32)->System.Void`, original0x0600040c. Original: fat12/maxstack29/12locais Int32/EH0/709IL. Atual: fat12/maxstack29/20locais Int32/EH0/727IL. Primeiro par em1: o índice do loop vazio fica no slot4 original, no slot0 atual.

As quatro coordenadas dos três loops usam os **mesmos slots0–3** no original. Os três índices iniciais são4/5/6. Depois, os resultados de `RateConvert` são7/8, o limite25 é9, o índice do último loop é10 e o índice de espessura é11. Essa identidade é comprovada pelos stores/loads ancorados nas chamadas e arrays, não pela assinatura (todos são Int32).

Composição calculada:

1. `int num, num2, num3, num4;` antes do primeiro loop; retirar os tipos das12 atribuições correspondentes nos três loops.
2. `int x, x2;` imediatamente antes de `int c1max = 25;`, retirando os tipos dos dois resultados RateConvert dentro do último loop. Os resultados continuam sendo calculados no mesmo ponto do corpo. **Não** colocar os seis no primeiro grupo, pois isso deslocaria os três índices iniciais.

Mapa completo atual→original está em `slot-constraints.json`. O cálculo abstrato resulta em **379instruções,709IL,zero objetos diferentes**.

### Tentativa privada retirada R1

A primeira forma calculada incluía somente o primeiro grupo de quatro coordenadas. NoEmit aprovava a sequência de12Int32, mas os anchors do IL mostraram25objetos em slots errados: c1max/l eram7/8 na proposta, enquanto o original precisa de x/x2 nesses slots. A forma foi **retirada antes de qualquer fixture ou build de alvo**, com fonte/modelo/diagnóstico preservados em `graph/attempt01-root-coordinates-only`. Não se declara o modelo incorreto para a assinatura: a assinatura homogênea simplesmente não resolve a identidade dos slots. A composição entregue inclui o segundo grupo tardio e passou pelos anchors e fixtures.

## Fontes primárias e emissão smcs

Assembly original SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`.

- Soccer: REA `ev_d708c543c0be63eb9f16f18a5ad8620c2d16a363a00767ee29fcd5e6eaaf8a0b`, IL SHA `bd4f4b1717a79c83024e6233f65641edf6895c3d5fc2c01f19fcf26cc3e5f799`.
- Graph: REA `ev_eb22086772fc825ca2900c6878a37779824667efd5220364ad91ee222f90e51b`, IL SHA `59034b576140d461d8ca7fa326bf9de27d4a2dc9e0198ee8b399042b93abdefe`.

Hashes conferidos sobre exatamente o IL após o fat header12 no dump congelado; não confundidos com SHA do assembly/cabeçalho.

Código Mono revisão `b6587926c0f146c8b12985744f33a13b39a22d57`:

- `analysis/tools/round5-mono-source/mcs/mcs/statement.cs:1550–1555`: `ListDictionary` das variáveis de bloco;2032–2047: `Block.EmitMeta`, variáveis, temporários e filhos nessa ordem;1225–1239: `LocalInfo.ResolveVariable/DeclareLocal`.
- `analysis/tools/round5-mono-source/mcs/mcs/assign.cs:399–407`: atribuição usada como expressão solicita cópia do resultado.
- `analysis/tools/round5-mono-source/mcs/mcs/expression.cs:4321–4355`: variável não içada emite origem, `dup` quando `leave_copy` e store. Isso explica a cadeia de x/y em Soccer.
- `statement.cs:569–619`: a inicialização do `for` emite no próprio ponto, e a ordem de declaração do slot não exige antecipar sua inicialização. Nenhum incremento/dono de loop é modificado.

O código histórico fundamenta a construção; sua igualdade integral com o fonte interno Unity é UNKNOWN. As fixtures usam o smcs instalado e fixado.

## NoEmit e fixtures próprias

Modelos NoEmit11 genuínos, auxiliar já existente: Soccer atualcomplete12tipos, proposta **complete11tipos exatos**; Graph atualcomplete20tipos, proposta **complete12tipos exatos**. Nenhum modelo foi alterado/incompleto forçado; assinatura real continua obrigatória no build do operador. Blocos/variáveis AST e hashes de fonte/cliente/auxiliar estão preservados. A igualdade de tipos não dispensa os anchors descritos acima.

**10 compilações próprias,5por método**, somente WIP. Compiler SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`; flags/referências fixadas B2, `-out` privado e `-debug-`, sem PDB/MDB de produção.

Soccer: molde anterior298IL, candidato320IL e controle manual independente320IL; candidato/controle batem em header/11locais/IL/EH completos. Negativos com ordem incorreta das âncoras e sem store inicial de x diferem.

Graph: molde anterior727IL, candidato709IL e controle manual709IL; candidato/controle batem em header/12locais/IL/EH completos. Negativos com ordem de coordenadas e com o escopo tardio errado diferem. Este último possui os mesmos12tipos e tamanho709, demonstrando por fixture por que a assinatura homogênea não basta. São métodos próprios em classes stub; não são builds de Assembly-CSharp.

Quatro negativos textuais do produtor recusam x faltante, captura, loop inicial trocado e segundo output ausente, sem compilação adicional.

## Histórico negativo e interfaces

Soccer:77recibos,16registros de trials. R4 adotou reordenação133→165, que produz o grupo hoje recuperado. Os outros trials de hoist/reordenação mantiveram133 ou165; moviam âncoras após a primeira declaração TeamData e em ordem num2/3/4/5, sem o cálculo conjunto de ordem num2/5/3/4 e x/y compartilhados. A proposta atual preserva a sequência de inicializadores e o prefixo anterior, mas corrige declarativamente esse conjunto inteiro. Não repete apenas o hoist isolado recusado.

Graph:76recibos,5trials. Os cinco hoists R4 tiveram **compile_pass=false**, pois mantinham declarações homônimas de coordenadas nas regiões seguintes (o último ainda duplicava uma declaração raiz). A proposta remove todos os12tipos de coordenadas antes de agrupar, e também calcula os dois outputs tardios; não repete payload inválido. Os logs/relatórios negativos estão vinculados por SHA. Contagens de trials podem incluir sondas repetidas e não são inferidas como compilações distintas. Sem payload/SHA, ausência universal de tentativa continua UNKNOWN.

APIs puras em `calculate.py`: `transform_score(method_source)->str` e `transform_graph(method_source)->str`. Cada subpasta possui patch independente, método inteiro antes/depois, UTF16, SHA, REA, modelos, histórico, fixtures e constraints. Todas as chamadas/strings permanecem na mesma ordem; nenhuma classe gerada ou byte original é escrito no resultado. **READY_FOR_GAME_PROBE**, potencial em métodos/bytes UNKNOWN até oráculo e verificação completa sem regressões.
