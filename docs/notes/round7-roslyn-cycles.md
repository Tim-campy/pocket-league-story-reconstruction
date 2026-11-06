# Rodada 7: hipóteses Roslyn para os ciclos autônomos

Esta análise usa a baseline `cdd0f7f3b8dda09f296a7cfa16cd00236a3346ae`. As contagens são da medição anterior à adoção da Parte A; o histograma posterior pertence ao relatório do agente principal. Não foram compilados candidatos do jogo por este trabalho.

## Switch convertido em if com controle dos breaks

O par alinhado `brtrue.s => switch` reunia 3 métodos e 4720 bytes originais: `ResourceManager.LoadStart(JarInflater,Int32)`, `TextFormat.Init(String)` e `TextLayout._makeTextBlocks`. Nos três originais, o desvio inicia a seleção do caso zero; o recompilado usa uma tabela. Isso é compatível com uma cadeia de ifs no original, mas permanece hipótese para o corpo completo do jogo.

A transformação anterior `switch-if` tinha três limites: retirava somente break diretamente na seção, confundia breaks de loops/switches internos com os do switch removido e excluía mais de oito seções. ResourceManager.LoadStart usa `{ ... break; }` nos cases; preservar esse break depois de remover o switch pode fazê-lo sair do for exterior. TextFormat.Init contém um switch de caracteres no case 1/2. TextLayout._makeTextBlocks tem dez seções.

O arquivo separado `tools/Round7RoslynCycleRules.cs` contém `AddRound7RoslynCycleRules`, com a mesma API principal. O hint `round7_cycle_rule=switch-owned-break-if-chain` é obrigatório. A regra associa cada break ao primeiro switch/loop ancestral, remove o terminal mesmo em bloco de case, preserva os de controles internos e transforma breaks próprios não terminais em goto para uma saída comum. Preserva blocos e conteúdo, rejeita goto case/default, seletor com chamada e campo volátil, e aceita até 32 seções. Sem hint, não gera propostas.

**CONFIRMADO em fixture:** quatro corpos completos no csc 1.1 ficaram iguais aos controles escritos como ifs: bloco de case com loop/break interno, switch aninhado, dez seções e break não terminal. Ausência de hint, recusa de goto case, recusa de seletor com chamada e limites 32/33 também passaram. Foram compilados três assemblies artificiais e zero assemblies com fontes do jogo.

O helper ativo não foi integrado durante a execução completa. Os três métodos podem continuar bloqueados pelo modelo global parcial: capturas, scheduler, try/finally e switches restantes não desaparecem ao converter o primeiro switch. Uma recusa `incomplete` é UNKNOWN, não refutação da leitura de IL.

## Acesso condicional nullable com fallback constante

O par alinhado `brfalse.s => brtrue.s` reunia 8 métodos e 4676 bytes. Ele contém construções distintas.

Em `StreamUtil.WriteArray(OutputStream,Array,Int32)`, o original inicia com `ldarg.1; brfalse.s`, chama Array.GetLength com callvirt no ramo não nulo e converte o resultado para Int64 depois da escolha. O recuperado `array?.GetLength(0) ?? (-1)` inicia com brtrue, emite o fallback primeiro e usa call. A forma proposta é `array != null ? array.GetLength(0) : -1`. A regra usa cast para objeto no teste, evitando igualdade sobrecarregada; a fixture prova que isso não altera o IL do teste normal de referência.

`SaveRecorder.Unlock(Boolean)` tem outra causa: o recuperado retorna antes do laço quando forced é falso, e o original envolve o restante em `if (forced)`. `Offscreen.LateUpdate` funde condições com &&, enquanto o original contém guardas com retorno. Portanto o par de opcodes não define uma transformação única. O JSON conserva doze instruções lado a lado dos três exemplos e dos três switches.

O hint `nullable-conditional-coalesce-explicit` habilita somente a regra nullable. Exige receptor local, parâmetro ou campo não volátil de referência; membro que devolve value type não nullable; fallback constante. Não remove a coalescência quando o próprio membro retorna Nullable<T>: o receptor pode existir e ainda devolver null. Receptor com efeitos, fallback com chamada e resultado de referência são recusados.

**CONFIRMADO em fixture:** três corpos completos ficaram iguais aos controles de ternário explícito com csc 1.1: Array.GetLength, método não virtual e índice de array. As quatro formas recusadas e ausência de patches sem hint passaram. Foram compilados três assemblies artificiais e zero assemblies do jogo. A correspondência integral de StreamUtil.WriteArray permanece UNKNOWN até a verificação do candidato autorizado pelo modelo.

## Artefatos

- `tools/Round7RoslynCycleRules.cs`, SHA-256 `fa8f6622a4d6a45f19a889ee48f490906750ce7d8e28962063b82b72a874cfbb`, separado do helper ativo.
- `tools/round7_cycle_roslyn_fixture.py` e `reports/round7-roslyn-cycle-switch-fixture.json`: quatro controles de corpo e testes de segurança.
- `tools/round7_cycle_roslyn_nullable_fixture.py` e `reports/round7-roslyn-cycle-nullable-fixture.json`: três controles de corpo e quatro formas recusadas.
- `reports/round7-roslyn-cycle-diagnosis.json`: IDs, contagens congeladas, contextos de IL e motivos de modelo incompleto por exemplo.

Nenhum estado oficial, classe gerada ou assembly alvo foi escrito por esta análise.
