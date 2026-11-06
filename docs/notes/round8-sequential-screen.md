# C4 Roslyn switch diagnosis: candidate source was not the compiler input

No new source rule is justified by the six C4 negative outcomes. The ordinary zero-first if/else-if chains were present in trial.after, but the sequential incomplete-model route compiled the unchanged screening source. Thus these hypotheses were not tested by those compiler invocations. No game assembly was compiled and no active file edited during this diagnosis.

## Observed paths and bodies

`tools/round8_engine.py` creates `target_screen` from accepted `text` and assigns `proposed_screen[logical]=target_screen` around lines833–836. The sequential trial writes `candidate` only into `c.paths[logical]` at879; it calls `c.build(screen_paths=screen_paths)` at881. That build selects the alternate target_screen path, which has never received source_filter(candidate). The parallel complete-model TrialPool receives source_filter and does not share this omission.

Read-only proof: C4-probe ResourceManager screen compile.rsp line136 selects target-screen/ResourceManager.cs; that source still contains switch(num2), while the reported variant is an if chain. The preserved screen method is completely identical to the accepted baseline, including headers, locals, IL and EH (`differences=[]`). TextFormat's preserved final screen method is also completely identical to baseline; both original switch(i) and switch(c) remain in its effective source. Paths and SHA256 hashes are in stale-screen-diagnosis.json.

TextLayout's report shows the same sequential incomplete route; its preserved final screen artifact belongs to a later target, so this diagnosis does not claim a retained TextLayout candidate binary. The route-level defect applies, but that additional artifact proof is UNKNOWN. C4 reports six incomplete compiler invocations across five methods; those counts are invocations, not six proved candidate-source emissions.

## IL readings and why no new case rotation is proposed

Original ResourceManager.LoadStart at657–659 loads selector8 and brtrue.s725, emits case0, then br855; at725–728 it tests case1 before case2. Original TextLayout similarly starts selector26/brtrue.s762 and emits sequential tests1,2,... with explicit join transfers. TextFormat starts selector2/brtrue.s93, emits case0/br264, tests1||2, and later tests3. These are compatible with the zero-first if chains already constructed by the existing rule. The original inner character dispatch of TextFormat has '+','-','*','/' tests in that exact source order; C4's inner variant follows that order. A new permutation rule would repeat an untested existing hypothesis without independent evidence.

The first-branch raw prefixes (ResourceManager431, TextLayout316, TextFormat49) did not move because the baseline target body was recompiled; these measurements cannot diagnose a compiler-version or recovered-source failure. Whole-assembly metadata equality remains a legitimate baseline measurement, but it says nothing about whether a source variant was supplied.

## Independent real-compiler regression fixture

Run `python tools/test_round8_sequential_screen.py`. It uses the pinned Roslyn1.1.0.51109 SHA53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3 and independent toy C# only. Five toy compilations demonstrate:

1. Baseline Target returns1; another method is stubbed by a source_filter.
2. Main candidate is changed to return2, but compile.rsp selects stale target_screen: the emitted Target remains completely equal to baseline and contains ldc.i4.1.
3. Synchronizing source_filter(candidate) into the selected target_screen emits ldc.i4.2 and a different target body.
4. The direct candidate source path emits the same target body as the corrected screen path.
5. Explicitly restoring both paths before the next baseline check restores complete equality with the original toy baseline.

The fixture records hashes of the actual selected source and the main source per invocation. Zero game compilations, no body-oracle changes, no normalized IL, no copied original bytes. Suggested correction is to synchronize the selected source immediately before each sequential build and audit its hash. Existing source-model, compiled-signature, prefix and full-verifier gates must remain unchanged. Re-run the existing family after that implementation fix; do not label these C4 outcomes source-hypothesis refutations.

## Correção compartilhada e ciclo científico

A fixture final usa `tools/round8_candidate_sources.py::write_candidate_sources` nos caminhos sincronizado, direto e reset. Testa ainda preservação exata de CRLF/Unicode e rejeição de filtro com o mesmo caminho do fonte completo. O caminho stale é reproduzido deliberadamente sem fornecer a alternativa ao escritor. A implementação compartilhada foi criada pelo agente principal; esta auditoria não altera o engine nem o oráculo.

Os relatórios originais C4/C5 registram zero etapas aceitas, zero fontes adotados, zero ganhos e zero perdas, com verificação oficial concluída. Isso permite ao processo do agente principal arquivar as experiências inválidas preservando hashes/recibos, commits e bytes dos ledgers e repetir as famílias, sem editar estados manualmente ou reescrever Git. A auditoria apenas registra evidência; não executa esse arquivamento. Os paths de C4/C5 abaixo são os observados na captura e podem passar a nomes initial-screen no arquivo histórico; os hashes vinculam os mesmos bytes.
