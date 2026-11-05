# Recuperar inicializadores estáticos sem mudar BeforeFieldInit

Os tipos `kairo.common.cfg.Config` e `kairo.unity.native.AdMobPlugin` têm `TypeAttributes=1048577` (`0x100001`), incluindo **BeforeFieldInit**, no original. Declarar um construtor estático explícito removeria esse atributo. Por isso, os corpos de `.cctor()` recuperados pelo Codemerx não podem ser simplesmente copiados como declarações de construtores para o fonte principal.

## Config

O construtor Codemerx contém 46 atribuições simples a campos estáticos existentes. Seus lados direitos podem ser usados nos próprios inicializadores dos campos, mantendo campos, nomes, ordem e atributos.

O detalhe que muda o IL está na forma das condições. O fonte ILSpy usava, por exemplo, `DEBUG ? true : false`, enquanto o Codemerx preservou `!Config.DEBUG ? false : true`. São valores equivalentes, mas o Roslyn 1.3.2 emite os desvios em ordens diferentes. A segunda forma reproduz o original. Os ternários com constantes iguais também foram preservados em vez de simplificados.

## AdMobPlugin

O `.cctor()` original contém somente `ret`, e o fonte recuperado não continha inicializadores estáticos, portanto a recompilação nem sequer emitia esse método.

Uma sonda própria confirmou uma regra do Roslyn 1.3.2: uma classe cujos inicializadores estáticos são todos valores padrão, como `private static object value = null;`, ainda recebe um `.cctor()` implícito, mas seu corpo é apenas `ret`. O atributo BeforeFieldInit permanece. Se houver um inicializador não padrão, os stores dos valores padrão também são mantidos, como ocorre em Config.

Assim, restaurar `private static IApplication app_ = null;` em AdMobPlugin gera o corpo vazio correto, sem declarar construtor estático explícito nem alterar campos. O teste cobriu oito classes próprias, com seis corpos vazios e dois casos de stores. Fonte das sondas, opções, hashes e resultados: [KairoLibrary-static-initializer-probe.json](../../reports/KairoLibrary-static-initializer-probe.json).

## Verificação dos candidatos

`python tools/recover_kairo_initializers.py --verify` prepara os dois candidatos somente em `analysis/variants/kairo-static-initializers/`. Faz dois builds integrais com Roslyn 1.3.2 a partir de um snapshot congelado dos fontes para evitar interferência de edições concorrentes.

Resultado observado:

- Ambos os corpos `.cctor()` correspondem integralmente com tokens resolvidos.
- Nenhum corpo anteriormente correspondente regrediu.
- Os atributos dos dois tipos permanecem `1048577`, iguais ao original.
- Os campos completos e a ordem dos métodos dos dois tipos correspondem ao original.
- Nenhum byte de DLL foi copiado para o build ou alterado depois da compilação.

O helper não altera fontes principais nem promove estados. A integração deve respeitar os hashes dos arquivos de base guardados no relatório; se outra seleção já mudou os arquivos, os candidatos precisam ser regenerados antes da aplicação. Relatório: [KairoLibrary-static-initializers.json](../../reports/KairoLibrary-static-initializers.json).

## SoundPlayer e CRC64

O seletor encontrou posteriormente mais dois construtores estáticos implícitos sem um membro C# correspondente: `SoundPlayer::.cctor()` e `CRC64::.cctor()`. Ambos os originais possuem somente `ret` e `TypeAttributes=1048577`; os builds principais mantinham esse atributo, mas omitiam os métodos.

A mesma regra comprovada pelas sondas resolveu os dois casos, acrescentando apenas inicializadores de valor padrão aos campos existentes: `SoundPlayer.instance_ = null` e `CRC64.crcTable_ = null`. Não foi preciso alterar a tabela de CRC, os métodos de cálculo ou declarar construtores explícitos.

O novo helper `python tools/recover_kairo_empty_initializers.py --verify` usa diretórios de snapshot e build separados, preservando o relatório anterior de Config/AdMob. As duas compilações integrais confirmaram os dois corpos exatos e zero perdas de corpos anteriormente correspondentes. Atributos, campos completos e ordem dos métodos continuam iguais aos originais. Resultados: [KairoLibrary-empty-initializers.json](../../reports/KairoLibrary-empty-initializers.json); mapa dos candidatos: `analysis/kairo-empty-initializers-source-map.json`.
