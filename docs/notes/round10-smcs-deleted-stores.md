# Stores ausentes: cinco composições smcs distintas

Snapshot imutável: `build/round10/cycles/cycle15/after/Assembly-CSharp`. A pasta contém apenas análise privada, fontes calculadas, previsões NoEmit e fixtures próprias. **Zero compilações de alvo, nenhuma escrita pública e nenhuma alteração de ledger/estado do jogo.**

Estado final previsto após as checagens de `finalize.py`: `READY_FOR_GAME_PROBE`. Isso significa somente que os controles próprios e a assinatura passaram. Os corpos reais continuam **UNKNOWN_UNTIL_VERIFIER**, com ganho possível **0..5 métodos / 0..4.627 IL**.

## 1. Inventário medido e disambiguador de overloads

|Método|IL original/atual|Locais original/atual|Causa do inventário|
|---|---|---|---|
|Ball.SetShoot(Player,int)|1.863/1.908|27/28|Um temporário Int32 reutilizado por três switches, ausente no original|
|GameManager.Collision()|523/524|13/14|Boolean sintético nomeado, ausente no original|
|Player.Move(Vector3D,int,int,bool)|974/951|11/11|Mesmos locais; stores foram fundidos e guardas incorporadas ao for|
|UserData.Update_gameParam()|819/813|14/14|Mesmos locais; stores por braço e ordem dos braços foram fundidos|
|GameView.AddTouch(Graphics,int,int,int,Seb,int,int,TouchOption)|448/449|10/10|Há um cache Int32 nos dois lados, mas em pontos/donos diferentes|

Todos os cinco baselines foram medidos pelo modelo real: complete=true e assinatura prevista igual à recompilação frozen. A declaração fonte foi selecionada **pelo stable_id completo no modelo**, com offsets UTF16; não pela primeira ocorrência de nome. Isso evita confundir Move com o overload de dois argumentos ou AddTouch com os overloads de retângulo/imagem.

O histórico usa o cabeçalho lexical completo da declaração selecionada, incluindo tipos/nomes dos parâmetros, para localizar o mesmo overload. Nenhum dos 280 vínculos históricos ficou sem mapeamento. Spans, hashes e DTOs estão em `*/analysis.json`, fontes integrais em `*/method.before.cs`.

## 2. Ball.SetShoot: stores por braço, três caches e uma única negação

Original683 e706 são stloc8 nos ramos de num7. A reconstrução tem um único store após o join. A proposta mantém o inicializador num7=0 e usa:

```csharp
if (num8 > rand_cos[1]) num7 = 2;
else if (num8 > rand_cos[0]) num7 = 1;
else num7 = 0;
```

O switch num7 e as duas ocorrências switch(shoot_mode) tornam-se if/else-if na ordem dos testes originais. Os seletores já são local/argumento; nenhum alias é acrescentado. O temporário Int32 atual27 deixa de existir. Os blocos/declarações dos braços e os default são preservados.

Original1700 neg, versus atual1718 ldc.r4(+0) e1725 sub, sustenta trocar **somente** `(0f - shootV0y)` por `(-shootV0y)`. O trecho anterior `0f - num13 / 4f` e também `0f + num13 / 4f` já correspondem e permanecem intocados. Não se aplicou substituição indiscriminada de zero-minus.

Diagnóstico por papéis: 13 trechos de sequência, cobertos por stores num7 (0–1), dispatch num7 (2–4), primeiro shoot_mode (5–6,9), neg (7–8), segundo shoot_mode (10–12). Os retargets685/708 são joins dos stores por braço. Nada mais foi descartado para declarar a composição.

## 3. GameManager.Collision: Boolean recuperado não é temporário original

`recoveredCondition_20510` ocupa o slot8 Boolean atual; o original tem Player nesse slot e **nenhum Boolean em sua assinatura**. Esse símbolo é uma variável introduzida numa rodada anterior, não evidência de alias original.

A composição remove sua declaração e restaura guardas continue nos dois loops de adversários:

```csharp
if (num2 == 0 && l == m) continue;
Player p2 = /* mesmo acesso atual */;
Collision_proc(p, p2);
```

O segundo loop usa num2==1 e l==n. Original165 bne175;170 br207 e259 bne269;264 br301 conservam os destinos dos incrementos correspondentes. Os blocos que contêm p2/p3 são mantidos, sem trocar donos de continue.

Também se expande `num2 = k==0 ? num : 1-num` em atribuições por braço; original103 stloc4 precede o br105. A guarda final com return torna-se `if(state_==0 && gameState_==2 && BitUtil.Check(...)){tail}`. Original421 brfalse522 alcança o ret final; os testes anteriores383/395 têm a mesma saída comum. Não é caso de avaliação de campo/argumento em outra ordem.

Após projetar os nomes atuais aos slots calculados, as 19 diferenças brutas ficam em quatro causas reais: store por braço, primeiro continue/Boolean extra, segundo continue e guarda final. O diagnóstico não aceita corpos nem modifica DTOs.

## 4. Player.Move e UserData.Update_gameParam: toda a árvore de stores importa

**Move:** o primeiro store ausente não se resolve com uma inversão isolada. A seleção de num6 é reconstruída como cadeia explícita de ifs: Cos22→0; Cos67→1/7; Cos112→2/6; Cos157→3/5; else4. Os testes `>=` dos cossenos e `>` das coordenadas float são preservados na forma positiva indicada pelo IL. Original376/431 ble.un e409 blt.un não autorizam trocar por um complemento relacional que altere NaN.

A seleção de num7 também tem quatro stores por braço, usando os testes Int32 temp_direct_s[0]>0 e [1]>0. O for final mantém somente i<Length no cabeçalho e introduz `if(num9!=directBox_[i])break;` no corpo. Original906–922 testa a igualdade e salta para saída973;968 blt906 retoma o teste no corpo. Esse break não é um continue.

Os 13 trechos diagnósticos são: num6 (0–6), num7 (7–10), guarda do for (11–12). Os retargets340/392/804 são os joins das atribuições expandidas.

**Update_gameParam:** a cadeia inteira vira stores por braço, mantendo num2=0 inicial e as quatro chamadas RateConvert com seus argumentos. O then de flag contém if(num>=0), ambos com store; segue else-if(flag2) com limites6/−3 e else com0/−6. O atual usa `!flag2` com ordem dos ramos invertida e store comum. Original202/226/257 e a orientação234 brfalse264 pedem esses stores e essa ordem. São quatro trechos de sequência e um join228.

Não se introduziram aliases artificiais nesses dois métodos; a lista inteira de locais permanece a original. O modelo real confirmou11 e14 tipos exatos.

## 5. AddTouch: trocar o dono do cache, não acrescentar uma variável

Este é o exemplo que impede a regra universal “stloc ausente = alias nomeado”. O original tem slot9 Int32, alocado após todos os nomeados, com store38 e cargas40/48/57 para os testes de **id**. O atual usa slot9 para **num6**, armazenando em268 antes do switch interno.

A proposta troca as duas formas de dispatch em conjunto:

```csharp
if (g != null) {
    switch (id) {
        case 1: case 18: case 26:
            // mesmo bloco e mesmo payload atual
            // num6 selecionado por ifs quando id==18
            break;
    }
}
```

O original61 beq71 seguido66 br366 é o teste final/default desse switch externo, e361 br366 é seu break terminal. O switch interno num6 vira if(num6==3)/else-if(num6==1), na ordem original281–317. Assim há novamente um único cache Int32, agora no ponto correto. Não se tentou hoistar um alias nomeado para forçar o slot.

Também se restaura o ternário num6 `(frame==0||frame==3)?3:1` e separa a atribuição de num5 em if(CheckTouch) com ternário2/3 e else com ternário0/1. A chamada CheckTouch permanece uma vez. Os stores de cada ramo e as cargas de frame seguem a ordem original; não se complementam comparações float das expressões de posição/imagem.

Diagnóstico:15 trechos, cobertos por switch externo/cache (0–3,14), ternário num6 (4–5), if/ternários num5 (6–10), ifs internos num6 (11–13). Os quatro destinos não coincidentes no alinhamento por papéis pertencem aos braços num5. No alinhamento bruto havia um retarget e três destinos diferidos por ranges ausentes; esses contextos estão preservados.

## 6. Modelo real e precisão

São dez consultas NoEmit: cinco baselines e cinco composições completas. Todos os candidatos retornaram **complete=true** e tipos originais exatos:27,13,11,14,10. Nenhuma previsão incompleta foi forçada, nenhum local sem acessos foi acrescentado, nenhuma classe gerada foi escrita.

Produtores e hashes: `stage-results.json`. O manifest privado usa fontes absolutas do snapshot e foi conferido por hash antes das consultas. O cliente e o binário do modelo cached permaneceram intactos. `model-results.json` e `*/model-after.json` são saídas reais do produtor.

`role-diagnostic.json` é somente diagnóstico: resolve papéis de locais e destinos para localizar causas, nunca serve de verificador. O judge do alvo deverá comparar cabeçalho, maxstack, assinatura, bytes, larguras, EH e tokens resolvidos sem essa projeção.

## 7. Fixtures próprias e negativos

`FixtureCurrent.cs` contém cinco moldes próprios pequenos, com stubs de gravação/contadores. Nenhum corpo de classe do jogo foi compilado. Os controles manuais independentes estão em `FixtureControl.methods.cs.txt`, com os mesmos membros compartilhados.

|Toy|IL Current/Control/Candidate|Locais Current→Control/Candidate|maxstack Current→Control/Candidate|
|---|---|---|---|
|Shoot|347/299/299|8→7/7|19→17/17|
|Collision|443/441/441|12→11/11|13→11/11|
|Move|333/347/347|5→5/5|21→12/12|
|GameParam|107/110/110|1→1/1|14→11/11|
|Touch|302/301/301|10→10/10|13→12/12|

Os cinco corpos candidatos são integralmente iguais aos controles. As diferenças de tamanho entre toys e alvos não foram usadas para prometer bytecount do alvo; os slots compactos/longos e os payloads são diferentes.

Oito compilações próprias: Current,Control,Candidate e cinco negativos. Negativos compilados: num7 com store único; Boolean sintético mantido em Collision; negação relacional float errada; cadeia GameParam original com store comum; ambos os switches de Touch mantidos. Todos diferem. O último produz **11 locais em vez de10**, comprovando que o cache externo ainda está vivo durante o corpo e não pode ser reutilizado pelo switch interno.

Cinco recusas lexicais aprovadas: âncora unary alterada; efeito adicional no alias Boolean; operador Cos157 alterado; argumentos RateConvert alterados; conjunto de ids alterado. Nenhum controle final falhou. A fixture Shoot conserva também 0f−num13/4f e0f+num13/4f, provando que a regra não os reescreve indiscriminadamente.

Compilador smcs2.6.5 Unity5.3.4f1 SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0; flags/ref/runtime pinned pelo manifest baseline10. Inspector existente validado, sem rebuild. Hashes, logs e diferenças completas em `fixture-results.json`; não há normalização do oráculo para aceitar os toys.

## 8. Histórico: resultados negativos conservados

|Método|Relatórios|SHA compatíveis|Trials compatíveis|Trials totais|
|---|---:|---:|---:|---:|
|Ball.SetShoot|56|56|0|0|
|GameManager.Collision|56|56|0|0|
|Player.Move4args|56|56|12|12|
|UserData.Update_gameParam|56|55|0|17|
|GameView.AddTouch Seb8args|56|56|17|17|

Move: os12 trials reais foram hoists, agrupamento, polaridades e uma expansão **parcial** da atribuição num6, além do root-guard que regrediu289→89. Nenhum restaurou toda a árvore num6, num7 e o break do for em conjunto. Os negativos parciais continuam válidos; não foram rotulados “não testados”.

AddTouch: os17 trials reais alteraram hoists/declarações, polaridade e assignment-if isolado, todos sem restaurar o switch(id) externo e retirar o cache interno em conjunto. Vários pararam no prefixo33, antes do store id original. A nova proposta inclui esses três pontos e as orientações por braço, não repete um after anterior inteiro.

UserData: os17 trials pertencem ao R7 ParteA, cujo baseline integral difere do before atual. **Um deles foi aceito parcialmente**, corrigindo para num>=0, e essa forma já está no before e foi conservada. Os outer-if isolados e demais negativos mantinham ternários internos/store comum; a composição nova expande toda a árvore e restaura flag2 positivo. Não se afirma que os17 foram todos recusados nem que não houve tentativa neste método.

`history.json` preserva todos os snippets/regras/prefixos/aceitações. A comparação por cabeçalho completo evita falsa incompatibilidade de overload. A revisão é limitada aos relatórios selecionados até cycle15, não prova inexistência global de uma emissão equivalente.

## 9. Regras do compilador e entrega

Mono pin b6587926c0f146c8b12985744f33a13b39a22d57, fontes locais em `analysis/tools/round5-mono-source/mcs/mcs/`:

- statement.cs247–264: If emite ramos/stores e join conforme os comandos.
- statement.cs1110–1150: Break/Continue preservam LoopEnd/LoopBegin e EH.
- statement.cs3830–3842 e3865–3874: switch cria/guarda LocalTemporary antes dos testes e o libera só após emitir as seções.
- assign.cs182–205/270–286: LocalTemporary armazena pelo builder e devolve-o ao pool em Release.
- codegen.cs357–385: GetTemporaryLocal reutiliza a pilha por tipo; FreeTemporaryLocal devolve o builder. O cache externo não está livre durante o inner switch.
- expression.cs3493–3498/3528–3567: float é tratado como unordered no ramo falso; manter `>=`/`>` em Move conserva as variantes .un.

Trechos e SHA exatos em `compiler-evidence.json`. Não se usou leitura de memória em execução nem inferência de valores de argumentos para substituir o IL.

Entrega: `source_calculator.py`, APIs puras/header-free transform_shoot_stores, transform_collision_stores, transform_move_stores, transform_gameparam_stores e transform_touch_selector_layout. Import/calls dos cinco inputs frozen passaram com leituras Path proibidas e reproduziram os after medidos pelo modelo. `probe-bundle.json` tem patches completos engine-compatible com before, after, spans UTF16 e SHA do arquivo.

O operador deve revalidar/recalcular spans e fonte atual, sobretudo em arquivos compartilhados com outros métodos alterados. Os corpos, cabeçalhos/maxstack/EH/metadados efetivamente emitidos no jogo seguem UNKNOWN até a sonda e o verificador completo. Este pacote é somente preparação comprovada para esse teste. A preparação desta família termina no congelamento, sem ampliar a fila com outra pesquisa.
