# Locais explícitos de árvore, resultado protegido e índices no pai

Estado WIP: propostas para sonda, **não resultados do jogo**. Snapshot exclusivo `build/round10/cycles/cycle15/after/KairoLibrary`, medição `reports/round10-cycle15-after-measurement.json`. Nenhum alvo foi compilado, nenhum arquivo público ou bundle anterior foi alterado. O vínculo com Main continua UNKNOWN e exige rebase único futuro.

## Três exemplos e peso

| ID abreviado | IL original / próprio | Locais original / próprio | maxstack original / próprio | Diagnóstico |
|---|---:|---:|---:|---|
| `Compressor::Compress(Byte[])->Byte[]` | 784 / 786 | 22 / 22 | 5 / 8 | Slots com papéis diferentes e8regiões reais após associação |
| `Compressor::_readNodeFromBitBuffer()->Node` | 91 / 89 | 2 / 2 | 2 / 2 | Uma região: falta atribuição à variável num existente |
| `Matrix::PreConcat(Single[])->Matrix` | 149 / 149 | 4 / 4 | 6 / 6 | Permutação pura dos quatro índices |

IDs completos, cabeçalhos, IL, assinatura, EH, votos de papéis e primeiros/últimos usos estão em `diagnostics.json`. Todos os corpos são fat. O total de1024bytes originais é peso de seleção; ganho comprovado nesta tarefa é0métodos/0bytes. `role-regions.json` omite destinos de branch somente para diagnóstico de sequências, sem alterar os DTOs ou qualquer assembly. Não é normalização nem oráculo.

## Compress: composição inteira, sem alias só na pilha inventado

O próprio tem dois papéis sintetizados ausentes no original e dois papéis nomeados ausentes/deslocados no próprio. Contar22contra22oculta isso:

- O original Node9 tem primeiro store184 e últimos loads até201, dentro do for que cria as folhas. O próprio usa o Node2 antecipado no método e só o usa184..199. Reverter **somente esse hoist antigo** para `Node node = new Node(...)` no if do for recupera a propriedade do bloco. FastVector continua no pai; nenhuma inicialização é movida.
- No ramo de um único nó, o original faz `newobj269; stloc11@274; ldloc10; ldloc11; stfld parent_@280`. O próprio mantém o receiver Node9 na pilha antes do construtor, faz dup/store no temporário Node11, grava o campo e depois copia esse temporário para Node10. A forma original sustentada é `Node node3 = new Node(...); node2.parent_ = node3;`, seguida da folha existente que atualiza elementData[0]. Isso não é a variante `campo=(local=new)` do Canvas: aqui o original mostra **store do local e carga posterior do receiver**, sem dup nessa região.
- No laço de combinação, o original faz `newobj362; stloc15@367`, depois Node13.parent=Node15, Node14.parent=Node15 e só então carrega FastVector/AddElement. O próprio carrega FastVector e os dois receivers antes do construtor e reaproveita o mesmo temporário Node11 em várias atribuições encadeadas. A proposta declara um Node nomeado `recoveredCombinedNode`, preserva o construtor literal e aplica as duas atribuições na ordem original, antes de AddElement. O nome histórico é UNKNOWN; nenhuma classe ou membro é criado.
- O original inicializa Byte[]3 com null em717..718, atribui a ele ToByteArray em764 e o devolve após finally em782..783. O próprio elimina o `byte[] array2=null` sem usos, aloca retorno sintetizado Byte[]21 e o preenche em764, devolvendo-o em783..785. A proposta mantém array2 inicializada no ponto existente, atribui no try e faz o return fora do finally. O null inicial passa a ter local vivo, como no original. Não se remove nem modifica Close/null no finally.

Os oito ranges de sequência após papéis começam nos offsets originais248,274,276,341,367,378,387,718. Os dois primeiros pares são a atribuição no ramo único; os quatro seguintes são a combinação explícita; o último é o resultado inicializado. Todos têm motivos de fonte calculados acima. As diferenças de slots restantes refletem esses donos e a substituição de temporários por locais com identidades distintas.

Ordem desejada de tipos/papéis:

| Slots originais | Papel |
|---|---|
| 0..4 | Int64 relógio; FastVector; contador num2; Byte[] resultado array2; ByteArrayOutputStream |
| 5..8 | Índices dos quatro primeiros for: i,j,k,l |
| 9 | Node folha dentro do if do quarto for |
| 10..11 | Node node2 e Node node3 no ramo único |
| 12..15 | Int32[] seletor e três Nodes no laço de combinação |
| 16..20 | Índice m; símbolo num3; Node node6; comprimento; Node parent_ |
| 21 | Byte[] data2 no try final |

O próprio Node11 sintetizado é usado274..390 nos dois caminhos. Esse reaproveitamento não o transforma no Node15 original: este só existe367..388e tem três leituras posteriores distintas. A proposta exige materializá-lo; não tenta eliminá-lo por scheduling na pilha.

O EH original é try722..767/finally767..782; próprio722..768/finally768..783. O candidato ainda não foi compilado como alvo, então os offsets finais e a igualdade de cláusulas permanecem UNKNOWN. Os construtores e chamadas foram preservados literalmente; a avaliação de receivers deslocada envolve apenas locais estáveis já avaliados, nunca getter ou chamada.

## Segunda leitura: reatribuir o local existente

Em `_readNodeFromBitBuffer`, todo o resto do IL e a assinatura já se alinham. O original em57..64 contém `call _readBit; stloc.1; ldloc.1; brtrue.s`, enquanto o próprio usa call/brtrue diretamente. A proposta muda somente o segundo `if(_readBit()==0)` para:

```csharp
num = _readBit();
if (num == 0) {
    // Folhas existentes.
}
```

Reutiliza-se o Int32 num já declarado e usado pelo primeiro teste. Não se introduz um Boolean nem um local novo, não se altera a polaridade e não se reavalia _readBit. O destino e as folhas são preservados.

## Matrix: os três índices posteriores têm de ser movidos juntos

Não há construção aritmética diferente depois de associar os papéis. Os primeiros/últimos usos originais são j/slot0:42..130; k/slot1:46..122; l/slot2:67..114; i/slot3:10..30. No próprio os mesmos usos e offsets existem nos slots1,2,3,0. Isso associa os papéis por uso efetivo, não só pelos quatro tipos Int32 iguais.

A proposta declara `int j,k,l;` no pai antes do primeiro for i e retira apenas os tipos dos três headers posteriores. Inicializações, testes, incrementos, corpos, `+=` flutuante e SetValues permanecem literalmente iguais. O índice i continua local do primeiro for. Com CodeGen definindo os locais do pai antes de seus filhos, j/k/l recebem0/1/2 e só depois o filho i recebe3.

Não se alteram expressões de ponto flutuante, associatividade ou formas .un. Não há capturas, goto, yield ou await no método. A proposta é estreita e recusa essas construções se aparecerem no molde.

## Histórico: negativos preservados, composições novas

`history.json` captura103registros para os três alvos. Registros sem trials não são tratados como refutação. Trials reais disponíveis:

- Compress:18trials/2passos em R4, prefixo17→154, incluindo os hoists atuais;12trials de hoist em R6 sem avanço. O novo candidato **não** repete o hoist de Node: devolve-o ao filho, separa ambas as cadeias e recupera o resultado nomeado após finally de uma vez.
- Leitura de Node:4trials em R4, sem avanço30→30. Tentaram inverter o primeiro if ou criar Boolean condition-local para comparações. Não houve o segundo _readBit atribuído ao Int32 num já existente.
- Matrix:6trials R7partA de hoists **individuais** de j, k ou l para diferentes pais, todos sem avanço10→10. R7cycle16moveu apenas i para imediatamente antes do próprio for, também sem avanço. A nova composição move **os três** índices posteriores para o pai simultaneamente; mover somente um deixa i no slot errado e não resolve o prefixo.

Os relatórios e seus SHA estão no histórico. `history-sha-audit.json` compara o SHA dos candidatos completos com proveniências anteriores e não encontra fonte candidato idêntico; há trials antigos sem SHA completo, portanto não há alegação de exaustividade global. Oito regiões não significam oito correções independentes já testadas.

## Regra de emissão fixada

Revisão Roslyn1.1.0 `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`. As cópias em `pinned/` têm SHA256 e SHA1 de Git blob autenticados em `provenance.json`:

- `src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs:594..629` define todos os locais de cada BoundBlock antes dos seus comandos. Isso sustenta a ordem dos índices de Matrix e a diferença entre Node no pai e Node no if filho. Linhas1499..1505não devolvem locais nomeados ao pool como temporários sintetizados.
- `src/Compilers/CSharp/Portable/CodeGen/EmitExpression.cs:2122..2161`: uma atribuição cujo valor é usado emite dup e, se o lhs usa a pilha, AllocateTemp/store. Linhas2245..2250carregam/liberam o temporário no postfix. Isso explica o Node11 do próprio criado pelas atribuições a campo usadas como expressões; a sequência original sem dup pede statements explícitos.
- `EmitStatement.cs:648..663,676..730` distingue retorno indireto em contexto protegido e usa LazyReturnTemp; depois, em situação normal, emite ret diretamente. O original devolve o local nomeado fora do finally, em vez de pedir ao emitter um retorno protegido. A fixture própria confirma o spill e os slots efetivos da composição; a identidade do alvo ainda depende da sonda.
- `Optimizer.cs:1629..1680,1866..1881` impõe restrições para alias agendado na pilha. Aqui não se inventa um alias desse tipo: os stores explícitos originais dos Nodes e do resultado impedem classificar esses papéis como temporários que deveriam desaparecer. A leitura do bit reaproveita um local que já existe.

## Fixtures próprias e modelo congelado

Nove compilações próprias, três trios before/controle manual independente/candidato, com csc1.1.0.51109 SHA53b6935… e referências/opções C15. Os controles são literais manuais em `write_manual_fixtures.py`, não produto das transformações.

- Árvore/resultado reduzido:470→468IL,18→18locais,maxstack8→5. O delta de−2IL e o maxstack5/8 correspondem à relação original/próprio do alvo, mas não provam seu corpo.
- Segunda leitura:91→93IL,2→2locais; o candidato exibe exatamente call/stloc.1/ldloc.1/brtrue.s após a segunda chamada.
- Índices da matriz:158→158IL,4→4locais; inclui uma chamada Observe própria para ser fixture independente. A rotação de slots é observável e mantém o corpo com os mesmos bytes de comprimento.
- Todos os corpos candidatos das três fixtures igualam integralmente os controles; todos os before diferem. Seis negativos textuais recusados: uso extra de node, nome novo em colisão, comando após finally, segunda ocorrência do teste _readBit, uso anterior de índice e captura.

O NoEmit usou helper já em cache e163fontes C15 congelados por input_path/SHA. Os três candidatos tiveram **sequência de tipos prevista igual à assinatura original**, sem editar a previsão. Compress permanece complete=false (concatenação/array postfix/conditional access/EH); leitura de Node permanece complete=false (scheduler do node); Matrix é complete=true com4tipos. Nenhum incompleto foi promovido ou vetado artificialmente. A assinatura compilada dos alvos continua UNKNOWN até a sonda.

## Handoff

Produtor puro `calculate.py`, APIs sem I/O no import ou chamada: `transform_compress_explicit_nodes_result`, `transform_readbit_reassign_existing`, `transform_preconcat_outer_indices`. Prefixo das três propostas: `d10-explicit-tree-locals-and-index-owners`.

Os dois primeiros métodos compartilham Compressor.cs: qualquer adoção deve ser serial e fazer rebase exato do antes restante, sem presumir SHA de arquivo inteiro ainda igual após a primeira edição. Nenhuma alteração de bytes de DLL, classe gerada, compilador, referência, normalização, DEX ou ledger foi feita. **READY_FOR_GAME_PROBE** autoriza apenas a avaliação pelo executor; corpo/metadados/ganho dos alvos continuam UNKNOWN/0.
