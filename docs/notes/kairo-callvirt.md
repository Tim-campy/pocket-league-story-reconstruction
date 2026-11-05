# Diagnóstico de call e callvirt

O grupo inicial contém **19 métodos e 21 pontos de chamada**: `FormManagerBase.OnDraw` tem três pontos, os outros métodos têm um cada. Em todos, o original usa `callvirt`, a recompilação usa `call`, e a identidade completa do método chamado é igual. Os demais operandos e posições das instruções também correspondem.

As causas observadas são formas do C# recuperado:

| Forma recuperada | Métodos | Forma candidata para recuperar callvirt |
|---|---:|---|
| `receiver?.Method()` | 16 | `if (receiver != null) { receiver.Method(); }` |
| `new Type(...).Method()` | 3 | Declarar uma variável local, construir o objeto e chamar através da variável |

Em `SecureByteArray.ToArray`, a forma com retorno exige `if (ary == null) return null; return ary.Get();`, preservando o retorno antecipado original.

`tools/diagnose_kairo_callvirt.py` compila uma classe sintética, independente do jogo, com o pacote Microsoft.Net.Compilers 1.3.2, executável 1.3.1.60621, otimização ligada e C# 6. O resultado confirma:

```csharp
receiver?.Run();                         // call
if (receiver != null) receiver.Run();    // callvirt
new Receiver().Run();                    // call
Receiver value = new Receiver();
value.Run();                            // callvirt
```

No teste otimizado, a variável local da última forma desaparece do IL; a diferença restante é o opcode de chamada. Isso permite recuperar o opcode por C#, sem acrescentar instruções artificiais, editar IL ou modificar o assembly após compilar.

## Declarações e referências

Todos os 21 alvos são métodos de instância não virtuais. Vinte pontos chamam membros da própria KairoLibrary, com os mesmos atributos de declaração no original e no recompilado: `0x0086`, público e HideBySig, sem Static ou Virtual. Incluem `JarInflater.Close`, `SecureByteArray.Get`, `Vector3D.Normalize`, `RecordStore.CloseRecordStore`, `Storage.Close`, os três métodos de toque de `SurfaceManagerBase`, `JThread.Start` e `Image.Dispose`.

O único ponto externo chama `UnityEngine.AndroidJavaObject.GetStatic<T>(string)`, na lambda original `<LaunchLobi>b__0`, IL offset 60. A referência usada no build é a própria UnityEngine.dll do APK; sua declaração também é de instância, não virtual, atributos `0x0086`. O C# recuperado invoca o método diretamente sobre `new AndroidJavaClass(...)`; a sonda explica a escolha de `call` sem exigir uma referência diferente.

Não há evidência de `base.Method()` trocado por `this.Method()` neste grupo. Também não foi encontrada diferença de declaração que indique um problema global nas referências. Isso não demonstra que todos os arquivos históricos de referência eram idênticos; a conclusão é restrita aos 21 pontos examinados.

## Reparos e verificação

`tools/fix_kairo_callvirt.py --apply` mantém um plano explícito de 19 métodos. Ele altera somente um método por tentativa, recompila e verifica KairoLibrary inteira, exige correspondência integral do alvo e nenhuma regressão entre os métodos antes confirmados. Se a tentativa falhar, restaura o fonte anterior e verifica a restauração. O estado do cadastro é alterado somente pelo verificador existente.

Os reparos nos métodos gerados são feitos dentro da lambda no método de origem. Nenhuma classe gerada é escrita à mão. Os três pontos de `OnDraw` são tratados juntos porque pertencem ao mesmo corpo.

Resultado concluído: **19 de 19 métodos confirmados**, KairoLibrary passou de **2.783 para 2.802 matched** (85,25%). Os corpos iguais passaram de 2.787 para 2.806; quatro continuam retidos pela regra dos caches compartilhados. Cada ajuste passou pela verificação integral sem regressões. Uma tentativa da última lambda foi restaurada porque a preparação do inspetor falhou em paralelo; ela não avaliou o C#/IL. A repetição com recibo novo passou. O script agora diferencia falha de infraestrutura de diferenças verificadas e rejeita recibos cujos hashes não correspondem ao fonte atual.

Evidências: [diagnóstico com tokens, offsets, alvos e sonda](../../reports/KairoLibrary-callvirt-diagnostic.json), [tentativas de reparo e resultados integrais](../../reports/KairoLibrary-callvirt-repairs.json). O segundo relatório contém as contagens antes e depois de cada ajuste, hashes do fonte, diferenças restantes e eventuais rollbacks.
