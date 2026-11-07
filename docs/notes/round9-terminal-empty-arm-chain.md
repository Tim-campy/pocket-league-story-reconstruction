# Cadeia terminal de oito braços vazios — D19

Base55f1c2fe7f4c198c8cca75a5edf4b3fd3d32db50, medição fresca `reports/round9-cycle18-after-measurement.json`. O maior par smcs continua bne.un=>beq(17 métodos/32.242 bytes). Seu maior caso FriendForm._draw6.994 é a aplicação desta composição; SubForm.Init5.191 e Ball.Update4.629 foram lidos como diagnósticos diferentes, não analogias forçadas.

Friend tem93 tipos de locais originais e atuais exatos; fonte atual6.955 bytes de IL. Os oito hunks remanescentes correspondem ao guard OR de type4..11 que retorna, versus oito braços else-if vazios que saltam ao retorno comum6993. O corpo de type12 e a cadeia seguinte devem permanecer ligados ao último else. Trocar só uma ocorrência altera destinos anteriores; compõe-se a lista inteira, preservando o tail lexical sem saltar para fora do método ou de um loop.

É diferente do D01, que já avançou63→700 com transformação terminal de um braço, sem fechar este método. Os novos oito braços não repetem o mesmo candidato. O produtor exige lista OR plana do mesmo seletor simples, constantes únicas, guard somente return sem valor, pai raiz ou else final imediatamente sob raiz e cadeia seguinte esgotando o pai. Nenhum tipo, chamada, literal, local ou classe gerada é inventado.

O seletor real é Int32 público não volatile, conforme metadados; não se repete uma expressão com efeito. O smcs emite bne entre os testes e br para a saída comum nos braços vazios; a leitura original e as fixtures sustentam essa forma. Fonte de referência Mono b6587926c0f146c8b12985744f33a13b39a22d57, statement.cs If.DoEmit247–264; não se normaliza IL para comparar.

Fixtures próprias raiz/else terminal: dois corpos completos idênticos ao controle manual, seis recusas de scope/selector/guard; quatro builds, zero jogo nessa etapa. O modelo fresco aprovou previsão **COMPLETA** de93 tipos exatamente originais. Recibos reports/round9-terminal-empty-arms-fixtures.json e reports/round9-terminal-empty-arms-patches.json vinculam fonte, produtor, modelo e fixture. Corpo e estado só são decididos pela compilação e verificação completas.
