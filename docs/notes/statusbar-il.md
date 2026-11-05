# KairoPlugin.GetStatusBarHeight: IL integral

ID: `kairo.unity.native.KairoPlugin::GetStatusBarHeight()->System.Int32`.

Extraído das duas listagens congeladas no começo da rodada 2. Nenhuma nova compilação, alteração do método ou tentativa de reprodução foi executada para este extrato.

Aliases usados somente na apresentação dos operandos:

- `D` = `kairo.unity.ui.IApplication/UIMethod`
- `C` = `kairo.unity.native.KairoPlugin/<>c`
- `K` = `kairo.unity.native.KairoPlugin`
- `UI` = `kairo.unity.ui.IApplication`

## Original

Token: `[Method:0x075e]`. Cabeçalho `fat`, 12 bytes; flags `0x13`; MaxStack 3; InitLocals `true`; CodeSize 102 bytes; 29 instruções.

Locais: `[]`. Regiões de exceção: `[]`. Seções adicionais: `[]`.

```il
IL_0000: ldsfld K::statusBarHeight_
IL_0005: ldc.i4 2147483647
IL_000a: beq.s IL_0014
IL_000c: ldsfld K::statusBarHeight_
IL_0011: ldc.i4.0
IL_0012: pop
IL_0013: pop
IL_0014: ldsfld K::app_
IL_0019: callvirt UI::CheckUiThread()->System.Boolean
IL_001e: brfalse.s IL_002c
IL_0020: call K::_getStatusBarHeight()->System.Int32
IL_0025: stsfld K::statusBarHeight_
IL_002a: br.s IL_0060
IL_002c: ldsfld K::app_
IL_0031: ldsfld C::<>9__29_0
IL_0036: dup
IL_0037: brtrue.s IL_0050
IL_0039: pop
IL_003a: ldsfld C::<>9
IL_003f: ldftn C::<GetStatusBarHeight>b__29_0()->System.Object
IL_0045: newobj D::.ctor(System.Object,System.IntPtr)->System.Void
IL_004a: dup
IL_004b: stsfld C::<>9__29_0
IL_0050: ldc.i4.1
IL_0051: callvirt UI::RunOnUiThread(D,System.Boolean)->System.Object
IL_0056: unbox.any System.Int32
IL_005b: stsfld K::statusBarHeight_
IL_0060: ldsfld K::statusBarHeight_
IL_0065: ret
```

Fonte: `build\round2\KairoLibrary-original-methods.json`; SHA-256 `d3340a2a774d1c4a604c750ca0e7b36b5fbb15da96a4d0dabc60033066dc844a`.

## Recompilação da linha de base

Token: `[Method:0x03ba]`. Cabeçalho `fat`, 12 bytes; flags `0x13`; MaxStack 3; InitLocals `true`; CodeSize 100 bytes; 27 instruções.

Locais: `[]`. Regiões de exceção: `[]`. Seções adicionais: `[]`.

```il
IL_0000: ldsfld K::statusBarHeight_
IL_0005: ldc.i4 2147483647
IL_000a: beq.s IL_0012
IL_000c: ldsfld K::statusBarHeight_
IL_0011: pop
IL_0012: ldsfld K::app_
IL_0017: callvirt UI::CheckUiThread()->System.Boolean
IL_001c: brfalse.s IL_002a
IL_001e: call K::_getStatusBarHeight()->System.Int32
IL_0023: stsfld K::statusBarHeight_
IL_0028: br.s IL_005e
IL_002a: ldsfld K::app_
IL_002f: ldsfld C::<>9__29_0
IL_0034: dup
IL_0035: brtrue.s IL_004e
IL_0037: pop
IL_0038: ldsfld C::<>9
IL_003d: ldftn C::<GetStatusBarHeight>b__29_0()->System.Object
IL_0043: newobj D::.ctor(System.Object,System.IntPtr)->System.Void
IL_0048: dup
IL_0049: stsfld C::<>9__29_0
IL_004e: ldc.i4.1
IL_004f: callvirt UI::RunOnUiThread(D,System.Boolean)->System.Object
IL_0054: unbox.any System.Int32
IL_0059: stsfld K::statusBarHeight_
IL_005e: ldsfld K::statusBarHeight_
IL_0063: ret
```

Fonte: `build\round2\KairoLibrary-rebuilt-methods.json`; SHA-256 `474c290beec00bbe8836a955c04653fa6240812ba8355046d38b51ad40c0a6f5`.

## Diferença observada

O original contém `ldc.i4.0; pop; pop` nos offsets `IL_0011`, `IL_0012` e `IL_0013`; a linha de base recompilada contém somente `pop` em `IL_0011`. Os offsets seguintes e os destinos de desvios refletem dois bytes a menos no recompilado. A causa continua **UNKNOWN**. Não foram feitas novas tentativas.
