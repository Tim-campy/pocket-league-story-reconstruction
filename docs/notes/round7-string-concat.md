# Rodada 7: preservar os quatro argumentos originais de String.Concat

Publicação preparada após `ca144e1`. A análise abaixo usa o original congelado da rodada 7 e o candidato da execução cycle05. Nenhuma fonte do jogo foi compilada ou modificada por este trabalho.

## Medição e hipótese

O método `form.SubForm::_draw5(kairo.unity.ui.Graphics)->System.Void` tem 12062 bytes originais; o dump cycle05 consultado tinha 12025. Há exatamente duas sequências `ldstr "\t"; ldstr " "` em todo o Assembly-CSharp original, ambas nesse método:

| Ocorrência | Literais originais | Chamada original | Candidato cycle05 |
| --- | --- | --- | --- |
| 1 | 1391: tab; 1396: espaço | 1408: Concat(String,String,String,String) | 1391: tab+espaço; 1403: Concat3 |
| 2 | 3380: tab; 3385: espaço | 3397: Concat(String,String,String,String) | 3370: tab+espaço; 3382: Concat3 |
| Controle vizinho | 1335: tab | Concat3 | Continua Concat3 |

O fonte atual das duas ocorrências divergentes é da forma `playerData.name_ + "\t" + " " + playerData.name2_`. Hipótese: o original usava String.Concat4 explicitamente, mantendo os literais separados. A regra de emissão foi confirmada em fixtures independentes; a correspondência do método inteiro do jogo permanece UNKNOWN até o verificador.

## Evidência do compilador

Foi consultado novamente o snapshot oficial Unity-Technologies/mono `b6587926c0f146c8b12985744f33a13b39a22d57`. O arquivo `mcs/mcs/expression.cs` tem SHA-256 `3e6392384936d08bd7142f8bf68d21ae4f8c429f9c8b0ac099da97bff53c74af`, idêntico aos caches da rodada 5 e ao snapshot pre-release. A revisão exata usada para fabricar o smcs instalado continua UNKNOWN.

- [StringConcat.Append:3818–3837](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/expression.cs#L3818-L3837) combina StringConstant adjacentes antes da seleção da sobrecarga.
- [Append:3841–3848](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/expression.cs#L3841-L3848) incorpora os argumentos de StringConcat aninhado.
- [Emit:3853–3863](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/expression.cs#L3853-L3863) resolve uma chamada comum a System.String.Concat somente depois desse folding.

Uma soma de quatro partes com dois literais adjacentes passa por Append e pode acabar como Concat3. A chamada explícita Concat4 não passa pelo folding dessa soma; a fixture confirma a diferença no executável instalado, além da leitura do código.

## Regra opt-in e evidência do original

`tools/Round7StringConcatRules.cs` expõe `AddRound7StringConcatRules` com a mesma API dos helpers. Exige `round7_cycle_rule=explicit-string-concat-preserve-literals` e `original_string_concat_arity=4`. A existência do arquivo não ativa a regra.

São exigidas quatro partes semanticamente String, soma sem operador definido pelo usuário e duas constantes String adjacentes. A regra emite `global::System.String.Concat` com as quatro expressões originais, na ordem. Não inventa nem divide literais já fundidos. Não reescreve uma subcadeia de quatro partes dentro de uma cadeia maior. Cada campo ou chamada do fonte permanece uma vez na mesma ordem.

O helper Python puro `original_concat_hints(original, offset)`, em `tools/round7_concat_hints.py`, fornece a evidência de aridade. Ele examina até 64 instruções do mesmo trecho sem desvio e considera somente a primeira chamada a System.String.Concat. Exige o formato canônico OLD: chamada estática não genérica, quatro parâmetros class System.String escalares e retorno String. Commas de identidade de assembly não são separadores de parâmetros. Não pula Concat3 para procurar Concat4 mais adiante e não infere aridade do C#.

No sucesso devolve aridade, tipos, offset e assinatura originais; no desconhecido ou ambíguo devolve `{}`. Os pontos 1391 e 3380 provam respectivamente as chamadas 1408 e 3397. O controle em 1335 devolve `{}`.

## Validação histórica

Foi usado o smcs 2.6.5 instalado, SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, com runtime Unity, referências e flags da baseline Assembly-CSharp.

Foram compilados três assemblies artificiais: fonte recuperado com `+`, controle explícito Concat4 e fonte transformado. Quatro corpos transformados foram integralmente iguais aos controles: parâmetros, campos, chamadas com efeitos e constantes nomeadas. Os quatro corpos com `+` diferiram dos controles; não houve acerto vazio. A fixture com chamadas comprova por IL completo que Read(1) precede Read(2), uma vez cada.

Também passaram as recusas de ausência de hint, aridade 3, três partes, cinco partes, quatro partes sem constantes adjacentes, parte Object e operador `+` definido pelo usuário. Os testes puros dos hints exercitam assinaturas mistas, array, genérica, de instância, incompletas, Concat3 antes de Concat4, desvio e saída antes da chamada.

Nenhum assembly alvo, classe gerada, modelo de slots, helper ativo ou estado oficial foi escrito por esta análise. Ganhos de método e bytes pertencem ao recibo do ciclo que efetivamente compilar o candidato.

## Artefatos

- `tools/Round7StringConcatRules.cs`, SHA-256 `6d07715abc01006c6103e58777ae507fb4d37629f956171ff7c7198f3f0581a4`.
- `tools/round7_string_concat_fixture.py` e `reports/round7-string-concat-fixture.json`.
- `tools/round7_concat_hints.py`, `tools/test_round7_concat_hints.py` e `reports/round7-string-concat-hints-tests.json`.
- `reports/round7-string-concat-diagnosis.json`: os dois casos e o controle vizinho, com IL lado a lado.

Reprodução: `python -X utf8 tools/round7_string_concat_fixture.py` e `python -X utf8 tools/test_round7_concat_hints.py`.
