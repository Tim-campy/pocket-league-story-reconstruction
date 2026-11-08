# Diario da rodada 12

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | KairoLibrary / INSERTION_POINT => stloc.2 | Move the named cache value inside indexer assignment, removing the extra assignment-result temporary; struct getter receiver allocation is predicted by corpus-validated model12. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 251 |
