# br → br: donos de joins e saídas em cinco métodos smcs

**READY_FOR_GAME_PROBE**, não matched. Zero compilações do assembly do jogo, zero adoções/commits e nenhuma escrita pública. Os cinco corpos reais continuam **UNKNOWN_UNTIL_VERIFIER**.

Snapshot somente leitura: `build/round10/cycles/cycle13/after/Assembly-CSharp`. A identidade dos cinco métodos, spans UTF16, hashes das fontes, DTOs e alinhamentos estão em `snapshot-index.json` e `*/analysis.json`.

## 1. O grupo medido

|Método|IL original/atual|Locais originais/atuais|Edições de sequência|Retargets reais|Destinos pendentes de alinhamento|Diferenças de largura|
|---|---|---|---:|---:|---:|---:|
|GameForm.Draw_topBarParts(Graphics,int,int,int)|508/508|6/6 exatos|0|2|0|0|
|MyFormBase.SetTouchValue(int,int)|1.240/1.225|7/7 exatos|3|1|1|0|
|SoccerForm.Draw_newPlayerReserach(Graphics)|1.269/1.269|10/10 exatos|0|1|0|0|
|SpForm.Update()|2.052/2.052|24/24 exatos|10|3|0|0|
|GameView.OnTouchUnMarker(int,int,TouchEvent)|270/260|2/2 exatos|2|1|0|0|

São cinco métodos / **5.339 IL**. O par inicial é uma mudança real de destino, não apenas deslocamento numérico por instruções inseridas. O aligner conserva a correspondência entre instruções e informa oito retargets reais no grupo. O destino diferido de MyFormBase é examinado pelo contexto do original, não forçado a uma correspondência inexistente.

## 2. As duas formas de join local, sem alterar a sequência de instruções

### GameForm.Draw_topBarParts

Atuais159 br507 e368 br507 são gotos ao rótulo global dispatchExit. O original tem159 br244 e368 br453. Em244 e453 há outros br507, joins dos braços2/4 da cadeia externa. Os dois corpos têm os mesmos 508 bytes, slots, opcodes e nenhuma diferença de largura; só esses destinos divergem.

Fonte atual de cada braço: `if(w<=TBAR_W[type]){DrawImage;goto dispatchExit;} int n=w;...while...`. Fonte calculado: `if(w<=TBAR_W[type]){DrawImage;}else{int n=w;...while...}`. A cadeia externa e os payloads permanecem idênticos. As duas declarações e seus loops passam ao else local, cuja alocação foi verificada pelo modelo. O rótulo global sem usos é retirado. Não se copia nem edita operando de salto.

### SoccerForm.Draw_newPlayerReserach

Original635 br1009; atual635 br1098. Em1009 está `br1098`, o join do braço select_==0;1098 é incremento do loop i. A guarda atual `if(num5<0)continue;` pula direto ao incremento. O original pede `if(num5<0){}else{resto do braço select_==0}`. Todo o tail é preservado, incluindo variáveis num6/num7, desenho, clipping e seus blocos. O else exterior de select_ permanece no mesmo dono.

Essas duas propostas são exemplos da diferença entre saída direta e passagem pelo join local. Não se generaliza “todo br deve saltar ao incremento” nem “todo salto intermediário deve ser removido”.

## 3. O caso inverso: break de switch em lugar de braço vazio local

### MyFormBase.SetTouchValue

- Em 370, br1239 original versus br456 atual: o then vazio do teste `sysSave_.ints[3]==value` precisa de **break do switch**, que alcança o ret comum1239. O after é guarda com break e o payload existente depois, em vez de then vazio/else.
- Em 480, bne490;485 br1239 original versus480 beq566 atual: mesma guarda de break para ints[4], preservando os operandos Int32. O payload da alteração de volume fica depois da guarda.
- Em 719, brfalse729;724 br839 original versus714 brtrue829 atual: aqui o destino839 é outro br1239, o join do braço720896. Portanto a forma é **`if(Config.ADMOB_ENABLE){}else{payload}`**, não um break antecipado. O diagnóstico distingue os dois donos.
- O original tem dois breaks separados em1187 e1192 para os casos1441792/1507328. A reconstrução fundiu os rótulos numa seção única. Separar `case A:break;case B:break;` recupera o segundo bloco, sem mudar o conjunto de valores reconhecidos. O destino diferido em238 aponta exatamente ao segundo bloco1192 ausente.

As três edições de sequência correspondem às duas guardas/orientações e ao segundo break. A guarda de ints[3] é apenas retarget e não acrescenta instrução.

### GameView.OnTouchUnMarker

O case7 atual tem `if(markComponentKey_==-1){}else{...}`. Original97 br269 alcança o ret do switch; o atual vai ao join local127. Restaurar break no then e deixar o payload depois da guarda.

No case1, os GetTouchEventId12/14/15 já têm guardas break corretas e são preservados. Depois de GetTouchEventId22 faltam duas guardas: original224 brfalse234;229 br269 e249 brfalse259;254 br269. A forma é `if(touchEvent!=null)break; GetTouchEventKey; if(touchEvent!=null)break; ClearMarker();`. O break terminal do case1 permanece.

Isso explica um retarget e dois trechos de sequência, incluindo os dez bytes ausentes no atual. markComponentKey_ é Int64 e a comparação ==−1 é preservada literalmente; não há complemento de comparação float.

## 4. SpForm.Update: joins protegidos precisam de composição

Este método tem sete regiões EH. A proposta não move nenhum try/catch/finally nem nenhuma chamada de leitura/gravação:

- Original1334 brtrue1344;1339 br1603: trocar `if(j!=0){try...catch...}` por guarda `if(j==0)continue; {try...catch...}`.1603 é o incremento do mesmo for j, dentro do try do recordStore; o salto é br, não leave.
- Restaurar os quatro ternários: `(j==3||j==1)?0:1`, `flag?11:6`, `flag?8:4`, `flag?6:2`. Original1406 beq1419;1414 bne1425;1419 ldc0;br1426;1425 ldc1 prova a primeira orientação. Os demais brfalse escolhem os braços conforme as constantes originais. Somente comparações Int32/Boolean são alteradas.
- Retirar o else vazio que foi introduzido depois de `if(num5==-1){...}`. Ele gera o br intermediário atual1658. Original1278 br1663 e leaves1652/1658→1663 já alcançam o teste do formato seguinte. Remover esse else conserva os donos das saídas protegidas e elimina o br inserido.

O continue acrescenta cinco bytes e a retirada do else elimina cinco: saldo zero. Os quatro ternários mudam orientação/posição das constantes, sem alteração de tamanho nesse snapshot. Isso cobre as dez edições de sequência e três retargets; não se aceita cada peça isolada por esse cálculo.

## 5. Modelos reais e falha de implementação preservada

O modelo NoEmit real conferiu os cinco baselines contra as assinaturas efetivamente recompiladas. Depois previu as cinco composições completas: **complete=true e tipos originais exatos**, com6,10,7,2,24 locais. Em cada método a lista inteira `(nome,tipo,kind,slot)` também permaneceu idêntica entre os modelos before/after; a igualdade de tipos não esconde permutação de nomes. São dez consultas source-only no total, nenhuma emissão do alvo.

A primeira tentativa lexical de OnTouchUnMarker procurava só `if(markComponentKey_==-1)` e encontrou duas ocorrências. Não foi produzido candidato nem chamada de modelo de candidato nessa tentativa. O seletor foi restringido ao padrão completo **then vazio seguido de else**; a previsão posterior passou. `model-results-initial.json` preserva essa recusa; `retry_unmarker.py` e `model-results.json` registram a correção. É falha de seleção do implementador, não refutação da forma IL nem tentativa do alvo.

Produtores pinned: Round7SlotModel.cs / round7_slot_model.py e binário cached validados por hash; manifest privado contém fontes absolutas do snapshot. Hashes em `model-results.json`. Nenhuma flag complete foi editada. A geração standalone reproduz byte por byte os after medidos pelo modelo, mesmo após restringir o seletor.

## 6. Fixtures próprias completas

`FixtureCurrent.cs` foi escrito do zero com cinco moldes pequenos e membros compartilhados. `FixtureControl.methods.cs.txt` contém controles manuais independentes no mesmo tipo. Receive inclui try/catch/finally e retorno protegido, além das guardas/ternários, para que o comparador examine EH e epílogo.

|Toy|IL Current/Control/Candidate|Locais|maxstack|Resultado|
|---|---|---|---|---|
|Bar|229/229/229|6/6/6|17/17/17|Corpo completo igual|
|Research|107/107/107|3/3/3|9/9/9|Corpo completo igual|
|Touch|205/220/220|1/1/1|12/12/12|Corpo completo igual|
|Unmarker|219/229/229|2/2/2|18/18/18|Corpo completo igual|
|Receive|229/229/229|5/5/5|17/17/17|Corpo completo igual|

São sete compilações próprias: Current,Control,Candidate e quatro negativos. Negativos compilados: Research voltando a continue; cases vazios de Touch fundidos; Unmarker voltando a then vazio; Receive recuperando o else vazio extra. Todos diferem dos controles. Quatro recusas lexicais aprovadas: goto com outro rótulo; guarda Research que retorna em vez de continue; then Touch com payload; else Receive não vazio. Todos os controles finais passaram. Os Current de Bar/Research têm exatamente o mesmo tamanho e opcodes dos controles, mas destinos diferentes; são controles diretos da hipótese sobre donos.

Compilador smcs2.6.5 Unity5.3.4f1, SHA26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0. Flags, runtime e referências do manifest baseline10, conferidos. O inspector existente foi validado e usado sem reconstrução. `fixture-results.json` contém diferenças completas, headers, locais, hashes e logs; o comparador não normaliza saltos, locais, EH ou instruções para aceitar os toys.

## 7. Histórico e novidade delimitada

`history.json` conserva os relatórios R7/R8/R9/R10 fechados até cycle13 e compara SHA da declaração completa do baseline do relatório ao before atual:

|Alvo|Relatórios|SHA compatíveis|Trials compatíveis|Trials totais, incluindo outros before|
|---|---:|---:|---:|---:|
|GameForm topbar|54|43|0|10|
|Soccer research|54|54|3|3|
|MyForm touch|54|47|0|6|
|GameView unmarker|55|47|0|7|
|SpForm Update|56|32|0|32|

Os três negativos reais de Soccer foram examinados, não omitidos: R7 ParteA agrupou declarações; C06 trocou a guarda por `if(num5>=0){}else{continue;}`; C17 envolveu o tail em `if(num5>=0){tail}`. C06/C17 mantiveram outro molde de join e foram recusados com prefixo636→105. A proposta nova é **`if(num5<0){}else{tail}` sem continue**, conservando o br local1009 anterior ao br exterior1098. Portanto não repete essas variantes. Os relatórios mantêm os snippets e resultados negativos completos. Ausência de trial compatível dos outros métodos não prova ausência global de fonte semanticamente equivalente em outros snapshots.

## 8. Emissão, entrega e limites

Mono pin b6587926c0f146c8b12985744f33a13b39a22d57. Em `analysis/tools/round5-mono-source/mcs/mcs/statement.cs` SHA063a437095e1d608d045383d76291c9359e9a98220eea7750018a3d42712437a:

- If247–264 emite teste falso, braço true e br de join quando existe else; o else vazio ainda preserva essa estrutura.
- Goto833–838 emite Br ou Leave para seu rótulo resolvido.
- Break1110–1119 / Continue1141–1150 usam LoopEnd / LoopBegin e a fronteira de proteção correta.
- TableSwitchEmit3556–3572 emite as seções e seus blocos separadamente; dois rótulos na mesma seção não são duas seções vazias.

Não há nova regra de emissão inventada; os trechos pinned e as fixtures dão suporte às formas calculadas. Não se usou equivalência CFG como teste de aceitação.

APIs autocontidas/puras/header-free em `source_calculator.py`: transform_topbar_joins, transform_research_joins, transform_touch_joins, transform_unmarker_joins e transform_receive_joins. Import/call dos cinco inputs frozen passou com leituras Path proibidas e reproduziu os after aprovados pelo modelo. `probe-bundle.json` fornece before/after, spans UTF16 e hashes de arquivo no formato de proposals do engine.

É necessário revalidar identidade/spans na fonte mais recente antes da sonda, especialmente se outro método no mesmo arquivo tiver mudado. Corpo, cabeçalho, maxstack, offsets/EH e metadados emitidos do alvo permanecem UNKNOWN. Potencial **0..5 métodos /0..5.339 IL**, nunca ganho atribuído ao diagnóstico ou à assinatura. Nenhum dos estudos anteriores congelados foi alterado.
