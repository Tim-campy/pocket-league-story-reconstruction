# R12 — UserData.SetTrainingCommonMethod

Proposta privada, um único alvo: `game.UserData::SetTrainingCommonMethod(System.Boolean)->System.Void`, token original `0x06000403`, 3.287 bytes de IL. Nenhuma compilação do jogo e nenhum verificador foram executados nesta investigação. O resultado do alvo permanece **UNKNOWN**.

## Medição e composição

O snapshot `build/round12/baseline/Assembly-CSharp` contém o avanço R11 C4 já adotado, de 802 para 1.334 bytes de prefixo. O arquivo inteiro `UserData.cs` era idêntico ao Main no momento da preparação; `bindings.json` registra seu SHA, o método completo, o intervalo UTF16 e as quebras de linha mistas. A transformação mantém esse avanço anterior.

Original: IL 3.287, fat header 12, maxstack 116, 63 locais, nenhuma região EH. Atual: IL 3.292, mesmo maxstack, mesma assinatura de 63 locais e nenhuma EH. Há somente **dois trechos de sequência diferentes**, sem retargets independentes, segundo o alinhamento por instruções/rótulos.

O primeiro byte divergente, 1.334, pertence ao operando do `br` em 1.333: original vai para 1.778; atual para 1.783. A causa está adiante. O primeiro trecho realmente inserido começa em 1.531: a guarda de tamanho no topo do corpo do `for num18`. A condição original, em 1.709–1.729, testa o índice e depois o tamanho; a atual, em 1.730–1.734, testa somente o índice.

```csharp
// Atual
for (int num18 = 0; num18 < areaMaxNum; num18++) {
    if (array[num14].Size() <= 0) { break; }
    // folhas e laços existentes
}
// Proposta calculada
for (int num18 = 0; num18 < areaMaxNum && array[num14].Size() > 0; num18++) {
    // as mesmas folhas e laços existentes
}
```

A condição é integral; não se altera nenhuma comparação de ponto flutuante. O teste do limite continua antes da chamada `Size`. A chamada ocorre uma vez por tentativa válida de iteração. Não há `continue` nesse laço que precise ser reescrito. O `break` de sucesso dentro do laço `num19` continua pertencendo a `num19`; os `break` de limite de `num15` continuam pertencendo respectivamente a `num18` e `num17`. A guarda retirada era o primeiro comando do corpo, sem folhas anteriores que fossem reordenadas.

Destinos originais: incremento de `num18` em 1.703, condição em 1.709, corpo em 1.531 e saída em 1.734. O `br` em 1.698 vai para 1.734. O `break` de `num19` em 1.652 vai para 1.689. O `break` de `num17` em 1.743 vai para 1.772. Os CFGs e as árvores lexicais/variáveis do modelo estão preservados em JSON.

A realocação abstrata dos objetos de instrução atuais, retirando a guarda superior e inserindo o teste na condição, resulta em 3.287 bytes e 1.373 instruções, com **zero objetos de instrução diferentes** do original. Isso é diagnóstico de alinhamento, não uma compilação, não produz DLL/IL binário e não substitui o oráculo.

## Evidência primária e emissão

REA: `ev_f869e96d4a2a945e6b754c970c7bf525be8c6382f30783753a938dcbf70bce78`, método `0x06000403`. Assembly original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`; IL original SHA256 `2abc5f3513f5819ca23c4fb269e6febcecc172c251a0d9abae9be06a10ee6728`. A extração dos 3.287 bytes após o fat header do dump congelado foi conferida com o SHA de IL do REA.

Fonte Mono fixado na revisão `b6587926c0f146c8b12985744f33a13b39a22d57`:

- `analysis/tools/round5-mono-source/mcs/mcs/statement.cs:569–619`, `For.DoEmit`: emite `br test`, corpo, incremento e `Test.EmitBranchable(ec, loop, true)`, com `LoopEnd` após o teste.
- `analysis/tools/round5-mono-source/mcs/mcs/expression.cs:3450–3457`, `Binary.EmitBranchable`, `LogicalAnd/on_true`: o operando esquerdo salta para `tests_end` quando falso; o direito salta para o corpo quando verdadeiro. Isso explica `bge saída` e `bgt corpo` no original.
- `statement.cs:1117–1120`, `Break.DoEmit`: desvia para o `LoopEnd` atual; 1.148–1.151 fazem o análogo para `Continue/LoopBegin`.
- `statement.cs:2032–2047`, `Block.EmitMeta`: locais do bloco, temporários registrados e filhos. A remoção da guarda não é presumida neutra para slots: foi medida separadamente no modelo fonte.

SHA dos dois arquivos de compilador: `statement.cs` = `063a437095e1d608d045383d76291c9359e9a98220eea7750018a3d42712437a`; `expression.cs` = `3e6392384936d08bd7142f8bf68d21ae4f8c429f9c8b0ac099da97bff53c74af`. A correspondência exata dessa revisão histórica com todo o fonte interno do Unity é UNKNOWN; as fixtures usam o **smcs instalado e fixado**, não um compilador recompilado desses fontes.

## Fixtures e modelo

Cinco compilações de fixtures próprias, somente na pasta privada, com smcs SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, Mono SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`, mesmas opções/referências do manifesto App. `-out` foi redirecionado para cada `Fixture.dll`; `-debug-`, sem PDB/MDB de produção.

O candidato transformado e o controle manual independente, com nomes de locais distintos, batem em header, assinatura de quatro locais, IL completo e EH. Ambos têm 192 bytes; o molde anterior tem 197. Maxstack 14 contra 15 no molde anterior. Os negativos `Size() >= 0` e troca do `break` exterior por `continue` diferem. São controles da **região**, não uma réplica de 3.287 bytes do método inteiro. Não há promessa de maxstack do jogo baseada no toy.

NoEmit R11 genuíno, sem compilar auxiliar compartilhado novo: original atual e candidato foram previstos como **complete=true**, 63 tipos, exatamente iguais à assinatura original. O candidato é elegível ao gate completo, sem marcar incompleto artificialmente e sem dispensar a assinatura real após compilação. O JSON inclui os blocos/variáveis AST fonte; slots 27–34: num14, num15, num16, num17, areaMaxNum, num18, areaMaxNum2, num19. Nenhuma fonte de modelo foi alterada.

## Histórico e limites

Busca por esse ID em relatórios `*engine.json`: 76 arquivos encontrados, 75 recibos de método. Há 62 registros de trials somados entre relatórios; incluem sondas e execuções que podem repetir o mesmo candidato, portanto **não são 62 compilações distintas inferidas**. O relatório registra o SHA de cada recibo e conserva as rejeições.

R4 levou 456→802 com `switch-if`. R11 C4 levou 802→956 com a composição de população de array e 956→1.334 com `smcs-empty-arm-restore`; sonda e execução oficial registram esses mesmos passos. Outros trials existentes são variantes de declarações/escopo e solver. Nenhum trial registrado contém `areaMaxNum &&`; não foi encontrada a SHA deste candidato. Relatórios sem payload/SHA completo não permitem provar ausência universal de tentativa, e execuções com zero candidatos não refutam a construção.

API pura: `calculate.py::transform_training_num18_condition(method_source) -> str`. Aceita o método normal, sem dependência de cabeçalho/classe ou I/O. Requer exatamente o `for num18` esperado e a declaração imediatamente após a guarda, preservando todas as demais folhas. `patches.json` contém antes/depois completos, UTF16, bindings, modelos e produtor; não escreve classe gerada nem importa bytes originais no resultado.

Estado: **READY_FOR_GAME_PROBE**, para o operador exclusivo. Ganhos em métodos/bytes: **UNKNOWN** até compilação do alvo, assinatura/header/IL/EH reais e verificação completa. Nenhuma alteração pública, de estado ou de assembly foi feita por esta investigação.
