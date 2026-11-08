# Pocket League Story — reconstrução C# concluída

Reconstrução dos fontes C# de **Pocket League Story 2.0.2**, Android, Unity Mono **5.3.4f1**. Os dois assemblies são compilados a partir dos fontes recuperados; não se copiam corpos originais para a saída nem se editam DLLs após a compilação. O texto histórico exato do fonte e a revisão de fabricação dos compiladores permanecem **UNKNOWN**.

| Assembly | Métodos matched | Bytes IL matched | Tipos aprovados | Bytes do PE iguais fora das exceções |
|---|---:|---:|---:|---:|
| KairoLibrary | 3287/3287 | 269894/269894 | 337/337 | 569816/569816 |
| Assembly-CSharp | 1958/1958 | 601603/601603 | 129/129 | 895468/895468 |
| **Total** | **5245/5245 (100%)** | **871497/871497 (100%)** | **466/466** | **1465284/1465284** |

Os métodos incluem 82 declarações sem CIL, aprovadas por metadados. Os 5.163 corpos têm IL, cabeçalhos, assinatura de locais e regiões de exceção correspondentes. A prova final compara também os **arquivos inteiros literalmente**: tabelas, heaps, recursos, RVAs, padding e cabeçalhos. Zero diferenças residuais além dos campos comprovadamente não determinísticos. O progresso e os vínculos de membros gerados são produzidos pelo verificador, não atribuídos manualmente.

## Seis exceções permitidas

Offsets absolutos, base zero, iguais no original e reconstruído. São 60 bytes de extensão em dois PEs de 1.465.344 bytes; nem todo byte desses campos precisa mudar a cada compilação.

| DLL | Campo | Offset | Bytes |
|---|---|---:|---:|
| KairoLibrary | COFF.TimeDateStamp | 136 | 4 |
| KairoLibrary | Module.Mvid | 527096 | 16 |
| KairoLibrary | IMAGE_DEBUG_DIRECTORY.TimeDateStamp | 550520 | 4 |
| KairoLibrary | CodeView.RSDS.PdbGuid | 550548 | 16 |
| Assembly-CSharp | COFF.TimeDateStamp | 136 | 4 |
| Assembly-CSharp | Module.Mvid | 893568 | 16 |

A variação foi comprovada compilando o mesmo fonte duas vezes: [Kairo](reports/round3-Kairo-repeatability.json), [smcs revalidado](reports/round12-Assembly-CSharp-repeatability-profile.json). Não há normalização, hash normalizado ou exclusão genérica de cabeçalhos. O caminho PDB é reproduzido pelo compilador e continua sob comparação. Um SHA integral diferente entre builds não invalida o critério acima.

## Reprodução

**Assets, APKs, DLLs e outros binários originais não estão no repositório.** Tampouco são distribuídos os executáveis dos compiladores, referências, DLLs reconstruídas ou chaves. É necessário fornecer sua própria cópia do APK. `original/`, `analysis/`, `ref/` e `build/` são privados e ignorados pelo Git.

Siga **[docs/toolchain.md](docs/toolchain.md)**: contém a preparação de um clone, extração autenticada da cópia própria, aquisição portátil do compilador, hashes, todas as opções, referências na ordem, os 260 fontes na ordem com hashes, ambiente Mono, SUBST E:, caminho PDB e comandos completos de compilação/verificação/comparação literal. Não é necessário executar os motores históricos.

Após preparar as dependências e a extração conforme essa receita:

```powershell
python tools/regenerate_references.py
python tools/verify --all
```

O verificador compila os dois alvos. Confira retorno 0 e execute também o teste literal documentado na receita. Os compiladores selecionados são **Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109** e **Unity smcs 2.6.5.0**. Os [seis ajustes de emissão de metadados](docs/notes/kairo-metadata-emission.md), inclusive constantes e aliases não usados, são necessários para reproduzir o resultado.

Estado gerado: [PROGRESS.md](PROGRESS.md). Hashes das saídas deste marco e provas: [manifesto final](reports/reconstruction-closure.json), [nota de release](docs/reconstruction-release.md). Uma recompilação requer recibos e comparação literal novos; não se reutiliza um SHA antigo como prova.

## Componentes externos e limites

As sete DLLs externas — `Mono.Security`, `mscorlib`, `System`, `System.Core`, `UnityEngine.Networking`, `UnityEngine.UI`, `UnityEngine` — correspondem por SHA-256 à distribuição oficial Unity 5.3.4f1. **UnityEngine.dll é a variante Android Mono**, 970.752 bytes, não a do editor: SHA-256 `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. [Auditoria das DLLs](config/dll-audit.json).

As seis bibliotecas nativas `libmain.so`, `libmono.so` e `libunity.so`, nas arquiteturas ARMv7 e x86, também correspondem à distribuição oficial; `libunity.so` identifica o player **Mono Release**. [Prova Android e limites](docs/notes/unity-android.md). Externos e dados são preservados como componentes opacos; não entram na porcentagem de código reconstruído.

**DEX/Java está fora do escopo desta reconstrução.** Existe apenas inventário estático: 79 classes Unity, 3.052 candidatas a SDKs e 101 candidatas a código próprio, incluindo cinco R geradas. Autoria e função detalhada não foram verificadas por decompilação. A [lista de 101 classes por função](docs/java-phase2-inventory.md) é para decisão sobre uma segunda fase.

A assinatura original do APK está fora do escopo e não será reproduzida. O critério futuro preserva os payloads comprimidos das 1.213 entradas não substituídas; as duas DLLs serão comparadas descomprimidas, com somente as seis exceções acima, e os três portadores exatos de assinatura serão novos. Ordem, método e congruência de alinhamento das 1.218 entradas deverão ser mantidos. Não se afirma identidade integral de um APK assinado com outra chave.

## Teste funcional e APK final

O teste **já realizado com a baseline anterior** do commit `aecb02b8a7b0965508e223335e44ae36a95a0939` abriu o jogo, chegou ao menu e executou uma partida completa no Samsung Galaxy S22 Ultra SM-S908E, Android 16/API 36, ARMv7: Sun Rays 1–0 Blue Stars. A busca nos buffers capturados não encontrou exceções ou erros fatais Mono/Unity; isso se limita ao cenário observado. [Evidências e limitações](docs/notes/round10-functional-test.md).

**As DLLs finais ainda não foram testadas em aparelho.** O APK anterior era apenas um teste de outra baseline e não integra a comparação final. Nesta rodada não se montou, instalou nem executou APK. O [procedimento do APK final](docs/apk-final.md) está pronto como documentação para uma execução futura, com DEFLATE nível 6, chave própria de teste, comparação por entrada e roteiro logcat.
