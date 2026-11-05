# Contrato do verificador

`tools/verify` será o único componente autorizado a promover métodos a `matched`. Sua implementação atual valida a referência e bloqueia a verificação da reconstrução: o comparador completo ainda está pendente.

## Build e evidência

Cada execução futura deverá compilar os assemblies inteiros e registrar executável do compilador, hash, versão, argumentos, ordem dos fontes, hashes de fontes e referências e hash do resultado. Uma DLL copiada do original ou compilada a partir de IL não satisfaz esse contrato.

## Comparação por método

Conferir tipo de cabeçalho tiny/fat, flags, `maxstack`, `initlocals`, assinatura de variáveis locais, sequência de instruções, valores constantes, desvios, todos os destinos de `switch` e regiões de exceção. Operandos de metadados devem resolver para a identidade completa de tipos, campos, métodos, assinaturas e strings, com suas referências de assembly. Um token numérico igual não comprova identidade; um token diferente pode designar o mesmo membro.

Hashes normalizados e listas parciais de instruções não substituem essa comparação. Se houver resolução parcial ou ambígua, o método permanece sem correspondência comprovada. Declarações sem corpo exigem conferir assinatura, flags e implementação nos metadados.

## Tipos e assemblies

Conferir ordem e conteúdo de campos, métodos, propriedades, eventos, atributos, tipos aninhados, constantes e layout. A comparação do assembly abrange tabelas, heaps, recursos e referências. O relatório deve distinguir igualdade estrutural com tokens resolvidos, igualdade bruta e diferenças explicitamente autorizadas.

## Exceções

A lista de exceções começa vazia. Uma exceção de MVID ou timestamp exige demonstração com duas compilações consecutivas do mesmo fonte e identificação exata do campo que variou. Nenhum outro campo poderá ser normalizado por conveniência, e a DLL de build nunca será editada depois da compilação.

## Estados e regressões

O verificador atualiza `methods.csv` somente com resultados da compilação e comparação. Toda alteração exige verificar o assembly completo; uma regressão de método `matched` falha a execução. `PROGRESS.md` é regenerado depois, nunca editado à mão. Até essa implementação existir, os scripts rejeitam qualquer promoção manual a `matched`.
