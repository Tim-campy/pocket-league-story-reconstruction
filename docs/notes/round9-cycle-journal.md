# Diario da rodada 9

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | KairoLibrary / stloc.s=>stloc.s | FilaA: quatro scratch locais compartilhados nos blocos de GetCode reproduzem os31slotsoriginais; candidatos exclusivamente do bundle preparado. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1326 |
| 2 | KairoLibrary / stloc.s=>stloc.s | FilaA: contador no escopo do laco e inline do alias de array em DrawArc; modelo incompleto pode compilar sob orcamento e assinatura real decide. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1188 |
| 3 | KairoLibrary / brfalse.s=>brtrue.s | FilaA: ternario negativo preserva orientacao call-first e callvirt do original; teste do candidato preparado sem normalizacao. | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |
| 4 | Assembly-CSharp / stloc.s=>stloc.s | FilaA: coalescencia de coordenadas e escopos calculados preservam inicializadores e igualam assinaturas originais. | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 4316 |
| 5 | Assembly-CSharp / ldarg.0=>dup | FilaA: *= em campos float ou double de receiver estavel vira atribuicao explicita com recarga do receiver, sem mudar ordem ou agrupar RHS. | Avanco parcial; nenhum novo matched; 9 tentativas da regra, 9 avancos | 0 | 0 |
