# AGENTS.md — Decompilação com correspondência byte a byte de um jogo Unity (Mono) para Android

## 1. Missão

Você vai fazer a engenharia reversa de um jogo Android (`.apk`) feito em Unity com o backend Mono e reconstruir o código-fonte C# dele, de forma que a recompilação produza **IL idêntico byte a byte ao original**.

O critério de sucesso é objetivo: um método só está pronto quando o corpo que o compilador gera a partir do seu fonte é exatamente o corpo do método original. "Comportamento equivalente" não conta como concluído, e comparar comportamento não substitui comparar bytes.

O trabalho é organizado **por método**, agrupado por tipo (classe, struct, enum) e por assembly. Cada método tem estado próprio, e o progresso do projeto é a soma deles. Isso existe para que qualquer sessão possa parar e ser retomada sem perda de contexto.

## 2. Escopo

O inventário inicial encontrou Unity com Mono. Em `assets/bin/Data/Managed/` estão as DLLs .NET; cada componente do APK cai em uma destas categorias:

| Categoria | Exemplos | Tratamento |
|---|---|---|
| **Alvo** | `Assembly-CSharp.dll`, `KairoLibrary.dll`, e `Assembly-CSharp-firstpass.dll` ou outras DLLs próprias do jogo, se existirem | Decompilar e corresponder método a método |
| **Externo** | `UnityEngine*.dll`, `mscorlib.dll`, `System*.dll`, `libunity.so`, `libmono.so`, classes Java do Unity e SDKs de terceiros no `classes.dex` | Não decompilar. Identificar a versão e, quando possível, conferir o hash contra a distribuição oficial |
| **Dados** | cenas, texturas, áudio, `resources.arsc`, arquivos em `assets/` | Não são código. Conferidos por hash e contados à parte; nunca entram na porcentagem de código correspondente |

Se aparecer código próprio do jogo fora das DLLs (plugin nativo `.so` ou classes Java específicas), registre e pergunte ao usuário antes de incluí-lo no escopo.

## 3. Ferramentas

### REA (Reverse Engineer Anything)

Você tem acesso ao REA, um conjunto de ferramentas de engenharia reversa para agentes, disponível como CLI (`rea`) e como servidor MCP.

- No início de cada sessão, confirme que o REA responde (`rea doctor`). Se as ferramentas MCP não estiverem carregadas, avise o usuário em vez de seguir sem elas.
- Liste as ferramentas que o REA expõe nesta instalação antes de usá-las. Não presuma nomes de ferramentas ou parâmetros.
- Verifique se o REA desta instalação analisa assemblies .NET (IL e metadados). Se analisar, use-o como fonte principal de evidência. Se não, use-o para o contêiner APK e as bibliotecas nativas, e as ferramentas .NET abaixo para as DLLs. Registre em `docs/toolchain.md` qual foi o caso.

### Ferramentas .NET

- `ilspycmd` (ILSpy em linha de comando) ou dnSpy: decompilação de IL para C#.
- `monodis` ou `ildasm`: listagem de IL e de tabelas de metadados.
- Mono.Cecil, dnlib ou `dnfile`: leitura programática dos assemblies, usada pelos scripts de verificação.
- O compilador C# que acompanha a versão exata do Unity usada pelo jogo (ver Fase 1).

### Outras

- `unzip`, `apktool`, `aapt2`, `zipalign` para o contêiner APK.
- AssetRipper ou equivalente apenas para consultar nomes de scripts e a versão do Unity; não faz parte do build.
- `cmp` e `sha256sum` para verificação de arquivos.

Registre a versão de tudo o que usar em `docs/toolchain.md`.

## 4. O que significa "100%"

A correspondência é medida em camadas, da menor para a maior:

| Camada | Alvo | Como medir |
|---|---|---|
| Método | Corpo do método: cabeçalho (tiny ou fat), `maxstack`, assinatura das variáveis locais, bytes de IL e cláusulas de exceção | Comparação byte a byte do corpo, com os tokens de metadados resolvidos para os nomes que representam |
| Tipo | Campos, métodos, propriedades, eventos, atributos, tipos aninhados, e a **ordem** em que aparecem | Comparação das linhas de metadados do tipo |
| Assembly | Tabelas de metadados, heaps (`#Strings`, `#US`, `#Blob`), recursos embutidos, referências | Comparação estrutural completa e, por fim, `sha256sum` do arquivo |
| APK | Todas as entradas do ZIP, na mesma ordem, com a mesma compressão e alinhamento | `cmp` do APK sem a assinatura |

Os tokens precisam ser resolvidos na comparação por método porque o número de um token depende da ordem das tabelas do assembly inteiro: dois corpos idênticos podem diferir nesses bytes enquanto o resto do assembly ainda não bate.

### Limites conhecidos

1. **Campos não determinísticos do assembly.** Compiladores C# antigos geram um MVID (GUID do módulo) aleatório e gravam a data de compilação no cabeçalho PE. Esses campos não podem ser reproduzidos por código-fonte. Um campo só entra na lista de exceções em `docs/toolchain.md` se você provar que ele muda entre duas compilações seguidas do mesmo fonte. Nada mais pode ser normalizado.
2. **Assinatura do APK.** `META-INF/` e o APK Signing Block dependem da chave privada do desenvolvedor original. Nunca tente contornar, forjar ou copiar a assinatura para dentro do build.

O alvo final é, portanto: todo método e todo metadado idênticos, com diferença restrita aos campos comprovadamente não determinísticos e à assinatura.

## 5. Regras invioláveis

1. **O original é somente leitura.** Nada em `original/` é modificado, nunca.
2. **Sem atalhos de bytes.** É proibido copiar IL ou trechos do assembly original para o resultado, ou editar a DLL compilada depois do build para forçar a correspondência. Só fonte C# compilado conta.
3. **Fato ou hipótese.** Toda afirmação cita a evidência (assembly, tipo, método, token, string). O que não foi confirmado é marcado como `UNKNOWN`.
4. **O estado é gerado, não declarado.** Um método só passa a `matched` quando o script de verificação diz que ele bate. Nunca edite o estado à mão.
5. **Um método por vez** durante os ajustes. Termine, verifique e registre antes de abrir o próximo.
6. **Não regrida.** C# compila o assembly inteiro de uma vez, então qualquer alteração pode afetar outros métodos. Rode a verificação completa antes de cada commit; se algo que estava `matched` deixou de bater, conserte ou desfaça.
7. **Não renomeie nem reorganize** o que veio do original. Nomes de tipos, membros e namespaces já estão nos metadados e fazem parte da correspondência; a ordem dos membros no fonte também.
8. **Pare e pergunte** quando faltar uma ferramenta, quando a versão do Unity ou do compilador não puder ser determinada, ou quando uma ação exigir instalar algo no sistema (por exemplo, o editor Unity).
9. **Arquivos do original não vão para o repositório.** `original/` e `build/` ficam no `.gitignore`.

## 6. Estrutura do repositório

```
AGENTS.md
PROGRESS.md                # gerado por tools/progress — não editar à mão
original/                  # APK e conteúdo extraído (somente leitura, fora do git)
  app.apk
  extracted/
config/
  methods.csv              # lista mestra de métodos (fonte da verdade)
  types.csv                # tipos, com a ordem original no assembly
  compile-order/<assembly>.txt   # ordem dos arquivos-fonte passada ao compilador
  inventory.csv            # todos os arquivos do APK: caminho, sha256, categoria
src/
  Assembly-CSharp/<Namespace>/<Tipo>.cs     # um tipo de nível superior por arquivo
  KairoLibrary/<Namespace>/<Tipo>.cs
ref/                       # referência extraída do original, gerada por script
  <assembly>/<Tipo>/<metodo>.il
build/                     # saída da recompilação (fora do git)
tools/
  extract                  # extrai o APK, gera inventory.csv, ref/ e os CSVs iniciais
  build                    # recompila cada assembly alvo
  verify                   # compara método a método, tipo a tipo e arquivo a arquivo
  progress                 # regenera PROGRESS.md a partir de methods.csv
docs/
  toolchain.md             # versão do Unity, compilador, flags, e a evidência de cada um
  methods/<id>.md          # notas por método, só quando houver algo a registrar
  notes/                   # descobertas gerais: formatos de save, sistemas do jogo
```

## 7. Registro de métodos

`config/methods.csv` é a fonte da verdade. Uma linha por método:

```
id,assembly,tipo,metodo,token,tamanho_il,estado,percentual,fonte,gerado_por,notas
```

- `id`: assinatura completa, por exemplo `Assembly-CSharp::Game.Shop::Buy(System.Int32)`.
- `token`: token de metadados do método no original.
- `tamanho_il`: tamanho do corpo em bytes; métodos abstratos ou externos têm 0.
- `percentual`: semelhança do corpo reportada por `tools/verify`.
- `fonte`: caminho do arquivo `.cs`.
- `gerado_por`: para membros criados pelo compilador (closures, iteradores de corrotinas, caches de delegates), o `id` do método de origem. Eles não têm fonte próprio: batem quando o método de origem é escrito da forma certa.

### Estados

| Estado | Significado |
|---|---|
| `todo` | Listado, ainda sem fonte que compile |
| `decompiled` | Existe fonte que compila, mas o corpo difere |
| `equivalent` | Mesma lógica conferida; diferença só em ordem de locais, forma de desvios ou instruções equivalentes |
| `matched` | Corpo idêntico, confirmado por `tools/verify` |
| `external` | Pertence a componente externo; não se decompila |

`PROGRESS.md` mostra: total de métodos e de bytes de IL, percentual `matched` por assembly, por namespace e por tipo, tipos com metadados já idênticos, e a lista dos próximos métodos sugeridos.

## 8. Fluxo de trabalho

### Fase 0 — Inventário

1. Registre o `sha256` do APK e de cada arquivo interno em `config/inventory.csv`, com a categoria (alvo, externo, dados).
2. Guarde a ordem das entradas do ZIP, o método de compressão e o alinhamento de cada uma.
3. Liste todos os assemblies em `assets/bin/Data/Managed/` e classifique cada um.
4. Verifique se os assemblies alvo foram ofuscados, criptografados ou reduzidos (stripping). Se sim, relate antes de continuar.

### Fase 1 — Toolchain

Sem o compilador exato não há correspondência. Determine e documente em `docs/toolchain.md`, com evidência:

- **Versão exata do Unity**, incluindo o sufixo (por exemplo `5.6.3p1`). Ela aparece no cabeçalho dos arquivos de dados em `assets/bin/Data/` e em strings de `libunity.so`.
- **Compilador C# de cada assembly alvo.** Versões antigas do Unity usam o `mcs` do Mono que acompanha o editor; versões recentes usam Roslyn. O padrão dos nomes gerados pelo compilador denuncia qual foi usado: `mcs` produz nomes como `<>c__AnonStorey` e `<>f__am$cache`, enquanto o compilador da Microsoft produz `<>c__DisplayClass` e `CS$<>9__CachedAnonymousMethodDelegate`.
- **`KairoLibrary.dll` é tratada à parte.** Bibliotecas pré-compiladas costumam vir de outro compilador e de outras opções. Não presuma que valem as do `Assembly-CSharp.dll`.
- **Opções de compilação:** otimização ligada ou não, informação de depuração, símbolos definidos (`UNITY_ANDROID` e os de versão), perfil de API (.NET 2.0 ou 2.0 Subset), versão da linguagem, referências e a ordem delas.

A forma mais segura de obter o compilador é a instalação do editor Unity na versão exata. Se ela não estiver disponível, pare e peça ao usuário.

Valide a toolchain com três ou quatro métodos simples de tipos diferentes. Se nenhuma combinação bater, não avance: relate o que foi tentado.

### Fase 2 — Referência e decompilação em massa

C# não compila um método isolado, então o projeto precisa compilar inteiro antes de qualquer verificação.

1. Rode `tools/extract` para gerar `ref/`, `config/methods.csv` e `config/types.csv` a partir do original.
2. Decompile cada assembly alvo inteiro para `src/`, com o decompilador configurado para a versão da linguagem C# que o compilador identificado aceita. Decompiladores modernos geram sintaxe nova (interpolação de strings, `?.`, membros com `=>`) que compiladores antigos rejeitam ou compilam de outra forma.
3. Corrija os erros até cada assembly compilar por completo.
4. Rode `tools/verify --all`. Boa parte dos métodos deve bater já nesta etapa; o restante é o trabalho da Fase 3.
5. Regenere `PROGRESS.md` e faça o commit da linha de base.

### Fase 3 — Ciclo por método

Para cada método que não bate:

1. **Escolher.** Siga a lista sugerida em `PROGRESS.md`. Prefira fechar um tipo inteiro a espalhar esforço; comece pelos tipos com menos métodos pendentes.
2. **Comparar.** Rode `tools/verify --method <id>` e leia o diff de IL entre `ref/` e o build.
3. **Ajustar o fonte** até bater. Causas comuns de diferença:
   - ordem de declaração das variáveis locais (define a assinatura de locais);
   - `foreach` contra `for`, e `foreach` sobre array contra sobre coleção;
   - `switch` contra cadeia de `if`, e `switch` sobre string;
   - concatenação com `+` contra `string.Concat` ou `string.Format`;
   - operador ternário contra `if`/`else`;
   - `using`, `lock` e `try`/`finally` escritos por extenso contra a forma direta;
   - lambda contra método anônimo contra método nomeado, e o que cada um captura;
   - inicializadores de campo contra atribuição no construtor;
   - inicializadores de objeto e de coleção contra atribuições separadas;
   - conversões explícitas, `checked`/`unchecked`, e tipos com ou sem sinal;
   - corrotinas: a ordem e a forma dos `yield` definem a classe iteradora gerada.
4. **Registrar.** Deixe `tools/verify` atualizar `methods.csv`, regenere `PROGRESS.md` e faça um commit: `match: <id>` ou `wip: <id> <percentual>`.

Se um método não bater depois de um esforço razoável, deixe-o em `decompiled` ou `equivalent`, anote em `docs/methods/<id>.md` o que difere e o que já foi tentado, e siga adiante.

### Fase 4 — Tipos e assembly

Com os métodos de um tipo em `matched`:

1. Confira os metadados do tipo: ordem de campos, métodos, propriedades e tipos aninhados, atributos, valores constantes, layout de structs. A ordem dos membros no fonte define a ordem nos metadados.
2. Ajuste `config/compile-order/` até a ordem dos tipos no assembly coincidir com a original; ela reflete a ordem em que os arquivos foram entregues ao compilador.
3. Compare o assembly inteiro: tabelas, heaps, recursos e referências. As únicas diferenças aceitas são as listadas como não determinísticas em `docs/toolchain.md`.

### Fase 5 — Remontagem do APK

1. Monte o APK com os assemblies reconstruídos, os componentes externos e os dados, na mesma ordem, com a mesma compressão e o mesmo alinhamento registrados na Fase 0.
2. Compare com o original excluindo a assinatura.
3. No relatório final, separe com clareza: o que foi reconstruído a partir de fonte, o que é externo, o que é dado, e quais bytes diferem e por quê.

## 9. Verificação

`tools/verify` é o único juiz. Ele deve:

- compilar cada assembly alvo com a toolchain documentada;
- comparar o corpo de cada método com o original, resolvendo tokens;
- tratar membros gerados pelo compilador junto com o método de origem;
- atualizar `methods.csv` com estado e percentual;
- com `--all`, comparar também tipos e assemblies, e falhar se algum método `matched` regrediu.

## 10. Protocolo de sessão

**Ao começar:**
1. Leia `PROGRESS.md` e os últimos commits.
2. Confirme que o REA está respondendo.
3. Rode `tools/verify --all` para confirmar que o estado registrado é verdadeiro.
4. Escolha os próximos métodos pela lista sugerida.

**Ao terminar:**
1. Regenere `PROGRESS.md`.
2. Faça commit do que está pronto; trabalho incompleto fica como `wip` com o percentual atual.
3. Relate em poucas linhas: métodos concluídos, percentual antes e depois, bloqueios, e o que recomenda atacar em seguida.

Leia apenas o que o método atual exige. Não decompile o assembly inteiro de novo nem liste todos os métodos a cada sessão: `methods.csv` e `ref/` já guardam isso.

## 11. Convenções

- Um tipo de nível superior por arquivo, em pasta que espelha o namespace. Tipos aninhados ficam no arquivo do tipo que os contém.
- Não escreva à mão classes geradas pelo compilador (closures, iteradores). Escreva o método de origem de forma que o compilador as gere.
- Comentários explicam o que o código faz. Notas de investigação vão em `docs/methods/`.
- Não reformate nem "melhore" código que já está `matched`.
- Conhecimento sobre o jogo (formato de save, sistemas, tabelas de dados) vai em `docs/notes/`, com a evidência de cada afirmação.
