# Diario da rodada 7

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / bne.un => beq | Guarda terminal recuperada como retorno deve ser um if positivo envolvendo o restante do bloco final | Novos corpos aprovados; 18 tentativas da regra, 13 avancos | 7 | 14902 |
| 2 | Assembly-CSharp / br => ret | Quando o br original termina no ret comum, if com ramo verdadeiro vazio e else preserva o desvio explicito | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 6 | 2669 |
| 3 | Assembly-CSharp / br => ret (destino incremento do laco) | O desvio original para incremento do for exige continue na guarda recuperada com return | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 5 | 4608 |
| 4 | Assembly-CSharp / brfalse => brtrue | Em if vazio, inverter apenas o ultimo operando booleano preserva chamadas e reproduz a polaridade original | Novos corpos aprovados; 4 tentativas da regra, 3 avancos | 2 | 658 |
