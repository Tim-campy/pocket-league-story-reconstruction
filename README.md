# Pocket League Story

Reconstrução do código C# de `Assembly-CSharp.dll` e `KairoLibrary.dll`, seguindo [AGENTS.md](AGENTS.md). Um método só estará concluído quando a recompilação corresponder ao corpo original, com tokens resolvidos, cabeçalho, locais e exceções conferidos.

A estrutura cobre **5.245 métodos e 466 tipos**. KairoLibrary foi recuperada em 163 fontes C# e compilada integralmente, sem erros, com Roslyn 5.8 e com o pacote Microsoft.Net.Compilers 1.3.2. O segundo reproduz **2.787 dos 3.211 corpos**, com tokens resolvidos; **2.783 métodos foram promovidos pelo verificador (84,7% da KairoLibrary)**. A comprovação automática por nomes e referências no IL liberou 310 dos 314 corpos inicialmente retidos; quatro continuam sem vínculo comprovado.

O pacote 1.3.2 é o melhor candidato testado, mas a versão histórica exata continua `UNKNOWN`. O arquivo DLL inteiro ainda difere. Assembly-CSharp passou em quatro sondas Mono, mas sua decompilação e compilação integral estão pendentes. Resultados e limites em [docs/notes/kairo-baseline.md](docs/notes/kairo-baseline.md).

## Arquivos de trabalho

| Caminho | Finalidade |
|---|---|
| `config/methods.csv` | Cadastro mestre de métodos, com assinatura completa e token original |
| `config/types.csv` | Ordem original dos tipos e relação entre tipos aninhados |
| `config/compile-order/` | Ordem inicial dos arquivos por assembly, derivada de TypeDef |
| `config/inventory.csv` | Hash e categoria de cada uma das 1.218 entradas do APK |
| `src/Assembly-CSharp/`, `src/KairoLibrary/` | Fontes por tipo superior; KairoLibrary recuperada, Assembly-CSharp pendente |
| `ref/` | Listagens IL geradas e mapas de tokens; referência, nunca entrada de build |
| `original/` | APK e extração somente leitura |
| `build/` | Saídas e relatórios de verificação |
| `docs/toolchain.md` | Ferramentas, evidências e pendências por DLL |

`original/`, `ref/`, `analysis/` e `build/` ficam fora do Git. A estrutura genérica anterior foi preservada em `analysis/legacy-generic-structure/`.

## Comandos disponíveis

Execute na raiz do projeto, com Python 3.12:

```powershell
python tools/progress
python tools/verify --all --reference-only
python tools/build --assembly KairoLibrary
python tools/verify --all --assembly KairoLibrary
python tools/verify --all
python tools/test_comparison.py
```

Pré-requisitos locais: Windows, Python 3.12, Unity 5.3.4f1 (`UNITY_ROOT` pode indicar outra pasta), compilador .NET Framework e sua própria cópia do APK extraída em `original/`. Execute explicitamente `python tools/fetch_roslyn.py` para obter o pacote portátil 1.3.2 com hash fixado. O comparativo moderno também requer o Roslyn 5.8 instalado, indicado por `ROSLYN_CSC` se necessário. Nenhum desses binários acompanha o repositório.

O build Kairo retorna 0; a verificação Kairo retorna **1, diferenças conhecidas**, e a verificação global retorna **2, escopo incompleto**, porque Assembly-CSharp e a comparação de metadados completos estão pendentes. `--reference-only` verifica a referência e os recibos locais, sem declarar reconstrução concluída. Em um clone novo, primeiro recompile/verifique KairoLibrary para regenerar os recibos ignorados pelo Git. Os arquivos `ref/` exigem a extração de evidências descrita abaixo; não são distribuídos.

`python tools/extract` gera os cadastros e `ref/` a partir das evidências REA retidas e dos arquivos originais. Ele recusa sobrescrever `methods.csv`, para preservar estados. Os complementos de metadados são gerados com `tools/rea_metadata.mjs`, cujo adaptador exige REA 6.5.0. O procedimento está em [docs/notes/reference.md](docs/notes/reference.md).

O plano de execução está em [DECOMP_PLAN.md](DECOMP_PLAN.md), e o estado atual em [PROGRESS.md](PROGRESS.md).
