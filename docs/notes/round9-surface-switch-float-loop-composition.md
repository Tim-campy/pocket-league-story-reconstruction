# Surface: tabela vazia e ordem de fluxo — D17

Base826502490d3d4697a11c25f604c4c6d0710fccdd; medição fresca `reports/round9-cycle16-after-measurement.json`. Alvo SurfaceBase.DoEvent2.305, atual2.299. Scan limitado de59 métodos próprios pendentes encontrou apenas esse análogo; HttpObject._connect não confirma a mesma forma. Não se inventam segundo/terceiro exemplos reais.

Três hipóteses conjuntas explicam os cinco trechos alinhados remanescentes:

1. O switch original0..3 tem quarto destino1743 igual ao default358→1743, sem normalização da chave. Fonte tem0..2; inserir `case3:break` acrescenta quatro bytes à tabela, sem mudar o comportamento do default. O seletor é int action explicitamente declarado; enum/var/shadow são recusados.
2. Original535 bge.un.s558: Add5 no braço true537..551;556 br.s573; else558 cria evento1 e goto case2;573 é continuação comum. Fonte recuperado põe fallback primeiro como guarda. Restaurar `if(expr<225f){Add5;}else{fallback;}` acrescenta dois bytes; preserva `<` e NaN/.un, sem substituir por >=.
3. Original1946 testa IsPositive no corpo, salta para saída antes do incremento1966; Size1972/blt1946 faz o teste de repetição. Fonte recuperado fundiu Size&&Positive na condição de for vazio. Restaurar Size na condição e `if(!Positive)break` no corpo (zero bytes líquidos), preservando int i existente e ordem/quantidade das chamadas.

A composição evita tentar as três peças isoladamente, que podem deixar os destinos iniciais errados. Fonte do switch Roslyn1.1 fixado4cb203dab89e1542cdf7d88cb6d32767bc0e5460: SwitchBucket77–94 usa End-Start+1, não arredonda a potência de dois; emissores de If/For preservam a ordem indicada. Nenhum destino de IL é corrigido manualmente.

Fixture própria com buffer/chamadas: corpo completo idêntico ao controle manual, antes demonstravelmente diferente, aridade3→4, bge.un preservado; seis recusas de tipos, owner, shadow e corpo não vazio. São três builds de fixture, zero jogo nessa etapa. Recibos reports/round9-surface-composition-fixture.json e reports/round9-surface-composition-patches.json.

Modelo fresco **INCOMPLETO** prevê30 tipos contra29 originais; limitações incluem scheduler e switch_lowering, com primeira divergência prevista12. Isso não vira veto completo nem prova de que o candidato introduziu local. O baseline real tem29 locais corretos; assinatura real deve ser provada na compilação e o limite de incompletos12 permanece. Sem classe gerada manual, cópia de IL, normalização ou estado editado.
