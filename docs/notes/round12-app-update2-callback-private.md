# Update2, primeiro callback de nome — composição no pai normal

Entrega privada READY_FOR_GAME_PROBE. Corpo do jogo, metadados e regressões oficiais UNKNOWN. Zero compilações do alvo, verificadores oficiais, alterações públicas ou de ledger. Nenhuma classe gerada foi escrita à mão. A frente termina neste pacote, sem outra investigação.

## Identidade e foco

Foco: `form.SubForm/<Update2>c__AnonStorey0::<>m__0(System.String)->System.Void`, token MethodDef0x06000797, original196IL. Pai de fonte normal: `private bool Update2()`, ID `form.SubForm::Update2()->System.Boolean`, em `src/Assembly-CSharp/form/SubForm.cs`. Base oficial B22: `build/round12/runs/b22/commit/postverify01/Assembly-CSharp`.

Original: fat12/flags19/maxstack11, sete locais, EH0. Atual: fat12/flags19/maxstack13, seis locais, EH0,188IL. Primeira diferença bruta é o byte2, destino de brfalse@1 para o ret final195/187. A primeira edição causal alinhada é offset55, onde as assinaturas/slots começam a divergir; não se tenta corrigir isoladamente o comprimento do salto inicial.

REA original `ev_931c5226c1216405cbb616db2b59c46739ece172764bc81844783bb924e8ba50`, pai `ev_7049670414002846e32631451a6d512f8da62f2285de611bbb5fad0c8b14435e`. Tipo0x02000075, campos0x040012d1 t_data e0x040012d2 <>f__this. O view REA desta instalação seleciona o nome de metadados sem owner `<Update2>c__AnonStorey0`; as grafias com / ou + retornaram seleção vazia, preservando UNKNOWN de seleção até a consulta correta. O vínculo ao pai completo usa o stable_id resolvido do dump e a mesma linha MethodDef: o dump escreve `[Method:0x0797]`, canonizado como0x06000797. O IL SHA foi conferido nos196bytes exatos, com artefato Assembly-CSharp SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`.

Os bindings guardam todos os hashes, spansUTF16 do método pai e callback, fonte/receipt/manifest/dumps. O before normal Update2 ocorreu uma vez no Main observado; novas alterações no arquivo exigem rebase por corpo único e vínculo fresco.

## Dois componentes calculados

1. A assinatura original contém Int32 no slot0 sem nenhum stloc/ldloc/ldloca em todo o corpo. O String text é slot1, num38 slot2, num39 slot3, contador num40 slot4, Char c slot5 e stringLength slot6. O atual começa no String0 e tem todos os demais deslocados−1. A proposta declara `int recoveredUnusedNameWidth;` dentro do if da primeira lambda, antes de text. Esse nome e esse escopo são escolhidos para reproduzir a assinatura; nome/escopo históricos e se o slot era variável declarada ou temporário posteriormente não usado permanecem UNKNOWN. Não há inicialização emitida nem referência ao slot0.

2. O original atribui num39 por braços explícitos: se MyFormBase.Jp() for true, stringLength==1 grava5 em118, caso contrário grava4 em125; se Jp for false grava8 em132. Há brfalse104→131, bne.un112→124, br119→126 e br126→133. A proposta substitui somente a atribuição de ternários negados por `if(Jp()){if(stringLength==1){num39=5;}else{num39=4;}}else{num39=8;}`. O if num40==0 ao redor permanece igual. Isso restaura três stores por braço, sem fundi-los em um store no encontro dos ternários.

São preservadas a condição temp_name!=null, as duas chamadas JString.New e seus destinos, a ordem de CharAt/ValueOf/GetStringLength/Jp, o acumulador num38, break para183 quando o comprimento excede o limite, concatenação e a escrita final name2_. Não se muda a captura externa t_data nem o campo de this, e a segunda lambda Update2/<>m__1 fica byte idêntica no fonte. Nenhuma região EH é movida: os try/finally pertencem ao pai, não ao corpo da lambda.

O cálculo de slots, opcodes/larguras e destinos coincide nos72objetos de instrução originais,196IL. A assinatura efetivamente emitida pela fixture própria também coincide exatamente com a assinatura original de sete tipos. Isso é evidência calculada e de compilação própria, não um build/oráculo do alvo. Não há cópia de IL original para um resultado.

## NoEmit: limite explícito

O modelo REAL da lambda é **INCOMPLETO**, antes e depois: `generated_or_unmapped_source_method`, zero tipos previstos. Não foi inventada uma previsão de sete tipos nem promovida a marca complete. O modelo do pai Update2 é uma medida separada: complete=true,103tipos originais exatos antes/depois. Ela não substitui a previsão ausente da lambda.

O executor deve usar a política vigente para incompletos: até12variantes por passagem e24por ciclo, exigindo a assinatura REAL de sete locais antes de avaliar avanço de prefixo. Uma previsão completa errada continua veto. As sete entradas de assinatura provêm da emissão das fixtures, separadamente dos resultados do modelo. Todos os hashes do modelo/client/helper e fontes de entrada estão em model.json; parent-model.json documenta apenas o pai.

Um check diagnóstico inicial comparava o DTO inteiro do pai e falhou porque offsets de fonte, variáveis e blocos mudam de coordenada quando o callback cresce. Foi corrigido para conferir complete e a sequência de tipos original, preservando os DTOs completos no relatório. Isso não foi falha de compilação nem de assinatura.

## Compilador e captura

Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs`:

- statement.cs1225–1238: LocalInfo.ResolveVariable chama DeclareLocal quando não há HoistedVariableReference; não há teste de uso para eliminar esse local antes da declaração.
- statement.cs2032–2047: Block.EmitMeta resolve os locais do bloco, temporários registrados e blocos filhos.2050–2065 relata CS0168 de local não usado. A fixture reserva esse slot e registra exatamente esse warning, sem tratar warning como erro ou normalizar a assinatura.
- expression.cs4196–4215: Conditional.Emit emite os valores dos braços para um encontro comum, explicando o store único atual. statement.cs225–267, If.DoEmit, emite cada atribuição dentro do respectivo braço, como no original.

Cinco fixtures próprias foram compiladas a partir de um pai normal que retorna um delegate. O compilador gerou `Fixture/<Run>c__AnonStorey0` e <>m__0; nenhum fonte de storey foi escrito. O pai Run e o construtor gerado têm corpos integralmente iguais entre before/candidato. REA dos binários próprios confirma exatamente dois campos capturados, t_data e <>f__this, com mesma ordem/tokens/flags/assinaturas. Os offsets físicos das linhas de metadados deslocam com o tamanho dos corpos; os valores brutos de antes/depois são preservados, e só esses offsets de arquivo são excluídos da comparação semântica de campos próprios. Nenhuma DLL foi reescrita ou normalizada.

O fonte real fora da primeira lambda, incluindo declaração/escopo de t_data, todas as folhas do pai e a segunda lambda, é textualmente idêntico. Não se exige ajuste da captura externa na proposta. A ausência de regressões de metadados no jogo permanece UNKNOWN até a verificação oficial completa.

## Fixtures e negativos

Cinco compilações smcs2.6.5 próprias, referências/flags fixadas, saída privada Fixture.dll e `-debug-`, sem sidecar de produção:

| Variante | IL | Locais | maxstack |
|---|---:|---:|---:|
|Before|188|6|13|
|Composição|196|7|11|
|Controle manual independente|196|7|11|
|Só slot reservado|194|7|13|
|Só braços if|190|6|11|

Candidato e controle coincidem integralmente em cabeçalho/locais/IL/EH; todos os negativos diferem. O controle mantém os nomes das capturas (pois eles viram campos) e renomeia apenas os locais não capturados. Os logs preservam CS0168 do slot reservado. Três negativos do produtor rejeitam colisão do nome reservado, constante8 alterada e folha name2_ alterada, sem compilação. Não se repetiu variante de alvo histórica.

## Histórico e pacote

Capturados97receipts e16trials deste método, todos os trials identificados em R4. Foram tentados condition-local, hoists/reordenação de locais já existentes, ternary-polarity e expansão isolada de um dos ternários; nenhum avanço além do prefixo2 nesses registros. Mover num38 antes do String não equivale a reservar um slot inteiramente sem uso, e expandir só o ternário externo deixa o interno invertido/store compartilhado. A composição atual restaura ambos os braços e reserva o slot sem mover capturas. Não apareceu hash da proposta inteira; receipts sem payload/SHA não excluem universalmente toda forma histórica. Zero candidatos não é refutação.

API pura `calculate.py::transform_update2_callback(method_source)->str`. before/after são o método pai **normal** Update2 completo; method_id do patch é o método gerado de foco e source_parent_id é o pai. patches.json inclui dependências/hash, evidência privada congelada e fonte do compilador fixado, compatíveis com a entrada external-proposals. ready-for-game-probe.json destaca explicitamente modelo gerado incompleto, fixture emitida de sete locais e orçamento. O executor decide a adoção após assinatura real, corpo e regressões completos. Nenhum método/byte é contado como ganho nesta entrega.
