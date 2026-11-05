# Linha de base de KairoLibrary

O mesmo conjunto ordenado de 163 arquivos C# foi compilado integralmente com Roslyn 5.8 e com Microsoft.Net.Compilers 1.3.2. Ambos terminaram sem erros. Opções e referências são iguais, exceto pelo caminho de saída. `tools/compare_kairo.py` confere hashes dos fontes, referências e artefatos antes da comparação.

O pacote [Microsoft.Net.Compilers 1.3.2](https://www.nuget.org/packages/Microsoft.Net.Compilers/1.3.2) existe e é da geração [Visual Studio 2015 Update 3](https://learn.microsoft.com/en-us/visualstudio/extensibility/roslyn-version-support). Seu `csc.exe /version` informa **1.3.1.60621**: versão do pacote e versão interna do compilador são registradas separadamente. Não foi necessário instalar outro Visual Studio.

## Comparação completa

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

Resultados completos: [CSV por método](../../reports/KairoLibrary-compiler-comparison.csv), [resumo comparativo](../../reports/KairoLibrary-compiler-comparison.json) e [resumo do verificador](../../reports/KairoLibrary-verification.json). Os rótulos de padrões são sugestões automáticas, não diagnóstico comprovado da causa.

O primeiro verificador promoveu 2.473 métodos e reteve 314 corpos iguais. A regra mecânica por nomes e referências no IL liberou mais 310: agora são **2.783 matched e 504 decompiled**, 84,7% da KairoLibrary. No projeto inteiro são 2.783/5.245, 53,06%; o percentual de bytes IL é diferente e aparece em PROGRESS.md. As 504 pendências são 424 corpos divergentes/ausentes, 76 declarações sem CIL e quatro corpos iguais sem vínculo comprovado. Nenhum tipo ou assembly foi declarado idêntico.

O verificador resolve referências genéricas locais para suas definições; testa criação de closures e ponteiros de delegates no original e no recompilado; confere nomes completos, nomes de origem embutidos e vínculos transitivos. Propriedades automáticas exigem accessor, backing field e metadados correspondentes. Campos de tabelas constantes exigem também os valores iniciais idênticos. Cada corpo continua sendo comparado individualmente. Um grupo pode ter alguns corpos correspondentes e outros pendentes: provar propriedade do membro não declara o grupo inteiro concluído.

Os quatro retidos são `.ctor`/`.cctor` de `Canvas/<>c` e `KairoBase/<>c`: os corpos correspondem, mas os caminhos de ligação ao método de origem não satisfazem a regra atual. Não foram liberados apenas por terem nomes reconhecíveis. Provas e pendências: [relatório de grupos gerados](../../reports/KairoLibrary-generated-groups.json).

## Grupos das falhas

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

Somam 424. São categorias de diagnóstico, não causas comprovadas. Em 330 desses casos os dois compiladores geram o mesmo resultado divergente. Nos outros 94, 55 identidades estão ausentes; restam 39 corpos presentes que diferem entre os compiladores e nenhum corresponde. Não foram testadas versões vizinhas ainda: a escolha do próximo grupo será feita com o usuário, antes dos ajustes individuais. [JSON com edições e exemplos](../../reports/KairoLibrary-failure-groups.json) e [CSV](../../reports/KairoLibrary-failure-groups.csv).

## O que a comparação permite concluir

O pacote 1.3.2 é um candidato melhor: acrescenta 51 correspondências e não perde nenhuma. Isso sustenta usá-lo na próxima etapa. Ainda não identifica inequivocamente o executável histórico: versões próximas, opções e fontes diferentes podem gerar os mesmos corpos.

Os 330 corpos iguais nas duas saídas afastam uma diferença de geração **entre esses dois compiladores, para esses fontes e opções**. Eles priorizam investigação do C# recuperado, referências e opções; não excluem todos os outros compiladores possíveis. Nos 94 casos restantes é preciso distinguir sensibilidade à versão, recuperação de fonte e identidade dos membros gerados, um método por vez.

## Cabeçalho e determinismo

KairoLibrary original: linker PE 48.0, timestamp `2016-11-04T08:59:39Z`; diretório de depuração com uma entrada CodeView tipo 2, assinatura RSDS, PDB `KairoLibrary.pdb`, age 1. Não há entrada tipo 16 REPRO. O caminho absoluto original do PDB não é publicado. Não há PDB nem MDB da KairoLibrary no APK.

Não foram encontrados TypeDefs `IsReadOnlyAttribute`, `NullableAttribute`, `NullableContextAttribute`, `RefSafetyRulesAttribute` ou `EmbeddedAttribute`. A ausência pode decorrer de recursos não utilizados, por isso não define um limite superior seguro da versão.

O padrão é compatível com compilação antiga não determinística. A data só limita as versões se realmente for um timestamp de compilação; ausência de REPRO não é prova universal para compiladores antigos. Um MVID determinístico também depende de [todos os insumos e opções](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/code-generation), não apenas da versão do compilador. Não é um teste isolado definitivo dessa versão.

Duas compilações idênticas das sondas demonstraram variação de MVID e timestamp com os candidatos testados. A repetição do **build completo com 1.3.2 e debug:pdbonly** mostrou 34 bytes diferentes, todos em quatro campos: MVID, timestamp COFF, timestamp do diretório de depuração e GUID do PDB CodeView. Nenhum byte inexplicado. Logo, neste candidato com debug não basta considerar apenas MVID e data COFF. [Evidência reproduzível](../../reports/KairoLibrary-repeatability.json). Nenhuma exceção foi aplicada e a lista de campos normalizados continua vazia; esses testes não demonstram retroativamente quais opções o desenvolvedor usou.

## Próxima etapa

Usar o pacote 1.3.2 como linha de base experimental e escolher um grupo de falhas com o usuário. Ajustes de fonte seguem um método por vez; mudanças justificadas de opções precisam passar pela comparação integral. Proteger os 2.783 métodos já correspondentes. Recuperar Assembly-CSharp separadamente; conferir metadados dos tipos, tabelas e heaps antes de tentar identidade do arquivo completo.
