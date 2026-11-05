# Pocket League Story

Reconstrução do código C# de `Assembly-CSharp.dll` e `KairoLibrary.dll`, seguindo [AGENTS.md](AGENTS.md). Um método só estará concluído quando a recompilação corresponder ao corpo original, com tokens resolvidos, cabeçalho, locais e exceções conferidos.

A estrutura cobre **5.245 métodos e 466 tipos**. Os dois assemblies foram recuperados e compilam integralmente: KairoLibrary tem 163 fontes C# e usa o candidato Microsoft.Net.Compilers 1.3.2; Assembly-CSharp tem 97 fontes e usa smcs 2.6.5.0. Os números atuais de métodos e bytes IL correspondentes são gerados em [PROGRESS.md](PROGRESS.md), com recibos vinculados aos fontes e ao build.

As versões históricas exatas continuam `UNKNOWN`. A linha de base Kairo comparou o mesmo fonte com Roslyn 5.8 e pacote 1.3.2, ganhando 51 correspondências sem perdas com o antigo. A recuperação prossegue por ordem dos membros, variantes de dois decompiladores e verificação por método. Em Assembly-CSharp, a configuração validada usa stdlib default; a chamada original de IndexOf foi restaurada. Metadados completos e identidade dos arquivos DLL ainda estão pendentes. [KairoLibrary](docs/notes/kairo-baseline.md), [Assembly-CSharp](docs/notes/assembly-csharp-baseline.md).

As sete DLLs externas correspondem por SHA-256 à distribuição oficial do Unity 5.3.4f1, incluindo UnityEngine.dll do módulo Android. As bibliotecas nativas também correspondem ao player Mono Release. [Prova Android](docs/notes/unity-android.md).

## Arquivos de trabalho

| Caminho | Finalidade |
|---|---|
| `config/methods.csv` | Cadastro mestre de métodos, com assinatura completa e token original |
| `config/types.csv` | Ordem original dos tipos e relação entre tipos aninhados |
| `config/compile-order/` | Ordem inicial dos arquivos por assembly, derivada de TypeDef |
| `config/inventory.csv` | Hash e categoria de cada uma das 1.218 entradas do APK |
| `src/Assembly-CSharp/`, `src/KairoLibrary/` | Fontes recuperados, um tipo superior por arquivo |
| `ref/` | Listagens IL geradas e mapas de tokens; referência, nunca entrada de build |
| `original/` | APK e extração somente leitura |
| `build/` | Saídas e relatórios de verificação |
| `docs/toolchain.md` | Ferramentas, evidências e pendências por DLL |

`original/`, `ref/`, `analysis/` e `build/` ficam fora do Git. A estrutura genérica anterior foi preservada em `analysis/legacy-generic-structure/`.

## Comandos disponíveis

Execute na raiz do projeto, com Python 3.12:

```powershell
python tools/fetch_roslyn.py
python tools/regenerate_references.py
python tools/build
python tools/verify --all
python tools/progress
python tools/verify --all --reference-only
python tools/test_comparison.py
```

Pré-requisitos locais: Windows, Python 3.12, Unity 5.3.4f1 (`UNITY_ROOT` pode indicar outra pasta), compilador .NET Framework e sua própria cópia do APK extraída em `original/`. Execute explicitamente `python tools/fetch_roslyn.py` para obter o pacote portátil 1.3.2 com hash fixado. O comparativo moderno também requer o Roslyn 5.8 instalado, indicado por `ROSLYN_CSC` se necessário. Nenhum desses binários acompanha o repositório.

Os dois builds retornam 0. A verificação retorna **1 enquanto houver diferenças conhecidas**, e retornaria **2** se todos os corpos batessem mas a comparação completa dos metadados ainda estivesse pendente. Isso não declara reconstrução concluída. `--reference-only` verifica a referência e os recibos locais. Em um clone novo, regenere `ref/` a partir das DLLs da sua cópia, execute a verificação integral para criar os recibos ignorados e então gere o progresso. Os retornos diferentes de zero devem ser examinados no relatório, não ignorados como sucesso.

`python tools/extract` gera os cadastros e `ref/` a partir das evidências REA retidas e dos arquivos originais. Ele recusa sobrescrever `methods.csv`, para preservar estados. Os complementos de metadados são gerados com `tools/rea_metadata.mjs`, cujo adaptador exige REA 6.5.0. O procedimento está em [docs/notes/reference.md](docs/notes/reference.md).

O plano de execução está em [DECOMP_PLAN.md](DECOMP_PLAN.md), e o estado atual em [PROGRESS.md](PROGRESS.md).
