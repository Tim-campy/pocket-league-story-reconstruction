# R12 — GamePad.OnTouchEvent, duas expressões condicionais

Proposta privada finita, somente `surface.GamePad::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Void`, original `0x0600052c`,1.342IL. **Zero builds de alvo, verificador, alterações públicas ou estado. Corpo do jogo UNKNOWN.** Base oficial B5: `build/round12/runs/b05/commit/postverify01/Assembly-CSharp`.

## Medida e composição

Original: fat12/flags27/maxstack79/12locais/1.342IL, um catch de Exception. Atual: fat12/flags27/maxstack78/**mesmos12locais**/1.333IL, mesmo catch deslocado9bytes. O alinhamento encontra quatro hunks, correspondendo a somente **duas expressões**. Não há retarget independente. A assinatura de locais já está correta e não é a causa.

O prefixo bruto44 cai no primeiro operando da tabela switch em39: o destino do case5 está9bytes adiante no original. O primeiro comando causador é a atribuição do case14, em402, não a tabela switch.

### 1. Valor Boolean condicional, case14

Atual: `visibleTouchRange_ = !visibleTouchRange_;`, emitindo `ldc.i4.0; ceq` depois do load do campo. O original conserva a seleção de dois valores e um único store:

```text
395 ldarg.0
396 ldarg.0
397 ldfld     visibleTouchRange_
402 brfalse   413
407 ldc.i4.0
408 br        414
413 ldc.i4.1
414 stfld     visibleTouchRange_
```

Proposta: `visibleTouchRange_ = (visibleTouchRange_ ? false : true);`. A condição lê o mesmo campo uma vez, os ramos fornecem false/true e o destino recebe somente um store. Um if/else que grave o campo em cada braço não é o molde do original; foi mantido como negativo próprio.

Não se generaliza para os outros toggles do método: `hideDebugButton_`, por exemplo, continua com `ldc.i4.0; ceq` no original em635–636. A escolha é por ocorrência, sustentada pelos próprios opcodes.

### 2. Orientação do ternário integral, case21/22

Atual: `int num = ((id != 21) ? 1 : 0);`. Original em683–698: `ldloc.2; ldc.i4.s21; bne.un697; ldc.i4.0; br698; ldc.i4.1; stloc.s5`. Portanto a proposta mantém o ternário e usa `int num = ((id == 21) ? 0 : 1);`.

Ambos os testes são integrais/Boolean, sem inversão de comparação de ponto flutuante. Não se muda chamada, parâmetro, store ou folha de outro case.

Compostas, as duas mudanças produzem no cálculo abstrato **486objetos de instrução integralmente iguais e1.342IL**. A primeira acrescenta9bytes; a segunda troca condição/ramos sem mudar tamanho. Não há binário/IL emitido por esse cálculo e ele não é o verificador.

## EH e cabeçalho

No original: try `[725,737)`, catch `[737,743)`, tipo Exception, com leave para743. Atual: try `[716,728)`, catch `[728,734)`. Realocar todos os destinos conforme a composição desloca ambas as regiões exatamente9bytes e reproduz a descrição EH original, incluindo handlers e destino de leave. Nenhum novo comando é colocado dentro do try.

A diferença de maxstack79/78 é compatível com a emissão condicional e foi observada também nas fixtures próprias (13/12). **Não se afirma maxstack79 efetivamente emitido para o alvo** sem compilá-lo: header/locais/IL/EH reais continuam obrigação do oráculo.

## REA e código smcs

REA `ev_2231db74f18fe33d4af134b6e0fe13a6740268dc1705284e803b16ef4b576309`, token0x0600052c, assembly SHA `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`, IL SHA `ae6dc264df90b5bc56600ae6f9d7b7c380e69400e937bdbc5a12ee03d80b1d58`. Conferido com exatamente1.342bytes de IL após o fat header12 do dump congelado; não inclui EH/padding/cabeçalho.

Mono revisão fixada `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs/expression.cs`:

- linhas526–529: `Unary.EmitOperator/LogicalNot` emite a expressão, `ldc.i4.0` e `ceq`. Corresponde à forma atual de visibleTouchRange e à forma original de outros toggles.
- linhas4196–4215: `Conditional.Emit` emite a condição com salto falso, o braço verdadeiro, `br end`, o braço falso e o rótulo final. Reproduz a seleção Boolean e o ternário integral originais.
- linhas3500–3512: igualdade/desigualdade emitem `beq/bne.un` conforme o destino verdadeiro/falso. Aqui a forma fonte `id==21` é falsa no salto bne.un.

O código histórico explica o molde; não prova igualdade integral com o fonte interno Unity. As experiências usam o smcs real fixado, não um compilador reconstruído desses arquivos.

## NoEmit e seis fixtures próprias

NoEmit11 genuíno em auxiliar já existente: atual e candidato **complete=true/12tipos exatamente originais**. Nenhum modelo foi alterado/incompletude forçada. Metadados/tipos por si só não substituem as comparações reais posteriores.

Seis compilações de fixtures próprias, zero alvo. Smcs SHA `26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`; runtime SHA `3d102d9a63dae72e48396998bdbae4e9b2d8952ca6a8ebbcba698d3bc4345076`; flags/referências B5, `-out` privado e `-debug-`, sem sidecar PDB/MDB de produção.

- Candidato198IL = controle manual independente198IL em **header, assinatura, IL completo e EH**; ambos maxstack13.
- Molde anterior189IL/maxstack12 difere.
- Somente Boolean198IL/maxstack13 e somente ternário integral189IL/maxstack12 diferem do controle.
- If/else com dois stores no campo204IL/maxstack12 difere.

As fixtures incluem switch e try/catch próprios, não são um build do GamePad/jogo. Os controles isolados demonstram por que a composição completa é necessária. Três negativos do produtor, sem compilação adicional, recusam ocorrência duplicada, case errado e ausência do ternário esperado.

## Histórico negativo e limite

80recibos,78registros de trials (podem incluir sondas repetidas, não inferidos como compilações únicas). Nenhum avanço adotado consta desses recibos. **A polaridade do ternário num já foi compilada isoladamente em R4 e R5**, como `(id==21)?0:1`, e recusada com prefix44; duas versões if/else também ficaram44. Não se reapresenta essa tentativa isolada como nova descoberta. A atribuição Boolean anterior continuava9bytes menor e deslocava o primeiro alvo da tabela switch, explicando por que a alteração posterior não avançava o prefixo.

Os outros negativos de variáveis temporárias, hoists, guardas/retornos e solver continuam preservados, incluindo `smcs-empty-arm-restore` no if externo de case14, que manteve a atribuição `!` e ficou44. Nenhum payload registrado combina o **valor Boolean ternário** com a orientação integral original. Ausência universal sem payload/SHA continua UNKNOWN; zero candidatos não refuta construção.

API pura `calculate.py::transform_gamepad(method_source)->str`, sem classe/assinatura/I/O. `patches.json` guarda método normal completo, UTF16, SHA do arquivo/método B5, REA, modelos e produtor. O `before` foi confirmado único no Main ao preparar. Todas as chamadas/strings ficam na mesma ordem, nenhuma classe gerada é escrita e nenhum byte original é copiado para resultado.

Estado **READY_FOR_GAME_PROBE**, potencial de ganho em métodos/bytes **UNKNOWN** até build do operador exclusivo, assinatura/header/IL/EH reais e verificação completa sem regressões. Pacotes anteriores permanecem intactos.
