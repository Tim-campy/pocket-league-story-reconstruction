# Transporte integral da evidência bruta

O primeiro relatório bruto do Assembly-CSharp gerou dois CSVs de 161.294.119 e 161.293.727 bytes. O JSON principal tinha 71.175.649 bytes. O limite de 80 MB por arquivo do fluxo de commit recusou a publicação; a medição e seus arquivos originais foram preservados.

Os três arquivos são publicados em gzip com manifesto. A compressão usa nível 9, mtime zero e nome de arquivo vazio no cabeçalho; as versões do Python e zlib ficam registradas. Cada arquivo comprimido tem SHA-256, tamanho comprimido e SHA-256/tamanho do conteúdo original. A verificação lê toda a descompressão e exige identidade de cada byte, incluindo CRLF e Unicode. Nenhuma DLL, estado de método, perfil de exceção ou algoritmo de comparação é alterado.

Para restaurar uma medição depois de clonar o repositório:

```powershell
python tools/round12_raw_transport.py restore reports/round12-b33-postverify02-app-threshold-Assembly-CSharp-archive-manifest.json
```

Use o manifesto com o mesmo prefixo da medição desejada. O restaurador só admite os nomes gerados desses três relatórios dentro de `reports/`; recusa sobrescrever um arquivo existente com conteúdo diferente. Se os originais já existirem, verifica seus hashes e os arquivos gzip sem sobrescrevê-los.

O comparador continua produzindo seus JSON/CSV originais e seus hashes sem alteração. Depois disso, o hook de publicação cria os arquivos gzip e o manifesto. Os originais permanecem locais, ignorados pelo Git; os gzip são armazenados sem conversão de fim de linha. A prova do comparador ainda referencia o hash do conteúdo original, que pode ser restaurado integralmente.

A medição preservada `b33-postverify02` totaliza 393.763.495 bytes originais e 15.862.169 bytes comprimidos. A restauração integral foi verificada. Nove testes próprios cobrem ida e volta com CRLF/Unicode, cabeçalho determinístico, corrupção, limite de tamanho, identidade do comparador, recusa de sobrescrita e restrição dos caminhos. A medição desta tentativa não é recontada como um ciclo novo.

Provas: `reports/round12-raw-transport-review.json` e o manifesto de cada medição. A comparação literal mantém o SHA-256 do produtor aprovado: `1a61c60e846cb2e6ab29511fe9d70e6d38f032001e2a4587c13e639bddeeabe2`.
