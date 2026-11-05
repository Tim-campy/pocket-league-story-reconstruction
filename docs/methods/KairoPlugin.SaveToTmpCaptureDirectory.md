# SaveToTmpCaptureDirectory: comparação eliminada

ID gerado: `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass82_0::<SaveToTmpCaptureDirectory>b__0()->System.Object`.

A busca do diagnóstico B encontrou dois `pop` após carregar `Config.APPLI_TYPE`
e a constante 2. Foi testado um `if (Config.APPLI_TYPE == 2) { }` no método de
origem, sem escrever o método gerado à mão. O candidato preservou todos os
corpos antes idênticos, mas o corpo alvo completo continuou divergente.

O candidato não foi adotado. Esse padrão local, sozinho, não comprova a
recuperação do restante do método. Causa das demais diferenças: **UNKNOWN**.

Evidência: `reports/round3-diagnostic-B.json` e
`reports/round3-elided-guard-adoption.json`.
