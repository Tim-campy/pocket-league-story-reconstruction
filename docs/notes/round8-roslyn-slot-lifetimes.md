# Round 8: hipóteses Roslyn por identidade e vida dos locais

Fonte desta análise: medição após C3 (`round8-cycle03-after-measurement.json`), grupo `stloc.s => stloc.s`: **10 métodos, 10498 bytes de IL**. Os três maiores são Offscreen.OnPostRender (1978), StringUtil.GetCode (1326) e Graphics.DrawArc (1188): **4492 bytes potenciais**, não ganhos confirmados. Nenhum modelo, helper ativo, fonte oficial ou configuração foi alterado. Zero compilações do jogo; três compilações de fixtures artificiais pelo csc 1.1.0.51109.

## 1. DrawArc: escopo do contador do segundo laço

O alinhamento dos usos demonstra a permutação: o local recompilado 13 (`num15`, Int32) corresponde ao original 22; os locais recompilados 14–22 correspondem aos originais 13–21. Após aplicar essa correspondência somente ao diagnóstico, as sequências de instruções não têm outro trecho inserido/removido/substituído. Os dois corpos têm 1188 bytes. Isso não promove o método nem normaliza seu resultado.

O fonte declara `int num15 = 0;` no bloco pai, depois do ramo da malha. O Roslyn reserva esse local na entrada do bloco, antes de visitar os locais do ramo anterior. O original usa o slot 22 para esse contador e o primeiro uso só ocorre em IL_0322 (802), após os locais de MeshBuilder, escalares e array2. Hipótese: o contador pertence ao escopo do segundo laço:

```csharp
for (int num15 = 0; num7 < num6; )
{
    // Corpo original, inclusive num15++ ao final.
}
```

Nenhum comando ou inicializador muda de ordem. O corpo desse laço não contém continue que pudesse mudar seu destino. A variante mínima calcula 29 tipos, ainda incompleta, pois o modelo conta também o alias RenderPass[] já eliminado pelo compilador real. Uma segunda variante usa diretamente `foreach (RenderPass renderPass2 in GetRenderPass(...))`: esse alias só era usado para iniciar o foreach, sem comandos entre declaração e uso. A previsão passa a **28 tipos na ordem original**, mas continua incompleta por `stack_scheduler_multiple_use:array2`. Pela política da rodada 8, incompletude não veta a compilação; o corpo real permanece UNKNOWN.

## 2. GetCode: quatro locais de trabalho compartilhados

A reconstrução emite 40 locais; o original possui 31. Os num2 de seis laços correspondem ao mesmo slot original 2; quatro num3 correspondem ao 3; três num4 ao 4; um num5 ao 5. São 14 declarações em blocos distintos, com 4 identidades originais. As definições têm usos separados por atribuições novas em cada trecho; não há captura, yield, await, passagem por ref/out ou uso de uma definição anterior depois de sua substituição nesse conjunto.

O original reserva no bloco pai: string[] 0, num 1, os quatro inteiros 2–5, flags 6/7, contadores 8–10, arrays 11/12. A proposta declara `int num2, num3, num4, num5;` imediatamente após a declaração de num e substitui as 14 declarações internas por atribuições. Os valores continuam sendo calculados nos comandos originais; não há inicialização extra dos quatro inteiros.

Isso é diferente de pedir ao compilador que reutilize os slots de variáveis nomeadas distintas. O Roslyn 1.1 não libera esses slots; para gerar a identidade compartilhada, o fonte precisa compartilhar o símbolo. O scheduler é calculado por símbolo: unir definições também pode impedir que uma definição isolada fique só na pilha.

Há duas instruções ausentes na reconstrução atual, além da permutação/largura de locais: original `stloc.2` no offset 293 e `ldloc.2` no 321, no teste JIS. A definição local isolada desse num2 foi mantida na pilha pela recompilação. A identidade compartilhada é uma hipótese para restaurar esse armazenamento; somente o compilador real e o verificador podem confirmá-la.

**Previsão medida do candidato:** completa, **31 tipos exatamente na ordem original**, sem erros semânticos apontados pelo modelo. Correspondência de corpo: UNKNOWN. O modelo não foi modificado para aprová-lo. Os nomes num2…num5 são os do fonte reconstruído, não alegações sobre os nomes históricos.

## 3. OnPostRender: listas tardias em bloco filho

Os slots 0–3 (instance, meshManager, flag e graphics) já correspondem. O for inicial precisa de 4; o foreach seguinte precisa de array/índice/elemento em 5–7. A reconstrução antecipa as três listas e flag2 para 4–7 e joga o for/foreach para 8–11. As listas só começam seus usos nos offsets originais 287, 300 e 313; flag2 no 364. O for e o foreach terminam seus usos antes desses trechos.

A proposta abre um bloco imediatamente antes de textRenderTasks_ e fecha-o depois do último laço de material2, mantendo graphics no bloco exterior. Assim os quatro locais tardios pertencem a um bloco emitido após o for/foreach. Não há referência às listas ou a flag2 fora desse bloco no método.

Essa hipótese resolve uma classe de permutação, não o corpo completo: depois da correspondência de slots ainda há **16 trechos de sequência diferentes**, incluindo guardas/retornos e condições. O original tem 1978 bytes e a reconstrução 1994. O modelo continua explicitamente incompleto e calcula 40 tipos em lugar dos 36 reais, pelas limitações já conhecidas de scheduler, foreach e catch. Ganho de prefixo e assinatura emitida permanecem UNKNOWN; candidato adequado apenas como preparação no motor, seguido das regras de fluxo.

## Evidência primária do emissor

Snapshot Roslyn `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, cache analysis/tools/round6-roslyn-source:

- `src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs:594–613`: DefineLocal de todos os locais de um bloco acontece antes de emitir seus comandos. Isso explica a declaração no bloco pai aparecer antes dos locais de um ramo anterior no texto.
- `EmitStatement.cs:1395–1398`: IsStackLocal elimina a definição de slot.
- `EmitStatement.cs:1430–1442`: DeclareLocal recebe o símbolo e a política de reutilização do SynthesizedKind.
- `EmitStatement.cs:1499–1505`: FreeLocal só libera quando `local.Name == null`; o comentário identifica a liberação de locais nomeados como não implementada.
- `src/Compilers/Core/Portable/SynthesizedLocalKind.cs:242–261`: UserDefined retorna false para IsSlotReusable mesmo no Release.
- `Optimizer.cs:55–116`: o conjunto de locais de pilha é decidido por LocalSymbol; FilterValidStackLocals retira os símbolos marcados CannotSchedule. Linhas 254–258 tornam esse estado persistente no símbolo. Uma nova identidade compartilhada pode alterar a eliminação de stores/loads; não se adota essa hipótese sem o teste do corpo.

O commit exato de fabricação do executável instalado continua UNKNOWN. Hashes e intervalos lidos estão em analysis.json.

REA confirmou a identidade e os corpos originais de Offscreen/StringUtil, evidência `ev_ae8784214bec741959ca6cdcf6608e2fb48f6e837db79b79e37a37051205e344`, cobertura completa sem issues. DrawArc já consta da evidência Graphics da rodada 8. As três assinaturas e corpos originais usam o mesmo KairoLibrary SHA-256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`.

Os nomes de locais do recompilado vieram do PDB. Como o binário oficial inteiro foi reemitido durante os ciclos, a leitura dos símbolos só foi aceita após comparar os três métodos selecionados integralmente com os dumps congelados após C3: cabeçalhos, locais, IL resolvido e EH iguais. Não se presume que o MVID ou hash inteiro de um build posterior seja o do snapshot.

## Fixtures e transporte

Três fixtures independentes foram compiladas nas formas recuperada, controle escrito separadamente e candidata: três compilações artificiais totais. Cada candidato coincide integralmente com seu controle em cabeçalho, locais, IL e EH. Todos os três modelos de fixture são completos/exatos; a forma recuperada de cada uma difere:

| Fixture | Locais antes | Locais depois |
| --- | --- | --- |
| Identidade compartilhada | Int32 × 5 | Int32 × 4 |
| Contador em for | Int32, Object, Single | Object, Single, Int32 |
| Declarações tardias em filho | Object, Double, Boolean, Int32 | Object, Int32, Double, Boolean |

Os controles demonstram a emissão das formas de fonte; não demonstram que os corpos reais do jogo já batem. fixture-results.json conserva os dumps completos dos métodos artificiais.

patches.json tem quatro propostas para três métodos sob prefixo `roslyn-store-use-lifetime-scope`. Os campos before/after preservam **CRLF bruto**, sem leitura por universal-newline; start/end são posições UTF-16 do fonte congelado. Cada before tem seu hash e contagem LF=CRLF, evitando a anomalia de transporte observada no primeiro preparo de C3. Qualquer rebase futuro deve continuar sendo exato e único.

Recomendação: sonda isolada de GetCode primeiro (1326 bytes; previsão completa), depois DrawArc (1188; só permutação no diagnóstico). OnPostRender fica como preparação parcial (1978), sem promessa de fechamento. O máximo de corpos que essas propostas podem fechar é três; quantos fecharão é UNKNOWN. O verificador original continua sendo o único juiz.

Reprodu??o source-only, sem Emit dos alvos: `python tools/round8_roslyn_slot_lifetimes.py`. Fixtures artificiais (tr?s compila??es csc hist?ricas): `python tools/test_round8_roslyn_slot_lifetimes.py`. As fontes e s?mbolos do jogo n?o s?o recompilados pelo calculador.
