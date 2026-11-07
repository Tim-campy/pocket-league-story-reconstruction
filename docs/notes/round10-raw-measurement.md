# Rodada 10 — medição bruta da KairoLibrary

## Base e alcance

Esta etapa mede bytes; não corrige a reconstrução. A baseline congelada é o commit **aecb02b8a7b0965508e223335e44ae36a95a0939**.

| Entrada | SHA-256 |
|---|---|
| Original | bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee |
| Reconstrução congelada | 574e273060510e3f37729f66cf7486e01c714eb667c1f08c98581f118cd4cf37 |

O manifesto de entrada está em build/round10/baseline/assemblies/artifacts.json; o receipt está em build/round10/baseline/KairoLibrary/verification.json. Os hashes foram conferidos antes e depois da medição.

O receipt registra 3.241 métodos matched de 3.287, 3.167 corpos iguais, 76 declarações sem corpo, dois corpos iguais retidos por seus grupos gerados e **44 corpos divergentes**. A comparação estrutural existente passou em 337 de 337 tipos. Estes estados não foram modificados nem recalculados por este medidor.

Nenhuma DLL foi copiada, reescrita ou normalizada. Não houve compilação C#, execução do alvo, mudança de fonte de reconstrução, configuração, ledger ou nova exceção. Os relatórios desta publicação são arquivos novos.

## Instrumentos e métrica

O [utilitário reutilizável](../../tools/round10_raw_measurement.py) chama a medição existente de tools/measure_raw_assemblies.py e o adaptador tools/rea_raw_layout.mjs. O REA 6.5.0 fornece as coordenadas PE/CLI, streams e tabelas. Os hashes dos produtores e parsers são retidos no [relatório bruto](../../reports/round10-KairoLibrary-raw-measurement.json) e no [manifesto](../../reports/round10-KairoLibrary-raw-manifest.json).

Cada região é pareada pelo nome; bytes são comparados no mesmo offset relativo até o menor tamanho. As caudas sem par são contadas separadamente, sem preencher com zeros. Linhas de tabela usam o mesmo ordinal, mantendo índices e tokens brutos. **Seções, raiz de metadata, streams e tabelas se sobrepõem; não somar essas camadas.** Não foi produzido hash normalizado.

O arquivo inteiro tem **569.856 bytes de cada lado**: **311.216 pares posicionais diferentes**, 258.640 iguais e nenhuma cauda sem par. Isso não representa 311.216 erros semânticos. Os dumps declaram 269.894 bytes de IL originais e 269.898 reconstruídos.

## Regiões completas

A coluna de diferenças inclui bytes comuns diferentes e caudas sem par.

| Região | Bytes originais | Bytes reconstruídos | Diferenças |
|---|---:|---:|---:|
| PE_HEADERS | 512 | 512 | 9 |
| .text | 567.808 | 567.808 | 311.206 |
| .rsrc | 1.024 | 1.024 | 0 |
| .reloc | 512 | 512 | 1 |
| Raiz de metadata | 258.256 | 258.288 | 34.672 |
| #~ | 106.324 | 106.324 | 13.159 |
| #Strings | 59.692 | 59.692 | 0 |
| #US | 68.712 | 68.712 | 191 |
| #GUID | 16 | 16 | 16 |
| #Blob | 23.404 | 23.436 | 21.305 |

A raiz de metadata tem 34.640 bytes comuns diferentes e 32 bytes sem par na reconstrução. Em #Blob são 21.273 comuns diferentes mais os 32 sem par. Não há bytes exteriores aos headers e seções. Todos os gaps internos também foram medidos: o cabeçalho de metadata tem um byte diferente em 108; o cabeçalho de #~ com 116 bytes e sua cauda de quatro bytes são idênticos.

Os índices de #Strings, #GUID e #Blob têm dois bytes nos dois arquivos. Os offsets e as caudas estão no [CSV de regiões](../../reports/round10-KairoLibrary-raw-regions.csv).

## Tabelas completas

As **23 tabelas presentes têm as mesmas contagens de linhas e os mesmos tamanhos de linha**. Não há linhas extras ou ausentes. Treze tabelas têm diferenças brutas e dez são idênticas.

| Tabela | Linhas originais | Linhas reconstruídas | Linhas diferentes no mesmo ordinal | Bytes diferentes |
|---|---:|---:|---:|---:|
| Module | 1 | 1 | 0 | 0 |
| TypeRef | 187 | 187 | 68 | 225 |
| TypeDef | 337 | 337 | 3 | 3 |
| Field | 2.229 | 2.229 | 2.229 | 2.773 |
| MethodDef | 3.287 | 3.287 | 3.034 | 5.651 |
| Param | 3.869 | 3.869 | 0 | 0 |
| InterfaceImpl | 25 | 25 | 0 | 0 |
| MemberRef | 1.092 | 1.092 | 1.012 | 2.385 |
| Constant | 811 | 811 | 801 | 885 |
| CustomAttribute | 240 | 240 | 240 | 241 |
| ClassLayout | 29 | 29 | 0 | 0 |
| StandAloneSig | 534 | 534 | 533 | 591 |
| PropertyMap | 23 | 23 | 0 | 0 |
| Property | 52 | 52 | 52 | 52 |
| MethodSemantics | 78 | 78 | 0 | 0 |
| MethodImpl | 10 | 10 | 0 | 0 |
| TypeSpec | 136 | 136 | 133 | 174 |
| FieldRVA | 116 | 116 | 116 | 123 |
| Assembly | 1 | 1 | 0 | 0 |
| AssemblyRef | 4 | 4 | 3 | 3 |
| NestedClass | 173 | 173 | 0 | 0 |
| GenericParam | 15 | 15 | 0 | 0 |
| MethodSpec | 49 | 49 | 48 | 53 |

Os [dados por tabela](../../reports/round10-KairoLibrary-raw-tables.csv) e [por coluna](../../reports/round10-KairoLibrary-raw-columns.csv) preservam os valores exatos sem separadores de milhar.

### Os três bytes de AssemblyRef

São exclusivamente índices PublicKeyOrToken de #Blob nas linhas 1, 2 e 4: **11271 → 11302**. O conteúdo apontado é o mesmo token de oito bytes, b77a5c561934e089.

| Linha | Referência original e reconstruída |
|---|---|
| 1 | mscorlib 2.0.0.0 |
| 2 | System.Core 3.5.0.0 |
| 3 | UnityEngine 0.0.0.0 |
| 4 | System 2.0.0.0 |

Nomes, versões, culturas, flags e valores referidos coincidem. **Não são três referências ou versões erradas.** As [referências REA originais](../../reports/round10-KairoLibrary-raw-rea-original-references.json) e [reconstruídas](../../reports/round10-KairoLibrary-raw-rea-rebuilt-references.json) foram preservadas com SHA e ligação ao Evidence pai.

### Outras diferenças de índices já distinguidas

- Constant: 801 índices Value diferentes, mas os 811 payloads referidos são idênticos.
- CustomAttribute: os 240 índices Value diferem; os 240 payloads são idênticos.
- Property: os 52 índices Type diferem; os 52 payloads são idênticos.
- TypeRef: o multiconjunto das 187 referências canônicas é igual; 68 ordinais mudaram.
- As três células Extends de TypeDef resolvem à mesma base em cada lado: java.util.Vector de Object, System.IO.EndOfStreamException e System.IO.Stream.
- Field: os 2.229 índices Signature diferem; 2.214 payloads brutos referidos são iguais e 15 diferem.
- MethodDef: 2.886 RVAs e 2.370 índices Signature diferem; 3.265 payloads de assinatura são iguais e 22 diferem.
- MemberRef: 741 Class, 240 Name e 868 Signature diferentes. StandAloneSig tem 396 payloads diferentes; TypeSpec, 105; MethodSpec, um. A resolução semântica completa desses payloads e MemberRefs permanece **UNKNOWN** nesta medição, pois assinaturas podem conter índices codificados locais ao build.

Esses diagnósticos de conteúdo não removem bytes da contagem bruta. Detalhes estão em [raw-detail.json](../../reports/round10-KairoLibrary-raw-detail.json) e [value-order.json](../../reports/round10-KairoLibrary-raw-value-order.json).

## Heaps e dados

**#Strings:** os 59.692 bytes já coincidem.

**#US:** há 2.134 registros físicos em cada lado, com o mesmo multiconjunto de payloads. Quinze ordinais mudaram: doze rótulos TYPE_* e NONE[ em TouchEvent.ToString e três strings LOBI_CHAT, LOBI_RANKING e ACTION_MAIN na lambda de CreateLobiIntent. Os dumps congelados mostram essas mesmas ordens diferentes de ldstr. É um vínculo medido com dois corpos ainda divergentes, não texto faltante. Os 191 bytes brutos continuam contabilizados. A [ligação aos dumps](../../reports/round10-KairoLibrary-raw-body-links.json) registra SHA, MVID e métodos selecionados.

**#Blob:** 2.410 registros físicos originais e 2.414 reconstruídos, até a cauda toda zero; há 123 e 127 ocorrências de payload sem par nos respectivos multiconjuntos brutos. A cauda toda zero tem dois bytes no original e zero na reconstrução; #US tem dois em cada lado. Esses números descrevem registros físicos de comprimento comprimido, não linhas de metadata. A ambiguidade entre cauda e entrada vazia não é usada para excluir bytes.

**FieldRVA:** os 116 payloads têm tamanho identificado por assinatura primitiva ou ClassLayout. Os **16.491 bytes declarados coincidem**, com 116 payloads iguais, zero diferentes e zero UNKNOWN. Todos os RVAs estão deslocados em **+32**. Os dados de inicialização já coincidem; os 123 bytes brutos diferentes na tabela são endereços. O [relatório de payloads](../../reports/round10-KairoLibrary-raw-field-rva.json) conserva os tamanhos, offsets e hashes. Sua soma não deve ser adicionada à camada de seções.

**.rsrc:** os 1.024 bytes coincidem. O diretório CLI informa zero recursos embutidos; isso não é uma análise de todos os assets do APK.

## Somente quatro exclusões analíticas

A lista é exatamente a do toolchain congelado. A [prova de repetibilidade existente](../../reports/round3-Kairo-repeatability.json) observou 34 bytes diferentes em duas compilações idênticas, nenhum byte inexplicado e nenhuma edição pós-compilação. O medidor confere a prova e o SHA do compilador; não reutiliza seus offsets antigos. O determinismo histórico do original continua **UNKNOWN**.

| Campo | Tamanho por arquivo | Offset original | Offset reconstruído | Bytes diferentes no campo pareado |
|---|---:|---:|---:|---:|
| Module.Mvid | 16 | 527096 | 527096 | 16 |
| COFF.TimeDateStamp | 4 | 136 | 136 | 4 |
| IMAGE_DEBUG_DIRECTORY.TimeDateStamp | 4 | 550520 | 550552 | 4 |
| CodeView.RSDS.PdbGuid | 16 | 550548 | 550580 | 16 |

São 40 bytes de campo por arquivo; os 40 valores diferem nesta comparação. As únicas exclusões são esses quatro campos específicos. Não se excluem caminhos, idade PDB, registros de depuração inteiros, padding, RVAs, índices ou outros GUIDs. A idade PDB é 1 em ambos.

| Região pareada por nome | Diferenças brutas | Diferenças fora da exclusão |
|---|---:|---:|
| PE_HEADERS | 9 | 5 |
| #GUID | 16 | 0 |
| IMAGE_DEBUG_DIRECTORY | 6 | 2 |
| CodeView.RSDS | 113 | 97 |

O comparador absoluto também registra 311.160 pares diferentes fora de 56 pares que tocam um desses campos em algum lado. São 56 pares posicionais porque os campos de debug e CodeView estão deslocados e podem cruzar outra estrutura no arquivo oposto; não são 56 bytes de exceção por arquivo. Esse número não é comparação semântica alinhada, hash normalizado ou previsão de erros futuros. Os 311.216 brutos permanecem registrados.

## O que persistiria com os corpos iguais

**PROVADO, mantendo compilador, opções e caminho de saída atuais:** o caminho do PDB em CodeView continuaria diferente, fora das quatro exceções. O registro tem 284 bytes em cada lado; retirando somente o GUID, restam **97 bytes diferentes**, incluindo caracteres e terminadores do campo de caminho.

    Original: E:\workspace\Library\KairoLibrary\obj\Android\KairoLibrary.pdb
    Atual:    C:\Users\l_abn\Documents\Projetos\Pocket League Story\build\KairoLibrary-roslyn-1.1.0\KairoLibrary.pdb

Isso depende da localização de saída, não do IL dos 44 corpos. Não foi feita correção desse caminho nem criada exceção para ele.

**PROVADO apenas no estado atual:** #Blob é 32 bytes maior; a metadata termina 32 bytes depois; debug, CodeView e os 116 FieldRVAs estão deslocados em 32. A raiz de metadata começa no mesmo offset 292260. Constantes, atributos, propriedades e referências já têm valores iguais com índices diferentes. A ordem dos quinze registros de #US corresponde à ordem diferente de literais nos dois corpos identificados.

**UNKNOWN após os 44 corpos coincidirem:** quantos RVAs, índices TypeRef, MemberRef, Blob e StandAloneSig, import pointers, entrypoint, relocations ou bytes de padding ainda difeririam; se os 32 bytes extras de #Blob desapareceriam; e se somente as correções de corpos reproduziriam toda a ordenação bruta. Não houve compilação contrafactual nem normalização para inventar essa resposta. Corpos iguais com tokens resolvidos não provam, sozinhos, uma serialização bruta idêntica.

## Reprodução e limites do utilitário

A partir da raiz do repositório, uma nova execução pode gravar em outro diretório, sem sobrescrever esta publicação:

    python -X utf8 tools/round10_raw_measurement.py --baseline build/round10/baseline --assembly KairoLibrary --label replay --output-dir build/round10/raw-replay --rea-evidence-dir reports --rea-evidence-prefix round10-KairoLibrary-raw- --literal-seed TYPE_UP --literal-seed net.kairosoft.android.LOBI_CHAT

Também aceita um par explícito de caminhos por --original e --rebuilt, sem baseline; nesse caso não presume exceções. --no-exclusions conserva todos os bytes. --label e --output-dir permitem reutilização; arquivos existentes são recusados. As evidências portáteis são opcionais e sua identidade SHA, MVID, cobertura e Evidence pai são conferidos antes da publicação.

A camada bruta cobre o layout admitido pelo REA 6.5.0. A decomposição valida tamanhos de linha contra os esquemas ECMA; um esquema não suportado falha explicitamente. A resolução de TypeSpec nesta medição é limitada à forma GenericInst Class com um argumento Object observada no Extends alterado. Outros payloads não são automaticamente declarados semanticamente iguais. Tamanhos de dados de FieldRVA não reconhecidos permanecem UNKNOWN.

Validação do novo utilitário: todas as regiões, tabelas, colunas e exclusões reproduziram os números do WIP congelado; um par de controle com o original contra ele mesmo, sem exclusões, produziu zero diferenças. Essas são medições estáticas, sem compilação ou execução do jogo. O manifesto registra o SHA do produtor, das entradas e dos 13 arquivos de relatório desta publicação.

Esta etapa permanece somente medição. Não modifica DLLs, não normaliza assemblies, não altera métodos ou estados e não toma decisões sobre a Parte A funcional.
