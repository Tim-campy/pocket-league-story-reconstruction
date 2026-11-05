# Linha de base de KairoLibrary

O mesmo conjunto ordenado de 163 arquivos C# foi compilado integralmente com Roslyn 5.8 e com Microsoft.Net.Compilers 1.3.2. Ambos terminaram sem erros. Opções e referências são iguais, exceto pelo caminho de saída. `tools/compare_kairo.py` confere hashes dos fontes, referências e artefatos antes da comparação.

O pacote [Microsoft.Net.Compilers 1.3.2](https://www.nuget.org/packages/Microsoft.Net.Compilers/1.3.2) existe e é da geração [Visual Studio 2015 Update 3](https://learn.microsoft.com/en-us/visualstudio/extensibility/roslyn-version-support). Seu `csc.exe /version` informa **1.3.1.60621**: versão do pacote e versão interna do compilador são registradas separadamente. Não foi necessário instalar outro Visual Studio.

## Comparação inicial completa

| Resultado dos corpos de métodos | Quantidade |
|---|---:|
| Correspondem com os dois compiladores | 2.736 |
| Correspondem somente com o pacote 1.3.2 | 51 |
| Correspondem somente com o Roslyn 5.8 | 0 |
| Compiladores geram o mesmo corpo, diferente do original | 330 |
| Saídas diferentes ou método ausente, nenhum corresponde | 94 |
| Total de corpos originais | 3.211 |

Há também 76 declarações sem corpo, fora desse denominador. Com 1.3.2, 369 corpos presentes diferem e 55 identidades originais estão ausentes; o build contém 53 identidades adicionais, sobretudo relacionadas a geração de código. Esses números não significam perda automática da lógica: as identidades e estruturas geradas ainda precisam ser recuperadas.

A comparação mantém cabeçalhos tiny/fat, maxstack, assinatura dos locais, posições e larguras das instruções, destinos de switch, constantes, padding e seções de exceção. Somente operandos de tokens são substituídos por identidades resolvidas, conforme AGENTS.md. A igualdade dos bytes brutos sem essa resolução ocorre em apenas 195 corpos com 1.3.2 e 194 com 5.8. Nenhum assembly foi editado após compilar.

Resultados dessa linha de base: [CSV inicial por método](../../reports/KairoLibrary-compiler-comparison.initial.csv), [resumo inicial](../../reports/KairoLibrary-compiler-comparison.initial.json). Os relatórios sem `.initial` e o [resumo do verificador](../../reports/KairoLibrary-verification.json) refletem os fontes atuais. Os rótulos de padrões são sugestões automáticas, não diagnóstico comprovado da causa.

Na primeira linha de base o verificador promoveu 2.473 métodos e reteve 314 corpos iguais. A regra mecânica por nomes e referências no IL liberou mais 310: essa etapa encerrou com **2.783 matched e 504 decompiled**, 84,7% da KairoLibrary. Eram 424 corpos divergentes/ausentes, 76 declarações sem CIL e quatro corpos iguais sem vínculo comprovado. Esses números históricos foram superados pelos ajustes descritos abaixo; o estado atual é gerado em [PROGRESS.md](../../PROGRESS.md). Nenhum tipo ou assembly foi declarado idêntico.

O verificador resolve referências genéricas locais para suas definições; testa criação de closures e ponteiros de delegates no original e no recompilado; confere nomes completos, nomes de origem embutidos e vínculos transitivos. Propriedades automáticas exigem accessor, backing field e metadados correspondentes. Campos de tabelas constantes exigem também os valores iniciais idênticos. Cada corpo continua sendo comparado individualmente. Um grupo pode ter alguns corpos correspondentes e outros pendentes: provar propriedade do membro não declara o grupo inteiro concluído.

Os quatro inicialmente retidos eram `.ctor`/`.cctor` de `Canvas/<>c` e `KairoBase/<>c`. A regra atual também exige que todas as lambdas sem captura do tipo correspondam antes de promover seus dois helpers de cache; nomes isolados não bastam. Grupos podem mudar durante a recuperação. Provas e pendências atuais: [relatório de grupos gerados](../../reports/KairoLibrary-generated-groups.json).

## Grupos iniciais das falhas

Classificação primária exclusiva dos 424 corpos divergentes ou ausentes, baseada nas edições reais do IL:

| Grupo | Métodos |
|---|---:|
| Desvios e retornos antecipados | 106 |
| Locais e ordem de expressões | 61 |
| Identidades de membros gerados ausentes | 55 |
| Outras mudanças de instruções ou operandos | 43 |
| Exceções, using e finally | 37 |
| Aritmética, conversões e expressões redundantes | 37 |
| Nomes gerados ou destinos de delegates | 31 |
| Geração de switch | 20 |
| call contra callvirt | 19 |
| Geração de closures ou delegates | 8 |
| Concatenação de strings | 4 |
| Geração de lock | 3 |

Somam 424 na linha de base inicial. São categorias de diagnóstico, não causas comprovadas. Em 330 desses casos os dois compiladores geravam o mesmo resultado divergente. Nos outros 94, 55 identidades estavam ausentes; restavam 39 corpos presentes que diferiam entre os compiladores e nenhum correspondia. Cópias imutáveis desse diagnóstico foram preservadas nos arquivos `*.initial.*` em reports/. Os relatórios sem esse sufixo passam a refletir os fontes atuais. [JSON atual](../../reports/KairoLibrary-failure-groups.json) e [CSV atual](../../reports/KairoLibrary-failure-groups.csv).

## O que a comparação permite concluir

O pacote 1.3.2 é um candidato melhor: acrescenta 51 correspondências e não perde nenhuma. Isso sustenta usá-lo na próxima etapa. Ainda não identifica inequivocamente o executável histórico: versões próximas, opções e fontes diferentes podem gerar os mesmos corpos.

Os 330 corpos iguais nas duas saídas afastam uma diferença de geração **entre esses dois compiladores, para esses fontes e opções**. Eles priorizam investigação do C# recuperado, referências e opções; não excluem todos os outros compiladores possíveis. Nos 94 casos restantes é preciso distinguir sensibilidade à versão, recuperação de fonte e identidade dos membros gerados, um método por vez.

## Cabeçalho e determinismo

KairoLibrary original: linker PE 48.0, timestamp `2016-11-04T08:59:39Z`; diretório de depuração com uma entrada CodeView tipo 2, assinatura RSDS, PDB `KairoLibrary.pdb`, age 1. Não há entrada tipo 16 REPRO. O caminho absoluto original do PDB não é publicado. Não há PDB nem MDB da KairoLibrary no APK.

Não foram encontrados TypeDefs `IsReadOnlyAttribute`, `NullableAttribute`, `NullableContextAttribute`, `RefSafetyRulesAttribute` ou `EmbeddedAttribute`. A ausência pode decorrer de recursos não utilizados, por isso não define um limite superior seguro da versão.

O padrão é compatível com compilação antiga não determinística. A data só limita as versões se realmente for um timestamp de compilação; ausência de REPRO não é prova universal para compiladores antigos. Um MVID determinístico também depende de [todos os insumos e opções](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/code-generation), não apenas da versão do compilador. Não é um teste isolado definitivo dessa versão.

Duas compilações idênticas das sondas demonstraram variação de MVID e timestamp com os candidatos testados. A repetição do **build completo com 1.3.2 e debug:pdbonly** mostrou 34 bytes diferentes, todos em quatro campos: MVID, timestamp COFF, timestamp do diretório de depuração e GUID do PDB CodeView. Nenhum byte inexplicado. Logo, neste candidato com debug não basta considerar apenas MVID e data COFF. [Evidência reproduzível](../../reports/KairoLibrary-repeatability.json). As quatro exceções foram registradas por solicitação do usuário em docs/toolchain.md; nenhum normalizador foi aplicado. Esses testes não demonstram retroativamente quais opções o desenvolvedor usou.

## Próxima etapa

Continuar com o pacote 1.3.2 e com os grupos orientados pelo usuário. Ajustes de fonte seguem um método por vez; alterações globais de ordem autorizadas precisam de candidato isolado, build integral e prova de ausência de perdas. Todos os matched atuais ficam protegidos. Assembly-CSharp também já compila e tem verificação própria. Metadados dos tipos, tabelas e heaps seguem pendentes.

## Recuperação por ordem e variantes

As sondas próprias confirmaram a numeração Roslyn, incluindo o peso das propriedades e acessores. O merge das ordens de metadados com 133 restrições extraídas dos nomes gerados restaurou dez tipos; dois exigiram recuperar construtores vazios omitidos pelo ILSpy. O candidato integral acrescentou **72 corpos correspondentes, sem perder nenhum**. As alterações ficaram restritas à ordem das declarações e aos dois construtores legítimos; os corpos existentes foram preservados. [Prova de aplicação](../../reports/KairoLibrary-member-order-application.json), [sondas](../../reports/KairoLibrary-member-order-probe.json).

O grupo `call`/`callvirt` foi fechado em **19 de 19 métodos**, com verificação integral após cada ajuste. A causa confirmada foi `?.` ou chamada direta sobre `new`, sem divergência nas declarações das referências. [Diagnóstico e reparos](kairo-callvirt.md).

As variantes ILSpy e Codemerx são compiladas integralmente em cópias isoladas. A seleção compara cada corpo com o original, associa membros gerados à origem e transplanta somente C# candidato; o verificador integral precisa confirmar o alvo e a preservação dos matched antes da aceitação. Variantes incompatíveis com C# 6 recebem reparos de sintaxe documentados ou fallback identificado; resultados de fallback não são atribuídos ao decompilador independente. As contagens finais dessas tentativas ficam nos relatórios de variantes.

## Resultado desta rodada

KairoLibrary passou de **2.783 para 3.027 matched**, 92,09% dos 3.287 métodos. A evolução foi: 19 call/callvirt, 72 corpos por ordem dos membros, 141 seleções por método, quatro inicializadores estáticos, três casos rotulados lock, três rotulados concatenação e dois guardas de Image.Use/Unuse. Todas as adoções passaram pela compilação integral; nenhum corpo que correspondia na linha de base deixou de corresponder.

Nos fontes finais, pacote 1.3.2 reproduz **3.031/3.211 corpos**, contra **2.965** do Roslyn 5.8: 66 correspondências somente no antigo e zero somente no novo. Restam 180 corpos divergentes/ausentes: 151 saídas iguais entre os candidatos e 29 diferentes/ausentes. Os 76 métodos sem CIL e quatro helpers de caches ficam separados. [Comparação final](../../reports/KairoLibrary-compiler-comparison.json), [grupos restantes](../../reports/KairoLibrary-failure-groups.json).

A regra mais forte de cache compartilhado exige todas as lambdas do tipo correspondentes. Por isso dois helpers de KairoPlugin antes classificados como matched foram retidos, embora seus próprios corpos continuem iguais; a mudança foi de prova de vínculo, sem regressão de IL. Os dois helpers de Canvas foram liberados após recuperar a ordem. Permanecem quatro helpers retidos entre KairoPlugin e KairoBase. [Auditoria de estados e corpos](../../reports/Recovery-pass-regression-audit.json).

Dos 145 alvos oferecidos pelas quatro variantes, 141 foram adotados pelo seletor; quatro `.cctor` implícitos exigiram recuperar inicializadores de campos. Config recebeu 46 RHS do fonte independente e AdMobPlugin, SoundPlayer e CRC64 receberam `= null`; não foram criados construtores estáticos explícitos que alterassem BeforeFieldInit. [Seleção](kairo-variant-selection.md), [inicializadores](kairo-static-initializers.md), [aplicação final por alvo](../../reports/KairoLibrary-manual-recovery.json).

Os rótulos lock e concatenação não eram causas confirmadas: os três casos de cada grupo foram resolvidos com escopos, locais, ordem de atribuições e desvios. Monitor e String.Concat já tinham assinaturas corretas. As duas funções Image.Use/Unuse exigiram separar guardas que o decompilador havia juntado; suas lambdas foram preservadas. KairoPlugin.GetStatusBarHeight continua documentado como UNKNOWN após testes limitados, sem aceitar código apenas equivalente.
