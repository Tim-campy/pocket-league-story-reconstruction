# Rodada 10: teste funcional em Android

O APK de teste abriu, exibiu o menu, carregou a equipe e executou uma partida
completa no Samsung Galaxy S22 Ultra SM-S908E, Android 16/API 36, usando a variante
armeabi-v7a. Sun Rays venceu Blue Stars por 1 a 0. Foram observados primeiro
tempo, gol, intervalo, segundo tempo, apito final, resultado e bônus dos jogadores.
As nove observações e os hashes das evidências estão em
[`reports/round10-functional-test.json`](../../reports/round10-functional-test.json).

**Este APK é exclusivamente de teste. Não é a reconstrução final e não entra na
comparação byte a byte.** O teste funcional também não promove métodos a `matched`.
As DLLs usadas correspondem ao fonte do commit
`aecb02b8a7b0965508e223335e44ae36a95a0939`, com os corpos ainda pendentes daquela
linha de base. Nenhum assembly recebeu alteração de bytes após a compilação.

| Artefato | SHA-256 |
|---|---|
| APK original do usuário | `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8` |
| APK de teste | `8a832f276fa7b29c4e0d35121b563098543206c3119aff4a3f21325896b5f333` |
| KairoLibrary reconstruída usada no teste | `574e273060510e3f37729f66cf7486e01c714eb667c1f08c98581f118cd4cf37` |
| Assembly-CSharp reconstruída usada no teste | `212953c694223ef321f1dcc5d131c2d62d7f0588feb770c17c95cac2b985f6c7` |
| Certificado de teste, DER | `047ac38b79a872db60fde16253ef33762806fa2d495b57aeb92386d5d2745998` |

`tools/build_test_apk.py` preservou a ordem das 1.218 entradas, seus métodos de
compressão e o resto de cada offset de dados na divisão por 4.096, incluindo
os alinhamentos não uniformes do original. Os payloads comprimidos de todas as
1.213 entradas inalteradas permaneceram idênticos. Mudaram somente as duas DLLs
e os três arquivos de assinatura JAR em META-INF. O nível de compressão original
é UNKNOWN; os cinco payloads substituídos usam DEFLATE nível 9. Cabeçalhos, tamanhos
e offsets necessários foram recalculados; não se afirma identidade do ZIP inteiro.

Uma chave RSA de 3.072 bits foi gerada localmente apenas para este teste. A chave
original não foi usada. A assinatura v1 JAR/SHA-256 foi verificada pelo `jarsigner`
do JDK Microsoft 21.0.12.101. Os avisos de confiança decorrem do certificado
autoassinado. A chave privada, sua senha, APKs, capturas do telefone e logs brutos
ficam em `build/round10/runtime/`, fora do Git. Nenhuma ferramenta foi instalada.
`zipalign` e `apksigner` não foram executados: a construção ZIP própria confere os
offsets de todas as entradas e a instalação no dispositivo foi efetivada.

A primeira instalação terminou com `INSTALL_FAILED_VERIFICATION_FAILURE: Install
not allowed`. Após a solicitação do usuário para tentar novamente, a segunda
terminou com `Success`. Foi usada a opção ADB
`--bypass-low-target-sdk-block`, necessária neste teste do aplicativo antigo.
Nenhuma proteção global foi desativada. O pacote não estava instalado na consulta
anterior à instalação; nenhum aplicativo existente foi desinstalado.

O menu principal foi registrado em 2026-11-07 às 15:01:57 UTC; a tela de resultado,
às 15:13:49 UTC. O logcat foi consultado somente para os processos do jogo, sem
limpar os buffers globais. As 37 capturas de logs contêm 677 linhas distintas
com data/hora; a busca por exceções, falhas fatais e erros Unity não encontrou
ocorrências. Isso limita a conclusão ao cenário e aos buffers capturados.
As transições anteriores para segundo plano não apresentaram prova de crash;
sua causa permanece UNKNOWN. Os caminhos não exercitados também permanecem UNKNOWN.

Para reproduzir a montagem com sua própria cópia do APK, consulte
`python tools/build_test_apk.py --help`. O script exige recibos de verificação
e uma pasta de saída nova, gera outra chave de teste e recusa sobrescrever a
evidência anterior. Por isso, uma nova execução não reproduzirá o hash deste APK.
