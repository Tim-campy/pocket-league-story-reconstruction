# Team.Update_coatEvaNewOffence — proposta privada

Estado: READY_FOR_GAME_PROBE; resultado do alvo UNKNOWN. Nenhuma compilação do jogo, execução do alvo, verificação oficial ou alteração pública foi feita nesta frente.

## Vínculo e diagnóstico

Método `game.Team::Update_coatEvaNewOffence()->System.Void`, token `0x060003a8`, 723 bytes de IL. Base congelada B9/commit0059982 em `build/round12/runs/b09/commit/postverify01/Assembly-CSharp`. `bindings.json` guarda SHA do artefato original, dumps, receipt, manifest, fonte e spans UTF16. REA primário `ev_a3d9e761f9901f0e874d587ae3be0eab9b4c94051b7070389400391caa204633`; IL SHA256 `76abf62af647ff30b49ae7055a4954ef9fbacd7aa0585daf01cd93cc5c4fffc8`. O SHA do artefato original, diferente do hash de IL, é `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`.

Original e reconstruído têm fat12, flags19, maxstack16, EH0 e 723IL. O reconstruído tem 17 locais, contra16 originais, por um Boolean artificial no slot2. A primeira diferença bruta está no offset85. O original desvia diretamente com `brtrue`; o fonte atual materializa a igualdade em Boolean antes do teste. O modelo genuíno prevê corretamente17 no fonte atual e16 na proposta; não há defeito do modelo nesta medida.

## Composição calculada

1. Remover somente o wrapper `bool recoveredCondition_27548 = team_id_ == 0; if(recoveredCondition_27548)` e usar `if(team_id_ == 0)`, conservando os dois braços. Há exatamente dois usos desse identificador; nenhum escapa.
2. Nos dois ramos, substituir `if(ChPAreaLineOver()){loops}` por `if(!ChPAreaLineOver()){}else{loops}`. Cada chamada continua sendo avaliada uma vez; loops, folhas e comparações numéricas permanecem iguais. Os corpos não contêm transferências break/continue/goto/return.

Retirar apenas o Boolean prevê713IL: some sua materialização, mudam os índices posteriores e o float atual4 passa a usar opcodes compactos do slot3. Restam dois controles reais. No original: offset230 `brtrue240; br315` e offset472 `brtrue482; br573`. No atual, os guards estão em240 e477, antes da remoção do Boolean. Cada else vazio restaurado acrescenta5bytes. A composição completa volta a723IL. O cálculo de índices, larguras e rótulos em `abstract-report.json` coincide nos323 objetos de instrução originais, com zero diferenças. Isto é diagnóstico calculado, não emissão nem aprovação do alvo.

`slot-constraints.json` registra primeiro/último uso e stores/loads dos16 slots: i,j,k,num,l,m,n,num2,num3,num4,num5,num6,team,cEVA_PASSER_MP,num7,player. O Boolean removido não existe na assinatura original. NoEmit completo aprova a sequência original integral, sem bypass de previsão completa incorreta.

## Regra de emissão

Fonte Mono fixado na revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, arquivos em `analysis/tools/round5-mono-source/mcs/mcs`:

- `statement.cs:225–267`, `If.DoEmit`: emite teste para false_target, TrueStatement; a existência de FalseStatement emite `br end` nas linhas251–257 antes do braço falso. Assim, o braço verdadeiro vazio continua produzindo o salto incondicional necessário.
- `expression.cs:553–558`, `Unary.EmitBranchable`: LogicalNot inverte `on_true` da expressão, sem materialização Boolean. Aqui o teste é Boolean, sem inversão de relação de ponto flutuante.
- `statement.cs:2032–2047`, `Block.EmitMeta`: resolve variáveis do bloco, temporários registrados e filhos. Remover o Boolean explica os slots posteriores; nenhuma renomeação ou reorganização de membros é usada.

Os SHA desses arquivos e dos produtores/modelos ficam em `freeze.json`.

## Ensaios próprios

Seis compilações smcs2.6.5 de fixtures próprias, com referências e flags da base, saída privada `Fixture.dll`, `-debug-`, sem PDB/MDB de produção. O candidato e um controle manual independente coincidem integralmente em cabeçalho, locais, IL e EH. Ambos têm722IL,16locais, fat12/flags19/maxstack16/EH0. O fixture possui campo estático CEVA_PASSER_MP2; o original possui `ldarg.0@608; ldfld@609`, explicando o byte adicional do método do jogo. Não se apresenta o fixture como reprodução do alvo.

Controles negativos: fonte anterior722IL/17locais diverge; somente remoção do Boolean712IL/16 diverge; somente guards732IL/17 diverge; polaridade errada no primeiro guard722IL/16 também diverge. Três negativos do produtor rejeitam escape do Boolean, apenas um guard e introdução de break no corpo. Todos os negativos estão retidos, sem repetição de busca.

## Histórico e limites

Foram examinados86 receipts de método em reports existentes, com49 registros de trials incluindo repetições. R4 adotou `condition-local`, prefixo75→85; R4 teve21 trials e R5 teve28 sem novo avanço. R4 tentou condition-local, hoist, tail-return e if-polarity; R5 tentou aliases de campo, hoist, condition-local, tail-return e if-polarity. A hipótese atual retira essa adoção antiga e compensa dois controles posteriores ao mesmo tempo. A retirada isolada seria regressiva no prefixo devido aos destinos deslocados: não deve ser adotada separadamente. Não foi encontrado hash da proposta inteira nos trials capturados; ausência de payload/SHA em parte dos receipts impede provar que todas as formas históricas foram excluídas.

A API pura `transform_offence(method_source)->str` não faz IO nem compila. `patches.json` contém o método completo before/after, CRLF original, spansUTF16 e hashes. Para adoção após eventual KickOff no mesmo Team.cs, o executor precisa rebase único por corpo e novos hashes do arquivo; o before deste método foi conferido uma vez no Main observado. Resultado de corpo, metadados e regressões globais permanece UNKNOWN até o oráculo oficial. Não há ganho contado nesta entrega.
