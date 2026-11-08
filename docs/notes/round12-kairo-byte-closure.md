# KairoLibrary: fechamento literal na rodada 12

O verificador confirmou 3.287 métodos, 269.894 bytes de IL e 337 tipos. A aceitação independente do root está em [round12-kairo-independent-raw-acceptance.json](../../reports/round12-kairo-independent-raw-acceptance.json).

O PE original e o recompilado têm 569.856 bytes. Todos os 569.816 bytes fora dos quatro campos comprovadamente não determinísticos são idênticos. Os únicos 40 bytes diferentes pertencem a `Module.Mvid`, `COFF.TimeDateStamp`, `IMAGE_DEBUG_DIRECTORY.TimeDateStamp` e `CodeView.RSDS.PdbGuid`, nas mesmas posições dos dois arquivos. Não houve normalização ou edição de DLL.

A comparação inicial falhou apesar dos métodos correspondentes: faltavam quatro linhas de `StandAloneSig`, deslocando os heaps em oito bytes. Nenhuma referência ou opção de compilação foi trocada para corrigir isso. Os testes mostraram que dados de depuração nativos também influenciam metadados do PE.

| Experimento | Alteração C# | Resultado medido |
| --- | --- | --- |
| R01 | Remover alias `Object` adicionado na reconstrução e qualificar sua chamada | 187 linhas de `TypeRef` voltaram à ordem e aos bytes originais |
| R02 | Acrescentar aliases genéricos `MeshPair` e `MeshTable` | Ordem dos 136 `TypeSpec` restaurada; `TypeRef` preservado |
| R03 | Constante local `Int32` em `Language.LoadLanguagePack` | FieldSig `0608`, linha 43 |
| R04 | Constante local `String` em `Log.GetThrowableName` | FieldSig `060e`, linha 48 |
| R05 | Constante local `Byte` em `StringUtil.GetCode` | FieldSig `0605`, linha 85; índices de `AssemblyRef` também restaurados |
| R06 | Constante local `Single` em `SurfaceManagerBase.GetSelectPaintPoint` | FieldSig `060c`, linha 310; 534 assinaturas e zero bytes residuais |

As quatro constantes são locais e não usadas. Seus nomes e valores foram escolhidos; os históricos são **UNKNOWN**. Os tipos e posições foram inferidos do assembly e confirmados pelo compilador. Os aliases também têm nomes escolhidos para conservar a ordem ordinal de emissão; não são apresentados como nomes originais recuperados.

Seis bibliotecas próprias demonstraram o mecanismo antes de alterar o jogo: constantes usadas e não usadas geraram quatro assinaturas FIELD sem alterar os corpos; `/debug-` retirou essas assinaturas; aliases não usados mudaram a ordem de `TypeRef`/`TypeSpec` sem mudar os corpos. Todos os caminhos de saída e PDB desses testes ficaram isolados no diretório privado.

Cada experimento do jogo preservou todos os métodos, headers, locais, regiões de exceção e tipos. O diário separado [round12-raw-source-journal.json](../../reports/round12-raw-source-journal.json) registra snapshots, manifests, fontes, PE e recibos. Esses passos ganharam **zero métodos**; não alteram a contagem dos onze ciclos de corpos da Parte A nem consomem ciclos da Parte B.

O caminho do PDB histórico continua vindo do compilador por `/pdb:`. A versão e as referências permaneceram fixas. Nenhum IL, corpo, blob ou campo do PE original foi transplantado para o resultado.

`PROGRESS.md` é produzido pelo gerador. O rótulo **CONCLUÍDA** exige uma prova literal ligada ao SHA do PE canônico e às fontes atuais. Uma recompilação muda os campos não determinísticos; a prova é renovada no novo arquivo antes de continuar exibindo o rótulo.
