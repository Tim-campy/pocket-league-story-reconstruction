# OnTouchCamera — seções vazias distintas

Entrega privada READY_FOR_GAME_PROBE; corpo do jogo e regressões oficiais UNKNOWN. Zero compilações do alvo, verificadores oficiais, alterações públicas ou de estado. Somente este método foi estudado; todos os pacotes anteriores permanecem congelados.

## Identidade e diferença real

`public void OnTouchCamera(int type,int v,TouchEvent e)`, ID `form.SoccerForm::OnTouchCamera(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void`, token0x060001ea. Base oficial B13 em `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`.

Original322IL e atual289IL; fat12, flags19, maxstack20, EH0 e os mesmos cinco locais: Single0,Single1,Int32[]2,Int32 3 e temporárioInt32 4 do switch. A primeira diferença bruta/causal alinhada é offset41: opcode switch original contra ldc.i4.1 do atual. A fonte atual já contém switch(type); o opcode diferente é escolha de lowering causada pelo inventário de casos, não evidência de que se deve trocar todo switch por if.

REA primário `ev_e5687c2968879a8919db2cf71fd08d405dc0045b5a3975dbe2cf5f9e6af85fae`, pai `ev_630f98352807fac305364880ca215fc4c5b94a049f8f76f6d69e71cfb2f83414`. Artefato original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. `bindings.json` guarda hash dos322bytes de IL conferido no dump, hashes de fonte/receipt/manifest/dumps e spansUTF16. O método foi extraído pela declaração, não pela chamada anterior no arquivo. Before ocorre uma vez no Main observado; mudanças posteriores em SoccerForm.cs exigem rebase por corpo único e vínculos novos.

## Composição proposta

O original em41 despacha valores0–5 por tabela `[92,123,70,70,70,102]`:0 vai92,1 vai123,2/3/4 caem no fallback70,5 vai102. O fallback compara8 em73 para97 e12 em82 para229; os demais vão321.321 é ret. Em92 e97 existem dois br distintos para321, sem folhas entre eles.

A proposta insere, ANTES do caso5 existente, duas seções separadas: `case0:break; case8:break;`. Não as agrupa em `case0: case8: break;`, pois isso fundiria os destinos92/97. A ordem textual das seções passa a0,8,5,1,12, preservando integralmente os corpos5/1/12. Não há loop nem EH no método; todos esses breaks pertencem ao switch. Os branches de encerramento118,224 e316 também chegam ao mesmo ret321.

Uma diferença posterior independente está no teste vazio do caso5: original `callvirt Size@108; brtrue113→118`, atual `callvirt Size@75; brfalse80→85`. A proposta muda somente `if(touchCameraPoint_.Size()!=0){}` para `if(touchCameraPoint_.Size()==0){}`. O corpo foi confirmado vazio; Size continua avaliado uma vez, com as mesmas possíveis exceções/efeitos. Não altera nenhum teste de ponto flutuante ou chamada do caso1. Inserir somente os rótulos dá o tamanho certo322 mas mantém esse opcode errado; a condição é parte indispensável da composição.

As expressões e.dx*100/scale_ e e.dy*100/scale_, inclusive conversão para Single depois da divisão, permanecem textualmente iguais. Mantêm-se Set da velocidade/decimal, avaliação de GetScaleMin/GetScaleMax, Clamp/ClampMin, valores dos campos e sequência dos casos reais. Inventário e ordem de chamadas permanecem iguais.

O cálculo explícito do novo dispatch, larguras e relocação produz exatamente111objetos de instrução originais,322IL, com zero diferença, locais/EH iguais. Isso é diagnóstico analítico, não um build nem aprovação do jogo. Nenhum IL original foi copiado para um resultado.

## Regra do smcs e NoEmit

Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs/statement.cs`:

- TableSwitchEmit3375–3380 descreve grupos pelo menos meio cheios;3392–3423 inicializa blocos e os une quando `(SizeA+SizeB)*2 >= TotalLength` em3407. Adicionar0 torna0,1,5 um grupo de três chaves em seis posições, exatamente meio cheio.8 e12 continuam esparsos.
-3463–3480 usa comparações diretas para blocos de comprimento até2 e tabela para os maiores. Assim, o current1/5/12 esparso usa apenas comparações, enquanto a composição emite a tabela0–5 e fallback8/12.
-3552–3572 emite salto para default e depois percorre Sections em ordem textual, marcando rótulos e emitindo cada corpo. Duas seções vazias com break continuam duas instruções, como92 e97; fundi-las elimina uma.
-3823–3842 mantém o mesmo temporário de switch no slot4;3852/3869 define e marca sua saída. Break1117–1119 emite br para esse destino. If.DoEmit225–267 testa o falso da expressão; ==0 vazio explica brtrue sem inventar uma folha.

NoEmit genuíno before e candidato: complete=true, cinco tipos originais exatos, helper existente/hash conferido. Nenhum estado complete incorreto foi relaxado para incomplete. Os locais e suas âncoras completas estão em slot-constraints.json.

## Fixtures e negativos

Seis compilações smcs2.6.5 de fixtures próprias com flags/referências fixadas, saída privada Fixture.dll e `-debug-`, sem sidecar de produção. Candidato e controle manual independente com variáveis renomeadas coincidem integralmente em cabeçalho/locais/IL/EH:322IL, maxstack20, cinco locais, fat12/flags19/EH0.

Resultados negativos preservados: before289 difere; somente labels322 ainda difere na condição vazia; somente condição289 mantém dispatch errado; seções vazias fundidas317 também diferem. Igualdade de tamanho não foi usada como aprovação. O controle manual confirma a forma calculada no compilador histórico, não prova o corpo real do alvo. Os stubs próprios não são o jogo.

Três negativos do produtor rejeitam inventário de casos diferente, corpo não vazio no teste Size e condição diferente. Não foram compilados. A API pura não faz IO nem emite código.

## Histórico e entrega

Capturados91receipts do método e22trials incluindo repetições. R4 tentou switch-if, split-and, condition-local e hoists; R6/R7ParteA tentaram agrupamento, troca de declarações, hoist e split-and. R7C06 tentou cinco variantes empty-arm, inclusive `if(Size()==0){}else{}` isolado, sem avanço41. R11C07 tentou mirror-primitive-operands, também41. Não se repete nenhum desses candidatos isolados: a proposta atual restaura o inventário/ordem de seções e a polaridade do teste vazio juntos. Nenhum hash da proposta inteira apareceu; receipts sem payload/SHA não excluem universalmente toda forma histórica. Zero candidatos não é refutação.

API `calculate.py::transform_touchcamera(method_source)->str`. patches.json guarda o método normal completo before/after e spansUTF16; ready-for-game-probe.json aponta produtor, pacote e freeze. A produção e a verificação oficial completa pertencem exclusivamente ao executor. Nenhum ganho foi contado nesta entrega.
