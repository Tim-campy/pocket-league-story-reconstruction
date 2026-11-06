# Coalescência calculada: rodada 7

Os três métodos do grupo `stloc.3 => stloc.0` somam 31.804 bytes de IL:
`SubForm._draw3` (13.116), `SubForm._draw5` (12.062) e
`UserData.Draw_edit2` (6.626). A investigação usa o snapshot congelado de
`build/round7/cycles/cycle02/baseline/Assembly-CSharp`, correspondente à medição
após o ciclo 01. Nenhum candidato desses métodos foi compilado nesta frente.

O alinhamento seleciona várias variáveis recompiladas para o mesmo slot 0
original: nove declarações de `num11` em `_draw3`, sete declarações de `dx`
em `_draw5` e duas de `num5` em `Draw_edit2`. Todas têm tipo Int32, intervalos
de uso disjuntos e ancestral comum igual ao bloco do método. O IL original
inicializa quatro slots juntos: `ldc.i4.0; dup; stloc.3; dup; stloc.2; dup;
stloc.1; stloc.0`. O fonte recuperado inicializa somente três. A declaração
comum e a cadeia de quatro zeros são uma proposta conjunta, não um palpite
baseado apenas no tamanho da assinatura.

O solver anterior só agrupa declarações de um mesmo bloco lexical e exige
identidades injetivas em slots consecutivos. Ele não unifica variáveis de
blocos diferentes. Os seus 24 candidatos para `_draw3`, 17 para `_draw5` e
10 para `Draw_edit2` mantiveram respectivamente 126, 86 e 47 slots;
nenhum passou pelo gate de assinatura original.

| Método | Recuperado | Após merge + quatro zeros | Composição final | Original |
|---|---:|---:|---:|---:|
| `_draw3` | 126 | 118 | 119 | 119 |
| `_draw5` | 86 | 80 | 79 | 79 |
| `Draw_edit2` | 47 | 46 | 48 | 48 |

As três composições finais têm previsão independente completa, com todos os
tipos na ordem original. A previsão vem do modelo smcs congelado; a assinatura
original é somente a restrição comparada depois da previsão.

Em `_draw3`, o slot 20 não está sem uso: `GetMatchInfoJoin` é chamado em IL1847,
guardado em 20 em IL1852 e testado em IL1854–1856. O fonte o testava diretamente.
Também há uma segunda cópia explícita do retorno em 61: chamada em IL5683,
store em IL5688, load em IL5690 e `brtrue` em IL5692. Após somente o merge e a
restauração de 20, a contagem já era 119, mas a assinatura ainda diferia na
posição 61 e nos tipos posteriores. A composição final recupera os dois locais
nomeados e converte os quatro switches em cadeias de if; o switch sobre chamada
avalia a chamada uma única vez e usa o segundo local. A previsão coloca
`coalescedValue` em 0, `testedValue` em 20 e `switchValue` em 61.

Em `_draw5`, dois switches sobre locais estáveis compartilham um temporário
extra. O original tem zero instruções `switch`, contra duas na recompilação.
A transformação conserva a ordem dos casos, o default e os breaks de laços
internos; saltos que saem do switch viram saltos para uma saída explícita.
Converter esses dois switches em if remove o temporário e leva a 79 slots.
A hipótese de controle de fluxo ainda depende do verificador do jogo.

Em `Draw_edit2`, os slots originais 8 e 38 não têm nenhum load, store ou tomada
de endereço. Acrescentar declarações sem inicializador nesses slots é permitido
pela evidência, sem inventar valores. Uma primeira tentativa de inseri-las
antes das próximas declarações de comando produziu tipos iguais, mas os nomes
novos ficaram nos slots 10 e 40: essa tentativa foi rejeitada. A versão final
insere os declaradores sem inicializador antes de `j` e `num24` nos respectivos
inicializadores de for. O modelo confirma exatamente os slots 8 e 38.

Os nomes `coalescedValue`, `testedValue`, `switchValue` e `unusedOriginalSlotN`
são escolhidos para representar locais recuperados. Os nomes históricos
originais desses locais são UNKNOWN. Nenhum é capturado e nenhuma classe
gerada é escrita à mão.

O auxiliar experimental usa a análise semântica moderna para renomear somente
referências vinculadas às variáveis selecionadas, preserva os inicializadores
nas posições antigas e declara o local comum no ancestral comum. Recusa grupos
capturados, tipos diferentes, const, declarações não suportadas e intervalos
de uso sobrepostos. Patches são métodos inteiros, com offsets UTF-16 e finais
de linha preservados; `before` permite rebase apenas quando a declaração é
encontrada de forma única.

As fixtures independentes, compiladas pelo smcs instalado, confirmaram:

* Fonte dividido: cinco locais; merge sem restaurar a cadeia: quatro locais,
  mas corpo ainda divergente; merge semântico mais cadeia: corpo completo,
  assinatura de locais e EH iguais ao exemplo original da fixture.
* Captura: o auxiliar recusou a transformação na análise semântica.
* Usos intercalados: os intervalos reais da fixture sobrepõem-se e são recusados.
* Switch convertido em if, incluindo default e break de laço interno: corpo
  completo igual ao exemplo escrito com if.
* Declarador sem inicializador no for: local na posição prevista e corpo igual
  ao exemplo que declara o slot sem uso explicitamente.

As fixtures têm nomes de métodos diferentes para os exemplos de comparação;
os relatórios distinguem essa diferença de identidade da igualdade do corpo
completo. Não existe normalização de assemblies nem promoção de estados do jogo.

Os três patches em `round7-coalesce-patches.json` continuam hipóteses de fonte
até compilação isolada, comparação completa, proteção de todos os corpos já
iguais e verificação oficial pelo processo principal. O total de métodos e
bytes efetivamente ganhos nesta frente é zero; o potencial dos três alvos é
31.804 bytes. Não é uma estimativa de fechamento integral dos três corpos.

## Reprodução e consumo

`python tools/round7_coalesce_fixture.py`, seguido de
`python tools/round7_coalesce_semantic_fixture.py` e
`python tools/round7_coalesce_extra_fixture.py`, executa somente as fixtures.
`python tools/round7_coalesce_calculate.py` regenera o documento
`reports/round7-coalesce-patches.json` sem compilar candidatos do jogo.
Esses comandos exigem o snapshot local congelado do ciclo 02 e a toolchain
já instalada; não instalam ferramentas.

O documento oferece `patches`, cada qual com `method_id`, `logical_path`,
`start`, `end`, `before`, `after`, `rule`, assinatura prevista e hashes do
modelo/cliente. A família é `calculated-original-local-coalescence`.
`round7_coalesce_calculate.rebase_unique_patch(source, patch)` recalcula os
offsets UTF-16 somente quando `before` aparece uma única vez, preservando CRLF.
Falha se a declaração mudou ou se o vínculo é ambíguo. O consumidor precisa
reexecutar a previsão independente sobre o fonte corrente e aplicar todos os
gates do motor antes de aceitar qualquer hipótese.
