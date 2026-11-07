# Round 9 D38: whole reference/integer guard exits

Baseline D37 HEAD62148ff5dd0813c7e5a25de1c351bb14ceb9241c. Fresh weighted histogram reports/round9-cycle37-after-measurement.json. Three targets total1,130 IL: SurfaceManagerBase.Draw955, SaveRecorder.Unlock117, Offscreen.LateUpdate58. Main covers all51 pending Kairo bodies. Game outcomes UNKNOWN before oracle.

Surface converts only the method-root combined negative debug/int count guard into positive DEBUG && Size>0 && Count>0 around the entire remaining tail. Prefix rendering, local names and copied tail/finally remain. Unlock similarly puts the forced-save loop inside positive forced, preserving preceding lock-count operations and all catch bodies. Neither adds a named local or changes a floating relation.

Offscreen uses TWO explicit early-return sites: image==null OR dispose returns; then null GetLayout returns; unchanged four-out-local/delegate/Resize payload follows. The prior compiled combined-three-term OR guard and nested positive-AND forms failed and remain historical negatives. The new two-statement source is distinct, not renamed repetition. The API requires the positive branch to be the sole existing method statement and copies exactly the declared payload; no hidden effect is discarded.

Three full header/locals/IL/EH fixture controls and nine negatives passed in nine own builds, without failures. Heavy Surface tail retains finally; Unlock retains catch; Offscreen retains named delegate/out types. Reader source templates were not tested before this executor ran them. Evidence reports/round9-whole-guard-fixtures.json; fresh source/dependency/model payload reports/round9-whole-guard-patches.json.

Source models: Surface INCOMPLETE15 original types equal; Unlock INCOMPLETE2 equal; Offscreen COMPLETE4 equal. No complete flag is promoted; actual signatures15/2/4 and full body mandatory, unchanged12-incomplete budget. Pinned Roslyn4cb203dab89e1542cdf7d88cb6d32767bc0e5460 EmitStatement.cs200-268 condition statements and648-728 direct/indirect return handling support distinct return/join emission. Integer count comparisons and reference null tests retain their types and side-effect order; no float complement.

REA primary: parent ev_d050d2a8b24669939f3d55d97be63a783ba0e2bb98868f91ff5ae5aadbdc2d9b, resume-20261107/rea-surface-draw-method.json060006e8/392decoded/1EH and rea-save-unlock-method.json060004c2/62/1EH. Offscreen06000932/27decoded/0EH, existing parent ev_22df527da74456521ff5b8507edf30502801409504d7275bc4038e075baa1c06. Original hashes unchanged; no raw assembly repair, manual generated type or state edit. Full verification precedes commit.

D38 full result: all three matched, +3/1130IL after51/51 pending Kairo bodies; actual15/2/4 signatures passed, zero regressions. One whitespace-only blank line cleaned with preserved line count and repeated full verification. Commit0f93d3bf9e723db64da4055685e6296dc69753d8 pushed after staged proof.
