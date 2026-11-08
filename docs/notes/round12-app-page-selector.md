# R12 — três métodos do par stloc.2 → stloc.s

Preparação privada finita: **zero compilações de alvo, verificador, edição pública ou estado**. Todos os resultados do jogo continuam **UNKNOWN**. Base imutável B6: `build/round12/runs/b06/commit/postverify01/Assembly-CSharp`, commit `f873747`.

IDs/tokens/tamanhos conferidos no ledger somente por leitura e nos dumps originais:

| ID estável (assembly Assembly-CSharp) | Token | IL original |
|---|---|---:|
| `game.TPlayer::CollisionSaving(game.TBall)->System.Void` |0x06000398|584|
| `surface.GameView::OnTouchDlgPage(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void` |0x06000565|286|
| `surface.GameView::OnTouchDlgPage2(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void` |0x06000566|393|

Total **1.263IL**. `ledger-read.json` preserva as três linhas observadas e o SHA do CSV, sem alterá-lo.

## 1. TPlayer.CollisionSaving

Original: fat12/maxstack46/17locais/EH0/584IL. Atual: fat12/maxstack46/13locais/EH0/594IL. Primeiro par alinhado em140: seletor Random vai para slot2 original, mas para temporário de switch12 atual. O primeiro ramo original testa2 e o segundo1; o switch atual compara1 antes de2.

Original: duas arrays0/1; seletor2; ramo1 angle3, **Single4/5sem uso**, speed6, sample7, distance8, time9; default angle10, **Single11/12sem uso**, speed13, sample14, distance15, time16. Os quatro slots reservados não têm load, store ou endereço em **nenhum** ponto do corpo original. Não se atribuem nomes/valores históricos a eles.

Proposta conjunta:

```csharp
int recoveredSavingChoice = GameUtil.Random(3);
if (recoveredSavingChoice == 2) { /* folhas do case2 */ }
else if (recoveredSavingChoice == 1) {
    int angle2 = /* inicializador existente */;
    float recoveredUnusedX2, recoveredUnusedY2;
    float num2 = /* inicializador existente */;
    // demais folhas existentes
} else {
    int angle = /* inicializador existente */;
    float recoveredUnusedX, recoveredUnusedY;
    float num = /* inicializador existente */;
    // demais folhas existentes
}
```

Random(3) é chamado **uma vez**, no mesmo ponto lógico do antigo seletor. Os três `break` terminais pertenciam exclusivamente a esse switch e são retirados; nenhuma guarda, comparação de ponto flutuante, cálculo, chamada ou inicialização de folha muda. Os nomes novos dos Single são escolhas de reconstrução; sua origem como declaração nomeada versus outro artefato não é provada. A existência/tipo/ausência de usos é fato, e a declaração sem uso é um molde suficiente no smcs real.

A realocação abstrata resulta em **227objetos de instrução integralmente iguais,584IL,zero hunks**. O mapa dos locais vivos e os quatro slots vazios estão em `saving/slot-constraints.json`.

## 2. GameView.OnTouchDlgPage

Original: fat12/maxstack19/3locais/EH0/286IL. Atual: fat12/maxstack19/7locais/EH0/299IL. Original tem TouchComponent0, **TouchEffect1 compartilhado** e o temporário do switch externo Int32slot2. Atual tem quatro efeitos distintos1–4 e temporários5/6 para os switches externo/interno.

O IL original **não guarda v em temporário nem contém a tabela do switch interno**. Ele testa diretamente o argumento, em ordem16,18,17,19. Os AddEffect e stores de vx/vy são os mesmos, todos usando effect1.

Proposta: `TouchEffect touchEffect;` antes de `switch(type)`; retirar os quatro tipos das atribuições AddEffect; converter **somente `switch(v)`** em `if(v==16)...else if(v==18)...else if(v==17)...else if(v==19)...`. O switch externo permanece. Os seus donos/breaks não são removidos; somente os quatro breaks terminais do switch interno deixam de existir, com as saídas da cadeia emitidas pelo compilador. Não há comando/interação entre os testes puros de v que possa alterar esse argumento por valor.

O cálculo abstrato reproduz **109objetos e286IL** integralmente. Não é reutilização do mesmo temporário para os dois switches: o switch interno foi escrito como cadeia sem temporário.

## 3. GameView.OnTouchDlgPage2

Original: fat12/maxstack27/3locais/EH0/393IL. Atual: fat12/maxstack27/7locais/EH0/409IL. Requer a mesma declaração compartilhada/cadeia interna acima, **mais a retirada da seção vazia externa** com labels3/4/7/8/9 e seu break.

Fato do IL: a tabela externa original cobre1–10; os buracos3/4/7/8/9 e o default vão diretamente para392, que é `ret`. A tabela atual aponta esses cinco valores para403 (`br408`) e default para408 (`ret`). A seção vazia explicitamente recuperada acrescenta esse br de5bytes. Retirar somente essa seção mantém os buracos da tabela e faz o compilador apontá-los ao mesmo default/ret. Não se apaga caso com folhas, nem se inventa ação para esses valores.

O cálculo abstrato com esse componente resulta em **128objetos e393IL**, inclusive todos os destinos da tabela e das transferências existentes. O negativo que mantém os labels vazios tem398IL, três locais corretos, mas o corpo continua diferente.

## Emissão smcs e modelos

Fonte Mono fixado `b6587926c0f146c8b12985744f33a13b39a22d57`:

- `analysis/tools/round5-mono-source/mcs/mcs/statement.cs:1550–1555,2032–2047`: variáveis em ListDictionary; alocação dos locais do bloco, temporários registrados e filhos. Explica o efeito compartilhado antes do temporário externo e os espaços Single nos dois ramos de Saving.
- `statement.cs:1225–1239`: `LocalInfo.ResolveVariable` declara o local não içado sem condicionar a declaração a uso. A retenção de Single não usados foi medida nas fixtures.
- `statement.cs:3823–3843`: switch não constante cria `LocalTemporary` e guarda a expressão. O seletor nomeado de Saving mais ifs evita esse temporário emitido tarde.
- `statement.cs:3385–3572`: `TableSwitchEmit` emite dispatch e os corpos das seções; `ss.Block.Emit` ocorre em3572. `Switch.DoEmit` só libera o temporário **depois**, em3865–3866. Portanto dois switches aninhados não podem simplesmente reutilizar o temporário externo ainda ativo. Converter o interno é necessário nesta hipótese.
- `analysis/tools/round5-mono-source/mcs/mcs/assign.cs:271–277`: `LocalTemporary.Store` pede `GetTemporaryLocal` ao emitir o store;203–206: liberação.
- `analysis/tools/round5-mono-source/mcs/mcs/codegen.cs:357–372,375–390`: reutilização por tipo de temporários livres, senão DeclareLocal. Não é um algoritmo de hash de nomes de locais.
- `statement.cs:225–267`: emissão de if/else e saídas da cadeia;1117–1120: break para o LoopEnd atual. Os produtores recusam corpos de case com outro dono aninhado.

Esses fontes fundamentam a hipótese de emissão, mas a coincidência integral com o código interno Unity é UNKNOWN. Experimentos usam o smcs instalado e fixado.

NoEmit11 genuíno, auxiliar já existente: Saving atualcomplete13tipos, candidato **complete17tipos exatamente originais**; Page e Page2 atualcomplete7tipos, candidatos **complete3tipos exatamente originais**. Nenhum modelo foi alterado ou incompletude forçada. A identidade dos usos/slots é conferida pelos anchors e constraints, além da assinatura de tipos. Header/locais/IL/EH reais continuam dependentes do alvo.

## Evidência REA e bindings

Mesmo original somente leitura, SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`.

| Método | Evidência REA | SHA do IL |
|---|---|---|
|Saving|`ev_d09ca6b812da4abd5cf4619bb08e9878c48ba143c9ef81d915e50b0e2dd7b9b5`|`8f9eedf3fe384109e567b401d067799125691dc4d6537f263f37b1e18eea8086`|
|Page|`ev_00ee781ff6f6a8d33f160c0b999c1d107e78e137e4eeb1aaf6722b52579aa93e`|`a199f7f1a18a225ddb2c9ae5fcf8f391fd737b2c95429ab9b71e2b4b1d153e4e`|
|Page2|`ev_2cde847e1df918905617eacfcb00bd83f9b050bbf413e06fc1cb51bc4468e227`|`866675092a053bab57fa973ed992d470e45c44f0a083a192d433edfd4590da05`|

SHA conferido sobre exatamente o IL depois do fat header12, com o tamanho de cada método. Não inclui cabeçalho/padding. Cada `bindings.json` registra token, arquivo, método antes/depois, SHA e UTF16 do snapshotB6. O `before` foi confirmado único no Main na preparação. Os dois patches GameView são independentes no mesmo arquivo: o operador deve rebasear cada método de forma única após adotar o anterior e renovar as provas; não exigir o SHA inteiro antigo depois dessa adoção.

## Quatorze fixtures próprias e negativos

Somente métodos Run de classes stub, nunca assemblies do jogo. Mesmas flags/referências B6, compiler SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`. `-out` privado e `-debug-`; nenhum sidecar de produção. As DLLs próprias só foram inspecionadas, não executadas.

- Saving:5compilações; candidato584IL/17locais = controle manual independente em header/locais/IL/EH. Antes594IL/13locais difere. Retirar os quatro Single mantém584IL, mas deixa13locais e operandos diferentes; negativo recusado. Os slots4/5/11/12 do controle e candidato têm zero usos. Trocar o seletor2 por0 também difere.
- Page:5compilações; candidato286IL/3locais = controle integral. Antes299IL/7locais, só dispatch direto292IL/6locais e só compartilhamento291IL/4locais diferem. A composição é necessária.
- Page2:4compilações; candidato393IL/3locais = controle integral. Antes409IL/7locais e componente com holes mantidos398IL/3locais diferem.

Quatro controles textuais do produtor, sem emissão adicional, recusam velocidade alterada, break de dono aninhado, grupo de holes ausente/forma já transformada e nome do seletor em colisão. As fixtures provam os mecanismos, não o fechamento real dos três alvos. A igualdade de instruções abstratas não escreve bytes e não é oráculo.

## Histórico negativo preservado

Saving:82recibos/72registros de trials, sem avanço. Quatro `reserve-unused-original-slot` R5 já compilaram e foram recusados prefix113: cada um inseria **um** Single antes da guarda, mantendo o switch e seu temporário tarde. A proposta entrega os quatro nas posições de dois blocos filhos e o seletor nomeado com cadeia2/1/default; não repete essas reservas isoladas. Variantes antigas de ordem/escopo, guardas e condições permanecem registradas.

Page:81recibos/5trials. R4 `switch-if` interno compilou isolado, mas ficouprefix8. Quatro hoists falharam compilação: mantinham declarações homônimas das outras seções e alguns duplicavam a raiz. A nova forma elimina **todas as quatro** declarações de efeito antes de compartilhar, além de converter o switch interno; não reapresenta apenas o dispatch isolado.

Page2:81recibos/8trials. Mesmo `switch-if` isolado prefix8 e quatro hoists inválidos, mais condições/polaridade sem ganho. O pacote inclui ainda a remoção comprovada dos holes que geravam br extra. Cada recibo está vinculado por SHA. Registros podem incluir sondas repetidas, não inferidos como compilações únicas. Sem payload/SHA, ausência universal de tentativa permanece UNKNOWN; zero candidatos não refuta construção.

## Interfaces e limite

`calculate.py` contém três APIs puras, sem classe/cabeçalho/I/O:

- `transform_saving(method_source)->str`.
- `transform_page(method_source)->str`.
- `transform_page2(method_source)->str`.

Cada subpasta tem `patches.json` próprio, método normal completo antes/depois, modelos/constraints/REA/histórico e fixtures. Todas as folhas/chamadas/strings permanecem na mesma ordem; valores e operações de ponto flutuante não mudam. Nenhuma classe gerada ou byte original é escrito no resultado.

Estado dos três: **READY_FOR_GAME_PROBE**. Ganhos em métodos/bytes **UNKNOWN** até compilação e verificação oficial sem regressões pelo operador exclusivo. Pacotes anteriores intactos; nenhum outro alvo foi aberto.
