# Rodada10: ampliação independente de árvores smcs

O núcleo AST/IL foi ampliado em quatro etapas sobre o baseline congelado, sem recompilar targets nem editar src, ledger ou arquivos R9. O agregado está em `reports/round10-control-model-summary.json`; contratos e APIs em `tools/round10_control_schema.json`. As DLLs de análise e fixtures são próprias; não são reconstruções do jogo.

## Base e proveniência

Corpus:1838 corpos matched,114 pendentes e6 declarações fora do denominador de corpos. Um dos114 tem corpo igual, retido por vínculo gerado. O recibo conta1844 matched incluindo as6 declarações e1839 corpos iguais incluindo o retido. Não confundir esses denominadores.

O ledger do baseline tem SHA256 `092413cc63d62569be00231f27305cd437819eb859cd072e412361050e0519c3`; os97 fontes conferem com o recibo. A DLL App congelada tem SHA256 `212953c694223ef321f1dcc5d131c2d62d7f0588feb770c17c95cac2b985f6c7`. Frente ao cache final R9, todos1958 DTOs próprios são exatamente iguais; o dump global difere somente no MVID e o recibo somente no hash binário. Não houve normalização de arquivos nem alegação de identidade binária dessas DLLs.

O AST usa apenas C# congelado e metadados originais de mscorlib, KairoLibrary, UnityEngine, System.Core e System. Não recebe IL original, IL próprio ou MDB para inferir estrutura. Os arquivos originais de referência conferiram com os hashes do manifesto congelado. Provas completas estão em `build/round10/control-review/{baseline-authentication,cached-provenance-continuity,semantic-references-provenance}.json`.

## Validações aceitas

Os números abaixo medem cobertura dos modelos de controle, **não métodos novos matched no jogo**. “Árvores iguais” compara somente pares em que ambos os extratores declararam estrutura completa.

| Etapa | Planos matched completos/iguais | Árvores matched pareadas/iguais | Árvores pendentes pareadas/iguais |
|---|---:|---:|---:|
| Base R9 |1459/1459|1267/1267|15/11|
| C1 switch |1487/1487|1267/1267|18/11|
| C2 EH corrigida |1555/1555|1292/1292|19/11|
| C3 lógica corrigida |1555/1555|1305/1305|19/11|
| C4 owners |1555/1555|1305/1305|19/11|

Todos os relatórios aceitos têm inputs estáveis e cobertura exata de instruções. C4 tem66 planos próprios completos/iguais e47 árvores próprias pareadas/iguais, além de66 provas de entradas EH e6 verificações de ownership corretas. Os fixtures foram previstos a partir do fonte antes da sua compilação smcs. Nenhuma compilação de target foi executada por este trabalho de IR.

Cada etapa tem fonte, relatório, hashes e cópia reproduzível em `build/round10/control-review/stage*-snapshot`. As cópias R9 permanecem com os hashes anteriores.

## Moldes e limites

C1 prevê a formação de blocos de keys do Mono: ordena constantes, une blocos com ocupação de pelo menos metade, prioriza os maiores, usa comparações para comprimentos1/2 e tabelas para blocos maiores. Mantém normalização do índice, holes, default branch, seções em ordem do fonte e branches para a instrução seguinte. Árvores completas ficam restritas ao dispatch denso Int32 com join/owners provados. Sparse, híbrido e string têm planos previstos onde suportados, mas não ganham ownership por isso.

No pin UnityMono `b6587926c0f146c8b12985744f33a13b39a22d57`, statement.cs3385–3582 define esse algoritmo. String não constante chama ResolveStringSwitchMap sempre (3698–3700); o código tem TODO para otimizar caso único. O contador switch$map é estático global (3145), logo o contexto de nomes gerados não pode ser adivinhado por método. Não há switch System.String nos nós AST mapeados do corpus; sua prova atual é de fixtures, sem ganho de jogo atribuído.

C2 acrescenta prologue de catch, tipos/ordem dos handlers, leave implícito de Reflection.Emit e endfinally. Preserva o leave duplicado após retorno protegido: FriendData.WriteGame tem leave40 em15 e20, endfinally39 e reload/ret40–41. Não escreve classes geradas, nem converte using/lock/nested EH em try completo por aproximação. statement.cs3936–3979,4607–4644 e4839–4868 sustentam esses pontos.

Labels coincidentes com EH são tratados pelo produtor como o mesmo instruction slot; metadata EH é zero-width. A correção não muda destinos nem reordena o DTO. Fixtures distinguem loop externo ao try, loop interno e ambos combinados; uma prova separada compara entradas de branches em cada fronteira EH.

C3 torna recursiva a equivalência de && sem else e suporta cadeias de testes de loop com destino comum (|| e negação explícita de &&). Conserva ordem, cada opcode e as variantes .un. Não complementa comparações float para “embelezar” o fonte. Os demais OR com continuations misturadas, lógica sobrecarregada/lifted e redução de constantes com efeitos permanecem UNKNOWN.

C4 serializa owner loop/switch e profundidade de breakables separada de profundidade de loops. Continue pula frames switch. Offsets coincidentes geram conjunto de candidatos e INCOMPLETE, sem escolher o primeiro. Fixture própria provou corpos brutos iguais para switch-break e loop-continue no fim do for, mas diferentes quando existe tail após o switch. Isso limita o que pode ser reconstruído apenas do IL.

## Tentativas negativas preservadas

C2 tentativa1 declarou14 planos matched incorretos: label e eh_begin coincidentes tinham ordem virtual errada. A tentativa está arquivada como reprovada em `reports/round10-control-validation-stage02-attempt01.json`; a versão corrigida é `stage02-fixed`. Foram acrescentadas fixtures que cobrem a causa, e o corpus corrigido não contém divergências nesses1555 planos.

C3 tentativa1 perdeu completude de Tenant.Draw ao renormalizar um for já transformado durante a expansão de &&. Corrigiu-se a idempotência, com fixtures &&3 contendo for/foreach. A retirada desapareceu na validação `stage03-fixed`; a tentativa inicial permanece arquivada. Nenhum estado de método do jogo foi editado.

## Aplicação separada

O aplicador auditou os114 pendentes e publicou uma proposta fonte de717 bytes para GameView.OnTouchEvent, retirando um case25 vazio que corresponde ao hole/default da tabela original. O vínculo de folhas é literal e a previsão de slots é completa/exata. O bundle é `reports/round10-control-rewrite-patches.json`; a auditoria é `reports/round10-control-application-audit.json`.

Isso continua sendo proposta até a compilação real, verificação completa e adoção pelo controlador. O aplicador também preservou o negativo ReturnVoidFinally de seu roundtrip: o footer void não pode ser duplicado. IR completo ou igualdade canônica não substitui o verificador de corpos/metadados. Esta nota não declara ParteC concluída nem ganho matched antes desse oracle.
