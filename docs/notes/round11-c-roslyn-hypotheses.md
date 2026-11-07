# Rodada11 — hipóteses Roslyn C6/C1/C2/C3, somente WIP

Status: **READY_FOR_GAME_PROBE**, com resultado dos quatro alvos **UNKNOWN**. Não houve compilação do jogo, adoção, alteração pública, mudança de modelo, fonte alvo, ledger, configuração, compilador ou referência. Tudo parte do snapshot congelado `build/round10/final-corpus/KairoLibrary`; qualquer vínculo futuro com Main exige rebase exato único.

## Evidência primária REA e vínculo dos bytes

Original KairoLibrary.dll: SHA256 `bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`, MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`.

| Alvo | Token original | IL original / próprio finalR10 | maxstack original / próprio |
|---|---|---:|---:|
| Seb.Draw(Graphics,Single,Single,Image[],Int32[]) | 0x060004e5 | 661 / 655 | 8 / 8 |
| SoundOGG lambda Play | 0x06000bc9 | 383 / 376 | 3 / 3 |
| ShaderBuilder.Init com Color[] | 0x0600095b | 467 / 467 | 3 / 3 |
| SaveRecorder.AutoSaveRun | 0x060004c4 | 537 / 531 | 5 / 5 |

Os quatro somam2048bytes originais, **peso de seleção**, sem estimativa comprovada de ganho. Os tokens só têm sentido com SHA/MVID acima.

As páginas completas REA são `rea-methods-page.json` (Evidence `ev_70995a109f1fc4076d0bbd64ac1a59748965bdda5b00014eea3f7e48f5a48121`) e `rea-sound-methods-page.json` (`ev_a80323e59f39ab3438938043da9065f8ee39b898be32c51ccac5a00161032a5a`). Elas registram todos os IL bytes por hash, tamanho, contagens de opcodes, anchors e EH sem carregar/executar o assembly. O decoder REA representa a display class pelo nome não qualificado `<>c__DisplayClass4_0`; a identidade exata foi vinculada pelo token0x06000bc9, SHA e MVID, não por suposição sobre a grafia do tipo.

`rea-byte-binding.json` verifica que o SHA de cada trecho IL retirado do raw body Cecil congelado é exatamente o `il_sha256` observado pelo REA. Dessa forma, os offsets/instruções detalhados usados em `diagnostics.json` estão vinculados aos mesmos bytes da evidência primária, não a uma DLL recompilada ou a uma descompilação inferida.

`rea doctor` respondeu, mas o audit-wide informa providers nativos opcionais ausentes e skill/registrations desatualizadas. Isso não bloqueou as APIs managed conectadas: os dois scans usados retornaram cobertura completa, sem issues de metadata. Não foi tentado setup, atualização ou instalação.

## C6 — constante na pilha depois do scheduler

**CONFIRMADA NA FIXTURE:** um local Boolean **não const**, inicializado com true e usado uma vez, pode desaparecer da assinatura e deixar exatamente `ldc.i4.1; brfalse` sob optimize+. O scheduler elimina o armazenamento, mas conserva a condição como BoundLocal sem ConstantValue; o emitter mantém o branch e suprime seu load. O literal `if(true)` segue outro caminho e é dobrado.

Proposta estreita em Seb.Draw:

```csharp
bool recoveredDrawRegion = true;
if (recoveredDrawRegion) {
    // Corpo inteiro do if(true) recuperado, literalmente preservado.
}
```

O original tem `ldc.i4.1@266; brfalse@267->637`; o próprio passa diretamente de stloc7@264 para a folha seguinte. Essa é a única região de sequência divergente, sem retarget independente e com as mesmas9variáveis locais. O branch salta para a saída comum depois dos demais braços da cadeia; não se inventa um else nem se altera qualquer folha ou comparação flutuante.

Fixture própria:373→379IL, exatamente+6;5→5locais,maxstack8→8; candidato inteiro e todos os auxiliares iguais ao controle manual independente, before diferente. O par longo foi reproduzido. Controles compilados na mesma fixture:

- Literal `if(true)` e `const bool=true` removem o teste: **REFUTADOS como forma que produz o par**.
- Boolean cujo endereço é observado não vira o mesmo alias só na pilha: ausência do par adjacente, local mantido.
- Mesmo alias com corpo curto produz `ldc.i4.1;brfalse.s`, confirmando que os seis bytes dependem do destino exigir a forma longa.

Três negativos textuais também recusam colisão de nome, segundo if(true) ou folha aritmética diferente. O nome histórico do Boolean é UNKNOWN; nomes de locais não constam do metadata alvo.

### Pipeline fixado e trechos

Roslyn1.1, revisão `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`; cópias autenticadas por SHA1 de Git blob e SHA256 em `pinned/provenance.json`:

1. `src/Compilers/CSharp/Portable/Lowering/LocalRewriter/LocalRewriter_IfStatement.cs:40..63` cria a forma de controle:

   ```csharp
   builder.Add(new BoundConditionalGoto(rewrittenCondition.Syntax,
       rewrittenCondition, false, afterif));
   builder.Add(rewrittenConsequence);
   ```

2. `CodeGen/EmitStatement.cs:364..385` trata **somente** uma condição cujo ConstantValue existe como branch constante:

   ```csharp
   if (condition.ConstantValue != null) { /* constant branch handling */ return; }
   ```

   A condição do local não const não recebe o ConstantValue do literal inicializador. Logo o branch não se transforma em if(true) constante.

3. `CodeGen/Optimizer.cs:1629..1680,1866..1881` impõe restrições de pilha e reescreve acessos de locais agendados; não é uma propagação do literal true para a condição. As fixtures mostram o resultado concreto neste caso de um uso.
4. `CodeGen/EmitExpression.cs:2188..2195` não emite store para IsStackLocal e `1069..1081` não emite load, pois o valor já está na pilha. O ldc1 do inicializador permanece para o brfalse consumir.

O NoEmit **não foi corrigido nem forçado por método**: complete=false,10tipos contra9originais, com `stack_scheduler_single_use:recoveredDrawRegion` e `stack_scheduler_multiple_use:num4`. Portanto não existe veto de previsão completa errada. A incompletude genuína permite medir o candidato com o compilador alvo conforme a política vigente, sem alegar antecipadamente que o corpo fecha.

## C1 — três returns dentro do try da lambda

**CONFIRMADA NA FIXTURE:** a composição põe return depois de SetVolume/time=0 no primeiro braço; transforma o guard negativo de mute em `if(type_==1 && mute)return;`; transforma o guard port!=−1 em `if(port==-1)return;`. Os antigos blocos else/if positivos viram blocos simples, preservando suas folhas e escopos. O método pai Play continua criando a lambda pelo compilador; nenhuma display class é escrita.

Original: leave@101->382; brfalse@138 +leave@140; bne.un@231 +leave@233. Próprio: br@101->345; brtrue@138->345; beq@229->345. É a mesma saída lógica, mas return protegido exige leave para fora da região, em vez de encaminhar cada ramo à saída comum dentro do try.

EH original: try6..354/catch354..382; próprio6..347/catch347..375. A fixture final reproduz exatamente a extensão de7bytes:370→377IL,3→3locais, mesmo início do try e fim/handler+7. O candidato inteiro e todos os métodos auxiliares igualam o controle manual; before difere.

O primeiro toy tinha padding excessivo: o beq original virou longo e o delta foi+4, embora candidato e controle já fossem iguais. Essa tentativa está preservada em `c1-initial-too-long-fixture`/`c1-initial-fixture-report.json` com3compilações, não é um teste do alvo nem refutação da hipótese. A fixture revisada usa a distância e o catch compatíveis com o padrão real.

NoEmit: complete=false,0tipos, `generated_or_unmapped_source_method`. Não se inventa assinatura de previsão para a lambda.

## C2 — cópia do argumento de array e dispatch em ordem

**CONFIRMADA NA FIXTURE:** dentro do guard existente, copiar `colors` para Color[] local, usar esse array no comprimento e no elemento do for, e trocar o switch opaTransOrder1/2 pela cadeia if1/else-if2 com as mesmas duas atribuições.

O original249..257 tem `ldarg.3; stloc.0; ldc0; stloc.1; br; ldloc.0; ldloc.1; ldelem.any Color`, e Color fica em2. O próprio omite a cópia e usa argumento3 diretamente; Color cai em1. O original378/392 tem testes bne inline por braço; o próprio usa beq/beq e um dispatch antes dos corpos. As onze regiões de sequência se dividem apenas entre essas duas causas e os slots consequentes.

Não se alteram os testes r/g/b, comparações de alpha ou a ordem de avaliação de campos. O argumento é por valor, o alias é avaliado uma vez dentro do guard já não nulo e o corpo não reatribui o parâmetro. O modelo NoEmit é complete=true e prevê os3tipos exatamente iguais aos originais.

Fixture:115→116IL,2→3locais; igualdade integral candidato/controle manual, before diferente. O switch do toy é terminal e o real tem comandos depois dele; o delta de1do toy não é apresentado como reprodução do delta0do alvo. Um controle foreach independente teve os mesmos bytes/locais/cabeçalho/EH da cópia explícita com for. A comparação de métodos declarados apontou só `signature`, porque os nomes são Run/ForeachControl; nenhum DTO ou assembly foi normalizado. Assim a sintaxe histórica **for com alias versus foreach permanece UNKNOWN**, e a proposta escolhe a cópia explícita por ser a menor edição do fonte recuperado.

A primeira tentativa de comparar esses dois nomes como se fossem a mesma assinatura parou o driver, embora o candidato Run já igualasse o controle Run integralmente. Os6compiles daquela execução (Sound+Shader) ficaram preservados em `c123-initial-named-method-comparison` e no recibo correspondente. O check corrigido compara explicitamente as facetas de corpo desse contraste, mantendo o oráculo integral para candidato versus controle de mesmo nome.

## C3 — cancelamento retorna de dentro do try e log precede relógios

**CONFIRMADA NA FIXTURE:** trocar apenas os três `if(asCancel_){break;}` por return e restaurar a ordem das declarações do corpo do do: AutoSaveLogData log, relógio inicial num, relógio de pack num2. Inicializações continuam nos pontos antigos; o log só é preenchido pelo mesmo out de MakeSaveData. Os três continue do do e todo o for de logs permanecem.

Os guards ficam no corpo externo do do, antes de qualquer for/switch; o produtor recusa owners não comprovados. O original tem brfalse+leave nos offsets49/51,104/106,154/156; o próprio só encaminha os três brtrue ao final do loop/try. A mudança explica+6bytes e as extensões dos dois EH. Os tipos são10em ambos, mas o original pede **log1, num (tempo inicial)2, num2(pack)3**, seguido de Property4 e num3(Int64)5. Os nomes parecidos não foram usados para adivinhar os papéis: os usos CurrentTimeMillis/MakeSaveData/out e os tempos de leitura os vinculam.

Fixture:648→654IL,10→10locais; igualdade integral candidato/controle manual, before diferente. NoEmit permanece complete=false por concatenação/scheduler/EH, mas prevê os10tipos na ordem original. Não foi promovido a completo nem rejeitado por estar incompleto.

## Histórico e validação

`history.json` captura172registros antigos. Trials reais: Seb25 (24R4 e1root-guardR10), Sound11R4, Shader6R4, Save6R4. Registros com0trials não são negativos.

- SebR4condition-local tentou as condições de sp/num4, **não** o literal true; o novo molde não repete aquele payload.
- Sound tentou guards/fluxo isolados, sem a composição dos três returns protegidos.
- Shader tentou condição/escopo/guard de loop, sem a cópia do array junto da cadeia de seletor.
- Save teve um partial de ordenação, mas nenhum conjunto com os três returns e a ordem log/num/num2.

`history-sha-audit.json` registra hashes dos relatórios, snippets e fontes candidatos completos disponíveis. Não houve SHA completo repetido. Trials antigos sem proveniência completa impedem afirmar exaustividade histórica global; nomes/rules isolados não são tratados como igualdade de candidatos.

Foram12compilações próprias no conjunto final (3C6+9C1–3), e9anteriores preservadas, total21. Todos os candidatos finais igualam integralmente seus controles manuais independentes, com todos os before diferentes. Há3recusas textuais C6,5recusas C1–3 e os4controles compilados C6 (três negativos e o branch curto). Esses checks não contam como ganhos nem compilações do jogo.

O compiler é o csc1.1.0.51109 fixado, SHA256 `53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`, optimize+,debug:pdbonly,deterministic−,langversion6 e referências exatas do manifesto finalR10. Os runners verificam o cache do inspector antes de usá-lo e não reconstroem helper compartilhado.

## Bundle e limites finais

`c6_calculate.py::transform_seb_nonconstant_true` e `c123_calculate.py::{transform_sound_protected_returns,transform_shader_array_alias_and_ordered_selector,transform_save_cancel_returns_and_log_scope}` são APIs puras sem I/O no import/chamada. As propostas são métodos pai/normal completos com UTF16 exato, antes/depois e SHA de fonte, recibo e dumps. Os campos de modelo são os resultados brutos do NoEmit, não estados editados à mão.

Readonly/PDB/heap/raw-metadata, DEX e normalização não foram usados para corrigir qualquer resultado. Toda assinatura real, cabeçalho, byte IL, EH e metadado dos alvos permanece sujeito à compilação e ao verificador completos do root. Esta frente termina no congelamento privado; Image.Use e qualquer família posterior aguardam autorização separada.
