# Diario da rodada 10

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / switch targets => switch targets | Reemitir controle completo com folhas existentes e slots calculados remove o case25 vazio ausente no dispatch original | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 717 |
| 2 | Assembly-CSharp / br => ret | Ramo mode2 com saida comum e cadeia direta 0/1/3 remove cache do switch e preserva cinco slots | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 412 |
| 3 | Assembly-CSharp / brtrue => brfalse | Inverter o ternario do campo estatico com vinculo AST e stsfld preserva o cctor implicito e reproduz a polaridade original | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 1 | 1816 |
