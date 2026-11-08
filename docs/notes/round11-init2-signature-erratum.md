# Correção da assinatura de SubForm.Init2

A afirmação anterior de que os 68 locais eram iguais confundiu contagem com assinatura. No snapshot inicial desta rodada, 25 posições de tipo diferem: os slots 0–31 mantêm sua posição; os reconstruídos 32–66 correspondem aos originais 33–67; o reconstruído 67 corresponde ao original 32, um Int32 usado pelo seletor de GetMatchInfoJoin.

O pacote `reports/round11-initial-init2-patches.json` recupera um seletor nomeado na posição 32 e compõe os controles documentados pelo analista. A leitura NoEmit privada calculou os 68 tipos na ordem original; os controles próprios e a auditoria abstrata estão vinculados por hash no pacote. Isso é evidência para um candidato de fonte, não uma confirmação do corpo do jogo. O negativo antigo de switch para if isolado continua registrado. A compilação e o verificador ainda precisam decidir a composição integral; nenhum relatório histórico congelado foi reescrito.
