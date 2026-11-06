# Diario da rodada 7

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / bne.un => beq | Guarda terminal recuperada como retorno deve ser um if positivo envolvendo o restante do bloco final | Novos corpos aprovados; 18 tentativas da regra, 13 avancos | 7 | 14902 |
| 2 | Assembly-CSharp / br => ret | Quando o br original termina no ret comum, if com ramo verdadeiro vazio e else preserva o desvio explicito | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 6 | 2669 |
| 3 | Assembly-CSharp / br => ret (destino incremento do laco) | O desvio original para incremento do for exige continue na guarda recuperada com return | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 5 | 4608 |
| 4 | Assembly-CSharp / brfalse => brtrue | Em if vazio, inverter apenas o ultimo operando booleano preserva chamadas e reproduz a polaridade original | Novos corpos aprovados; 4 tentativas da regra, 3 avancos | 2 | 658 |
| 5 | Assembly-CSharp / stloc.3 => stloc.0 | Variaveis locais separadas pelo decompilador devem compartilhar declaracao e slot original, junto das construcoes que explicam os temporarios | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 1 | 6626 |
| 6 | Assembly-CSharp / brfalse => brtrue / bne.un => beq (braco vazio perdido) | O smcs preserva br sobre o else mesmo com then vazio; restaurar esse braco explica a condicao invertida e o desvio extra | Novos corpos aprovados; 195 tentativas da regra, 42 avancos | 9 | 14874 |
| 7 | Assembly-CSharp / stloc.s => stloc.s | Ancoras unicas dos calculos e stores provam uma permutacao de locais entre escopos; reordenar declaracoes conserva as avaliacoes | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |
| 8 | Assembly-CSharp / ldfld=>DELETION_POINT | Separar a segunda condição do cabeçalho do laço em guarda com break no início do corpo restaura a ordem original dos blocos smcs. | Novos corpos aprovados; 4 tentativas da regra, 4 avancos | 2 | 889 |
| 9 | Assembly-CSharp / ldstr=>ldstr | Chamar String.Concat com quatro strings preserva os literais adjacentes separados que o operador mais do smcs funde. | Avanco parcial; nenhum novo matched; 2 tentativas da regra, 2 avancos | 0 | 0 |
| 10 | Assembly-CSharp / brtrue=>stloc.s | Os switches recuperados introduzem um seletor temporário ausente no original; convertê-los em ifs com dono de cada break comprovado restaura os blocos e os slots. | Novos corpos aprovados; 11 tentativas da regra, 9 avancos | 4 | 4935 |
| 11 | Assembly-CSharp / br=>ret | Um ramo terminal com ações seguido de return deve ser restaurado como if/else quando o restante do bloco vai à saída comum comprovada do método. | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 6 | 27583 |
