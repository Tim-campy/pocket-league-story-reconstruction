# Rodada 5: ordem dos slots no smcs do Unity 5.3.4f1

Hipótese de nomes/hash: **REFUTADA** para os caminhos ordinários, lambdas e lambdas com resolução de sobrecarga testados. A fixture compilada oito vezes pelo smcs instalado manteve os corpos canônicos ao trocar apenas os nomes; inverter a ordem das declarações mudou a assinatura de locais. Evidência reproduzível: `reports/round5-slot-fixture.json`. Isto não afirma que todos os caminhos internos possíveis do compilador foram exercitados.

## Identidade e limite da fonte histórica

Foi consultado o repositório oficial Unity-Technologies/mono, branch `unity-5.3`. O snapshot anterior ao lançamento consultado é `b6587926c0f146c8b12985744f33a13b39a22d57` (2016-03-11). A revisão exata usada para fabricar o smcs do editor permanece **UNKNOWN**: não foi localizada uma referência `unity-5.3.4f1`. A ponta consultada da branch foi `12ac9d2d802867d06c066b5b330e50df94bf1f13` (2016-11-19). Os arquivos statement.cs, codegen.cs, expression.cs, cs-parser.jay e configure.in são idênticos nos dois snapshots. O [configure.in, linha 9](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/configure.in#L9) identifica Mono 2.6.5.

O próprio executável instalado foi inspecionado: assembly `smcs, Version=2.6.5.0`; SHA-256 `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`. As chamadas observadas confirmam as estruturas e caminhos descritos abaixo. Os tokens e resumos estão em `reports/round5-smcs-source.json`; o IL de inspeção permanece na área ignorada de análise.

## Regra de alocação

O [getter Variables](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1550-L1555) cria `ListDictionary`. [AddVariable](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1791-L1794) insere pelo nome. [EmitMeta](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2032-L2047) visita locais declarados, temporários registrados e filhos, nessa ordem. [ResolveVariable](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L1225-L1239) declara o slot; capturas são exceção. O [ListDictionary](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/class/System/System.Collections.Specialized/ListDictionary.cs#L73-L110) usa lista encadeada: inserção acrescenta ao fim, enumeração percorre a lista. Logo, a chave nome não determina a ordem.

Trecho mínimo de EmitMeta:

```csharp
foreach (LocalInfo vi in variables.Values)
    vi.ResolveVariable (ec);
```

A árvore interna não corresponde somente às chaves do C#. O [parser](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/cs-parser.jay#L6062-L6105) cria bloco implícito quando a declaração aparece depois de um comando; inicializadores também são comandos. A [produção da gramática](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/cs-parser.jay#L4431-L4437) instala o bloco como corrente. Assim, `object outer; { double child; ... } int laterOuter; ...` com inicializadores/comandos na fixture produz Object, Double, Int32. O último int pertence a um filho implícito posterior ao bloco double. Declarações sem inicializador podem compartilhar o bloco ainda não usado.

## Hashtable encontrada: caminho de sondagem

[CloneTo](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/statement.cs#L2299-L2317) cria Hashtable. Entretanto, [CompatibleMethod](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L1168-L1185) seleciona a cópia apenas em modo de sondagem. [Compatible](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/anonymous.cs#L1024-L1063) guarda o resultado somente fora desse modo; a conversão efetiva utiliza o bloco original. A Hashtable encontrada não sustenta busca de nomes para os casos testados.

## Temporários

[TemporaryVariable.DoResolve](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/ecore.cs#L5933-L5957) registra o temporário no bloco corrente durante resolução. Ele participa da etapa de metadados. Já [LocalTemporary.Store](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/assign.cs#L271-L283) pede slot durante emissão. [GetTemporaryLocal](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/codegen.cs#L357-L390) reutiliza slots liberados por tipo; na falta, declara outro. [TemporaryReturn](https://github.com/Unity-Technologies/mono/blob/b6587926c0f146c8b12985744f33a13b39a22d57/mcs/mcs/codegen.cs#L412-L434) também aloca durante emissão quando necessário.

No mesmo statement.cs: foreach de coleção registra enumerador (5674–5675); foreach de array registra cópia e contadores, acrescentando comprimentos para rank maior que um (5187–5203). using de expressão registra cópia (4899–4936); using com declaração usa o local declarado (5040–5113). lock registra cópia do objeto (4111–4137). switch usa LocalTemporary durante emissão (3823–3866). A liberação de enumerador pode acrescentar temporário IDisposable (5790–5815). Esses pontos impedem deduzir todos os slots apenas contando variáveis explícitas.

## Consequência para a rodada

Preferir recuperar declaração, posição e árvore de blocos implícitos, distinguindo temporários registrados dos alocados durante emissão. Busca por nomes não é indicada pela evidência. Cada aplicação ao jogo continua dependente de compilação e do verificador; esta investigação não alterou fonte do jogo nem estados.
