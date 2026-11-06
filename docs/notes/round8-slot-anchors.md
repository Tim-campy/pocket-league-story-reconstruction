# Round 8: ?ncoras de stores e declara??o de locais smcs

Estado: hip?teses de fonte aprovadas pelo modelo, corpos reais UNKNOWN. Nenhum candidato do jogo foi compilado por este trabalho. O verificador completo do motor continua sendo o ?nico juiz.

Baseline: commit 25b8108711a4a8920a2600cf9ffea5b710b3a481, snapshots imut?veis em build/round8/baseline/Assembly-CSharp. O c?lculo confronta os descritores de m?todo e o fonte bruto com essa baseline e verifica rebase ?nico da declara??o inteira no Main atual, usando offsets UTF-16 e preservando CRLF.

## BootForm.DivServerData ? 261 bytes de IL

O recuperado declara byte[] array3 fora do for (slot 7) e int num2 dentro dele (slot 8). Tr?s stores com contextos ?nicos ligam o tamanho calculado ao slot original 7 e o array rec?m-criado ao original 8. A corre??o move apenas a declara??o de tamanho antes da declara??o do array, conservando a inicializa??o num-i dentro do for. O nome chunkLength ? escolhido para a reconstru??o; n?o ? recupera??o de nome original. Modelo completo: 11 tipos exatamente iguais ao original; chunkLength=7, array3=8.

## Ball.Update ? 4.629 bytes de IL

Os dois bra?os exclusivos da sele??o de atrito redeclaram num/num2. Seis stores ancorados por contexto assim?trico ?nico comprovam que os pares recuperados 7/9 e 8/10 correspondem aos mesmos slots originais 8 e 9. O c?lculo une cada par no menor bloco expl?cito que cont?m ambos, renomeando apenas refer?ncias vinculadas semanticamente e conservando cada inicializador em sua posi??o original. Os intervalos de uso de cada par s?o disjuntos; refer?ncias em lambdas/capturas s?o rejeitadas pelo transformador.

Os slots originais 7 e 12 s?o System.Single e n?o aparecem em nenhum ldloc, stloc ou ldloca de todo o m?todo. Eles s?o reservados com declara??es sem inicializador, cujo posicionamento ? confirmado independentemente pelo modelo. A assinatura resultante tem 19 tipos exatamente iguais ao original: unusedOriginalSlot7=7, dragX=8, dragZ=9, unusedOriginalSlot12=12. Os nomes novos s?o sint?ticos, n?o nomes hist?ricos.

O corpo ainda tem outras diverg?ncias: ap?s o segundo RateConvert o original recarrega v_ e o recuperado usa dup de atribui??o composta. A transforma??o atual trata somente as identidades/escopos dos locais, deixando as diverg?ncias seguintes ao repert?rio do motor.

## Terceiro caso lido: SoccerForm.Draw_bonusFan ? 3.472 bytes

O modelo completo prev? 60 slots para 59 originais. H? cadeia de inicializa??o com zero cuja ordem difere e dois tempor?rios de switch; os votos de alinhamento sugerem um seletor nomeado na posi??o 32. O solver congelado calculou 11 propostas, nenhuma aprovada por assinatura. INCONCLUSIVO: n?o se promoveu corre??o neste bundle.

## Fixture pr?pria e limites

Duas formas de fixture smcs reproduzem as opera??es de hoist e coalesc?ncia/reservas: Boot, corpo completo 39 bytes/3 locais; Ball, corpo completo 135 bytes/7 locais. Ambos d?o corpo can?nico, assinatura local e EH exatos em compara??o com a forma declarativa de refer?ncia. Um caso com captura ? rejeitado semanticamente e um caso com intervalos de uso intercalados ? rejeitado pelo teste de lifetimes. Duas execu??es de valida??o da mesma fixture foram realizadas durante o preparo; cada execu??o compila apenas um assembly independente de fixture. Nada ? escrito no original, no fonte do jogo, nos modelos ou no estado oficial.

As normaliza??es de operandos locais e destinos num?ricos de desvio usadas na busca de ?ncoras servem somente ao diagn?stico de identidade; n?o participam da compara??o final, n?o alteram assemblies e n?o s?o usadas para declarar matched. As ?ncoras selecionadas deste bundle n?o atravessam destinos de desvio.

## API e ado??o

Fam?lia: calculated-store-anchored-local-lca. Bundle: patches com method_id, logical_path, start/end UTF-16, before/after, complete, predicted_types, model_source_sha256, client_sha256, calculator_sha256, syntax_source_sha256, dependency_sha256. Rebase deve exigir before exato e ?nico; reavaliar modelo com a fonte corrente e executar verifica??o completa de todos os corpos antes de adotar. Ordem dos probes: Ball.Update e BootForm.DivServerData.

O calculador ? opt-in, source-only, usa o modelo congelado Round7SlotModel e uma c?pia isolada para an?lise sem Emit. Somente arquivos novos podem ser publicados quando a janela for aberta pelo orquestrador.
