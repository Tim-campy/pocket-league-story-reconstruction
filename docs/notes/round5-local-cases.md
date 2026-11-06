# Diagnostico dos locais ? rodada 5

Nenhum fonte do jogo ou estado foi alterado. Nomes abaixo sao os do C# reconstruido, lidos do MDB atual; os nomes originais permanecem UNKNOWN. O corpo de cada um dos 15 metodos de simbolos foi comparado integralmente ao snapshot antes de usar seus nomes.

As categorias sao nao exclusivas. As diferencas de IL sao medidas; os reparos e as correspondencias de nomes de locais originais sao hipoteses ate compilacao e verificacao.

| Categoria | Casos entre os 11 |
| --- | ---: |
| omitted_assignment | 10 |
| declaration_order | 11 |
| block_scope | 4 |
| split_reused_local_across_blocks | 3 |
| omitted_unused_declaration | 1 |

| Metodo | Classificacao | Evidencia |
| --- | --- | --- |
| `Assembly-CSharp::form.DebugForm::Draw_goal(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num5 later in the same method block. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.DebugForm::Draw_goalMini(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num5 later in the same method block. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.GameForm::_draw(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order, block_scope | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num13 later in nested scopes. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SoccerForm::Draw_bonusIncome(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num5 later in the same method block. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SoccerForm::Draw_matchElse(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num8 later in the same method block. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SoccerForm::Draw_newPlayerPopular(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num4 later in the same method block. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SoccerForm::Draw_newPlayerReserach(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num4 later in the same method block. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SubForm::_draw(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order, block_scope, split_reused_local_across_blocks | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num24 later in nested scopes. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SubForm::_draw4(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order, block_scope, split_reused_local_across_blocks | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num4 later in nested scopes. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::form.SubForm::_draw6(kairo.unity.ui.Graphics)->System.Void` | omitted_assignment, declaration_order, block_scope, split_reused_local_across_blocks | Initial zero chain stores original slots [3, 2, 1, 0] but rebuilt slots [1, 0, 2]. Current symbols/source place the likely original root coordinate num4 later in nested scopes. The first discrepancy is an omitted zero assignment plus declaration/scope layout, not a proved compiler temporary. |
| `Assembly-CSharp::system.form.LanguageForm::DrawButton(kairo.unity.ui.Graphics,System.Threading.ThreadStart,System.Int32)->System.Void` | omitted_unused_declaration, declaration_order | Original slot 1 is Int32 and has zero loads, stores and address loads; original x/y are slots 2/3, while MDB names current x/y as 2/1. An unused declaration before x and y can reproduce the missing slot; only compilation can confirm the full body. |

## DrawSeb

Os tres overloads examinados possuem um array-copy e um indice temporarios no foreach reconstruido. O original nao faz a copia adicional. Um for indexado e candidato para reproduzir os slots e a ordem de comandos. O formato historico exato do fonte continua UNKNOWN ate prova adicional.

## CheckTermScript

O original copia o parametro int[] para o slot 1 antes de indexa-lo. A assinatura tem um slot int[] ausente na recompilacao. Introduzir um alias e candidato; nenhuma correcao foi adotada aqui.

## Limites

O alinhamento de instrucoes que sugere nomes ignora destinos de desvios e numeros de slots somente para diagnostico; ele NAO participa do verificador. Os escopos do MDB nao estavam preenchidos. Profundidades de declaracoes foram lidas do C# com scanner que ignora strings, chars e comentarios. Os quatro testes desse extrator passaram.
