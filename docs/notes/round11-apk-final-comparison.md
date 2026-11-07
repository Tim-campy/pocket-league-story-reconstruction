# Critério final de comparação do APK

Definição solicitada na rodada 11. Esta etapa não monta, assina, instala ou
executa um APK. O original permanece somente leitura.

## Entradas e assinatura

O conjunto e a ordem das 1.218 entradas do ZIP devem corresponder ao original.
Para toda entrada, exceto as duas DLLs abaixo e os portadores da assinatura,
os bytes do payload comprimido devem ser idênticos byte a byte ao original.
Não basta descomprimir e obter o mesmo conteúdo. O inventário com SHA-256 de
cada payload está em `reports/round11-apk-contract.json`.

As duas exceções de código são exclusivamente:

- `assets/bin/Data/Managed/KairoLibrary.dll`;
- `assets/bin/Data/Managed/Assembly-CSharp.dll`.

No APK fornecido, os portadores de assinatura ZIP são
`META-INF/MANIFEST.MF`, `META-INF/CERT.SF` e `META-INF/CERT.RSA`.
Um eventual APK Signing Block também pertence à assinatura. Não se exclui
arbitrariamente todo `META-INF/`. A chave e a assinatura originais não serão
copiadas, forjadas ou usadas para assinar o resultado. Um artefato de teste
assinado com chave própria permanece identificado separadamente.

## Comparação das DLLs

As duas DLLs serão descomprimidas e comparadas como arquivos completos, sem
resolver tokens nem tolerar índices, RVAs, heaps ou diferenças de emissão.
As únicas diferenças de bytes permitidas são os campos não determinísticos
já comprovados por recompilações independentes:

| DLL | Campos permitidos |
|---|---|
| KairoLibrary | Module.Mvid; COFF.TimeDateStamp; IMAGE_DEBUG_DIRECTORY.TimeDateStamp; CodeView.RSDS.PdbGuid |
| Assembly-CSharp | Module.Mvid; COFF.TimeDateStamp |

As coordenadas desses campos precisam ser derivadas de cada PE/CLI, com nome,
largura, localização e evidência conferidos; não são intervalos arbitrários nem
uma máscara de todos os cabeçalhos. Todos os demais bytes permanecem sob
comparação, inclusive o caminho CodeView/PDB. A comparação não reescreve as DLLs
e não produz arquivo ou hash normalizado. O verificador por método continua
sendo uma etapa anterior necessária, mas não substitui este critério final.

## Compressão fixada para as DLLs

`tools/round11_apk_contract.py` mediu os níveis 0 a 9 de DEFLATE bruto sobre
as próprias DLLs originais, sem escrever uma DLL nem montar um ZIP. O nível 6
foi o único a reproduzir o tamanho de cada payload e também reproduziu
**todos os bytes comprimidos originais**, um resultado mais forte que o
critério de tamanho solicitado.

| DLL | Bytes descomprimidos | Bytes comprimidos originais | Nível que coincide | Stream comprimido idêntico |
|---|---:|---:|---:|---|
| KairoLibrary | 569856 | 248092 | 6 | Sim |
| Assembly-CSharp | 895488 | 321171 | 6 | Sim |

Parâmetros fixados: Python/zlib com runtime **1.3.1**, método DEFLATED,
`level=6`, `wbits=-15`, `memLevel=8`, `strategy=Z_DEFAULT_STRATEGY`, entrada
integral em uma chamada `compress`, seguida de `flush(Z_FINISH)`.
O relatório registra versões, hashes, todos os níveis e todos os tamanhos.
A identidade histórica da biblioteca de compressão original continua UNKNOWN;
igualdade do stream nesses dois arquivos não identifica o encoder histórico.

Na remontagem futura, essas opções serão usadas nas DLLs recompiladas.
Seus tamanhos comprimidos não precisam coincidir com os do original: os campos
permitidos variam entre builds. A aceitação das DLLs é sobre os bytes
descomprimidos, com somente as exceções comprovadas acima.

## Estrutura ZIP

Preservar os métodos de compressão, a ordem, os nomes e as propriedades das
entradas não alteradas. A remontagem deve manter os alinhamentos registrados
do original, incluindo os restos dos offsets de dados módulo 4096.
Offsets do diretório central e tamanhos/CRCs das entradas substituídas precisam
ser recalculados coerentemente. A identidade dos payloads comprimidos não é
uma afirmação de igualdade posicional de todos os cabeçalhos do ZIP, que podem
ser deslocados pela substituição das DLLs e da assinatura.

Arquivos DEX/Java e outros componentes externos entram apenas como payloads
opacos preservados. Esta definição não autoriza sua decompilação nem
normalização de assemblies.
