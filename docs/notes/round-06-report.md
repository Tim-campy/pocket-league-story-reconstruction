## 1. PROGRESSO

Projeto: Pocket League Story para Android, Unity Mono 5.3.4f1. APK: `original/app.apk`; SHA-256 `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8`.

Rodada anterior: commit `66eea9ddeb8e02a9f357f97f8e7c9dc60f54d6e9`. Percentuais abaixo são frações exatas multiplicadas por 100; bytes matched contam IL original somente quando todo o corpo, incluindo cabeçalho, locais e exceções, passa.

| Assembly | Total métodos | Matched anterior | % anterior | Matched atual | % atual | Ganhos | Regressões |
| --- | --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 3287 | 3187 | 3187/3287 × 100% | 3193 | 3193/3287 × 100% | 6 | 0 |
| Assembly-CSharp | 1958 | 1688 | 1688/1958 × 100% | 1703 | 1703/1958 × 100% | 15 | 0 |

| Assembly | IL total | IL matched anterior | IL matched atual | Ganho IL | % IL anterior | % IL atual |
| --- | --- | --- | --- | --- | --- | --- |
| KairoLibrary | 269894 | 204103 | 208123 | 4020 | 204103/269894 × 100% | 208123/269894 × 100% |
| Assembly-CSharp | 601603 | 203891 | 225924 | 22033 | 203891/601603 × 100% | 225924/601603 × 100% |

Total: 4875 → 4896 / 5245 métodos matched; 407994 → 434047 / 871497 bytes IL matched. Restam 346 corpos divergentes e 3 corpos iguais retidos pelo vínculo de auxiliares. Zero regressões adotadas de estados, corpos e tipos.

Originais: KairoLibrary SHA-256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`; Assembly-CSharp SHA-256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`.

## 2. O QUE FOI FEITO NESTA RODADA

Ganhos por transformação são atribuídos ao passo que fechou o corpo; passos parciais não são matched. Linhas de motor e adoção se sobrepõem e não são somadas. Candidatos smcs só foram compilados após previsão completa e assinatura prevista igual à original; o compilador e o verificador continuam sendo o juiz.

**KairoLibrary: regras e tentativas.**

| Regra | Tentativas compiladas | Passos aceitos | Corpos ganhos | Bytes IL ganhos | Perdas adotadas |
| --- | --- | --- | --- | --- | --- |
| alias-field-occurrence | 1 | 0 | 0 | 0 | 0 |
| assignment-chain-order | 0 | 0 | 0 | 0 | 0 |
| assignment-ternary-if | 1 | 0 | 0 | 0 | 0 |
| assignment-ternary-if-reverse | 1 | 0 | 0 | 0 | 0 |
| condition-local | 9 | 0 | 0 | 0 | 0 |
| condition-merge-and | 0 | 0 | 0 | 0 | 0 |
| condition-merge-or-guards | 0 | 0 | 0 | 0 | 0 |
| condition-split-and | 0 | 0 | 0 | 0 | 0 |
| condition-split-or-guards | 1 | 0 | 0 | 0 | 0 |
| declaration-ternary-if | 2 | 0 | 0 | 0 | 0 |
| declaration-ternary-if-reverse | 2 | 0 | 0 | 0 | 0 |
| foreach-array-indexed-for | 1 | 1 | 1 | 964 | 0 |
| guard-break | 0 | 0 | 0 | 0 | 0 |
| guard-break-reverse | 0 | 0 | 0 | 0 | 0 |
| guard-continue | 0 | 0 | 0 | 0 | 0 |
| guard-continue-reverse | 0 | 0 | 0 | 0 | 0 |
| guard-return | 0 | 0 | 0 | 0 | 0 |
| guard-return-reverse | 0 | 0 | 0 | 0 | 0 |
| if-polarity | 7 | 1 | 0 | 0 | 0 |
| local-declaration-order-earlier | 3 | 0 | 0 | 0 | 0 |
| local-declaration-order-later | 7 | 0 | 0 | 0 | 0 |
| local-declaration-scope-hoist | 122 | 0 | 0 | 0 | 0 |
| loop-cache-scope | 0 | 0 | 0 | 0 | 0 |
| loop-cache-scope-positive | 0 | 0 | 0 | 0 | 0 |
| loop-continue-guard | 0 | 0 | 0 | 0 | 0 |
| loop-continue-guards | 0 | 0 | 0 | 0 | 0 |
| null-conditional-explicit | 3 | 3 | 2 | 1143 | 0 |
| return-local | 1 | 0 | 0 | 0 | 0 |
| roslyn-declaration-slot-layout | 4 | 4 | 3 | 1913 | 0 |
| switch-if | 0 | 0 | 0 | 0 | 0 |
| tail-early-return | 0 | 0 | 0 | 0 | 0 |
| ternary-polarity | 13 | 0 | 0 | 0 | 0 |
| unary-negation | 0 | 0 | 0 | 0 | 0 |
| value-guard | 0 | 0 | 0 | 0 | 0 |
| value-guard-reverse | 0 | 0 | 0 | 0 | 0 |

Negativos KairoLibrary: 178 tentativas observadas; 0 não compilaram; 0 tiveram regressão de corpos; 0 foram rejeitadas por metadados. Cada passagem histórica e final é contada uma vez; candidatos sem avanço estrito no critério da respectiva passagem foram rejeitados. Zero tentativas em uma regra significa ausência de candidato elegível dentro do orçamento, não ausência histórica da construção.

**Assembly-CSharp: regras e tentativas.**

| Regra | Tentativas compiladas | Passos aceitos | Corpos ganhos | Bytes IL ganhos | Perdas adotadas |
| --- | --- | --- | --- | --- | --- |
| alias-field-occurrence | 3 | 0 | 0 | 0 | 0 |
| alias-parameter-block-tail | 5 | 2 | 0 | 0 | 0 |
| alias-parameter-command | 1 | 1 | 0 | 0 | 0 |
| alias-parameter-occurrence | 5 | 2 | 0 | 0 | 0 |
| assignment-chain-order | 0 | 0 | 0 | 0 | 0 |
| assignment-ternary-if | 34 | 9 | 3 | 1356 | 0 |
| assignment-ternary-if-reverse | 25 | 0 | 0 | 0 | 0 |
| calculated-parameter-alias | 3 | 3 | 0 | 0 | 0 |
| condition-local | 1 | 0 | 0 | 0 | 0 |
| condition-merge-and | 0 | 0 | 0 | 0 | 0 |
| condition-merge-or-guards | 4 | 0 | 0 | 0 | 0 |
| condition-split-and | 33 | 0 | 0 | 0 | 0 |
| condition-split-or-guards | 22 | 0 | 0 | 0 | 0 |
| declaration-ternary-if | 4 | 0 | 0 | 0 | 0 |
| declaration-ternary-if-reverse | 4 | 0 | 0 | 0 | 0 |
| foreach-array-indexed-for | 3 | 2 | 0 | 0 | 0 |
| guard-break | 0 | 0 | 0 | 0 | 0 |
| guard-break-reverse | 0 | 0 | 0 | 0 | 0 |
| guard-continue | 0 | 0 | 0 | 0 | 0 |
| guard-continue-reverse | 0 | 0 | 0 | 0 | 0 |
| guard-return | 0 | 0 | 0 | 0 | 0 |
| guard-return-reverse | 0 | 0 | 0 | 0 | 0 |
| if-chain-switch | 3 | 3 | 3 | 213 | 0 |
| if-polarity | 55 | 3 | 2 | 3356 | 0 |
| initialized-declaration-swap | 12 | 0 | 0 | 0 | 0 |
| local-declaration-order-earlier | 24 | 0 | 0 | 0 | 0 |
| local-declaration-order-later | 46 | 0 | 0 | 0 | 0 |
| local-declaration-scope-hoist | 142 | 0 | 0 | 0 | 0 |
| loop-cache-scope | 0 | 0 | 0 | 0 | 0 |
| loop-cache-scope-positive | 0 | 0 | 0 | 0 | 0 |
| loop-continue-guard | 25 | 3 | 2 | 8783 | 0 |
| loop-continue-guards | 7 | 1 | 0 | 0 | 0 |
| reserve-unused-original-slot | 3 | 1 | 1 | 2347 | 0 |
| return-local | 0 | 0 | 0 | 0 | 0 |
| solver-baseline-group-all-calculated-blocks | 48 | 0 | 0 | 0 | 0 |
| solver-baseline-group-block-declarations | 214 | 0 | 0 | 0 | 0 |
| solver-baseline-group-original-zero-chain | 4 | 3 | 1 | 1512 | 0 |
| solver-group-all-calculated-blocks | 102 | 0 | 0 | 0 | 0 |
| solver-group-block-declarations | 464 | 5 | 2 | 1568 | 0 |
| solver-group-original-zero-chain | 4 | 1 | 1 | 2898 | 0 |
| switch-if | 3 | 0 | 0 | 0 | 0 |
| tail-early-return | 51 | 2 | 0 | 0 | 0 |
| ternary-polarity | 70 | 21 | 0 | 0 | 0 |
| unary-negation | 3 | 0 | 0 | 0 | 0 |
| value-guard | 0 | 0 | 0 | 0 | 0 |
| value-guard-reverse | 0 | 0 | 0 | 0 | 0 |

Negativos Assembly-CSharp: 1427 tentativas observadas; 0 não compilaram; 0 tiveram regressão de corpos; 0 foram rejeitadas por metadados. Cada passagem histórica e final é contada uma vez; candidatos sem avanço estrito no critério da respectiva passagem foram rejeitados. Zero tentativas em uma regra significa ausência de candidato elegível dentro do orçamento, não ausência histórica da construção.

| Frente | Resultado | Matched ganhos | Matched perdidos |
| --- | --- | --- | --- |
| Modelo smcs | 1682 corpos matched na validação; nenhum candidato do jogo compilado para calcular previsões. | 0 | 0 |
| Solver de declarações | 15 assinaturas antes diferentes agora iguais; 9 desses corpos fecharam. Causalidade exclusiva por transformação depende das linhas de regras acima. | 9 | 0 |
| Cálculo em todos os pendentes | 269 métodos com intervalos de uso; 214 com posições calculadas; 105 com propostas aprovadas pelo modelo (285 propostas), antes de qualquer compilação candidata. | 0 | 0 |
| Alternativas combinadas | 105 métodos / 285 variantes recalculadas do snapshot; cadeias de zeros e agrupamentos combinados reapresentados sobre o método atual com previsão nova e proteção do prefixo. | 0 | 0 |
| Alias calculado de parâmetro | Três DrawGradation: alias dx, parâmetro 1, slot original 0; store 18, load 403, correspondente ao argumento direto 395. A hipótese de simples troca de duas declarações era insuficiente. | 0 | 0 |
| Alinhamento IL | Novo diagnóstico por edição de sequência; destinos de desvio reconciliados por rótulos. Nenhuma normalização usada na aceitação. | 0 | 0 |
| Controles independentes | 23/23 fixtures completas do alocador passaram; 72 controles sintáticos/de segurança das transformações passaram. Não promovem métodos do jogo. | 0 | 0 |
| Motor Assembly-CSharp | 269 pendentes percorridos, incluindo 162 antes sem nova busca e 18 com avanço parcial anterior. | 15 | 0 |
| Motor KairoLibrary | 18 controles selecionados; os outros pendentes foram medidos e protegidos na verificação integral. | 6 | 0 |
| Adoção | Produções completas dos dois alvos conferidas antes da cópia dos C#; verificação oficial integral após a adoção. | 21 | 0 |
| DEX/Java e bytes brutos | Fora do escopo; nenhuma decompilação DEX ou correção baseada em bytes brutos. | 0 | 0 |

Gate smcs medido: `{"approved": 1654, "incomplete": 403, "post_compile_disagreements": 0, "predicted": 5302, "wrong_signature": 3245}`. Previsões aprovadas e tentativas compiladas têm denominadores diferentes: a fila pode conter duplicatas, alternativas não consumidas ou tarefas canceladas após o primeiro avanço.

Assinaturas confirmadas nas tentativas compiladas, incluindo candidatos depois rejeitados por prefixo: 144 métodos distintos tiveram ao menos um candidato com assinatura igual à original (1426 tentativas); 0 métodos tiveram desacordo entre previsão e assinatura compilada. Essas contagens não promovem estados e não substituem as assinaturas adotadas medidas na seção 4.

Critério de busca parcial: a passagem completa e as retomadas iniciais exigiram avanço do prefixo bruto. Somente a última retomada dos três DrawGradation habilitou avanço estrito do prefixo de instruções alinhadas, preservando todos os bytes do prefixo bruto já igual. O salto no offset 122 dependia do tamanho das seis expressões do laço: corrigir uma expressão adiante mantinha esse operando divergente. Essa retomada permitiu corrigir as expressões uma por vez. O comparador de corpos, cabeçalho, locais e EH permaneceu inalterado, e somente ele promove matched.

Os 121 testes Python passaram. As 72 verificações sintáticas/de segurança e os 23 métodos de fixture smcs têm escopos separados. A prova adicional recompilou os 260 fontes exatos preparados para o Git: zero corpos alterados contra o build oficial e 466 tipos estruturalmente iguais. Resumo medido: reports/round6-validation-checks.json.

## 3. HIPÓTESES

**MODELO SMCS — taxa de acerto e cobertura.**

| População | Numerador / denominador | Percentual exato |
| --- | --- | --- |
| Todos os corpos matched anteriores | 1575/1682 | 1575/1682 × 100% |
| Métodos mapped ao fonte | 1575/1582 | 1575/1582 × 100% |
| Previsões declaradas completas pelo modelo | 1539/1539 | 1539/1539 × 100% |
| Cobertura completa em todos os matched | 1539/1682 | 1539/1682 × 100% |

Há 100 corpos gerados ou sem método de fonte mapeado; eles não são contados como acerto por terem previsão vazia. 143 previsões não são declaradas completas; uma assinatura coincidente em previsão incompleta não autoriza compilação. Previsões completas incorretas: 0. Divergências sem classificação: 0.

Proveniência da validação: o arquivo exato lido ao iniciar o motor foi preservado em reports/round6-slot-model-validation-engine-input.json. O relatório ampliado acrescenta somente seis campos de resumo derivados; todos os campos anteriores, inclusive as 1952 linhas de previsão, são idênticos. O guard confere o hash do input preservado e a igualdade desses dados, além dos hashes de modelo, cliente e solver.

| Classe residual de erro/limite | Métodos matched |
| --- | --- |
| anonymous_function_or_capture | 7 |
| generated_or_unmapped_source_method | 100 |

Limites das previsões incompletas, incluindo as que coincidiram incidentalmente com a assinatura atual:

| Classe de cobertura incompleta | Ocorrências em métodos matched |
| --- | --- |
| anonymous_function_or_capture | 15 |
| generated_or_unmapped_source_method | 100 |
| semantic_errors:CS0115 | 27 |
| semantic_errors:CS0155 | 1 |
| semantic_errors:CS1660 | 3 |

As classificações podem se sobrepor; não se deve somá-las para obter número de métodos. Diagnósticos CS0115, CS0155 e CS1660 pertencem ao helper semântico usado no cálculo; os fontes frozen já compilam no smcs. O modelo não usa essa coincidência de tipos para declarar completa uma previsão recusada.

Resultados negativos do desenvolvimento: as primeiras versões erraram a representação de tipos genéricos aninhados e omitiram temporários de incremento, atribuição e receptor struct; outra versão confundiu Parse estático com receptor struct. As fixtures também detectaram temporário de retorno de construção struct padrão e switch de string ainda incompleto. Essas regras foram corrigidas antes de liberar o motor smcs. A métrica preliminar que contava assinatura vazia de método não mapeado como acerto foi descartada. Os snapshots intermediários não foram todos preservados após a interrupção da sessão: sua quantificação reprodutível por versão é UNKNOWN; as taxas acima pertencem ao modelo final arquivado.

Controle adicional nos pendentes iniciais: 256/256 previsões completas coincidiram com a assinatura CURRENT da reconstrução; 13 são incompletas, das quais 6 coincidem incidentalmente. Há 7 métodos distintos com previsão diferente da assinatura atual. Isso valida o cálculo sobre o fonte recuperado; não significa que a assinatura atual já seja igual à original.

| Classe residual nos pendentes | Ocorrências |
| --- | --- |
| anonymous_function_or_capture | 1 |
| generated_or_unmapped_source_method | 6 |
| semantic_errors:CS1660 | 1 |

As classes de erro são diagnósticos, podem se sobrepor e não significam falha do corpo já matched. Capturas, membros gerados sem fonte e lowerings ainda não modelados são recusados pelo gate. A reconstrução integral do alocador não é declarada universal enquanto houver cobertura incompleta; causas históricas específicas não comprovadas continuam UNKNOWN.

CONFIRMADA na população completa validada: o modelo calcula slots a partir do fonte e dos tipos sem emitir o assembly candidato. A regra usa inserção de variáveis, temporários registrados, filhos e blocos implícitos; temporários de emissão podem reutilizar slots livres por tipo. A [fonte do smcs](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2032-L2047) e a evidência anterior permanecem fixadas. A hipótese de nomes/hash permanece REFUTADA nos caminhos testados.

INCONCLUSIVA como recuperação única do fonte histórico: assinatura e primeiro/último acesso de slots restringem declarações e escopos, mas não identificam nomes nem resolvem sozinhos todos os casos de locais do mesmo tipo. O solver gera propostas calculadas e usa a previsão da assinatura para eliminar candidatos antes da compilação. Declarações agrupadas e inicializações encadeadas são propostas de C#, não IL copiado.

REFUTADA como explicação suficiente de DrawGradation: não era apenas a ordem de duas declarações. Os três controles tinham 11 locais no original e 10 na reconstrução; faltava o alias dx no slot 0. O cálculo alinhou seu único uso original em 403 ao argumento direto recompilado em 395. A regra introduziu dxAlias antes do primeiro local e substituiu somente aquele uso; dx em dx+w permaneceu argumento direto. O nome dxAlias foi escolhido para a reconstrução, não recuperado do original. O estado oficial na tabela abaixo decide a correspondência do corpo completo.

Falha de instrumentação corrigida na revisão: uma tentativa de BillingManager.Init<T> compilou, mas o seletor passou a aridade do ID estável ao inspetor, enquanto Cecil seleciona pelo nome sem aridade. Essa tentativa histórica não foi contada como assinatura confirmada. O seletor foi corrigido, ganhou um teste de contrato e o método foi reexecutado: o prefixo avançou de 58 para 61, mas o corpo continua decompiled. Não houve promoção manual nem novo corpo completo nesse caso.

| Assembly | Controle | Hipótese de reconstrução | Estado oficial | Prefixo bruto anterior → final |
| --- | --- | --- | --- | --- |
| Assembly-CSharp | `surface.GameView::OnTouchDlgCmd(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void` | CONFIRMADA por corpo completo | matched | 1 → 75 |
| Assembly-CSharp | `form.BootForm::Load()->System.Void` | INCONCLUSIVA para o corpo completo | decompiled | 75 → 75 |
| Assembly-CSharp | `system.form.BillingForm::DrawGradation(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)->System.Void` | CONFIRMADA por corpo completo | matched | 17 → 452 |
| Assembly-CSharp | `form.SoccerForm::Draw_newPlayerLv(kairo.unity.ui.Graphics)->System.Void` | INCONCLUSIVA para o corpo completo | decompiled | 56 → 1903 |
| Assembly-CSharp | `system.form.FriendForm::DrawGradation(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)->System.Void` | CONFIRMADA por corpo completo | matched | 17 → 452 |
| Assembly-CSharp | `system.friend.ui.FriendData::DrawGradation(kairo.unity.ui.Graphics,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32)->System.Void` | CONFIRMADA por corpo completo | matched | 17 → 452 |
| KairoLibrary | `kairo.unity.util.Language::Init()->System.Void` | INCONCLUSIVA para o corpo completo | decompiled | 349 → 916 |
| KairoLibrary | `kairo.unity.util.Language::SetLanguagePack(System.String,System.Byte[],System.Boolean)->System.Boolean` | CONFIRMADA por corpo completo | matched | 842 → 855 |

**Roslyn 1.1: os quatro casos na linha de base 66eea9d não tinham locais a mais ou a menos.** Os estados da última coluna descrevem o build atual após a rodada.

| Método | Locais original / recompilado anterior | A mais / a menos | Estado oficial atual |
| --- | --- | --- | --- |
| Canvas.EndPaint | 6/6 | 0/0 | matched |
| SurfaceManagerBase.Draw | 15/15 | 0/0 | decompiled |
| Matrix.PostConcat | 4/4 | 0/0 | matched |
| Graphics.DrawString | 13/13 | 0/0 | matched |

CONFIRMADA por contagem e acessos: Canvas trocava índice i/array nos slots 0–1; SurfaceManagerBase trocava i/surfaceBase e text/height; Matrix emitia i,j,k,l onde os acessos originais indicam j,k,l,i; Graphics trocava num4,num2,num3 por num2,num3,num4 nos slots Single 3–5. Os nomes são do PDB recompilado; nomes históricos permanecem UNKNOWN. O resultado integral de cada ajuste vem exclusivamente do estado oficial acima.

CONFIRMADA na [fonte Roslyn consultada](https://github.com/dotnet/roslyn/blob/4cb203dab89e1542cdf7d88cb6d32767bc0e5460/src/Compilers/CSharp/Portable/CodeGen/EmitStatement.cs#L594-L630): EmitBlock define os locais do bloco antes dos comandos e dos filhos. Locais nomeados não são liberados para reutilização; temporários reutilizáveis usam pilhas por assinatura. O otimizador pode manter um local na pilha, evitando slot. Essas são regras distintas; os quatro exemplos são permutações e não demonstram eliminação/reutilização como causa. Não foi criado modelo Roslyn completo.

CONFIRMADA na análise estática: o acesso condicional permite ao emissor conhecer o receptor como não nulo e usar call para método não virtual. Reescrever ?. como if explícito foi testado pelo motor; só os corpos oficialmente iguais confirmam a reconstrução completa. O inventário AST da linha de base 66eea9d, antes dos guards adotados, encontrou 43 acessos condicionais a membro, 3 operadores ?? e 0 acessos condicionais por índice. Esses números descrevem o fonte recuperado daquele snapshot, não o build final nem o fonte histórico. ?? sobre nullable não foi tratado como referência. Fontes e intervalos completos estão em docs/notes/round6-roslyn-source-evidence.md.

## 4. PENDENTES POR GRUPO

| Assembly | Faixa IL | Métodos anteriores | Bytes anteriores | Métodos atuais | Bytes atuais |
| --- | --- | --- | --- | --- | --- |
| KairoLibrary | <=64 | 3 | 137 | 3 | 137 |
| KairoLibrary | 65-256 | 36 | 5528 | 35 | 5379 |
| KairoLibrary | 257-1024 | 36 | 19733 | 32 | 17310 |
| KairoLibrary | >1024 | 23 | 40375 | 22 | 38927 |
| Assembly-CSharp | <=64 | 12 | 607 | 11 | 544 |
| Assembly-CSharp | 65-256 | 63 | 9050 | 61 | 8900 |
| Assembly-CSharp | 257-1024 | 100 | 57129 | 94 | 53818 |
| Assembly-CSharp | >1024 | 94 | 330919 | 88 | 312410 |

| Assembly | Corpos pendentes atuais | Assinatura de locais já igual | Assinatura ainda diferente |
| --- | --- | --- | --- |
| KairoLibrary | 92 | 42 | 50 |
| Assembly-CSharp | 254 | 149 | 105 |

A igualdade da assinatura compara tipos por posição; não identifica qual variável ocupa cada slot quando várias têm o mesmo tipo. Só a comparação completa dos corpos confirma os acessos a esses slots.

Nas mesmas identidades dos 269 pendentes smcs iniciais: 149 → 164 assinaturas iguais; 15 recém-igualadas; 0 retiradas de igualdade; 9 recém-igualadas também fecharam o corpo. Essa medição é sobre as mesmas identidades; a tabela de pendentes atuais exclui os corpos que fecharam.

Histograma atual por alinhamento: destinos são reconciliados por rótulos, sequências são comparadas por diferença de instruções e a primeira edição real orienta o ponto de fonte. As instruções e slots continuam significativos. Operandos de desvio são preservados no verificador bruto; o alinhamento não prova equivalência. Limites: instruções repetidas podem admitir mais de um alinhamento; destinos originais em trecho não alinhado são adiados explicitamente; destino recompilado em trecho inserido pode permanecer branch_retarget e selecionar um comando anterior.

**KairoLibrary, par 1: 4 métodos / 592 bytes IL.** `{"opcode":"ldarg.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}`.

Exemplo: ``kairo.unity.util.FastVector`1/<GetEnumerator>d__42::MoveNext()->System.Boolean``; offsets alinhados original/recompilado: 24/24; edição: delete.

| IL original | IL recompilado |
| --- | --- |
| ``IL_0012: stfld {"field":"class kairo.unity.util.FastVector`1/<GetEnumerator>d__42 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>::<>1__state:valuetype System.Int32 @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}`` | ``IL_0012: stfld {"field":"class kairo.unity.util.FastVector`1/<GetEnumerator>d__42 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>::<>1__state:valuetype System.Int32 @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}`` |
| `IL_0017: ldarg.0` | `IL_0017: ldarg.0` |
| `IL_0018: ldarg.0` | ``IL_0018: ldfld {"field":"class kairo.unity.util.FastVector`1/<GetEnumerator>d__42 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>::<>4__this:class kairo.unity.util.FastVector`1 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>"}`` |
| ``IL_0019: ldfld {"field":"class kairo.unity.util.FastVector`1/<GetEnumerator>d__42 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>::<>4__this:class kairo.unity.util.FastVector`1 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>"}`` | ``IL_001d: ldfld {"field":"class kairo.unity.util.FastVector`1 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>::elementData:!0[]"}`` |
| ``IL_001e: ldfld {"field":"class kairo.unity.util.FastVector`1 @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null<!0>::elementData:!0[]"}`` | `IL_0022: stloc.1` |

**KairoLibrary, par 2: 3 métodos / 1741 bytes IL.** `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldc.i4.0","operand":null}`.

Exemplo: `kairo.unity.surface.SurfaceManagerBase::GetPaintRect(UnityEngine.Vector2[])->System.Int32[]`; offsets alinhados original/recompilado: 24/24; edição: insert.

| IL original | IL recompilado |
| --- | --- |
| `IL_0012: ldc.r4 {"float_bits":"FF-FF-7F-FF"}` | `IL_0012: ldc.r4 {"float_bits":"FF-FF-7F-FF"}` |
| `IL_0017: stloc.3` | `IL_0017: stloc.3` |
| `IL_0018: L0: ldarg.1` | `IL_0018: ldc.i4.0` |
| `IL_0019: stloc.s {"local":8}` | `IL_0019: stloc.s {"local":8}` |
| `IL_001b: ldc.i4.0` | `IL_001b: br.s L1` |
|  | `IL_001d: L0: ldarg.1` |
|  | `IL_001e: ldloc.s {"local":8}` |

**KairoLibrary, par 3: 3 métodos / 885 bytes IL.** `{"opcode":"stloc.0","operand":null} => {"opcode":"stloc.1","operand":null}`.

Exemplo: `kairo.unity.ui.Canvas::EndAlterStaticRender(kairo.unity.ui.Graphics,System.Int32)->System.Void`; offsets alinhados original/recompilado: 41/41; edição: replace.

| IL original | IL recompilado |
| --- | --- |
| `IL_0026: br.s L2` | `IL_0026: br.s L2` |
| `IL_0028: L0: ldc.i4.0` | `IL_0028: L0: ldc.i4.0` |
| `IL_0029: stloc.0` | `IL_0029: stloc.1` |
| `IL_002a: br.s O71` | `IL_002a: br.s R71` |
| `IL_002c: L1: ldarg.1` | `IL_002c: L1: ldarg.1` |

EH original: `[{"kind": "Finally", "try_start": 162, "try_end": 188, "handler_start": 188, "handler_end": 202, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 141, "try_end": 213, "handler_start": 213, "handler_end": 227, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 115, "try_end": 238, "handler_start": 238, "handler_end": 252, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Finally", "try_start": 162, "try_end": 188, "handler_start": 188, "handler_end": 202, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 141, "try_end": 213, "handler_start": 213, "handler_end": 227, "filter_start": -1, "catch_type": null}, {"kind": "Finally", "try_start": 115, "try_end": 238, "handler_start": 238, "handler_end": 252, "filter_start": -1, "catch_type": null}]`.

**KairoLibrary, par 4: 3 métodos / 1016 bytes IL.** `{"opcode":"stloc.1","operand":null} => {"opcode":"stloc.2","operand":null}`.

Exemplo: `kairo.unity.graphics.MeshBuilder::AddQuad(UnityEngine.Vector3[],System.Single,System.Single,System.Single,System.Single,UnityEngine.Color)->System.Void`; offsets alinhados original/recompilado: 38/38; edição: replace.

| IL original | IL recompilado |
| --- | --- |
| ```IL_0020: call {"method":"class kairo.unity.graphics.MeshBuilder @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null::GetLayer``0()->class kairo.unity.graphics.MeshBuilder/Layer @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;this=True;explicit=False;cc=Default"}``` | ```IL_0020: call {"method":"class kairo.unity.graphics.MeshBuilder @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null::GetLayer``0()->class kairo.unity.graphics.MeshBuilder/Layer @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;this=True;explicit=False;cc=Default"}``` |
| `IL_0025: dup` | `IL_0025: dup` |
| `IL_0026: stloc.1` | `IL_0026: stloc.2` |
| `IL_0027: stfld {"field":"class kairo.unity.graphics.MeshBuilder @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null::currentLayer:class kairo.unity.graphics.MeshBuilder/Layer @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}` | `IL_0027: stfld {"field":"class kairo.unity.graphics.MeshBuilder @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null::currentLayer:class kairo.unity.graphics.MeshBuilder/Layer @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}` |
| `IL_002c: ldloc.1` | `IL_002c: ldloc.2` |

**KairoLibrary, par 5: 2 métodos / 176 bytes IL.** `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldloca.s","operand":{"local":0}}`.

Exemplo: `kairo.unity.ui.Graphics::ArgbToColor(System.Int32)->UnityEngine.Color`; offsets alinhados original/recompilado: 0/0; edição: insert.

| IL original | IL recompilado |
| --- | --- |
| `IL_0000: ldloca.s {"local":0}` | `IL_0000: ldloca.s {"local":0}` |
| `IL_0002: ldarg.0` | `IL_0002: initobj {"type":"valuetype UnityEngine.Color @ UnityEngine, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"}` |
| `IL_0003: ldc.i4.s 24` | `IL_0008: ldloca.s {"local":0}` |
|  | `IL_000a: ldarg.0` |

<details><summary>Histograma completo por alinhamento — KairoLibrary</summary>

| Par exato original → recompilado | Métodos | Bytes IL |
| --- | --- | --- |
| `{"opcode":"ldarg.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 4 | 592 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldc.i4.0","operand":null}` | 3 | 1741 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"stloc.1","operand":null}` | 3 | 885 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"stloc.2","operand":null}` | 3 | 1016 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldloca.s","operand":{"local":0}}` | 2 | 176 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.2","operand":null}` | 2 | 251 |
| `{"opcode":"conv.r4","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 2345 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"stloc.2","operand":null}` | 2 | 770 |
| `{"opcode":"stloc.s","operand":{"local":5}} => {"opcode":"stloc.s","operand":{"local":4}}` | 2 | 873 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"brtrue.s","operand":{"branch":144}}` | 1 | 341 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"brtrue.s","operand":{"branch":146}}` | 1 | 396 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"dup","operand":null}` | 1 | 197 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.1","operand":null}` | 1 | 105 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.s","operand":{"local":16}}` | 1 | 893 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.s","operand":{"local":6}}` | 1 | 451 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.s","operand":{"local":7}}` | 1 | 421 |
| `{"opcode":"ble.s","operand":{"branch":15}} => {"opcode":"ble.s","operand":{"branch":15}}` | 1 | 215 |
| `{"opcode":"bne.un.s","operand":{"branch":158}} => {"opcode":"beq.s","operand":{"branch":159}}` | 1 | 182 |
| `{"opcode":"bne.un.s","operand":{"branch":197}} => {"opcode":"beq.s","operand":{"branch":218}}` | 1 | 402 |
| `{"opcode":"bne.un.s","operand":{"branch":74}} => {"opcode":"beq","operand":{"branch":266}}` | 1 | 372 |
| `{"opcode":"br.s","operand":{"branch":72}} => {"opcode":"leave","operand":{"branch":191}}` | 1 | 191 |
| `{"opcode":"br.s","operand":{"branch":749}} => {"opcode":"leave.s","operand":{"branch":770}}` | 1 | 789 |
| `{"opcode":"brfalse","operand":{"branch":1022}} => {"opcode":"brtrue.s","operand":{"branch":101}}` | 1 | 1059 |
| `{"opcode":"brfalse","operand":{"branch":3011}} => {"opcode":"brtrue.s","operand":{"branch":42}}` | 1 | 3012 |
| `{"opcode":"brfalse","operand":{"branch":954}} => {"opcode":"brfalse.s","operand":{"branch":111}}` | 1 | 955 |
| `{"opcode":"brfalse.s","operand":{"branch":116}} => {"opcode":"brtrue.s","operand":{"branch":48}}` | 1 | 117 |
| `{"opcode":"brfalse.s","operand":{"branch":12}} => {"opcode":"brtrue.s","operand":{"branch":6}}` | 1 | 1686 |
| `{"opcode":"brfalse.s","operand":{"branch":17}} => {"opcode":"brtrue.s","operand":{"branch":55}}` | 1 | 58 |
| `{"opcode":"brfalse.s","operand":{"branch":185}} => {"opcode":"brtrue.s","operand":{"branch":206}}` | 1 | 326 |
| `{"opcode":"brfalse.s","operand":{"branch":285}} => {"opcode":"brtrue.s","operand":{"branch":203}}` | 1 | 297 |
| `{"opcode":"brfalse.s","operand":{"branch":51}} => {"opcode":"brtrue.s","operand":{"branch":72}}` | 1 | 163 |
| `{"opcode":"brfalse.s","operand":{"branch":56}} => {"opcode":"brtrue","operand":{"branch":473}}` | 1 | 537 |
| `{"opcode":"brfalse.s","operand":{"branch":62}} => {"opcode":"brtrue.s","operand":{"branch":83}}` | 1 | 108 |
| `{"opcode":"brfalse.s","operand":{"branch":9}} => {"opcode":"brtrue.s","operand":{"branch":30}}` | 1 | 1921 |
| `{"opcode":"brtrue","operand":{"branch":1192}} => {"opcode":"switch","operand":{"switch":[97,1205,1608]}}` | 1 | 1928 |
| `{"opcode":"brtrue","operand":{"branch":322}} => {"opcode":"switch","operand":{"switch":[199,343,452,587]}}` | 1 | 737 |
| `{"opcode":"brtrue.s","operand":{"branch":163}} => {"opcode":"brfalse.s","operand":{"branch":64}}` | 1 | 164 |
| `{"opcode":"brtrue.s","operand":{"branch":222}} => {"opcode":"ldc.i4.4","operand":null}` | 1 | 1054 |
| `{"opcode":"brtrue.s","operand":{"branch":725}} => {"opcode":"switch","operand":{"switch":[681,742,803]}}` | 1 | 1345 |
| `{"opcode":"brtrue.s","operand":{"branch":762}} => {"opcode":"switch","operand":{"switch":[769,810,823,861,908,922,945,953,960,1072]}}` | 1 | 3097 |
| `{"opcode":"brtrue.s","operand":{"branch":78}} => {"opcode":"brfalse.s","operand":{"branch":118}}` | 1 | 303 |
| `{"opcode":"brtrue.s","operand":{"branch":93}} => {"opcode":"switch","operand":{"switch":[100,117,117,273]}}` | 1 | 278 |
| ```{"opcode":"constrained.","operand":{"type":"valuetype System.Char @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}} => {"opcode":"call","operand":{"method":"valuetype System.Char @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089::ToString``0()->class System.String @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089;this=True;explicit=False;cc=Default"}}``` | 1 | 128 |
| `{"opcode":"ldc.i4.0","operand":null} => {"opcode":"brtrue.s","operand":{"branch":1960}}` | 1 | 3513 |
| `{"opcode":"ldc.i4.1","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 661 |
| `{"opcode":"ldelem.any","operand":{"type":"valuetype kairo.unity.system.appli.GameAppli @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}} => {"opcode":"ldelema","operand":{"type":"valuetype kairo.unity.system.appli.GameAppli @ KairoLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"}}` | 1 | 192 |
| `{"opcode":"ldelem.ref","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 216 |
| `{"opcode":"ldloc.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 67 |
| `{"opcode":"ldloc.1","operand":null} => {"opcode":"ldelema","operand":{"type":"valuetype System.Int32 @ mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"}}` | 1 | 275 |
| `{"opcode":"leave","operand":{"branch":2}} => {"opcode":"br.s","operand":{"branch":170}}` | 1 | 175 |
| `{"opcode":"leave","operand":{"branch":382}} => {"opcode":"br","operand":{"branch":345}}` | 1 | 383 |
| `{"opcode":"leave.s","operand":{"branch":163}} => {"opcode":"br.s","operand":{"branch":146}}` | 1 | 164 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 78 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"dup","operand":null}` | 1 | 199 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"stloc.3","operand":null}` | 1 | 94 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 91 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"stloc.3","operand":null}` | 1 | 310 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.0","operand":null}` | 1 | 46 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.1","operand":null}` | 1 | 944 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.3","operand":null}` | 1 | 1125 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 2403 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.0","operand":null}` | 1 | 149 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.1","operand":null}` | 1 | 680 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.s","operand":{"local":5}}` | 1 | 416 |
| `{"opcode":"stloc.s","operand":{"local":10}} => {"opcode":"stloc.s","operand":{"local":11}}` | 1 | 1563 |
| `{"opcode":"stloc.s","operand":{"local":10}} => {"opcode":"stloc.s","operand":{"local":12}}` | 1 | 974 |
| `{"opcode":"stloc.s","operand":{"local":12}} => {"opcode":"stloc.s","operand":{"local":14}}` | 1 | 918 |
| `{"opcode":"stloc.s","operand":{"local":13}} => {"opcode":"stloc.s","operand":{"local":14}}` | 1 | 1188 |
| `{"opcode":"stloc.s","operand":{"local":13}} => {"opcode":"stloc.s","operand":{"local":9}}` | 1 | 1326 |
| `{"opcode":"stloc.s","operand":{"local":15}} => {"opcode":"stloc.s","operand":{"local":17}}` | 1 | 1863 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 190 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.3","operand":null}` | 1 | 365 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.s","operand":{"local":15}}` | 1 | 1044 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.s","operand":{"local":8}}` | 1 | 1978 |
| `{"opcode":"stloc.s","operand":{"local":7}} => {"opcode":"stloc.s","operand":{"local":8}}` | 1 | 1109 |
| `{"opcode":"stloc.s","operand":{"local":9}} => {"opcode":"stloc.2","operand":null}` | 1 | 784 |
| `{"opcode":"stloc.s","operand":{"local":9}} => {"opcode":"stloc.s","operand":{"local":8}}` | 1 | 1088 |
| `{"opcode":"switch","operand":{"switch":[363,476,788,1743]}} => {"opcode":"switch","operand":{"switch":[359,472,782]}}` | 1 | 2305 |

</details>

**Paradas do motor — KairoLibrary.**

| Parada final por ID único | Métodos |
| --- | --- |
| body_equal | 6 |
| no_transformation_advances | 12 |

Limites: 16 iterações por método, 96 candidatos compilados por etapa; 0 métodos atingiram o limite de iterações. Cada parada registra offset, comando e localização alinhada ou UNKNOWN. Histórico de retomadas não é somado às paradas finais.

**Assembly-CSharp, par 1: 5 métodos / 567 bytes IL.** `{"opcode":"stloc.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}`.

Exemplo: `data.LeagueData::IsVictoryLeague()->System.Boolean`; offsets alinhados original/recompilado: 10/10; edição: delete.

| IL original | IL recompilado |
| --- | --- |
| `IL_0000: ldsfld {"field":"class data.LeagueData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::udat_:class game.UserData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"}` | `IL_0000: ldsfld {"field":"class data.LeagueData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::udat_:class game.UserData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"}` |
| ```IL_0005: callvirt {"method":"class game.UserData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::GetMatchInfoJoin``0()->valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e;this=True;explicit=False;cc=Default"}``` | ```IL_0005: callvirt {"method":"class game.UserData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::GetMatchInfoJoin``0()->valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e;this=True;explicit=False;cc=Default"}``` |
| `IL_000a: stloc.0` | `IL_000a: brfalse L0` |
| `IL_000b: ldloc.0` | `IL_000f: ldc.i4.0` |
| `IL_000c: brfalse L0` | `IL_0010: ret` |
| `IL_0011: ldc.i4.0` |  |

**Assembly-CSharp, par 2: 4 métodos / 42013 bytes IL.** `{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.0","operand":null}`.

Exemplo: `form.SubForm::_draw2(kairo.unity.ui.Graphics)->System.Void`; offsets alinhados original/recompilado: 2/2; edição: replace.

| IL original | IL recompilado |
| --- | --- |
| `IL_0000: ldc.i4.0` | `IL_0000: ldc.i4.0` |
| `IL_0001: dup` | `IL_0001: dup` |
| `IL_0002: stloc.3` | `IL_0002: stloc.0` |
| `IL_0003: dup` | `IL_0003: dup` |
| `IL_0004: stloc.2` | `IL_0004: stloc.1` |
| `IL_0005: dup` |  |
| `IL_0006: stloc.1` |  |

**Assembly-CSharp, par 3: 3 métodos / 402 bytes IL.** `{"opcode":"br","operand":{"branch":133}} => {"opcode":"ret","operand":null}`.

Exemplo: `surface.GameView::OnTouchDlgSel(System.Int32,System.Int32,kairo.unity.surface.TouchEvent)->System.Void`; offsets alinhados original/recompilado: 23/23; edição: replace.

| IL original | IL recompilado |
| --- | --- |
| `IL_0011: ldc.i4.2` | `IL_0011: ldc.i4.2` |
| `IL_0012: beq L0` | `IL_0012: beq L0` |
| `IL_0017: br L2` | `IL_0017: ret` |
| `IL_001c: L0: ldarg.0` | `IL_0018: L0: ldarg.0` |
| `IL_001d: ldarg.2` | `IL_0019: ldarg.2` |

**Assembly-CSharp, par 4: 3 métodos / 220 bytes IL.** `{"opcode":"stloc.0","operand":null} => {"opcode":"ret","operand":null}`.

Exemplo: `data.PlayerData::GetPhyParamBarW(System.Int32,System.Int32)->System.Int32`; offsets alinhados original/recompilado: 39/39; edição: replace.

| IL original | IL recompilado |
| --- | --- |
| `IL_0021: ldc.i4.1` | `IL_0021: ldc.i4.1` |
| ```IL_0022: call {"method":"class game.GameUtil @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::RateConvert``0(valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Boolean @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e)->valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e;this=False;explicit=False;cc=Default"}``` | ```IL_0022: call {"method":"class game.GameUtil @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::RateConvert``0(valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e,valuetype System.Boolean @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e)->valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e;this=False;explicit=False;cc=Default"}``` |
| `IL_0027: stloc.0` | `IL_0027: ret` |
| `IL_0028: br O77` | `IL_0028: L0: ldarg.0` |
| `IL_002d: L0: ldarg.0` | `IL_0029: ldc.i4 500` |
| `IL_002e: ldc.i4 500` |  |

**Assembly-CSharp, par 5: 2 métodos / 228 bytes IL.** `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldarg.0","operand":null}`.

Exemplo: `system.form.FriendForm::<_update>m__17()->System.Void`; offsets alinhados original/recompilado: 18/18; edição: insert.

| IL original | IL recompilado |
| --- | --- |
| ```IL_000c: callvirt {"method":"class system.friend.ui.FriendData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::CgiSearchFriend``0(class System.String @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e)->class system.friend.ui.Friend @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;this=True;explicit=False;cc=Default"}``` | ```IL_000c: callvirt {"method":"class system.friend.ui.FriendData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::CgiSearchFriend``0(class System.String @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e)->class system.friend.ui.Friend @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null;this=True;explicit=False;cc=Default"}``` |
| `IL_0011: stloc.0` | `IL_0011: stloc.0` |
| `IL_0012: ldc.i4.4` | `IL_0012: ldarg.0` |
| ```IL_0013: newobj {"method":"class system.form.FriendForm @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::.ctor``0(valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e)->class System.Void @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e;this=True;explicit=False;cc=Default"}``` | `IL_0013: ldc.i4.4` |
| `IL_0018: stloc.1` | ```IL_0014: newobj {"method":"class system.form.FriendForm @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::.ctor``0(valuetype System.Int32 @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e)->class System.Void @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e;this=True;explicit=False;cc=Default"}``` |

EH original: `[{"kind": "Catch", "try_start": 0, "try_end": 45, "handler_start": 45, "handler_end": 87, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 0, "try_end": 87, "handler_start": 87, "handler_end": 95, "filter_start": -1, "catch_type": null}]`; recompilado: `[{"kind": "Catch", "try_start": 0, "try_end": 45, "handler_start": 45, "handler_end": 87, "filter_start": -1, "catch_type": "class System.Exception @ mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"}, {"kind": "Finally", "try_start": 0, "try_end": 87, "handler_start": 87, "handler_end": 95, "filter_start": -1, "catch_type": null}]`.

<details><summary>Histograma completo por alinhamento — Assembly-CSharp</summary>

| Par exato original → recompilado | Métodos | Bytes IL |
| --- | --- | --- |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 5 | 567 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.0","operand":null}` | 4 | 42013 |
| `{"opcode":"br","operand":{"branch":133}} => {"opcode":"ret","operand":null}` | 3 | 402 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"ret","operand":null}` | 3 | 220 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldarg.0","operand":null}` | 2 | 228 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldc.r4","operand":{"float_bits":"00-00-00-00"}}` | 2 | 1664 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.1","operand":null}` | 2 | 12768 |
| `{"opcode":"beq","operand":{"branch":30}} => {"opcode":"bne.un","operand":{"branch":32}}` | 2 | 92 |
| `{"opcode":"br","operand":{"branch":42}} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 112 |
| `{"opcode":"br","operand":{"branch":94}} => {"opcode":"ret","operand":null}` | 2 | 190 |
| `{"opcode":"dup","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 2478 |
| `{"opcode":"ldarg.2","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 880 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"dup","operand":null}` | 2 | 3176 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 209 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"ret","operand":null}` | 2 | 198 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.0","operand":null}` | 2 | 3787 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.s","operand":{"local":5}}` | 2 | 679 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"stloc.1","operand":null}` | 2 | 2955 |
| `{"opcode":"stloc.s","operand":{"local":15}} => {"opcode":"stloc.s","operand":{"local":5}}` | 2 | 1363 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.2","operand":null}` | 2 | 5079 |
| `{"opcode":"stloc.s","operand":{"local":5}} => {"opcode":"DELETION_POINT","operand":null}` | 2 | 1127 |
| `{"opcode":"IL_ALIGNED_IDENTICAL","operand":null} => {"opcode":"IL_ALIGNED_IDENTICAL","operand":null}` | 1 | 104 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"br","operand":{"branch":406}}` | 1 | 1188 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"brtrue","operand":{"branch":187}}` | 1 | 190 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldc.i4.s","operand":31}` | 1 | 707 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"ldloc.s","operand":{"local":8}}` | 1 | 653 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"leave","operand":{"branch":204}}` | 1 | 222 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.0","operand":null}` | 1 | 196 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.2","operand":null}` | 1 | 7669 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.3","operand":null}` | 1 | 325 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 302 |
| `{"opcode":"INSERTION_POINT","operand":null} => {"opcode":"stloc.s","operand":{"local":7}}` | 1 | 2263 |
| `{"opcode":"beq","operand":{"branch":107}} => {"opcode":"bne.un","operand":{"branch":84}}` | 1 | 576 |
| `{"opcode":"beq","operand":{"branch":26}} => {"opcode":"bne.un","operand":{"branch":35}}` | 1 | 51 |
| `{"opcode":"beq","operand":{"branch":410}} => {"opcode":"bne.un","operand":{"branch":402}}` | 1 | 1489 |
| `{"opcode":"beq","operand":{"branch":46}} => {"opcode":"bne.un","operand":{"branch":110}}` | 1 | 190 |
| `{"opcode":"beq","operand":{"branch":57}} => {"opcode":"bne.un","operand":{"branch":92}}` | 1 | 111 |
| `{"opcode":"beq","operand":{"branch":60}} => {"opcode":"bne.un","operand":{"branch":67}}` | 1 | 81 |
| `{"opcode":"bge","operand":{"branch":142}} => {"opcode":"blt","operand":{"branch":13}}` | 1 | 143 |
| `{"opcode":"bge","operand":{"branch":196}} => {"opcode":"blt","operand":{"branch":174}}` | 1 | 222 |
| `{"opcode":"bge","operand":{"branch":28}} => {"opcode":"blt","operand":{"branch":56}}` | 1 | 3042 |
| `{"opcode":"bge","operand":{"branch":533}} => {"opcode":"blt","operand":{"branch":741}}` | 1 | 999 |
| `{"opcode":"bge","operand":{"branch":576}} => {"opcode":"blt","operand":{"branch":646}}` | 1 | 1662 |
| `{"opcode":"bge","operand":{"branch":62}} => {"opcode":"blt","operand":{"branch":162}}` | 1 | 493 |
| `{"opcode":"bge.un","operand":{"branch":151}} => {"opcode":"blt","operand":{"branch":246}}` | 1 | 271 |
| `{"opcode":"bge.un","operand":{"branch":155}} => {"opcode":"blt","operand":{"branch":152}}` | 1 | 562 |
| `{"opcode":"bgt","operand":{"branch":242}} => {"opcode":"ble","operand":{"branch":81}}` | 1 | 3210 |
| `{"opcode":"ble","operand":{"branch":112}} => {"opcode":"bgt","operand":{"branch":55}}` | 1 | 113 |
| `{"opcode":"ble","operand":{"branch":362}} => {"opcode":"bgt","operand":{"branch":357}}` | 1 | 1989 |
| `{"opcode":"ble","operand":{"branch":491}} => {"opcode":"bgt","operand":{"branch":25}}` | 1 | 492 |
| `{"opcode":"ble","operand":{"branch":513}} => {"opcode":"bgt","operand":{"branch":369}}` | 1 | 538 |
| `{"opcode":"ble","operand":{"branch":820}} => {"opcode":"bgt","operand":{"branch":655}}` | 1 | 821 |
| `{"opcode":"ble","operand":{"branch":863}} => {"opcode":"bgt","operand":{"branch":249}}` | 1 | 2309 |
| `{"opcode":"ble","operand":{"branch":903}} => {"opcode":"bgt","operand":{"branch":277}}` | 1 | 904 |
| `{"opcode":"ble.un","operand":{"branch":643}} => {"opcode":"bgt","operand":{"branch":769}}` | 1 | 4343 |
| `{"opcode":"blt","operand":{"branch":1009}} => {"opcode":"bge","operand":{"branch":207}}` | 1 | 1010 |
| `{"opcode":"blt","operand":{"branch":189}} => {"opcode":"bge","operand":{"branch":45}}` | 1 | 217 |
| `{"opcode":"blt","operand":{"branch":209}} => {"opcode":"bge","operand":{"branch":208}}` | 1 | 819 |
| `{"opcode":"blt","operand":{"branch":423}} => {"opcode":"bge","operand":{"branch":181}}` | 1 | 424 |
| `{"opcode":"blt","operand":{"branch":480}} => {"opcode":"bge","operand":{"branch":480}}` | 1 | 481 |
| `{"opcode":"blt","operand":{"branch":75}} => {"opcode":"bge","operand":{"branch":70}}` | 1 | 253 |
| `{"opcode":"blt.un","operand":{"branch":112}} => {"opcode":"bge","operand":{"branch":591}}` | 1 | 594 |
| `{"opcode":"bne.un","operand":{"branch":102}} => {"opcode":"beq","operand":{"branch":122}}` | 1 | 270 |
| `{"opcode":"bne.un","operand":{"branch":106}} => {"opcode":"beq","operand":{"branch":116}}` | 1 | 258 |
| `{"opcode":"bne.un","operand":{"branch":1175}} => {"opcode":"beq","operand":{"branch":980}}` | 1 | 1310 |
| `{"opcode":"bne.un","operand":{"branch":1280}} => {"opcode":"beq","operand":{"branch":1085}}` | 1 | 1439 |
| `{"opcode":"bne.un","operand":{"branch":1307}} => {"opcode":"bne.un","operand":{"branch":1308}}` | 1 | 1308 |
| `{"opcode":"bne.un","operand":{"branch":1612}} => {"opcode":"beq","operand":{"branch":1624}}` | 1 | 2576 |
| `{"opcode":"bne.un","operand":{"branch":1809}} => {"opcode":"beq","operand":{"branch":1152}}` | 1 | 1810 |
| `{"opcode":"bne.un","operand":{"branch":2208}} => {"opcode":"beq","operand":{"branch":1721}}` | 1 | 2250 |
| `{"opcode":"bne.un","operand":{"branch":2420}} => {"opcode":"beq","operand":{"branch":2485}}` | 1 | 2611 |
| `{"opcode":"bne.un","operand":{"branch":260}} => {"opcode":"beq","operand":{"branch":277}}` | 1 | 390 |
| `{"opcode":"bne.un","operand":{"branch":270}} => {"opcode":"beq","operand":{"branch":453}}` | 1 | 1055 |
| `{"opcode":"bne.un","operand":{"branch":292}} => {"opcode":"beq","operand":{"branch":485}}` | 1 | 487 |
| `{"opcode":"bne.un","operand":{"branch":2990}} => {"opcode":"beq","operand":{"branch":2680}}` | 1 | 2991 |
| `{"opcode":"bne.un","operand":{"branch":319}} => {"opcode":"beq","operand":{"branch":338}}` | 1 | 349 |
| `{"opcode":"bne.un","operand":{"branch":340}} => {"opcode":"beq","operand":{"branch":5099}}` | 1 | 5191 |
| `{"opcode":"bne.un","operand":{"branch":343}} => {"opcode":"beq","operand":{"branch":597}}` | 1 | 710 |
| `{"opcode":"bne.un","operand":{"branch":346}} => {"opcode":"beq","operand":{"branch":280}}` | 1 | 347 |
| `{"opcode":"bne.un","operand":{"branch":346}} => {"opcode":"beq","operand":{"branch":371}}` | 1 | 809 |
| `{"opcode":"bne.un","operand":{"branch":375}} => {"opcode":"beq","operand":{"branch":451}}` | 1 | 1240 |
| `{"opcode":"bne.un","operand":{"branch":407}} => {"opcode":"beq","operand":{"branch":184}}` | 1 | 408 |
| `{"opcode":"bne.un","operand":{"branch":432}} => {"opcode":"bne.un","operand":{"branch":151}}` | 1 | 1982 |
| `{"opcode":"bne.un","operand":{"branch":4355}} => {"opcode":"beq","operand":{"branch":4134}}` | 1 | 4356 |
| `{"opcode":"bne.un","operand":{"branch":518}} => {"opcode":"beq","operand":{"branch":675}}` | 1 | 2052 |
| `{"opcode":"bne.un","operand":{"branch":5321}} => {"opcode":"beq","operand":{"branch":5033}}` | 1 | 5322 |
| `{"opcode":"bne.un","operand":{"branch":569}} => {"opcode":"bne.un","operand":{"branch":205}}` | 1 | 570 |
| `{"opcode":"bne.un","operand":{"branch":647}} => {"opcode":"beq","operand":{"branch":28}}` | 1 | 648 |
| `{"opcode":"bne.un","operand":{"branch":679}} => {"opcode":"beq","operand":{"branch":39}}` | 1 | 680 |
| `{"opcode":"bne.un","operand":{"branch":690}} => {"opcode":"beq","operand":{"branch":130}}` | 1 | 691 |
| `{"opcode":"bne.un","operand":{"branch":69}} => {"opcode":"beq","operand":{"branch":116}}` | 1 | 1082 |
| `{"opcode":"bne.un","operand":{"branch":701}} => {"opcode":"beq","operand":{"branch":617}}` | 1 | 702 |
| `{"opcode":"bne.un","operand":{"branch":721}} => {"opcode":"beq","operand":{"branch":28}}` | 1 | 722 |
| `{"opcode":"bne.un","operand":{"branch":72}} => {"opcode":"beq","operand":{"branch":208}}` | 1 | 948 |
| `{"opcode":"bne.un","operand":{"branch":763}} => {"opcode":"beq","operand":{"branch":672}}` | 1 | 1709 |
| `{"opcode":"bne.un","operand":{"branch":856}} => {"opcode":"beq","operand":{"branch":390}}` | 1 | 857 |
| `{"opcode":"bne.un","operand":{"branch":88}} => {"opcode":"beq","operand":{"branch":463}}` | 1 | 1970 |
| `{"opcode":"bne.un","operand":{"branch":943}} => {"opcode":"beq","operand":{"branch":629}}` | 1 | 995 |
| `{"opcode":"bne.un","operand":{"branch":95}} => {"opcode":"beq","operand":{"branch":93}}` | 1 | 3844 |
| `{"opcode":"br","operand":{"branch":1009}} => {"opcode":"br","operand":{"branch":1098}}` | 1 | 1269 |
| `{"opcode":"br","operand":{"branch":114}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 116 |
| `{"opcode":"br","operand":{"branch":114}} => {"opcode":"ret","operand":null}` | 1 | 115 |
| `{"opcode":"br","operand":{"branch":12483}} => {"opcode":"ret","operand":null}` | 1 | 12484 |
| `{"opcode":"br","operand":{"branch":129}} => {"opcode":"ret","operand":null}` | 1 | 130 |
| `{"opcode":"br","operand":{"branch":157}} => {"opcode":"ret","operand":null}` | 1 | 158 |
| `{"opcode":"br","operand":{"branch":169}} => {"opcode":"ret","operand":null}` | 1 | 170 |
| `{"opcode":"br","operand":{"branch":187}} => {"opcode":"ret","operand":null}` | 1 | 188 |
| `{"opcode":"br","operand":{"branch":2038}} => {"opcode":"ret","operand":null}` | 1 | 2039 |
| `{"opcode":"br","operand":{"branch":216}} => {"opcode":"ret","operand":null}` | 1 | 217 |
| `{"opcode":"br","operand":{"branch":310}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 384 |
| `{"opcode":"br","operand":{"branch":411}} => {"opcode":"ret","operand":null}` | 1 | 412 |
| `{"opcode":"br","operand":{"branch":486}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 488 |
| `{"opcode":"br","operand":{"branch":48}} => {"opcode":"ret","operand":null}` | 1 | 49 |
| `{"opcode":"br","operand":{"branch":514}} => {"opcode":"leave","operand":{"branch":1673}}` | 1 | 1674 |
| `{"opcode":"br","operand":{"branch":65}} => {"opcode":"ret","operand":null}` | 1 | 66 |
| `{"opcode":"br","operand":{"branch":660}} => {"opcode":"ret","operand":null}` | 1 | 661 |
| `{"opcode":"br","operand":{"branch":751}} => {"opcode":"ret","operand":null}` | 1 | 752 |
| `{"opcode":"br","operand":{"branch":95}} => {"opcode":"ret","operand":null}` | 1 | 96 |
| `{"opcode":"brfalse","operand":{"branch":100}} => {"opcode":"brtrue","operand":{"branch":157}}` | 1 | 274 |
| `{"opcode":"brfalse","operand":{"branch":105}} => {"opcode":"brtrue","operand":{"branch":105}}` | 1 | 113 |
| `{"opcode":"brfalse","operand":{"branch":130}} => {"opcode":"brfalse","operand":{"branch":131}}` | 1 | 131 |
| `{"opcode":"brfalse","operand":{"branch":147}} => {"opcode":"brtrue","operand":{"branch":147}}` | 1 | 155 |
| `{"opcode":"brfalse","operand":{"branch":150}} => {"opcode":"brtrue","operand":{"branch":150}}` | 1 | 177 |
| `{"opcode":"brfalse","operand":{"branch":165}} => {"opcode":"brfalse","operand":{"branch":166}}` | 1 | 166 |
| `{"opcode":"brfalse","operand":{"branch":236}} => {"opcode":"brtrue","operand":{"branch":265}}` | 1 | 1687 |
| `{"opcode":"brfalse","operand":{"branch":308}} => {"opcode":"brtrue","operand":{"branch":209}}` | 1 | 780 |
| `{"opcode":"brfalse","operand":{"branch":323}} => {"opcode":"brtrue","operand":{"branch":324}}` | 1 | 13213 |
| `{"opcode":"brfalse","operand":{"branch":339}} => {"opcode":"brtrue","operand":{"branch":359}}` | 1 | 629 |
| `{"opcode":"brfalse","operand":{"branch":346}} => {"opcode":"brtrue","operand":{"branch":21}}` | 1 | 347 |
| `{"opcode":"brfalse","operand":{"branch":381}} => {"opcode":"brtrue","operand":{"branch":381}}` | 1 | 534 |
| `{"opcode":"brfalse","operand":{"branch":413}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 1342 |
| `{"opcode":"brfalse","operand":{"branch":52}} => {"opcode":"brtrue","operand":{"branch":52}}` | 1 | 572 |
| `{"opcode":"brfalse","operand":{"branch":538}} => {"opcode":"brtrue","operand":{"branch":176}}` | 1 | 539 |
| `{"opcode":"brfalse","operand":{"branch":55}} => {"opcode":"brtrue","operand":{"branch":96}}` | 1 | 193 |
| `{"opcode":"brfalse","operand":{"branch":65}} => {"opcode":"stloc.s","operand":{"local":24}}` | 1 | 3297 |
| `{"opcode":"brfalse","operand":{"branch":76}} => {"opcode":"brtrue","operand":{"branch":102}}` | 1 | 391 |
| `{"opcode":"brfalse","operand":{"branch":780}} => {"opcode":"brtrue","operand":{"branch":779}}` | 1 | 11390 |
| `{"opcode":"brfalse","operand":{"branch":97}} => {"opcode":"brtrue","operand":{"branch":200}}` | 1 | 8704 |
| `{"opcode":"brtrue","operand":{"branch":102}} => {"opcode":"ldc.i4.0","operand":null}` | 1 | 5801 |
| `{"opcode":"brtrue","operand":{"branch":1090}} => {"opcode":"brtrue","operand":{"branch":66}}` | 1 | 2145 |
| `{"opcode":"brtrue","operand":{"branch":1171}} => {"opcode":"stloc.s","operand":{"local":41}}` | 1 | 4742 |
| `{"opcode":"brtrue","operand":{"branch":117}} => {"opcode":"brfalse","operand":{"branch":75}}` | 1 | 150 |
| `{"opcode":"brtrue","operand":{"branch":12}} => {"opcode":"brfalse","operand":{"branch":204}}` | 1 | 207 |
| `{"opcode":"brtrue","operand":{"branch":130}} => {"opcode":"stloc.s","operand":{"local":11}}` | 1 | 1599 |
| `{"opcode":"brtrue","operand":{"branch":1312}} => {"opcode":"brtrue","operand":{"branch":1130}}` | 1 | 1313 |
| `{"opcode":"brtrue","operand":{"branch":154}} => {"opcode":"ldc.i4.0","operand":null}` | 1 | 669 |
| `{"opcode":"brtrue","operand":{"branch":1550}} => {"opcode":"stloc.s","operand":{"local":30}}` | 1 | 2349 |
| `{"opcode":"brtrue","operand":{"branch":155}} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 460 |
| `{"opcode":"brtrue","operand":{"branch":165}} => {"opcode":"stloc.s","operand":{"local":10}}` | 1 | 693 |
| `{"opcode":"brtrue","operand":{"branch":169}} => {"opcode":"stloc.s","operand":{"local":34}}` | 1 | 2049 |
| `{"opcode":"brtrue","operand":{"branch":17}} => {"opcode":"stloc.3","operand":null}` | 1 | 102 |
| `{"opcode":"brtrue","operand":{"branch":18}} => {"opcode":"stloc.0","operand":null}` | 1 | 38 |
| `{"opcode":"brtrue","operand":{"branch":1977}} => {"opcode":"brfalse","operand":{"branch":1988}}` | 1 | 2501 |
| `{"opcode":"brtrue","operand":{"branch":210}} => {"opcode":"brfalse","operand":{"branch":347}}` | 1 | 527 |
| `{"opcode":"brtrue","operand":{"branch":238}} => {"opcode":"brfalse","operand":{"branch":245}}` | 1 | 251 |
| `{"opcode":"brtrue","operand":{"branch":23}} => {"opcode":"brfalse","operand":{"branch":37}}` | 1 | 50 |
| `{"opcode":"brtrue","operand":{"branch":243}} => {"opcode":"stloc.s","operand":{"local":7}}` | 1 | 487 |
| `{"opcode":"brtrue","operand":{"branch":248}} => {"opcode":"stloc.1","operand":null}` | 1 | 447 |
| `{"opcode":"brtrue","operand":{"branch":271}} => {"opcode":"stloc.s","operand":{"local":64}}` | 1 | 2605 |
| `{"opcode":"brtrue","operand":{"branch":27}} => {"opcode":"brfalse","operand":{"branch":118}}` | 1 | 1396 |
| `{"opcode":"brtrue","operand":{"branch":3000}} => {"opcode":"brfalse","operand":{"branch":2975}}` | 1 | 5315 |
| `{"opcode":"brtrue","operand":{"branch":30}} => {"opcode":"brfalse","operand":{"branch":236}}` | 1 | 241 |
| `{"opcode":"brtrue","operand":{"branch":320}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 723 |
| `{"opcode":"brtrue","operand":{"branch":336}} => {"opcode":"stloc.s","operand":{"local":5}}` | 1 | 786 |
| `{"opcode":"brtrue","operand":{"branch":34}} => {"opcode":"brfalse","operand":{"branch":34}}` | 1 | 341 |
| `{"opcode":"brtrue","operand":{"branch":353}} => {"opcode":"brfalse","operand":{"branch":356}}` | 1 | 654 |
| `{"opcode":"brtrue","operand":{"branch":44}} => {"opcode":"stloc.s","operand":{"local":5}}` | 1 | 272 |
| `{"opcode":"brtrue","operand":{"branch":477}} => {"opcode":"brfalse","operand":{"branch":517}}` | 1 | 832 |
| `{"opcode":"brtrue","operand":{"branch":49}} => {"opcode":"brfalse","operand":{"branch":49}}` | 1 | 1816 |
| `{"opcode":"brtrue","operand":{"branch":49}} => {"opcode":"brfalse","operand":{"branch":61}}` | 1 | 102 |
| `{"opcode":"brtrue","operand":{"branch":49}} => {"opcode":"stloc.s","operand":{"local":6}}` | 1 | 508 |
| `{"opcode":"brtrue","operand":{"branch":516}} => {"opcode":"stloc.s","operand":{"local":13}}` | 1 | 3587 |
| `{"opcode":"brtrue","operand":{"branch":604}} => {"opcode":"brfalse","operand":{"branch":12}}` | 1 | 605 |
| `{"opcode":"brtrue","operand":{"branch":76}} => {"opcode":"brfalse","operand":{"branch":94}}` | 1 | 164 |
| `{"opcode":"brtrue","operand":{"branch":903}} => {"opcode":"brfalse","operand":{"branch":946}}` | 1 | 1437 |
| `{"opcode":"ldarg.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 355 |
| `{"opcode":"ldarg.0","operand":null} => {"opcode":"dup","operand":null}` | 1 | 1789 |
| `{"opcode":"ldarg.1","operand":null} => {"opcode":"ldloc.s","operand":{"local":5}}` | 1 | 432 |
| `{"opcode":"ldarg.3","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 110 |
| `{"opcode":"ldarg.s","operand":{"parameter":3}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 506 |
| `{"opcode":"ldc.i4.0","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 286 |
| `{"opcode":"ldc.i4.0","operand":null} => {"opcode":"stloc.s","operand":{"local":6}}` | 1 | 623 |
| `{"opcode":"ldc.i4.1","operand":null} => {"opcode":"switch","operand":{"switch":[71,43,118,123]}}` | 1 | 178 |
| `{"opcode":"ldc.i4.1","operand":null} => {"opcode":"switch","operand":{"switch":[79,51,92,109,122,297]}}` | 1 | 378 |
| `{"opcode":"ldc.i4.s","operand":21} => {"opcode":"stloc.s","operand":{"local":54}}` | 1 | 3554 |
| `{"opcode":"ldfld","operand":{"field":"class form.DebugForm @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null::udat_:class game.UserData @ Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 13913 |
| `{"opcode":"ldloc.3","operand":null} => {"opcode":"dup","operand":null}` | 1 | 134 |
| `{"opcode":"leave","operand":{"branch":217}} => {"opcode":"leave","operand":{"branch":206}}` | 1 | 600 |
| `{"opcode":"ret","operand":null} => {"opcode":"br","operand":{"branch":1331}}` | 1 | 1324 |
| `{"opcode":"ret","operand":null} => {"opcode":"br","operand":{"branch":2267}}` | 1 | 2454 |
| `{"opcode":"ret","operand":null} => {"opcode":"br","operand":{"branch":6958}}` | 1 | 6994 |
| `{"opcode":"ret","operand":null} => {"opcode":"br","operand":{"branch":996}}` | 1 | 1007 |
| `{"opcode":"stloc.0","operand":null} => {"opcode":"stloc.s","operand":{"local":6}}` | 1 | 99 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 161 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"ret","operand":null}` | 1 | 52 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 1420 |
| `{"opcode":"stloc.1","operand":null} => {"opcode":"stloc.s","operand":{"local":86}}` | 1 | 9408 |
| `{"opcode":"stloc.2","operand":null} => {"opcode":"stloc.s","operand":{"local":12}}` | 1 | 584 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 132 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"dup","operand":null}` | 1 | 758 |
| `{"opcode":"stloc.3","operand":null} => {"opcode":"ret","operand":null}` | 1 | 183 |
| `{"opcode":"stloc.s","operand":{"local":10}} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 395 |
| `{"opcode":"stloc.s","operand":{"local":10}} => {"opcode":"stloc.s","operand":{"local":8}}` | 1 | 1251 |
| `{"opcode":"stloc.s","operand":{"local":12}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 1381 |
| `{"opcode":"stloc.s","operand":{"local":13}} => {"opcode":"stloc.s","operand":{"local":11}}` | 1 | 716 |
| `{"opcode":"stloc.s","operand":{"local":14}} => {"opcode":"dup","operand":null}` | 1 | 3287 |
| `{"opcode":"stloc.s","operand":{"local":16}} => {"opcode":"stloc.s","operand":{"local":12}}` | 1 | 3472 |
| `{"opcode":"stloc.s","operand":{"local":19}} => {"opcode":"stloc.s","operand":{"local":20}}` | 1 | 1685 |
| `{"opcode":"stloc.s","operand":{"local":28}} => {"opcode":"stloc.s","operand":{"local":31}}` | 1 | 2067 |
| `{"opcode":"stloc.s","operand":{"local":41}} => {"opcode":"stloc.s","operand":{"local":13}}` | 1 | 2207 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 523 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.0","operand":null}` | 1 | 709 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.1","operand":null}` | 1 | 1029 |
| `{"opcode":"stloc.s","operand":{"local":4}} => {"opcode":"stloc.s","operand":{"local":9}}` | 1 | 1128 |
| `{"opcode":"stloc.s","operand":{"local":5}} => {"opcode":"ret","operand":null}` | 1 | 134 |
| `{"opcode":"stloc.s","operand":{"local":5}} => {"opcode":"stloc.0","operand":null}` | 1 | 1091 |
| `{"opcode":"stloc.s","operand":{"local":5}} => {"opcode":"stloc.2","operand":null}` | 1 | 398 |
| `{"opcode":"stloc.s","operand":{"local":6}} => {"opcode":"stloc.s","operand":{"local":12}}` | 1 | 1937 |
| `{"opcode":"stloc.s","operand":{"local":6}} => {"opcode":"stloc.s","operand":{"local":4}}` | 1 | 1054 |
| `{"opcode":"stloc.s","operand":{"local":7}} => {"opcode":"stloc.s","operand":{"local":8}}` | 1 | 261 |
| `{"opcode":"stloc.s","operand":{"local":8}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 1863 |
| `{"opcode":"stloc.s","operand":{"local":8}} => {"opcode":"stloc.s","operand":{"local":7}}` | 1 | 4629 |
| `{"opcode":"stloc.s","operand":{"local":9}} => {"opcode":"DELETION_POINT","operand":null}` | 1 | 448 |
| `{"opcode":"stloc.s","operand":{"local":9}} => {"opcode":"dup","operand":null}` | 1 | 2114 |
| `{"opcode":"switch","operand":{"switch":[188,212,284,308,332,356,380,404,428,452,476,500,524,548,524,524,572,596,212,620,644,668,524,500,260,716,236,692]}} => {"opcode":"switch","operand":{"switch":[188,212,284,308,332,356,380,404,428,452,476,500,524,548,524,524,572,596,212,620,644,668,524,500,260,716,236,692]}}` | 1 | 717 |
| `{"opcode":"switch","operand":{"switch":[92,123,70,70,70,102]}} => {"opcode":"ldc.i4.1","operand":null}` | 1 | 322 |

</details>

**Paradas do motor — Assembly-CSharp.**

| Parada final por ID único | Métodos |
| --- | --- |
| body_equal | 15 |
| model_prediction_limit | 2 |
| no_model_approved_candidate | 122 |
| no_transformation_advances | 128 |
| source_mapping_UNKNOWN | 2 |

Limites: 16 iterações por método, 96 candidatos compilados por etapa; 0 métodos atingiram o limite de iterações. Cada parada registra offset, comando e localização alinhada ou UNKNOWN. Histórico de retomadas não é somado às paradas finais.

| Assembly / mesmos pendentes iniciais | Assinaturas iguais antes | Iguais depois | Novas iguais | Igualdades perdidas |
| --- | --- | --- | --- | --- |
| KairoLibrary | 46 | 48 | 3 | 1 |
| Assembly-CSharp | 149 | 164 | 15 | 0 |

Resultado negativo adotado em método ainda pendente: `KairoLibrary::kairo.unity.ui.Graphics::_drawBitmap(kairo.unity.ui.Image,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single,System.Single)->System.Void`. A assinatura antes igual deixou de coincidir; o prefixo IL bruto avançou de 1554 para 1822, mas o corpo continua decompiled. Isso não é regressão de um corpo anteriormente matched. A ordem de locais desse método precisa ser recuperada antes de concluir o corpo.

## 5. TOOLCHAIN

KairoLibrary: compilador `roslyn-csc`; versão `1.1.0.51109`; SHA-256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`. Flags: `/nologo /noconfig /nostdlib+ /target:library /platform:anycpu /optimize+ /debug:pdbonly /deterministic- /langversion:6 /unsafe- /checked- /codepage:65001 /utf8output`. Referências na ordem: mscorlib, Version=2.0.0.0; System.Core, Version=3.5.0.0; UnityEngine, Version=0.0.0.0; System, Version=2.0.0.0.

Assembly-CSharp: compilador `mono-smcs`; versão `2.6.5.0`; SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. Flags: `-nologo -noconfig -target:library -platform:anycpu -optimize+ -debug- -langversion:Default -unsafe- -checked- -codepage:65001`. Referências na ordem: mscorlib, Version=2.0.5.0; KairoLibrary, Version=1.0.0.0; UnityEngine, Version=0.0.0.0; System.Core, Version=2.0.5.0; System, Version=2.0.5.0.

Produção preservada: pacote Microsoft.Net.Compilers 1.1.0 / csc 1.1.0.51109 e Unity 5.3.4f1 / smcs 2.6.5.0. Nenhum compilador, define, perfil, referência ou opção de produção mudou. O helper moderno é usado somente para sintaxe/tipos e não emite os assemblies alvo. MDB é somente mapeamento com prova debug+/debug-; produção smcs usa -debug-.

UNKNOWN: revisão histórica exata de fabricação dos compiladores, comando original completo, referências históricas integrais e nomes dos locais sem símbolos. O snapshot Roslyn version-1.1.0 foi fixado em 4cb203dab89e1542cdf7d88cb6d32767bc0e5460; sua correspondência exata ao executável instalado não foi comprovada.

## 6. COMPONENTES EXTERNOS E METADADOS

Sem mudança de externos nesta rodada: sete DLLs verificadas contra Unity oficial, incluindo UnityEngine.dll do Android Mono (970752 bytes; SHA-256 7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421), e seis bibliotecas nativas verificadas contra Android Mono Release. Inventário: 1218 entradas, sendo dois alvos, 14 externos e 1202 dados.

| Assembly | Tipos estruturais iguais | Declarações sem corpo comprovadas |
| --- | --- | --- |
| KairoLibrary | 337/337 | 76 |
| Assembly-CSharp | 129/129 | 6 |

Os 466 tipos e as 82 declarações sem corpo passaram novamente: ordem de tipos/membros, parâmetros, atributos personalizados, layout, constantes, recursos e estruturas globais. Nenhum ID original ausente/adicional. Essa prova não significa igualdade bruta dos heaps, tabelas ou arquivos DLL.

Campos não determinísticos, sem acréscimos: KairoLibrary — Module.Mvid, COFF.TimeDateStamp, IMAGE_DEBUG_DIRECTORY.TimeDateStamp, CodeView.RSDS.PdbGuid; Assembly-CSharp — Module.Mvid, COFF.TimeDateStamp. As seis exceções conservam a prova anterior de duas compilações idênticas. Nenhum byte de assembly foi normalizado.

A prova de compilação dos 260 C# exatamente como preparados para o Git fica em reports/round6-git-source-proof.json. reports/round6-git-content-provenance.json relaciona hashes medidos e publicados quando .gitattributes converte CRLF para LF. Conversão de texto não é normalização de DLL.

REA respondeu para a inspeção estática gerenciada; evidência retida em reports/round6-rea-evidence.json. Limitações do diagnóstico global de registros opcionais do REA não foram confundidas com indisponibilidade dessa análise. DEX/Java permaneceu fora, e nenhuma correção baseada em bytes brutos foi executada.

## 7. BLOQUEIOS

Nenhuma instalação, arquivo ou decisão do usuário é necessária para continuar no C#. Os limites técnicos são a cobertura incompleta do modelo, soluções de declarações não únicas, candidatos sem avanço ou limitados pelo orçamento e divergências posteriores de fluxo/exceções. O modelo recusa previsões incompletas; essa recusa não é corpo matched. A revisão histórica exata continua UNKNOWN. DEX/Java permanece fora do escopo.

## 8. SUA RECOMENDAÇÃO

1. Continuar pelos 10 métodos smcs ainda divergentes com avanço parcial, a partir do offset/comando alinhado final. Potencial delimitado: 0–10 métodos; ganho exato UNKNOWN.
2. Investigar o grupo alinhado smcs com maior volume de IL (4 métodos / 42013 bytes), usando o modelo para distinguir declaração, bloco e temporário. Potencial: 0–4 métodos; causa comum ainda precisa de prova.
3. Calcular mudanças de escopo e possível união de variáveis nos 20 pendentes sem sequência injetiva consecutiva dentro do bloco lexical. Potencial delimitado: 0–20 métodos; ganho exato UNKNOWN. Primeiro/último uso não basta para provar uma união: exigir vínculos, tipos e fluxo, modelo atualizado e comparação completa. As populações das recomendações podem se sobrepor.
