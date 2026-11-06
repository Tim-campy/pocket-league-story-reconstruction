# Round 8: coordenadas compartilhadas e declara??es precoces

Status dos corpos do jogo: UNKNOWN. Zero compila??es de alvo; apenas an?lise de fonte, assinaturas previstas por modelo independente congelado e fixture smcs independente. O verificador completo decide a ado??o.

Baseline: 25b8108711a4a8920a2600cf9ffea5b710b3a481, build/round8/baseline/Assembly-CSharp. Todos os patches usam before/after da declara??o inteira, UTF-16/CRLF bruto. Foi comprovado rebase ?nico no Main corrente e nova previs?o independente exata.

| M?todo | Bytes IL originais | Locais antes ? original/candidato | Identidades calculadas |
|---|---:|---:|---|
| DebugForm.Draw_coatEva | 1937 | 35 ? 29 | coordenadas6/7; passer14; num10 15 |
| Ball.SetSavePos | 1251 | 16 ? 14 | fatores8/9 antes de flag10 e ?ndice11 |
| DebugForm.Draw_moveArea | 1128 | 22 ? 18 | coordenadas4/5; player11; player2 17 |

Os nomes coordinateX/coordinateY s?o sint?ticos para reconstru??o, n?o recupera??o de nomes hist?ricos. Nenhum membro do original ou classe gerada foi renomeado ou escrito ? m?o.

## Regra e provas

Contextos IL assim?tricos ?nicos, com identidade dos campos e chamadas, ligam cada vari?vel recuperada ao slot original. Todas as declara??es selecionadas t?m pelo menos uma ?ncora inequ?voca; nenhum par contradit?rio ? aceito. O c?lculo exige intervalos de uso recuperados disjuntos antes de unir as declara??es. Todas as inicializa??es continuam no seu ponto de execu??o original; somente as declara??es sobem para o bloco que inclui os controles de fluxo.

A unifica??o sozinha n?o reproduz o invent?rio e a ordem dos tipos: declara??es sem inicializador de passer/num10/player/player2 foram colocadas cedo por uma rodada anterior. O original guarda esses valores pela primeira vez exatamente nas chamadas/atribui??es que ainda aparecem no fonte. O novo primitive retira a declara??o sem inicializador e acrescenta seu tipo ? primeira atribui??o simples. Exige primeira refer?ncia vinculada como alvo da atribui??o, rejeita uso do pr?prio local no RHS, refer?ncias fora do bloco destino, inicializador pr?vio, declaradores m?ltiplos e captura por fun??o an?nima. Para Ball.SetSavePos, os fatores compartilhados precisam ser declarados antes de flag no bloco do m?todo: o menor bloco dos bra?os isolados daria ordem errada mesmo com n?mero correto.

Os slots especiais acima s?o verificados individualmente, al?m da igualdade completa de predicted_types ao original. Todos os tr?s m?todos ficam complete=True no modelo congelado; nenhum erro completo do modelo ? relaxado.

## Fixture

Uma fixture independente comprova o corpo completo can?nico, assinatura local e EH entre a forma de refer?ncia e a forma calculada: Coordinates143 bytes/8 locais; Ball138 bytes/5 locais. Negativos rejeitam captura, primeira atribui??o aninhada em chamada e intervalos intercalados. Apenas um assembly de fixture foi compilado nesta valida??o. Os m?todos do jogo n?o foram compilados nem marcados matched.

## API

Fam?lia calculated-coordinate-local-coalescence. Schema por patch: method_id/logical_path/start/end/before/after; complete/predicted_types; calculator_sha256/syntax_source_sha256/dependency_sha256/model_source_sha256/client_sha256; anchors/proofs/special_variables. Exigir rebase exato e ?nico, previs?o nova usando fonte corrente, avan?o estrito do prefixo e verifica??o completa de todos os corpos para ado??o.

Os contextos de ?ncora normalizam acesso local para fam?lia+tipo e destinos num?ricos de desvio somente durante o diagn?stico; isso n?o normaliza o assembly e n?o altera nenhum crit?rio do verificador final. Os descritores originais s?o somente leitura. Bytecode n?o entra no fonte reconstru?do.
