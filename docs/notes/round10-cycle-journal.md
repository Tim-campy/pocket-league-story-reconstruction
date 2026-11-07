# Diario da rodada 10

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / switch targets => switch targets | Reemitir controle completo com folhas existentes e slots calculados remove o case25 vazio ausente no dispatch original | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 717 |
| 2 | Assembly-CSharp / br => ret | Ramo mode2 com saida comum e cadeia direta 0/1/3 remove cache do switch e preserva cinco slots | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 412 |
| 3 | Assembly-CSharp / brtrue => brfalse | Inverter o ternario do campo estatico com vinculo AST e stsfld preserva o cctor implicito e reproduz a polaridade original | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 1 | 1816 |
| 4 | Assembly-CSharp / br => ret | Compor nove ajustes de controle e escopos preserva slots43-49 e91-92, calcula cadeia67-69 e remove o cache de switch93 | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 12484 |
| 5 | Assembly-CSharp / brfalse => brtrue | Guarda positiva no construtor com escopo e atribuicao composta originais | Novos corpos aprovados; 11 tentativas da regra, 1 avancos | 1 | 347 |
| 6 | KairoLibrary / brfalse.s => brtrue.s | Separar a guarda invalidTouch e manter corpo positivo com escopos e stores originais em Canvas.OnInputUpdate | Novos corpos aprovados; 13 tentativas da regra, 3 avancos | 2 | 600 |
| 7 | Assembly-CSharp / bne.un => beq | Restaurar por composicao os ramos vazios terminais, OR separado e cauda positiva nos seletores Int32 | Novos corpos aprovados; 6 tentativas da regra, 6 avancos | 6 | 3458 |
| 8 | KairoLibrary / brtrue.s => ldc.i4.4 | Separar licenca zero e restaurar switch denso com bucket default nas entradas um a tres | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1054 |
| 9 | Assembly-CSharp / brtrue => ldc.i4.0 | Restaurar os cinco inicializadores Boolean como ternarios orientados pelo IL, inclusive DEBUG com dois ramos false | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 341 |
