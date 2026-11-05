# KairoPlugin.GetStatusBarHeight

ID: `KairoLibrary::kairo.unity.native.KairoPlugin::GetStatusBarHeight()->System.Int32`.

Rodada 3: as formas `if (statusBarHeight_ != int.MaxValue && statusBarHeight_ > 0) { }`
e os dois `if` vazios aninhados produzem o corpo original integralmente. O desvio
da comparação interna é removido porque seu destino é a instrução seguinte;
os operandos ainda são carregados e descartados por `ldc.i4.0; pop; pop`.

O operador relacional histórico é **UNKNOWN**. Oito fixtures próprias, combinando
as formas aninhada/composta com `>`, `>=`, `<` e `<=`, produziram exatamente os
mesmos corpos. O `>` escolhido no C# é um representante comprovado, sem afirmar
que era o texto histórico. Nenhum byte da DLL foi alterado após a compilação.

Evidência: `reports/round3-diagnostic-B.json`; adoção e verificação integral em
`reports/round3-elided-guard-adoption.json`.
