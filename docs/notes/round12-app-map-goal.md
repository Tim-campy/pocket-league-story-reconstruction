# R12 — dois layouts smcs do par stloc.s → stloc.2

Dois pacotes privados **independentes**, somente para sonda futura do operador exclusivo. Nenhuma compilação do jogo, verificador, edição pública ou estado foi executado nesta frente. Resultado de ambos os alvos: **UNKNOWN**.

Base oficial imutável B1: `build/round12/runs/b01/commit/postverify01/Assembly-CSharp`, commit `7db4002`, App1.899matched. Os corpos e fontes desses dois métodos permanecem iguais aos da baselineR12. Os arquivos Main foram comparados byte a byte com esse snapshot no empacotamento. As propostas preservam quebras de linha mistas e registram offsets UTF16 e SHA do arquivo/método completos.

## O mecanismo comum e sua diferença

O par do histograma não representa uma única permutação: o smcs declara locais pelo bloco proprietário e sua ordem. A recuperação de C# mudou quais declarações pertencem à raiz e quais pertencem a blocos filhos. Nenhum dos dois casos requer busca por nomes ou edição de IL.

### form.SubForm.Draw_clearPoint — 1.476 IL

Original `0x0600024c`: fat12/maxstack92/7locais/EH0. Atual: fat12/maxstack92/6locais/EH0,1.474IL. Original `[Font,Int32,Int32,Int32,Int32,String,String]`; atual `[Font,Int32,Int32,Int32,String,String]`. Primeiro par real em145: original `stloc.s4`, atual `stloc.2`; o prefixo bruto140 também inclui o operando de desvio anterior e não representa a causa.

O slot2 original **não possui nenhuma instrução de load, store ou endereço em todo o corpo**. O slot3 é o y (`num`), já igual até essa região. O contador do bloco `clearPoint_mode == 4`, atualmente `num3` declarado na raiz, é gravado somente em145 e164 no **slot4 original**. As duas strings do bloco posterior ocupam5/6.

Proposta calculada:

```csharp
// grupo atual
int num2;
int num3;
int num = 105;
// grupo proposto
int num2;
int recoveredUnusedLayout;
int num = 105;
// no bloco mode4, e somente nele
int num3 = 0;  // atual: num3 = 0;
```

`recoveredUnusedLayout` é nome escolhido para representar o slot não usado, **não é um nome original recuperado**. A origem histórica desse slot como declaração nomeada ou outro artefato ainda é UNKNOWN. O que foi provado é sua existência, tipo e ausência de referências; uma declaração não usada é suficiente para produzir esse espaço no smcs real das fixtures. O contador volta ao bloco onde seus dois stores existem, sem nenhuma folha ou teste alterado.

Mapa atual→original `{0:0,1:1,2:4,3:3,4:5,5:6}`. A realocação diagnóstica resulta em527objetos de instrução integralmente iguais,1.476IL,zero hunks. Não produz bytes/DLL e não substitui a compilação real.

### game.Map.Draw_goal — 398 IL

Original `0x060002fc`: fat12/maxstack16/9locais/EH0. Atual: fat12/maxstack16/12locais/EH0,391IL. Todos os tipos são Int32; a assinatura sozinha não prova a posição. Primeiro par em7: original `stloc.s5` (índice do primeiro loop), atual `stloc.2`.

As expressões e os argumentos de `InsertGameObject` ancoram os valores x/y/depth. O original usa os mesmos slots2/3/4 para as duas partes, enquanto os índices dos quatro loops são5/6/7/8. O atual põe x/y/depth dentro de cada loop e aloca seis locais distintos. A proposta declara `int x, y, depth;` **uma vez**, imediatamente após `int num2 = 17;`, antes dos loops, e retira somente `int` das seis atribuições de x/y/depth. Inicializadores, chamadas, argumentos, ordem de avaliação e incrementos não mudam.

Mapa atual→original `{0:0,1:1,2:5,3:6,4:2,5:3,6:4,7:7,8:8,9:2,10:3,11:4}`. A realocação diagnóstica resulta em201objetos de instrução integralmente iguais,398IL,zero hunks. Novamente, isso não é um resultado de compilação do alvo.

`slot-constraints.json` de cada método contém tipo, nome calculado no fonte proposto, primeiro/último uso, stores/loads e mapa. Os nomes dos locais originais continuam UNKNOWN.

## REA e fontes do compilador

Assembly original comum SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`.

- Clear: REA `ev_5a9aa26e89284caacad70061890026e55f10e8055e0e676ea88e6066026ee0dd`, token0x0600024c, SHA de IL `b633738eb01d15abccf36479fb897f541c9766531d476203137ed2a8caed2e6c`.
- Goal: REA `ev_149b92383ab056de6b4ab886f9700d85060e409ec84b3b63ac480f29b92806e9`, token0x060002fc, SHA de IL `1f5e18c8bf8c5222b2441c34de61f64ea67e0be6d507543cf67a9b53bef22c9b`.

Ambos os hashes foram conferidos sobre exatamente `header.code_size` bytes do dump após o fat header12; não incluem alinhamento, assinatura de locais ou cabeçalho.

Mono fixado `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs/statement.cs`:

- linhas1550–1555: variáveis do bloco em `ListDictionary`.
- linhas2032–2047: `Block.EmitMeta` resolve variáveis do bloco, temporários registrados e blocos filhos, nessa ordem.
- linhas1225–1239: `LocalInfo.ResolveVariable` chama `DeclareLocal` quando não içado e ainda sem builder; não exige `IsUsed` para essa declaração. Isso fundamenta a hipótese de local não usado em Clear, confirmada no compilador instalado pelas fixtures.
- linhas569–619: `For.DoEmit` emite inicialização, corpo, incremento, teste e saída com seus donos. Os índices de Goal continuam dentro dos mesmos quatro `for` e não são substituídos por temporários.

A coincidência desse fonte histórico com a totalidade do compilador Unity é UNKNOWN. As experiências usam o smcs real fixado, não uma recompilação desses fontes.

## NoEmit e nove fixtures próprias

NoEmit R11 em auxiliar já existente, sem recompilar ferramenta compartilhada: Clear atualcomplete6tipos, candidato **complete7tipos exatamente originais**; Goal atualcomplete12tipos, candidato **complete9tipos exatamente originais**. Nenhuma incompletude foi forçada; previsões completas erradas não foram contornadas. O modelo contém os blocos/variáveis AST e cada prova registra suas dependências por SHA.

Compilador smcs SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`; runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`. Mesmas flags/referências do snapshot App, com `-out` privado e `-debug-`; nenhum PDB/MDB de produção.

Clear: cinco compilações. Candidato230IL = controle manual independente completo (header/7locais/IL/EH), anterior228IL difere. Os negativos retirar a reserva e manter o contador na raiz diferem. Foi confirmado que slot2 do candidato/controle não tem nenhum uso e permanece na assinatura.

Goal: quatro compilações. Candidato398IL = controle independente completo (header/9locais/IL/EH), anterior391IL difere. Trocar a ordem de x/y na declaração raiz mantém os tipos e tamanho, mas altera o IL; negativo recusado. Os moldes são fixtures próprias com classes stub; não correspondem a um build de `Assembly-CSharp`.

Quatro negativos adicionais do produtor, sem emissão, recusam contador escapando do bloco, colisão do nome da reserva, expressão de coordenada alterada e captura no corpo de Goal.

## Histórico negativo preservado

Clear:76recibos/23registros de trials. R4 adotou hoist de num2/num3:64→140; o fonte atual contém isso. R5–R7 já testaram **três** `reserve-unused-original-slot`, todos compilados e recusados, prefix140. Em R5 a reserva foi colocada antes do if, depois da declaração de num3 na raiz; R6/R7 dentro do if, mantendo o contador na raiz. Portanto nenhum desses candidatos deslocava o contador para slot4 preservando num em3. A proposta conjunta atual substitui a declaração raiz do contador pela reserva e volta a declarar o contador no bloco — não repete a reserva isolada refutada. Outros trials de ordem/escopo/condição/alias também estão preservados, sem omitir rejeições.

Goal:75recibos/5trials. Os cinco hoists R4 tiveram **compile_pass=false**: subiam x, depois x/y, depois x/y/depth da primeira metade, mantendo declarações homônimas da segunda metade (e os últimos ainda repetiam declarações raiz). Os erros estão preservados nos recibos. Não houve avaliação do corpo compilado dessa forma. A nova composição remove os tipos nos **dois** loops antes de emitir uma declaração raiz compartilhada; é fonte válido nas fixtures e não repete o payload inválido.

Os números são registros de trials de relatórios, que podem incluir sondas repetidas, e não uma contagem inferida de compilações distintas. Recibos sem payload/SHA não permitem excluir universalmente uma tentativa; zero candidatos não refuta hipótese.

## Interface e limite

Produtor puro `calculate.py`, duas APIs independentes:

- `transform_clear_point(method_source) -> str`.
- `transform_map_goal(method_source) -> str`.

Cada subpasta tem `patches.json` próprio, antes/depois completos, SHA e UTF16, bindings REA/snapshot e NoEmit, histórico e fixtures. Chamadas e strings permanecem na mesma ordem, não há literal numérico novo nem classe gerada escrita. Somente o verificador oficial decidirá assinatura/header/IL/EH reais e ausência de regressões. Estado **READY_FOR_GAME_PROBE**, ganhos de métodos/bytes **UNKNOWN**.
