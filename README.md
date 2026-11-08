# Pocket League Story

Reconstrução do código C# de `Assembly-CSharp.dll` e `KairoLibrary.dll`, seguindo [AGENTS.md](AGENTS.md). Um método só estará concluído quando a recompilação corresponder ao corpo original, com tokens resolvidos, cabeçalho, locais e exceções conferidos.

A estrutura cobre **5.245 métodos e 466 tipos**. Os dois assemblies foram recuperados e compilam integralmente: KairoLibrary tem 163 fontes C# e usa o candidato Microsoft.Net.Compilers 1.1.0; Assembly-CSharp tem 97 fontes e usa smcs 2.6.5.0. Os números atuais de métodos e bytes IL correspondentes são gerados em [PROGRESS.md](PROGRESS.md), com recibos vinculados aos fontes e ao build.

**KairoLibrary está CONCLUÍDA**: os 3.287 métodos e 337 tipos coincidem, e a comparação literal do arquivo inteiro não encontra diferenças fora dos quatro campos comprovadamente não determinísticos. O critério e as provas estão em [fechamento byte a byte](docs/notes/round12-kairo-byte-closure.md); [PROGRESS.md](PROGRESS.md) vincula o estado à comparação renovada após cada compilação. As diferenças de corpos e a comparação final de tabelas/heaps ainda pendentes pertencem ao Assembly-CSharp.

As versões históricas exatas continuam `UNKNOWN`. Matrizes de compiladores e referências sustentam a seleção atual em [docs/toolchain.md](docs/toolchain.md). As definições estruturais dos 466 tipos coincidem. A recuperação do Assembly-CSharp usa a primeira diferença de IL para mapear um comando por MDB e testar transformações individuais, preservando o prefixo correto e rejeitando regressões; a configuração validada usa stdlib default. As linhas de base anteriores permanecem documentadas em [KairoLibrary](docs/notes/kairo-baseline.md) e [Assembly-CSharp](docs/notes/assembly-csharp-baseline.md).

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
python tools/fetch_roslyn_neighbors.py
python tools/regenerate_references.py
python tools/build
python tools/verify --all
python tools/progress
python tools/verify --all --reference-only
python tools/test_comparison.py
```

Pré-requisitos locais: Windows, Python 3.12, Unity 5.3.4f1 (`UNITY_ROOT` pode indicar outra pasta), compilador .NET Framework e sua própria cópia do APK extraída em `original/`. Execute explicitamente `python tools/fetch_roslyn_neighbors.py` para obter os pacotes portáteis 1.0.0, 1.1.0 e 1.2.0 com hashes fixados; o build selecionado usa 1.1.0. Os auxiliares de sintaxe também usam o Roslyn 5.8 instalado, indicado por `ROSLYN_CSC` se necessário, e o runtime .NET 10.0.10 registrado na toolchain. Nenhum desses binários acompanha o repositório.

Os dois builds retornam 0. A verificação retorna **1 enquanto houver diferenças conhecidas**, e retornaria **2** se todos os corpos batessem mas a comparação completa dos metadados ainda estivesse pendente. Isso não declara reconstrução concluída. `--reference-only` verifica a referência e os recibos locais. Em um clone novo, regenere `ref/` a partir das DLLs da sua cópia, execute a verificação integral para criar os recibos ignorados e então gere o progresso. Os retornos diferentes de zero devem ser examinados no relatório, não ignorados como sucesso.

`python tools/extract` gera os cadastros e `ref/` a partir das evidências REA retidas e dos arquivos originais. Ele recusa sobrescrever `methods.csv`, para preservar estados. Os complementos de metadados são gerados com `tools/rea_metadata.mjs`, cujo adaptador exige REA 6.5.0. O procedimento está em [docs/notes/reference.md](docs/notes/reference.md).

O plano de execução está em [DECOMP_PLAN.md](DECOMP_PLAN.md), e o estado atual em [PROGRESS.md](PROGRESS.md).

O [motor da rodada 6](tools/round6_engine.py) opera sobre o snapshot congelado em `build/round6/baseline/`, com tentativas isoladas, checkpoints e limites explícitos. O modelo smcs calcula os slots a partir do fonte e filtra candidatos antes da compilação; o alinhamento de instruções localiza as edições adiante de desvios deslocados. A busca parcial exige avanço bruto por padrão; a retomada indicada dos gradientes usa avanço de instruções alinhadas, preservando os bytes que já coincidiam. Todos os corpos já iguais permanecem protegidos, e somente o comparador exato promove matched. Esses artefatos locais são gerados da cópia do APK e dos fontes do commit-base; não acompanham um clone. A triagem opcional exige igualdade do corpo atual antes de ser habilitada e recompilação completa real de cada avanço. O [relatório da rodada](docs/notes/round-06-report.md) contém as medições completas, a cobertura do modelo e as limitações; nenhum estado é promovido pelo modelo ou pela triagem.

O [relatório completo da rodada 7](docs/notes/round7-report.md) registra os vinte ciclos autônomos, o progresso em métodos e bytes, as validações dos dois modelos de slots, as hipóteses refutadas e os exemplos de IL dos grupos restantes. Ele é autossuficiente para análise sem acesso ao repositório.
