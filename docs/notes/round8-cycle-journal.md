# Diario da rodada 8

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | Assembly-CSharp / stloc.s=>stloc.s | Ancoras unicas de stores e intervalos sem interleaving calculam o bloco comum; reservas apenas para slots originais comprovadamente sem uso. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 1 | 261 |
| 2 | Assembly-CSharp / guard return=>positive if/else | SpForm.Update conserva o bloco positivo antes do retorno alternativo quando a guarda recuperada inverte a ordem original dos blocos. | Nenhum candidato compilado avancou; 6 tentativas da regra, 0 avancos | 0 | 0 |
| 3 | KairoLibrary / stloc.s=>stloc.s | O modelo void try-finally preserva o renderPass vivo no handler; indices e escopos calculados recuperam os 35 slots de Graphics.DrawPolygons. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1563 |
