# Pocket League Story

Reconstrução do código C# de `Assembly-CSharp.dll` e `KairoLibrary.dll`, seguindo [AGENTS.md](AGENTS.md). Um método só estará concluído quando a recompilação corresponder ao corpo original, com tokens resolvidos, cabeçalho, locais e exceções conferidos.

A estrutura está preparada para **5.245 métodos e 466 tipos**. Nenhum método foi recompilado ou marcado como `matched`. O **Unity 5.3.4f1** foi instalado, e seu gmcs 2.6.5.0 compilou um teste sintético. Cada DLL do jogo ainda precisa de validação própria do compilador e das opções.

## Arquivos de trabalho

| Caminho | Finalidade |
|---|---|
| `config/methods.csv` | Cadastro mestre de métodos, com assinatura completa e token original |
| `config/types.csv` | Ordem original dos tipos e relação entre tipos aninhados |
| `config/compile-order/` | Ordem inicial dos arquivos por assembly, derivada de TypeDef |
| `config/inventory.csv` | Hash e categoria de cada uma das 1.218 entradas do APK |
| `src/Assembly-CSharp/`, `src/KairoLibrary/` | Destinos dos futuros fontes, um tipo de nível superior por arquivo |
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
python tools/verify --all
python tools/build
```

`--reference-only` verifica a referência e pode retornar 0. Os comandos de build e verificação da reconstrução retornam **2, bloqueado**, enquanto faltarem fontes, validação do compilador e implementação da comparação completa. Isso não representa correspondência alcançada.

`python tools/extract` gera os cadastros e `ref/` a partir das evidências REA retidas e dos arquivos originais. Ele recusa sobrescrever `methods.csv`, para preservar estados. Os complementos de metadados são gerados com `tools/rea_metadata.mjs`, cujo adaptador exige REA 6.5.0. O procedimento está em [docs/notes/reference.md](docs/notes/reference.md).

O plano de execução está em [DECOMP_PLAN.md](DECOMP_PLAN.md), e o estado atual em [PROGRESS.md](PROGRESS.md).
