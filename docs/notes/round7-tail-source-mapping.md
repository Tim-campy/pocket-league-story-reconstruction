# Cycle14: wrong source selection after a do/while tail

CONFIRMED implementation bug; the IL/source hypothesis is not refuted. The sequence alignment identifies an inserted original `br` after the conditional. On the rebuilt side that position can already belong to the next source statement. Selecting the latest preceding MDB point at that position can therefore select the next do/while or no loop at all.

## Pure correction

`tools/round7_tail_source_mapping.py` exports `tail_source_mapping_offset(alignment_rebuilt_offset,hints,compiler='smcs') -> dict`. Use `result['new_offset']` before any `select_point` call, including the accepted-symbol cache path. Calculate the independent `infinite_tail_guard_hints` before this source selection. If `original_infinite_tail_guard is True` and its `infinite_tail_rebuilt_offset` is a nonnegative integer not later than the alignment offset, use that proved conditional's offset. Otherwise preserve the input offset. Roslyn is unchanged.

The returned diagnostic records `old_offset`, `new_offset`, `overridden`, `reason`, and `oracle_changed:false`. `tail_source_mapping_record(result,selected_point)` appends the actual selected sequence-point offset/document/line/column. This module does not import a compiler, mapper, model, verifier, IL mutator, or ledger. It changes only the source-location lookup; bodies, alignment, prefix metric, equality oracle and zero-regression policy stay unchanged.

## Reproduction with a real MDB

Run `python reports/round7-tail-source-mapping-fixture-driver.py`. Independently written C# has two successive do loops, guarded by separate if statements, matching the game failure's shape. The Unity smcs compiled it with debug enabled and disabled; full method-body equality was checked. MDB lookup at IL19 selects line9 (the second if), and the transform proposes changing the second do. The proved conditional is at IL14; selecting there returns the point at IL8/line6 inside the first do. The transform then proposes changing the first do, and the complete compiled candidate body equals the independently written reference body (50 IL bytes).

Three toy library compilations, zero game compilations. Six pure negative controls preserve the input for missing/false evidence, a future offset, negative offset, Boolean offset, or Roslyn. An already-correct conditional offset is idempotent. All controls passed. An initial fixture assertion incorrectly compared method signatures of `Pair` and `ExpectedPair` and failed only on their deliberately different names; it was replaced by the existing explicit full-body facet comparison. The emitted bodies were already equal; no oracle used for the game was changed.

## Read-only confirmation on Cycle14 game artifacts

`reports/round7-tail-source-mapping-game-readonly.json` reads the existing Cycle14 debug DLL/MDB, checks its bodies against the plain accepted dump, and records DLL/MDB hashes before and after. Both files stayed unchanged. The isolated syntax helper produced proposals in memory only.

| Method | Alignment offset | Corrected mapping offset | Old/new selected MDB offsets | Result |
|---|---:|---:|---|---|
| DebugForm.Update_gameParam | 11628 | 11623 | 11628 / 11574 | Old proposal changes PLAYER_FACE++; corrected proposal changes the preceding PLAYER_FACE-- loop. |
| AppData.Draw_helpMessage | 300 | 295 | 300 / 289 | Old selection yields zero do candidates; corrected selection yields the intended do candidate. |
| PlayerData.AddExp | 40 | 40 | 40 / 40 | Earlier unrelated difference remains; no tail evidence at this frontier, so no override or candidate is produced. |

All three reused debug bodies equal their accepted plain counterparts. This diagnosis explains Cycle14's negative trials without treating the source hypothesis as refuted, renaming it, or changing any matched status. Whether corrected mapping yields new game matches remains for the parent's normal compilation and full verification.
