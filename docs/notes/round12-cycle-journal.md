# Diario da rodada 12

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | KairoLibrary / INSERTION_POINT => stloc.2 | Move the named cache value inside indexer assignment, removing the extra assignment-result temporary; struct getter receiver allocation is predicted by corpus-validated model12. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 251 |
| 2 | KairoLibrary / stloc.0 => dup; INSERTION_POINT => dup | Pause requires two named Notify receivers and an explicit false-arm scope; Transpose requires sixteen explicit stores with the array consumed by its final read, without writing the field. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 396 |
| 3 | KairoLibrary / ldelem.any => ldelema | Copy the GameAppli array element into a typed local before reading Pid, preserving original value-copy emission; incomplete model prediction remains diagnostic and emitted five-local signature is mandatory. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 192 |
