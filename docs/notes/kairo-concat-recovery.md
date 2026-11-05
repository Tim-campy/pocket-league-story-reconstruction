# Três métodos restantes classificados como concatenação

As assinaturas e contagens de `System.String.Concat` já correspondiam ao original nos três métodos, tanto no fonte principal quanto na variante Codemerx. A investigação não encontrou uma referência errada nem uma sobrecarga diferente. O rótulo automático agrupava mudanças maiores de fluxo e variáveis locais próximas dessas chamadas.

Foram recuperados três corpos exatos por ajustes C# isolados:

| Método | Bytes IL | Locais | Resultado |
|---|---:|---:|---|
| `ShaderBuilder.GetName(int)` | 479 | 2 | Corpo completo idêntico |
| `StreamUtil.ReadArray(InputStream, Array, Type, int)` | 1.207 | 24 | Corpo completo idêntico |
| `Language.LoadLanguagePack(byte[])` | 3.837 | 89 | Corpo e exceções idênticos |

## ShaderBuilder.GetName

O original copia o argumento inteiro para uma variável local e usa essa cópia nas verificações de bits e no cache. O fonte ILSpy havia eliminado a cópia. A declaração `int bits = nameBits;` restaura a assinatura de locais `[int, string]`.

Duas expressões condicionais que concatenavam strings também haviam sido fundidas pelo decompilador. Reescrevê-las como `if`/`else`, com a atribuição à string em cada ramo, restaura os stores e os destinos dos desvios originais. As sobrecargas de Concat permanecem iguais.

## StreamUtil.ReadArray

A leitura do comprimento original usa uma cadeia ordenada de testes para 1, 2 e 8 bytes, seguida pelo caso padrão de 4 bytes. O `switch` recuperado emitia os testes juntos antes dos corpos. Restaurar `if`/`else if` conserva a ordem original das instruções.

O trecho final testa positivamente a interface `ISerializable`, desserializa nesse ramo e lança a exceção de tipo não suportado no `else`. O fonte ILSpy havia antecipado a exceção com uma guarda negativa. Essa mudança afeta a posição da concatenação da mensagem, embora a chamada Concat em si já estivesse correta.

## Language.LoadLanguagePack

O método maior exigiu combinar cinco recuperações de estrutura, com comparação após cada candidato:

- Um bloco lexical separado para as três strings de processamento da tradução restaura sua posição na assinatura de locais.
- O primeiro percurso dos segmentos volta a ser `foreach` sobre array. O compilador passa a reutilizar o slot do índice temporário em um `foreach` posterior, evitando o local inteiro adicional do `for` recuperado.
- As atribuições passam a ter a forma `dictionary[key] = (value = new List<T>())`, conservando a avaliação do receptor e da chave antes da criação e o store da variável no ponto original.
- Três acessos condicionais `?.` voltam a ser verificações explícitas de `null`; isso também restaura o `callvirt` de fechamento no `finally`.
- A etapa final fica em um ramo positivo quando existem registros, preservando a saída do `try` no ponto original em vez de introduzir um retorno antecipado.

O primeiro candidato ainda diferia: 3.841 bytes e 90 locais. O segundo restaurou os **3.837 bytes, os 89 locais e todas as cláusulas de exceção** do original.

## Verificação e integração

`tools/recover_kairo_concat.py` captura os 163 fontes cujos hashes correspondem a um recibo integral do verificador. Cada candidato é compilado dentro do assembly inteiro congelado com Roslyn 1.3.2. Os três candidatos também foram compilados e comparados juntos: todos continuam exatos e **nenhum corpo anteriormente correspondente foi perdido**.

O helper não modifica os fontes principais ou o cadastro. Os candidatos ficam em `analysis/variants/kairo-concat-agent/`, e o mapa dos três arquivos está em `analysis/kairo-concat-agent-source-map.json`. Se o seletor já mudou outros métodos desses arquivos, a integração deve transplantar somente o corpo do método alvo para o fonte atualizado, seguida da verificação integral oficial.

Resultados, hashes de base/candidatos, provas das assinaturas Concat e verificação conjunta: [KairoLibrary-concat-recovery.json](../../reports/KairoLibrary-concat-recovery.json). Fontes de referência, DLLs e o resultado compilado não foram editados nem usados como atalhos de bytes.
