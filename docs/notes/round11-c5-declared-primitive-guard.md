# Limite de pureza das comparações C5

Uma contraprova própria encontrou uma lacuna na ampliação com constantes: o tipo convertido podia ser Int32 enquanto o identificador tinha um struct com conversão implícita definida pelo usuário. O operador de comparação podia continuar sendo o builtin. Nesse caso, espelhar dois operandos muda a ordem das chamadas de conversão e pode mudar efeitos observáveis.

O repertório agora exige que o tipo declarado de cada identificador seja Int32, UInt32, Int64, UInt64, Single ou Double, além de exigir tipos convertidos compatíveis e nenhuma conversão ou operação definida pelo usuário. Constantes numéricas seguras continuam permitidas; a negação externa permanece intacta.

`reports/round11-c5-conversion-guard-fixtures.json` registra 12 verificações negativas de análise e a contraprova nos dois compiladores: a conversão dos valores 1 e 2 registra 12 na ordem original e 21 na forma espelhada. O estudo teve oito compilações próprias, incluindo duas bibliotecas de tentativas anteriores e dois runners. O runner herdado exigia métodos ausentes; a primeira substituição tinha o mesmo nome de assembly da biblioteca. Ambos os erros de infraestrutura foram registrados e corrigidos num Runner.exe distinto. Nenhum alvo do jogo foi compilado por esse estudo.

`reports/round11-c5-twelve-trial-semantic-audit.json` auditou os 12 candidatos C5 realmente compilados nos snapshots congelados: todos os operandos tinham tipo declarado e convertido Int32, sem conversões ou operadores definidos pelo usuário. Nenhum resultado negativo anterior foi invalidado e nenhum mirror havia sido adotado no jogo. Os vetos anteriores não são convertidos em testes de corpo.

`reports/round11-c5-declared-type-positive-parity.json` confirma que 32 requisições positivas ainda produzem exatamente os patches previamente compilados nos controles próprios. Não houve mudança nesses corpos de fixture nem na semântica de ponto flutuante testada. A correção restringe a admissão do repertório; não altera o oráculo ou o código já adotado.
