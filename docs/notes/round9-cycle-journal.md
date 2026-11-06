# Diario da rodada 9

| Ciclo | Assembly / par | Hipotese | Resultado | Metodos ganhos | Bytes de IL ganhos |
| --- | --- | --- | --- | --- | --- |
| 1 | KairoLibrary / stloc.s=>stloc.s | FilaA: quatro scratch locais compartilhados nos blocos de GetCode reproduzem os31slotsoriginais; candidatos exclusivamente do bundle preparado. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1326 |
| 2 | KairoLibrary / stloc.s=>stloc.s | FilaA: contador no escopo do laco e inline do alias de array em DrawArc; modelo incompleto pode compilar sob orcamento e assinatura real decide. | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 1188 |
| 3 | KairoLibrary / brfalse.s=>brtrue.s | FilaA: ternario negativo preserva orientacao call-first e callvirt do original; teste do candidato preparado sem normalizacao. | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |
| 4 | Assembly-CSharp / stloc.s=>stloc.s | FilaA: coalescencia de coordenadas e escopos calculados preservam inicializadores e igualam assinaturas originais. | Novos corpos aprovados; 3 tentativas da regra, 3 avancos | 3 | 4316 |
| 5 | Assembly-CSharp / ldarg.0=>dup | FilaA: *= em campos float ou double de receiver estavel vira atribuicao explicita com recarga do receiver, sem mudar ordem ou agrupar RHS. | Avanco parcial; nenhum novo matched; 9 tentativas da regra, 9 avancos | 0 | 0 |
| 6 | Assembly-CSharp / brfalse + br / brtrue | Separar uma guarda protegida no for preserva o salto ao incremento, sem inferir break pelo rotulo | INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos | 0 | 0 |
| 7 | Assembly-CSharp / bne.un + br / beq | Braco verdadeiro vazio e else preservam o join comum; return direto e controle negativo | INCONCLUSIVA: nenhum candidato autorizado pelo modelo/mapeamento; 0 tentativas da regra, 0 avancos | 0 | 0 |
| 8 | Assembly-CSharp / br / ret | Substituir uma guarda de retorno por braco vazio e else para cair no join do if | Novos corpos aprovados; 1 tentativas da regra, 1 avancos | 1 | 999 |
| 9 | KairoLibrary / brfalse / brtrue.s + ret | Corpo sob teste positivo de render com ramo GUI em else preserva saida final | Avanco parcial; nenhum novo matched; 1 tentativas da regra, 1 avancos | 0 | 0 |
| 10 | KairoLibrary / ldc.i4.0 + ceq / brtrue.s | Materializar condicao como valor bool antes do if preserva negacao ceq | Nenhum candidato compilado avancou; 1 tentativas da regra, 0 avancos | 0 | 0 |
| 11 | Assembly-CSharp / complete original control tree => existing-source leaves + calculated slots | Reemitir somente o controle completo com folhas bijetivas e locais calculados reproduz o corpo original | Novos corpos aprovados; 8 tentativas da regra, 8 avancos | 8 | 2269 |
| 12 | Assembly-CSharp / ret => br | Ret original contra join sintetico: encerrar then e retirar else preservando escopo e condicao | Novos corpos aprovados; 7 tentativas da regra, 7 avancos | 2 | 1514 |
| 13 | Assembly-CSharp / brfalse/br + selector stloc => combined flow and named selectors | Calcular num mesmo candidato fluxo e locais de seletores que impediam aprovar as correcoes isoladas | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 1 | 1687 |
| 14 | Assembly-CSharp / br => ret | Br original para join provado contra return recuperado: conservar operador e acoes e estruturar else ate o destino, aceitando somente join local ou braco nao linear novo. | Novos corpos aprovados; 2 tentativas da regra, 2 avancos | 2 | 2552 |
