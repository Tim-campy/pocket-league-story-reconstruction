# v1.0.0-csharp-reconstruction

Encerramento da reconstrução C# de Pocket League Story 2.0.2 (Unity Mono 5.3.4f1).

**5.245/5.245 métodos, 871.497/871.497 bytes IL e 466/466 tipos; zero regressões.** Os 1.465.284 bytes dos dois PEs fora de seis campos não determinísticos (60 bytes de extensão) são literalmente idênticos ao original. Zero diferenças residuais. Nenhuma DLL foi editada após compilar.

| Assembly | Métodos | IL bytes | PE bytes | SHA-256 reconstruída |
|---|---:|---:|---:|---|
| KairoLibrary | 3287 | 269894 | 569856 | `114f39eb77f294d33288915b7dc11bf3401478b9c7ed9beb6f94980bba02d376` |
| Assembly-CSharp | 1958 | 601603 | 895488 | `3a3939e52bee4404097083fb2ab3c7397b7107917dff551691735d59e2f5ee32` |

| Compilador | SHA-256 |
|---|---|
| Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109 | `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3` |
| Unity 5.3.4f1 / smcs 2.6.5.0 | `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0` |

Esses hashes das DLLs identificam esta verificação; outra compilação muda os campos comprovados e pode gerar outro SHA completo. A identidade histórica exata dos compiladores permanece UNKNOWN.

Receita: docs/toolchain.md. Critério e exceções: README.md. Provas: reports/reconstruction-closure.json. O repositório não distribui APKs, DLLs originais/reconstruídas nem assets. Nenhum APK foi montado, instalado ou executado nesta rodada. As DLLs finais ainda não foram testadas em aparelho. O teste anterior exercitou outra baseline. A montagem/teste final está apenas descrita em docs/apk-final.md.

Inventário para decisão futura: 101 classes Java candidatas (5 cobrança, 11 anúncios, 47 interface Android e 38 outros), sem decompilação; DEX/Java permanece fora do escopo.
