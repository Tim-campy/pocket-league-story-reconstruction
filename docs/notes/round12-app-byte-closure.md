# Assembly-CSharp concluído pelo critério literal

O verificador aprovou 1.958 métodos, 601.603 bytes de IL e 129 tipos. Não restam corpos divergentes nem auxiliares retidos. A comparação integral do PE confirma 895.468 bytes idênticos fora dos dois campos não determinísticos comprovados, em arquivos de 895.488 bytes.

| Campo | Offset absoluto nos dois arquivos | Tamanho |
| --- | --- | --- |
| COFF.TimeDateStamp | 136 | 4 |
| Module.Mvid | 893568 | 16 |

Esses campos ocupam 20 bytes; a quantidade efetivamente diferente varia conforme os valores de cada compilação. A lista de diferenças fora deles está vazia. Cabeçalhos, seções, tabelas, heaps, recursos, preenchimento e payloads dos campos RVA estão cobertos pela comparação literal, sem normalização ou edição posterior da DLL.

A prova independente inicial está em `reports/round12-app-independent-raw-acceptance.json`. O estado atual é vinculado à última comparação bruta do PE canônico em `config/round12-protocol.json`, e o progresso é gerado por `tools/progress`. Uma recompilação invalida o selo literal até a comparação do novo arquivo ser renovada.

Os 39 ciclos B foram preservados. O construtor implícito de sete bytes, cujo corpo já era idêntico, foi liberado por uma prova genérica de origem e disponibilidade compilada, registrada separadamente em `reports/round12-verification-origin-release.json`. Seu ganho de estado é +1 método e +7 bytes de IL contabilizado, sem alteração de fonte C# ou reescrita do ganho de B39. Ver [a certificação de origem](round12-static-data-constructor-ownership.md).

A prova antiga de repetibilidade foi revalidada fisicamente, preservando os dois relatórios e os binários retidos. A adaptação corrigiu um campo-resumo ausente do esquema antigo, sem acrescentar exceções. Os relatórios volumosos são publicados integralmente em gzip lossless com hashes e restaurador; ver [transporte da evidência](round12-raw-report-transport.md).

KairoLibrary também está concluída, com 569.816 bytes idênticos fora de seus quatro campos comprovados. Somados, os dois arquivos têm 1.465.344 bytes; 1.465.284 bytes fora dos seis campos correspondem literalmente ao original. As versões históricas exatas dos executáveis compiladores permanecem UNKNOWN; as receitas atuais foram comprovadas pelos arquivos produzidos.

Esta conclusão cobre os dois assemblies C#. Não foi montado nem executado APK novo nesta rodada. DEX/Java e normalização de assemblies permaneceram fora do escopo.
