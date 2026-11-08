# R12 — Player.Act, ramo vazio de action5

Proposta privada para `game.Player::Act(System.Int32)->System.Boolean`, original `0x06000317`,1.599IL. **Zero builds do jogo/verificador/alterações públicas; resultado do alvo UNKNOWN.** A base oficial usada é B4 `build/round12/runs/b04/commit/postverify01/Assembly-CSharp`.

## Evidência e causa

Original: fat12/maxstack97/11locais/EH0/1.599IL. Atual: fat12/maxstack97/mesmos11locais/EH0/1.587IL. O alinhamento por rótulos encontra **um único trecho removido**, sem retargets independentes. O prefixo bruto de IL é126: ali está o operando de `br` em125, que vai para o retorno comum deslocado pelos12bytes ausentes. Corrigir a instrução em126 seria atacar o sintoma.

O trecho original depois do action10:

```text
1022 br       1597
1027 ldarg.1
1028 ldc.i4.5
1029 bne.un   1039
1034 br       1597
1039 ldarg.1
1040 ldc.i4.6
1041 bne.un   1065
...
1597 ldc.i4.1
1598 ret
```

O atual, após seu action10, vai diretamente a `ldarg.1; ldc.i4.6; bne.un`. O destino1597 foi conferido como `ldc.i4.1; ret`, e o destino1039 começa o teste de6. Portanto o ramo5 não tem efeito/call e salta para a **mesma saída comum**, não para o retorno false do bloco6.

Forma calculada do método normal:

```csharp
// folhas do action10 intactas
} else if ((act_id == 5)) {
} else if ((act_id == 6)) {
    if (seb_ == 1) { return false; }
// demais ramos intactos
```

Não se troca o salto por `return true;`: essa variante altera a emissão. Não se introduz local, classe gerada, chamada ou alteração de folha. A comparação adicional é integral, sem ponto flutuante. As condições do restante da cadeia são os mesmos testes puros do argumento; a forma histórica exata do C# continua UNKNOWN até o oráculo.

A inserção diagnóstica dos quatro objetos de instrução `ldarg.1; ldc.i4.5; bne.un; br`, com realocação de todos os destinos, resulta em **575objetos integralmente iguais,1.599IL,zero resíduos**. Isso não emite bytes/DLL e não é verificação do alvo.

## Origem e compilador

REA `ev_0f0b4ff35a6ef7ac373ee0dc540344fe59b5daba3e90a367451e98e28bbcd1b7`, assembly SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`, token0x06000317, IL SHA `f3c24ff60776b49dc39f388ebb0e532d52dcd3ef3e53a47a3430d112b1aa3875`. O hash foi conferido sobre exatamente1.599bytes após o fat header12 do dump original.

Mono revisão fixada `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs/statement.cs:225–267`, `If.DoEmit`:

- linha247: emite a condição com salto falso.
- linha249: emite o bloco verdadeiro, que pode estar vazio.
- linhas251–264: havendo else, define saída e emite `br end` quando o verdadeiro não é um return, antes de emitir o bloco falso.

Isso explica o par `bne.un` para o próximo teste e `br` para a saída comum. Nas linhas255–256, um verdadeiro que seja return evita esse `br`: a fixture negativa com `return true` mede a diferença. `expression.cs:3500–3512` descreve os opcodes de igualdade/desigualdade; aqui a comparação false de `act_id==5` emite `bne.un`.

Essa leitura não prova igualdade integral do fonte Mono histórico com o interno Unity. As fixtures usam o smcs real instalado de SHA fixado, não um compilador reconstruído.

## NoEmit e fixtures próprias

NoEmit11 genuíno, auxiliar já existente: atual e candidato **complete=true/11tipos exatamente originais**. Nenhum modelo foi alterado, nenhuma incompletude forçada e nenhuma previsão completa errada foi dispensada. Os blocos/variáveis AST e hashes estão no relatório privado.

**Cinco compilações próprias** de um molde pequeno da região, nunca Player/Assembly-CSharp. Compiler SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`, runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`, flags/referências B4, `-out` privado, `-debug-`, sem PDB/MDB de produção.

- Candidato95IL = controle manual independente95IL em header, locais, IL completo e EH.
- Molde anterior83IL difere.
- Negativo action7 tem95IL, mas a constante difere.
- Negativo `return true;` no action5 tem92IL e difere do controle.

São provas do mecanismo nessa região; o maxstack/header do jogo não é inferido do toy. Três negativos textuais do produtor recusam action5 já existente, sequência de donos fora do molde e ocorrência duplicada.

## Histórico e rebase

79recibos,85registros de trials (possivelmente repetidos entre sondas e execução). R7 `smcs-owned-switch-if` avançou114→126 e permanece no fonte atual. O payload **antes** dessa transformação já não tinha `case 5`; sua saída não tinha action5. Portanto não se atribui a omissão à transformação antiga. Nenhuma restauração de5 aparece nos payloads de trials registrados; as hipóteses antigas de polaridade, espelhos, locais, guardas e retornos recusadas continuam preservadas. Recibos sem payload/SHA não provam ausência universal, e zero candidatos não refuta uma construção.

`Player.cs` pode receber Collision em paralelo pelo operador. O pacote está vinculado ao arquivo histórico B4 e ao SHA/UTF16 do método inteiro; o mesmo `before` foi confirmado uma única vez no Main durante a preparação. **Não exigir o arquivo inteiro histórico depois de Collision**: o operador deve fazer rebase único do método normal e renovar suas provas. Nenhuma edição de Collision foi lida como candidata ou alterada nesta frente.

API pura `calculate.py::transform_player_act(method_source)->str`, independente de classe/assinatura/I/O. `patches.json` contém antes/depois completos, bindings, NoEmit e produtor. Estado **READY_FOR_GAME_PROBE**; ganho em método/bytes **UNKNOWN** até assinatura/header/IL/EH reais e verificação completa sem regressões pelo operador exclusivo.
