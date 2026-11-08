# Controles verticais — duas composições independentes

Entrega privada READY_FOR_GAME_PROBE; corpos do jogo e regressões oficiais UNKNOWN. Zero compilações do alvo, verificações oficiais, mudanças públicas ou de ledger. Todos os pacotes anteriores continuam imutáveis. Esta frente cobre somente estes dois métodos.

## Identidade e medidas

Base B13 congelada em `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`. Declarações são `internal void OnTouchVScrollButton(int type,int v,TouchEvent e)` e `internal void OnTouchVScrollBar(int type,int v,TouchEvent e)`, ambas de `surface.GameView`; os IDs completos estão nos bindings por método.

| Método | Token | OriginalIL | AtualIL | Locais original/atual | maxstack original/atual | EH |
|---|---|---:|---:|---:|---:|---:|
|OnTouchVScrollButton|0x06000572|325|312|6/5|41/40|0|
|OnTouchVScrollBar|0x06000571|302|306|9/9|16/15|0|

Total627IL. Cabeçalhos fat12, flags19/initlocals=true. Primeiras diferenças brutas: Button40 e Bar37, operandos dos desvios que antecedem os braços. As primeiras edições causais alinhadas são Button52, INSERTION_POINT→stloc.3, e Bar44, INSERTION_POINT→stloc.s4. As gravações extras são explicadas abaixo; não foram presumidas como cache por causa do nome dos métodos.

REA primário, artefato Assembly-CSharp SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`:

- Button: `ev_7e7a7f3f4c1896ac380956829f1da8aed70b8aae90fb57e00fc59248e3da454e`.
- Bar: `ev_29b60ca662e2029b1fe000af0435f13e26ada3521edf2d3b2c601157d1389c28`.
- Pai comum: `ev_35f83b1f0115135d7753883b610e53198e9213fea77381bb48fb6e659a955436`.

Cada bindings.json contém hash dos bytes de IL conferido contra o dump, artefato, manifest/receipt/dumps, fonte, CRLF, spansUTF16 e SHA do Main observado. O before de cada método ocorreu uma vez no Main; rebase único por corpo e vínculos novos são necessários se outros métodos de GameView.cs forem alterados antes da adoção.

## Parte comum: valor condicional na pilha

Ambos os originais mantêm os dois possíveis valores de num3 na pilha e só depois executam um único stloc no encontro dos braços. Button: true ldelem@51; br52→58; false ldc.i4.m1@57; stloc.3@58. Bar: true ldelem@43; br44→50; false ldc.i4.m1@49; stloc.s4@50. O atual if/else armazena em cada braço, introduzindo um stloc extra.

A proposta restaura `int num3=(2 < option.args.Length)?option.args[2]:-1;` para Button e a forma com `args` para Bar. Mantém a direção 2<Length, ordem de avaliação, leitura condicional do elemento2 e constante−1. Isso remove1byte em Button e2bytes em Bar, mas sozinho não fecha nenhum dos dois. Os fixtures também medem maxstack+1 quando o valor permanece na pilha; não se trata de alterar manualmente o cabeçalho.

## Button: um switch de caso único, não alias nomeado

Além do ternário, Button precisa trocar o `if(type==13)` por `switch(type){case13:corpo;break;}`. Original armazena type no slot5 em76–77, carrega esse temporário em79 e emite beq83→93, seguido de br88→324 para o default. O corpo termina em br319→324, break pertencente ao switch.324 é ret. Esse conjunto (slot extra, dois saltos de dispatch e salto de saída do corpo) é produzido pelo switch, não apenas por copiar o parâmetro.

O while interno permanece igual: condição em291, corpo269 e ramo final blt314→269. Não se move incremento, Set/Get nem mudança de componente, e o novo break está fora do while. Todos os outros locais mantêm seus nomes/ordem; o temporário Int32 do switch é o slot5 após os cinco locais nomeados. NoEmit completo prevê exatamente seis tipos originais.

## Bar: dois guardas positivos, switch existente preservado

O switch(type)8/9 do Bar já é o molde certo e permanece intacto, inclusive o temporário Int32 slot8. Não se adiciona alias ou switch novo. Após o clamp do offset, o original usa `beq166→301` para evitar a região inteira quando Get(v)==num4. Depois de calcular num5 e executar Set(v,...), usa `beq201→301` se num3==−1.301 é o único ret comum dessa região. O atual usa bne/ret local nos dois pontos, acrescentando dois returns intermediários.

A proposta substitui os dois guardas por regiões aninhadas: `if(Get(v)!=num4){ int num5=...; Set(...); if(num3!=-1){ int num6=...; resto; Set(...); } }`. O primeiro guard não envolve o segundo retorno histórico de int.MaxValue, que continua antecipado130. Get e Set permanecem nas mesmas folhas e com o mesmo número/ordem de avaliações. Num5/num6 são declarados no início das respectivas regiões, preservando slots6/7; o temporário do switch permanece8. NoEmit completo prevê os nove tipos originais.

## Emissão fixada

Mono revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, arquivos `analysis/tools/round5-mono-source/mcs/mcs`:

- expression.cs4196–4215, Conditional.Emit: testa o falso, emite true_expr, br end, false_expr e marca o encontro; o store pertence à atribuição externa. Os tipos aqui são Int32, não acionando o tratamento especial de interface4205–4209.
- statement.cs3823–3842, Switch.DoEmit: para expressão não constante cria LocalTemporary, emite a expressão e Store.3469–3476 emite Beq para os poucos valores;3552–3554 adiciona br default.3852/3869 definem/marcam a saída do switch; Break1117–1119 usa esse LoopEnd.
- statement.cs225–267, If.DoEmit: o corpo sem else permite desvio direto ao encontro do bloco. A montagem dos guardas positivos explica os dois beq ao ret comum, em vez de introduzir novos ret.
- statement.cs2032–2047, Block.EmitMeta: locais nomeados e filhos são resolvidos antes dos temporários de emissão. Isso permite que num5/num6 aninhados conservem slots6/7 antes do temporário8.

Os modelos genuínos e seus inputs/helper têm hashes em model.json. Nenhuma previsão completa incorreta foi relaxada ou promovida artificialmente.

## Fixtures e negativos

Dez compilações smcs2.6.5 de fixtures próprias, cinco por método, com referências/flags fixadas, saída privada Fixture.dll e `-debug-`. Controles completos foram escritos manualmente com nomes distintos. Em ambos os grupos, candidato e controle coincidem integralmente em cabeçalho/locais/IL/EH.

| Grupo | Before | Candidato/controle | Só ternário | Só controle |
|---|---:|---:|---:|---:|
|Button|312IL/5loc/max40|325IL/6loc/max41|311IL/5loc/max41|326IL/6loc/max40|
|Bar|306IL/9loc/max15|302IL/9loc/max16|304IL/9loc/max16|304IL/9loc/max15|

Todos os três controles negativos por grupo divergem. Os componentes isolados não devem ser adotados como se fechassem o método. Três negativos do produtor rejeitam tipo diferente no dispatch Button, transferência extra no seu corpo e efeitos adicionais em um guard Bar. Uma primeira tentativa de passar a classe-fixture inteira ao produtor Button foi rejeitada antes de compilar; o driver foi corrigido para usar a API de método isolado. Isso é ajuste de integração, não ensaio de lowering do alvo.

Projeções adicionais comparam offsets/opcodes/destinos/slots/constantes e nomes de folhas das fixtures contra o original:139instruções Button e131 Bar, todas iguais, cabeçalhos e EH iguais. As assinaturas de stubs têm assembly Fixture, diferente de KairoLibrary; essa projeção exclui escopos de assembly dos tipos locais e assinaturas completas de membros. Ela não é o oráculo do alvo. O NoEmit do fonte real confirma independentemente os tipos completos esperados.

## Histórico e limites

Button:91receipts, nove trials incluindo repetições. R4 adotou declaration-ternary-if-reverse,39→40; também tentou a outra forma de ternário, polaridade, condition-local e hoist. R5 repetiu três controles isolados sem avanço40. R7ParteA tentou calculated-parameter-alias, sem adoção. A proposta atual restaura o ternário na orientação lida no original e compõe o switch completo, incluindo slot e saída, em vez de repetir um alias isolado.

Bar:91receipts,26trials. R4 adotou declaration-ternary-if-reverse,36→37; R4–R7 tentaram hoists, agrupamentos, polaridade e condition-local sem novo avanço. R10C04 tentou três root-guard-positive-tail,37→37. Esses guardas isolados continuam negativos; a composição atual restaura antes o ternário e envolve os dois controles correspondentes aos destinos301, mantendo o switch existente. Receipts sem payload/SHA não excluem universalmente toda forma histórica. Não apareceu hash da proposta inteira; zero candidatos não é refutação.

## Entrega

APIs puras em calculate.py: `transform_vscroll_button(method_source)->str` e `transform_vscroll_bar(method_source)->str`. Regras e bundles são independentes por método, pois o controle adicional é diferente. Cada patches.json contém before/after completo e spansUTF16; ready-index.json aponta ambos, hashes e freeze. A adoção e a verificação oficial completa pertencem exclusivamente ao executor. Nenhum método/byte desta frente é contado como ganho antes disso.
