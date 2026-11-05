# KairoPlugin.GetStatusBarHeight

Assembly original: KairoLibrary, token `0x0600075e`, SHA-256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`. Assinatura: `kairo.unity.native.KairoPlugin::GetStatusBarHeight()->System.Int32`. Continua divergente; nenhuma modificação deste método foi aceita nesta investigação.

Depois de restaurar a ordem dos membros, os nomes do cache e da lambda correspondem. A diferença restante observada no snapshot é anterior à chamada do delegate: o original tem 102 bytes IL, o candidato 100, ambos com cabeçalho fat, maxstack 3, InitLocals e nenhum local.

No ramo `statusBarHeight_ != int.MaxValue`, o original contém `ldsfld statusBarHeight_; ldc.i4.0; pop; pop`. O C# recuperado contém duas atribuições a locais não utilizados; o compilador gera somente `ldsfld statusBarHeight_; pop`. Isso desloca os destinos dos desvios e impede correspondência. Não é suficiente declarar o código equivalente.

Foram testadas classes próprias, separadas do jogo, com o compilador 1.3.2 e as mesmas opções relevantes: operações aritméticas/bit a bit, comparações não utilizadas, condições vazias, variantes de goto/switch e inicializadores de arrays. Nenhuma reproduziu esse trecho exato. Os testes ficaram em `analysis/unused-expression-probes/`; o fonte principal foi preservado.

A origem dessa sequência é **UNKNOWN**. As hipóteses de forma original do C#, opções ou versão não foram distinguidas para esse caso. Não foram acrescentados bytes, escrita de IL ou edição de DLL. O próximo teste deve partir dessa diferença pequena e objetiva, mantendo as demais correspondências protegidas.
