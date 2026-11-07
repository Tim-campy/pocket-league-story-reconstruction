# Rodada 11: caminho histórico do PDB

O CodeView RSDS da KairoLibrary original grava exatamente:

```text
E:\workspace\Library\KairoLibrary\obj\Android\KairoLibrary.pdb
```

O leitor `tools/pe_codeview.py` valida os cabeçalhos PE, o diretório de debug,
o mapeamento RVA/offset e a existência de um único registro RSDS. A identidade
do original foi corroborada pelo REA, SHA-256
`bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`,
MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`; evidência primária retida em
`reports/round11-KairoLibrary-rea-artifact.json`.

O sistema não tinha uma unidade E: ocupada. O helper de build criou um alias
SUBST E: para `build/historical-pdb-drive` deste projeto e produziu o PDB
naquele caminho absoluto usando a opção `/pdb:` do csc 1.1.0.51109.
Não houve instalação de software. O helper verifica a identidade do alias;
se outra unidade ocupar E:, ele recusa antes de escrever nela.

O arquivo PDB gerado é espelhado no diretório canônico do build para os
leitores de pontos de sequência existentes. A DLL é emitida diretamente pelo
compilador e nunca editada após a compilação. O manifesto registra caminhos,
hashes do helper/configuração e SHA-256 do PDB produzido.

`reports/round11-pdb-path.json` mede a mudança contra o snapshot b1116f9:

| Medida | Antes | Depois |
|---|---:|---:|
| Bytes diferentes no caminho e preenchimento CodeView | 97 | 0 |
| Tamanho do registro RSDS | 284 | 284 |
| Registro inteiro, excluído somente o GUID comprovado | Diferente | Idêntico |

A verificação completa preservou os estados de todos os métodos, os corpos
recompilados anteriores e os metadados dos 466 tipos. Ganhos/perdas: 0/0.
As demais opções e referências de compilação não foram trocadas.

O alias pode ser refeito pelo helper depois de reiniciar o Windows. A saída
física continua dentro de `build/`, fora do Git. O caminho original foi
reproduzido neste sistema; não há limitação de caminho pendente.
