# Receptor nomeado no mesmo try — D18

Base3112371e8c0076bc4c68eccffa8586a9fea32e05, medição fresca `reports/round9-cycle17-after-measurement.json`. Alvo FormManagerBase._execute1.088 bytes, original16 locais e atual15, faltando NotifyManager no slot8. A hipótese é isolada; Balloon/Offscreen têm permutações, Language extras diferentes, Capture faltaBoolean. Não há segundo/terceiro análogo de receptor nomeado demonstrado.

Original Notify: GetInstance194, stloc8@199, ldloc8@201, teste, ldloc8@205, callvirt UpdateValues207; são exatamente três acessos ao slot8, sem uso posterior escondido. O retorno protegido já grava slot9 em43/70/173, enquanto o atual grava Boolean8. Declaração no mesmo try fica reservada antes de LazyReturnTemp, mesmo aparecendo textualmente depois do primeiro return.

Fonte proposta: `NotifyManager recoveredNotifyManager=NotifyManager.GetInstance(); if((object)recoveredNotifyManager!=null)recoveredNotifyManager.UpdateValues();`. A expressão é avaliada uma vez; não há novas chaves nem bloco filho que mudaria alocação. O cast object garante teste de referência, sem pressupor operator==. Nome fresh escolhido, histórico UNKNOWN. A regra antiga de ?. exige receptor estável e rejeita a invocação GetInstance; não criava esse local nem alcançava a chamada quando o primeiro erro mapeava ao return43.

Roslyn fixado4cb203dab89e1542cdf7d88cb6d32767bc0e5460: EmitStatement594–612 define locais do bloco antes dos comandos; CodeGenerator139–173 LazyReturnTemp; Optimizer1153–1159 RecordBranch,1574–1605 dummies e LocalDefUseSpan310–325 explicam por que as duas leituras separadas pelo teste podem conservar o local. Não se acrescenta leitura artificial ou argumento ref para forçar isso.

Fixtures próprias: três corpos completos idênticos,9 builds, quatro controles negativos; comprovaram local antes do retorno protegido, callvirt e colisão de nome gerando sufixo2. O produtor rejeita alvo em comentário, duplicado ou em bloco filho. A assinatura prevista do jogo é **INCOMPLETA**,17 tipos contra16, portanto não é alegada correta nem rejeitada como previsão completa; candidato permanece no limite12 e deve provar assinatura real16 e corpo na compilação. Recibos reports/round9-named-receiver-fixtures.json e reports/round9-named-receiver-patches.json ligam fonte, produtor, modelo e fixture por hash.

Nenhuma classe gerada é escrita, nenhum IL é copiado ou normalizado, nenhum estado é editado manualmente. Esta etapa não presume que corrigir o receptor fecha eventuais divergências posteriores do método.

Resultado D18: assinatura real16 confirmada e payload aceito44→46, sem novo matched; uma regra aprendida avançou o alinhamento mantendo prefixo bruto46. O limite de12 candidatos incompletos por passagem foi atingido e respeitado. Zero perdas; commit55f1c2fe7f4c198c8cca75a5edf4b3fd3d32db50. Receptor/ordem local estão confirmados nessa aplicação, suficiência para corpo inteiro não. Não repetir o mesmo payload isolado para tentar contornar o limite.
