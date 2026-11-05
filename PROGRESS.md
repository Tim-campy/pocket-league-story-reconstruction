# Progresso dos métodos C#

Gerado por `python tools/progress` a partir de `config/methods.csv` e `config/types.csv`.

**4,755 de 5,245 métodos matched (90.66%).** 871,497 bytes de IL nos assemblies alvo.
363,493 bytes IL matched (41.71%).
Cabeçalhos, locais e exceções também precisam corresponder; o denominador acima mede somente bytes IL.

Ambos os assemblies foram recuperados e compilam integralmente. KairoLibrary usa o candidato Roslyn 1.1.0; Assembly-CSharp usa smcs 2.6.5.0.

KairoLibrary: 3070 corpos correspondem com tokens resolvidos; 2 ficam retidos por dependências de membros gerados. Declarações sem CIL só entram em matched com prova completa dos metadados do método e do tipo.

## Estados

| Estado | Métodos |
|---|---:|
| todo | 1 |
| decompiled | 489 |
| equivalent | 0 |
| matched | 4755 |
| external | 0 |

## Assemblies

| Grupo | Métodos | Bytes IL | Métodos matched | Bytes IL matched |
|---|---:|---:|---:|---:|
| `Assembly-CSharp` | 1958 | 601603 | 1611 / 82.28% | 170582 / 28.35% |
| `KairoLibrary` | 3287 | 269894 | 3144 / 95.65% | 192911 / 71.48% |

## Namespaces

| Grupo | Métodos | Bytes IL | Métodos matched | Bytes IL matched |
|---|---:|---:|---:|---:|
| `(global)` | 2 | 14 | 1 / 50.00% | 7 / 50.00% |
| `BuildInfos` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `UnityEngine.Purchasing` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `analytics` | 4 | 93 | 4 / 100.00% | 93 / 100.00% |
| `cfg` | 2 | 348 | 1 / 50.00% | 7 / 2.01% |
| `data` | 255 | 26353 | 234 / 91.76% | 19709 / 74.79% |
| `form` | 371 | 250649 | 280 / 75.47% | 38237 / 15.26% |
| `game` | 441 | 202949 | 326 / 73.92% | 60079 / 29.60% |
| `java.io` | 89 | 2223 | 89 / 100.00% | 2223 / 100.00% |
| `java.lang` | 171 | 4928 | 169 / 98.83% | 4898 / 99.39% |
| `java.native` | 34 | 13621 | 33 / 97.06% | 13481 / 98.97% |
| `java.net` | 5 | 502 | 5 / 100.00% | 502 / 100.00% |
| `java.util` | 107 | 3643 | 107 / 100.00% | 3643 / 100.00% |
| `kairo.common.cfg` | 7 | 2180 | 7 / 100.00% | 2180 / 100.00% |
| `kairo.unity.ad` | 15 | 837 | 13 / 86.67% | 641 / 76.58% |
| `kairo.unity.analytics` | 15 | 1284 | 15 / 100.00% | 1284 / 100.00% |
| `kairo.unity.form` | 88 | 6284 | 86 / 97.73% | 4512 / 71.80% |
| `kairo.unity.graph` | 7 | 680 | 6 / 85.71% | 583 / 85.74% |
| `kairo.unity.graphics` | 195 | 21048 | 181 / 92.82% | 14426 / 68.54% |
| `kairo.unity.iOSsupport` | 4 | 18 | 4 / 100.00% | 18 / 100.00% |
| `kairo.unity.io` | 149 | 9866 | 138 / 92.62% | 6705 / 67.96% |
| `kairo.unity.licensing` | 7 | 101 | 7 / 100.00% | 101 / 100.00% |
| `kairo.unity.m3d4.util3d` | 67 | 2810 | 64 / 95.52% | 2533 / 90.14% |
| `kairo.unity.math` | 57 | 5195 | 53 / 92.98% | 4742 / 91.28% |
| `kairo.unity.native` | 212 | 10404 | 208 / 98.11% | 9594 / 92.21% |
| `kairo.unity.native.util` | 16 | 1601 | 16 / 100.00% | 1601 / 100.00% |
| `kairo.unity.panel` | 37 | 765 | 36 / 97.30% | 719 / 93.99% |
| `kairo.unity.surface` | 187 | 14973 | 182 / 97.33% | 9820 / 65.58% |
| `kairo.unity.system` | 193 | 16116 | 182 / 94.30% | 10643 / 66.04% |
| `kairo.unity.system.appli` | 2 | 26 | 2 / 100.00% | 26 / 100.00% |
| `kairo.unity.system.lineup.ui` | 23 | 2607 | 21 / 91.30% | 1356 / 52.01% |
| `kairo.unity.system.lobi` | 7 | 249 | 7 / 100.00% | 249 / 100.00% |
| `kairo.unity.system.notification.ui` | 24 | 911 | 23 / 95.83% | 818 / 89.79% |
| `kairo.unity.system.payment` | 14 | 752 | 14 / 100.00% | 752 / 100.00% |
| `kairo.unity.system.tapjoy.ui` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `kairo.unity.ui` | 1021 | 100597 | 962 / 94.22% | 60303 / 59.95% |
| `kairo.unity.util` | 518 | 43955 | 499 / 96.33% | 33577 / 76.39% |
| `kfw.panel` | 15 | 1718 | 14 / 93.33% | 981 / 57.10% |
| `main` | 223 | 35448 | 189 / 84.75% | 15714 / 44.33% |
| `native` | 26 | 896 | 26 / 100.00% | 896 / 100.00% |
| `panel` | 5 | 49 | 5 / 100.00% | 49 / 100.00% |
| `surface` | 122 | 26518 | 92 / 75.41% | 10558 / 39.81% |
| `system` | 20 | 1636 | 19 / 95.00% | 1036 / 63.33% |
| `system.billing` | 8 | 956 | 6 / 75.00% | 688 / 71.97% |
| `system.billing.ui` | 153 | 13782 | 132 / 86.27% | 6870 / 49.85% |
| `system.billing.util` | 45 | 1788 | 43 / 95.56% | 1328 / 74.27% |
| `system.form` | 192 | 33269 | 168 / 87.50% | 10365 / 31.16% |
| `system.friend` | 4 | 570 | 3 / 75.00% | 284 / 49.82% |
| `system.friend.ui` | 47 | 4407 | 45 / 95.74% | 3175 / 72.04% |
| `system.lineup` | 3 | 46 | 3 / 100.00% | 46 / 100.00% |
| `system.notification` | 4 | 89 | 4 / 100.00% | 89 / 100.00% |
| `system.panel` | 4 | 38 | 4 / 100.00% | 38 / 100.00% |
| `system.surface` | 10 | 1289 | 9 / 90.00% | 898 / 69.67% |
| `system.tapjoy` | 2 | 55 | 2 / 100.00% | 55 / 100.00% |
| `system.tapjoy.ui` | 13 | 347 | 13 / 100.00% | 347 / 100.00% |

## Tipos

| Grupo | Métodos | Bytes IL | Métodos matched | Bytes IL matched |
|---|---:|---:|---:|---:|
| `Assembly-CSharp::<PrivateImplementationDetails>` | 1 | 7 | 0 / 0.00% | 0 / 0.00% |
| `Assembly-CSharp::BuildInfos.BuildInfo` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::TJPlacement` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::UnityEngine.Purchasing.Product` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::analytics.AnalyticsTracker` | 4 | 93 | 4 / 100.00% | 93 / 100.00% |
| `Assembly-CSharp::cfg.MyConfig` | 2 | 348 | 1 / 50.00% | 7 / 2.01% |
| `Assembly-CSharp::data.BaseData` | 2 | 22 | 2 / 100.00% | 22 / 100.00% |
| `Assembly-CSharp::data.EventData` | 7 | 342 | 7 / 100.00% | 342 / 100.00% |
| `Assembly-CSharp::data.FanActData` | 23 | 2141 | 21 / 91.30% | 1964 / 91.73% |
| `Assembly-CSharp::data.FanBonusData` | 15 | 4141 | 12 / 80.00% | 969 / 23.40% |
| `Assembly-CSharp::data.FormationData` | 12 | 796 | 12 / 100.00% | 796 / 100.00% |
| `Assembly-CSharp::data.LeagueData` | 17 | 1388 | 14 / 82.35% | 972 / 70.03% |
| `Assembly-CSharp::data.MapchipMain` | 3 | 179 | 3 / 100.00% | 179 / 100.00% |
| `Assembly-CSharp::data.MapchipMatch` | 3 | 161 | 3 / 100.00% | 161 / 100.00% |
| `Assembly-CSharp::data.PlayerData` | 55 | 6370 | 50 / 90.91% | 5356 / 84.08% |
| `Assembly-CSharp::data.QuestMatchData` | 16 | 1254 | 16 / 100.00% | 1254 / 100.00% |
| `Assembly-CSharp::data.SponsorData` | 16 | 1288 | 15 / 93.75% | 883 / 68.56% |
| `Assembly-CSharp::data.StadiumData` | 6 | 177 | 6 / 100.00% | 177 / 100.00% |
| `Assembly-CSharp::data.SupervisorData` | 15 | 1869 | 14 / 93.33% | 1430 / 76.51% |
| `Assembly-CSharp::data.TeamData` | 12 | 1963 | 10 / 83.33% | 1310 / 66.73% |
| `Assembly-CSharp::data.TenantData` | 26 | 2743 | 24 / 92.31% | 2558 / 93.26% |
| `Assembly-CSharp::data.TradeData` | 13 | 757 | 11 / 84.62% | 574 / 75.83% |
| `Assembly-CSharp::data.TrainingData` | 14 | 762 | 14 / 100.00% | 762 / 100.00% |
| `Assembly-CSharp::form.BootForm` | 21 | 7712 | 16 / 76.19% | 2628 / 34.08% |
| `Assembly-CSharp::form.DebugForm` | 21 | 45497 | 11 / 52.38% | 5156 / 11.33% |
| `Assembly-CSharp::form.FormManager` | 13 | 577 | 13 / 100.00% | 577 / 100.00% |
| `Assembly-CSharp::form.GameForm` | 51 | 16474 | 40 / 78.43% | 6437 / 39.07% |
| `Assembly-CSharp::form.MyFormBase` | 48 | 3906 | 46 / 95.83% | 2178 / 55.76% |
| `Assembly-CSharp::form.RankForm` | 14 | 1516 | 13 / 92.86% | 1269 / 83.71% |
| `Assembly-CSharp::form.SoccerForm` | 62 | 33639 | 42 / 67.74% | 5508 / 16.37% |
| `Assembly-CSharp::form.SpForm` | 19 | 7256 | 11 / 57.89% | 1599 / 22.04% |
| `Assembly-CSharp::form.SubForm` | 95 | 125345 | 68 / 71.58% | 9489 / 7.57% |
| `Assembly-CSharp::form.SubForm+<Update2>c__AnonStorey0` | 3 | 227 | 2 / 66.67% | 31 / 13.66% |
| `Assembly-CSharp::form.TitleForm` | 24 | 8500 | 18 / 75.00% | 3365 / 39.59% |
| `Assembly-CSharp::game.Ball` | 18 | 15652 | 11 / 61.11% | 1954 / 12.48% |
| `Assembly-CSharp::game.Camera` | 31 | 7399 | 26 / 83.87% | 3132 / 42.33% |
| `Assembly-CSharp::game.DelayEvent` | 4 | 456 | 3 / 75.00% | 286 / 62.72% |
| `Assembly-CSharp::game.GameManager` | 20 | 11549 | 14 / 70.00% | 3181 / 27.54% |
| `Assembly-CSharp::game.GameUtil` | 47 | 3570 | 43 / 91.49% | 3099 / 86.81% |
| `Assembly-CSharp::game.Map` | 18 | 13729 | 9 / 50.00% | 3335 / 24.29% |
| `Assembly-CSharp::game.MapMain` | 14 | 7415 | 8 / 57.14% | 1866 / 25.17% |
| `Assembly-CSharp::game.MapObject` | 2 | 46 | 2 / 100.00% | 46 / 100.00% |
| `Assembly-CSharp::game.Player` | 63 | 32346 | 48 / 76.19% | 11483 / 35.50% |
| `Assembly-CSharp::game.TArea` | 14 | 4070 | 10 / 71.43% | 1003 / 24.64% |
| `Assembly-CSharp::game.TBall` | 14 | 5087 | 12 / 85.71% | 1988 / 39.08% |
| `Assembly-CSharp::game.TGroupArea` | 14 | 7524 | 7 / 50.00% | 1031 / 13.70% |
| `Assembly-CSharp::game.TPlayer` | 34 | 13693 | 32 / 94.12% | 3701 / 27.03% |
| `Assembly-CSharp::game.Team` | 42 | 22251 | 25 / 59.52% | 9984 / 44.87% |
| `Assembly-CSharp::game.Tenant` | 14 | 2692 | 11 / 78.57% | 792 / 29.42% |
| `Assembly-CSharp::game.UserData` | 92 | 55470 | 65 / 70.65% | 13198 / 23.79% |
| `Assembly-CSharp::main.AppData` | 201 | 33806 | 167 / 83.08% | 14072 / 41.63% |
| `Assembly-CSharp::main.AppData+<DrawFukidashiKimochi>c__AnonStorey1` | 2 | 126 | 2 / 100.00% | 126 / 100.00% |
| `Assembly-CSharp::main.AppData+stringErrorCode` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::main.Main` | 11 | 1287 | 11 / 100.00% | 1287 / 100.00% |
| `Assembly-CSharp::main.WebBox` | 8 | 222 | 8 / 100.00% | 222 / 100.00% |
| `Assembly-CSharp::native.AppPlugin` | 6 | 245 | 6 / 100.00% | 245 / 100.00% |
| `Assembly-CSharp::native.AppPlugin+<LaunchMailer>c__AnonStorey3` | 2 | 47 | 2 / 100.00% | 47 / 100.00% |
| `Assembly-CSharp::native.AppPlugin+<ShowPurchaseLog>c__AnonStorey2` | 2 | 80 | 2 / 100.00% | 80 / 100.00% |
| `Assembly-CSharp::native.WebBoxPlugin` | 10 | 358 | 10 / 100.00% | 358 / 100.00% |
| `Assembly-CSharp::native.WebBoxPlugin+<LoadUrl>c__AnonStorey5` | 2 | 38 | 2 / 100.00% | 38 / 100.00% |
| `Assembly-CSharp::native.WebBoxPlugin+<SetViewRect>c__AnonStorey4` | 2 | 85 | 2 / 100.00% | 85 / 100.00% |
| `Assembly-CSharp::native.WebBoxPlugin+<SetVisible>c__AnonStorey6` | 2 | 43 | 2 / 100.00% | 43 / 100.00% |
| `Assembly-CSharp::panel.MainPanel` | 5 | 49 | 5 / 100.00% | 49 / 100.00% |
| `Assembly-CSharp::surface.GamePad` | 29 | 9686 | 23 / 79.31% | 3271 / 33.77% |
| `Assembly-CSharp::surface.GameView` | 66 | 13578 | 45 / 68.18% | 5801 / 42.72% |
| `Assembly-CSharp::surface.SurfaceManager` | 8 | 1729 | 7 / 87.50% | 430 / 24.87% |
| `Assembly-CSharp::surface.TouchEffect` | 5 | 821 | 4 / 80.00% | 472 / 57.49% |
| `Assembly-CSharp::surface.TouchEffectManager` | 14 | 704 | 13 / 92.86% | 584 / 82.95% |
| `Assembly-CSharp::system.KairoService` | 15 | 1353 | 14 / 93.33% | 753 / 55.65% |
| `Assembly-CSharp::system.KairoText` | 5 | 283 | 5 / 100.00% | 283 / 100.00% |
| `Assembly-CSharp::system.billing.MyBilling` | 8 | 956 | 6 / 75.00% | 688 / 71.97% |
| `Assembly-CSharp::system.billing.ui.ApplyTask` | 12 | 1507 | 11 / 91.67% | 603 / 40.01% |
| `Assembly-CSharp::system.billing.ui.BillingItem` | 26 | 1007 | 25 / 96.15% | 468 / 46.47% |
| `Assembly-CSharp::system.billing.ui.BillingManager` | 44 | 6863 | 36 / 81.82% | 3024 / 44.06% |
| `Assembly-CSharp::system.billing.ui.PurchaseLogView` | 3 | 130 | 3 / 100.00% | 130 / 100.00% |
| `Assembly-CSharp::system.billing.ui.PurchaseRecord` | 12 | 1155 | 11 / 91.67% | 1069 / 92.55% |
| `Assembly-CSharp::system.billing.ui.Receipt` | 26 | 932 | 26 / 100.00% | 932 / 100.00% |
| `Assembly-CSharp::system.billing.ui.StoreBase` | 28 | 2159 | 18 / 64.29% | 615 / 28.49% |
| `Assembly-CSharp::system.billing.ui.StoreBase+<SetRestoreRequest>c__AnonStorey7` | 2 | 29 | 2 / 100.00% | 29 / 100.00% |
| `Assembly-CSharp::system.billing.util.IabHelper` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::system.billing.util.Inventory` | 11 | 288 | 11 / 100.00% | 288 / 100.00% |
| `Assembly-CSharp::system.billing.util.Json` | 7 | 672 | 5 / 71.43% | 212 / 31.55% |
| `Assembly-CSharp::system.billing.util.Purchase` | 15 | 527 | 15 / 100.00% | 527 / 100.00% |
| `Assembly-CSharp::system.billing.util.SkuDetails` | 11 | 294 | 11 / 100.00% | 294 / 100.00% |
| `Assembly-CSharp::system.form.BillingForm` | 42 | 7424 | 36 / 85.71% | 2145 / 28.89% |
| `Assembly-CSharp::system.form.ConnectForm` | 11 | 400 | 11 / 100.00% | 400 / 100.00% |
| `Assembly-CSharp::system.form.FriendForm` | 53 | 17764 | 46 / 86.79% | 3528 / 19.86% |
| `Assembly-CSharp::system.form.HttpForm` | 16 | 894 | 16 / 100.00% | 894 / 100.00% |
| `Assembly-CSharp::system.form.HttpForm+HttpMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `Assembly-CSharp::system.form.HttpForm+ResultData` | 11 | 132 | 11 / 100.00% | 132 / 100.00% |
| `Assembly-CSharp::system.form.LanguageForm` | 13 | 1452 | 8 / 61.54% | 362 / 24.93% |
| `Assembly-CSharp::system.form.LineupForm` | 15 | 1711 | 12 / 80.00% | 1387 / 81.06% |
| `Assembly-CSharp::system.form.MatchForm` | 19 | 3263 | 16 / 84.21% | 1288 / 39.47% |
| `Assembly-CSharp::system.form.MatchForm+UserRecord` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::system.form.RestoreForm` | 7 | 222 | 7 / 100.00% | 222 / 100.00% |
| `Assembly-CSharp::system.friend.MyFriend` | 4 | 570 | 3 / 75.00% | 284 / 49.82% |
| `Assembly-CSharp::system.friend.ui.Friend` | 6 | 661 | 6 / 100.00% | 661 / 100.00% |
| `Assembly-CSharp::system.friend.ui.FriendData` | 37 | 3548 | 35 / 94.59% | 2316 / 65.28% |
| `Assembly-CSharp::system.friend.ui.FriendPlayTimeData` | 4 | 198 | 4 / 100.00% | 198 / 100.00% |
| `Assembly-CSharp::system.lineup.MyLineup` | 3 | 46 | 3 / 100.00% | 46 / 100.00% |
| `Assembly-CSharp::system.notification.MyNotify` | 4 | 89 | 4 / 100.00% | 89 / 100.00% |
| `Assembly-CSharp::system.panel.InputPanel` | 4 | 38 | 4 / 100.00% | 38 / 100.00% |
| `Assembly-CSharp::system.surface.GameBanner` | 10 | 1289 | 9 / 90.00% | 898 / 69.67% |
| `Assembly-CSharp::system.tapjoy.MyTapjoy` | 2 | 55 | 2 / 100.00% | 55 / 100.00% |
| `Assembly-CSharp::system.tapjoy.ui.Tapjoy` | 13 | 347 | 13 / 100.00% | 347 / 100.00% |
| `KairoLibrary::java.io.ByteArrayInputStream` | 3 | 67 | 3 / 100.00% | 67 / 100.00% |
| `KairoLibrary::java.io.ByteArrayOutputStream` | 7 | 101 | 7 / 100.00% | 101 / 100.00% |
| `KairoLibrary::java.io.DataInputStream` | 17 | 400 | 17 / 100.00% | 400 / 100.00% |
| `KairoLibrary::java.io.DataOutputStream` | 13 | 348 | 13 / 100.00% | 348 / 100.00% |
| `KairoLibrary::java.io.EOFException` | 1 | 8 | 1 / 100.00% | 8 / 100.00% |
| `KairoLibrary::java.io.InputStream` | 18 | 604 | 18 / 100.00% | 604 / 100.00% |
| `KairoLibrary::java.io.OutputStream` | 11 | 249 | 11 / 100.00% | 249 / 100.00% |
| `KairoLibrary::java.io.WrappedSystemStream` | 19 | 446 | 19 / 100.00% | 446 / 100.00% |
| `KairoLibrary::java.lang.JBoolean` | 5 | 78 | 5 / 100.00% | 78 / 100.00% |
| `KairoLibrary::java.lang.JByte` | 9 | 123 | 9 / 100.00% | 123 / 100.00% |
| `KairoLibrary::java.lang.JCharacter` | 12 | 127 | 12 / 100.00% | 127 / 100.00% |
| `KairoLibrary::java.lang.JDouble` | 14 | 157 | 14 / 100.00% | 157 / 100.00% |
| `KairoLibrary::java.lang.JFloat` | 15 | 172 | 15 / 100.00% | 172 / 100.00% |
| `KairoLibrary::java.lang.JInteger` | 16 | 228 | 16 / 100.00% | 228 / 100.00% |
| `KairoLibrary::java.lang.JLong` | 11 | 179 | 11 / 100.00% | 179 / 100.00% |
| `KairoLibrary::java.lang.JMath` | 20 | 199 | 18 / 90.00% | 169 / 84.92% |
| `KairoLibrary::java.lang.JRunnable` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::java.lang.JRuntime` | 6 | 56 | 6 / 100.00% | 56 / 100.00% |
| `KairoLibrary::java.lang.JShort` | 9 | 123 | 9 / 100.00% | 123 / 100.00% |
| `KairoLibrary::java.lang.JString` | 18 | 2685 | 18 / 100.00% | 2685 / 100.00% |
| `KairoLibrary::java.lang.JSystem` | 5 | 118 | 5 / 100.00% | 118 / 100.00% |
| `KairoLibrary::java.lang.JThread` | 16 | 484 | 16 / 100.00% | 484 / 100.00% |
| `KairoLibrary::java.lang.StringEx` | 14 | 199 | 14 / 100.00% | 199 / 100.00% |
| `KairoLibrary::java.native.JavaPlugin` | 8 | 12561 | 8 / 100.00% | 12561 / 100.00% |
| `KairoLibrary::java.native.JavaPlugin+<>c` | 6 | 407 | 6 / 100.00% | 407 / 100.00% |
| `KairoLibrary::java.native.JavaPlugin+<>c__DisplayClass8_0` | 2 | 147 | 1 / 50.00% | 7 / 4.76% |
| `KairoLibrary::java.native.PluginBase` | 6 | 312 | 6 / 100.00% | 312 / 100.00% |
| `KairoLibrary::java.native.PluginBase+<>c__DisplayClass0_0` | 2 | 31 | 2 / 100.00% | 31 / 100.00% |
| `KairoLibrary::java.native.PluginBase+<>c__DisplayClass1_0` | 2 | 31 | 2 / 100.00% | 31 / 100.00% |
| `KairoLibrary::java.native.PluginBase+<>c__DisplayClass2_0`1` | 2 | 36 | 2 / 100.00% | 36 / 100.00% |
| `KairoLibrary::java.native.PluginBase+<>c__DisplayClass3_0`1` | 2 | 36 | 2 / 100.00% | 36 / 100.00% |
| `KairoLibrary::java.native.PluginBase+<>c__DisplayClass4_0`1` | 2 | 30 | 2 / 100.00% | 30 / 100.00% |
| `KairoLibrary::java.native.PluginBase+<>c__DisplayClass5_0`1` | 2 | 30 | 2 / 100.00% | 30 / 100.00% |
| `KairoLibrary::java.net.URLDecoder` | 2 | 238 | 2 / 100.00% | 238 / 100.00% |
| `KairoLibrary::java.net.URLEncoder` | 3 | 264 | 3 / 100.00% | 264 / 100.00% |
| `KairoLibrary::java.util.IVector` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::java.util.JCalendar` | 21 | 1056 | 21 / 100.00% | 1056 / 100.00% |
| `KairoLibrary::java.util.JDate` | 11 | 299 | 11 / 100.00% | 299 / 100.00% |
| `KairoLibrary::java.util.JLocale` | 10 | 654 | 10 / 100.00% | 654 / 100.00% |
| `KairoLibrary::java.util.JRandom` | 6 | 85 | 6 / 100.00% | 85 / 100.00% |
| `KairoLibrary::java.util.JTimeZone` | 6 | 252 | 6 / 100.00% | 252 / 100.00% |
| `KairoLibrary::java.util.JTool` | 2 | 175 | 2 / 100.00% | 175 / 100.00% |
| `KairoLibrary::java.util.Vector` | 4 | 60 | 4 / 100.00% | 60 / 100.00% |
| `KairoLibrary::java.util.Vector`1` | 39 | 824 | 39 / 100.00% | 824 / 100.00% |
| `KairoLibrary::java.util.Vector`1+<GetEnumerator>d__40` | 7 | 238 | 7 / 100.00% | 238 / 100.00% |
| `KairoLibrary::kairo.common.cfg.Config` | 7 | 2180 | 7 / 100.00% | 2180 / 100.00% |
| `KairoLibrary::kairo.unity.ad.AdMobView` | 7 | 179 | 7 / 100.00% | 179 / 100.00% |
| `KairoLibrary::kairo.unity.ad.InterstitialAd` | 8 | 658 | 6 / 75.00% | 462 / 70.21% |
| `KairoLibrary::kairo.unity.analytics.AnalyticsTracker` | 10 | 1045 | 10 / 100.00% | 1045 / 100.00% |
| `KairoLibrary::kairo.unity.analytics.AnalyticsTracker+<>c` | 3 | 200 | 3 / 100.00% | 200 / 100.00% |
| `KairoLibrary::kairo.unity.analytics.AnalyticsTracker+AnalyticsData` | 2 | 39 | 2 / 100.00% | 39 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormBase` | 24 | 267 | 24 / 100.00% | 267 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase` | 53 | 4432 | 52 / 98.11% | 3344 / 75.45% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+<>c__DisplayClass85_0` | 2 | 691 | 1 / 50.00% | 7 / 1.01% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+<>c__DisplayClass85_1` | 4 | 887 | 4 / 100.00% | 887 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+<>c__DisplayClass85_2` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+OnCaptureCompleted` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graph.Graph` | 7 | 680 | 6 / 85.71% | 583 / 85.74% |
| `KairoLibrary::kairo.unity.graphics.Material` | 13 | 322 | 13 / 100.00% | 322 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MaterialManager` | 11 | 1735 | 7 / 63.64% | 337 / 19.42% |
| `KairoLibrary::kairo.unity.graphics.MeshBuilder` | 31 | 4626 | 27 / 87.10% | 3520 / 76.09% |
| `KairoLibrary::kairo.unity.graphics.MeshBuilder+Group` | 3 | 910 | 3 / 100.00% | 910 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshBuilder+Layer` | 3 | 106 | 3 / 100.00% | 106 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshManager` | 24 | 3389 | 23 / 95.83% | 3248 / 95.84% |
| `KairoLibrary::kairo.unity.graphics.MeshManager+LayerMesh` | 2 | 52 | 2 / 100.00% | 52 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshRecord` | 6 | 300 | 6 / 100.00% | 300 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen` | 29 | 3347 | 27 / 93.10% | 1311 / 39.17% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+<>c__DisplayClass33_0` | 2 | 209 | 2 / 100.00% | 209 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+<>c__DisplayClass34_0` | 2 | 617 | 2 / 100.00% | 617 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+<>c__DisplayClass35_0` | 2 | 638 | 2 / 100.00% | 638 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+GetLayoutMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+MultiRenderMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+RenderMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graphics.Point` | 7 | 137 | 7 / 100.00% | 137 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Rectangle` | 10 | 269 | 10 / 100.00% | 269 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.RectangleF` | 12 | 332 | 12 / 100.00% | 332 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.ShaderBuilder` | 14 | 3664 | 11 / 78.57% | 1723 / 47.03% |
| `KairoLibrary::kairo.unity.graphics.ShaderBuilder+Node` | 8 | 290 | 8 / 100.00% | 290 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.ShaderManager` | 4 | 105 | 4 / 100.00% | 105 / 100.00% |
| `KairoLibrary::kairo.unity.iOSsupport.DeviceUtil` | 2 | 9 | 2 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.iOSsupport.RecordUtil` | 2 | 9 | 2 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.io.AssetReader` | 12 | 1121 | 12 / 100.00% | 1121 / 100.00% |
| `KairoLibrary::kairo.unity.io.AssetReader+<>c` | 3 | 136 | 3 / 100.00% | 136 / 100.00% |
| `KairoLibrary::kairo.unity.io.AssetReader+<>c__DisplayClass14_0` | 2 | 66 | 2 / 100.00% | 66 / 100.00% |
| `KairoLibrary::kairo.unity.io.AssetReader+<>c__DisplayClass17_0` | 2 | 49 | 2 / 100.00% | 49 / 100.00% |
| `KairoLibrary::kairo.unity.io.AssetReader+<>c__DisplayClass18_0` | 2 | 161 | 2 / 100.00% | 161 / 100.00% |
| `KairoLibrary::kairo.unity.io.BitArrayInputStream` | 5 | 175 | 4 / 80.00% | 142 / 81.14% |
| `KairoLibrary::kairo.unity.io.BitArrayOutputStream` | 10 | 278 | 10 / 100.00% | 278 / 100.00% |
| `KairoLibrary::kairo.unity.io.ByteListOutputStream` | 14 | 786 | 14 / 100.00% | 786 / 100.00% |
| `KairoLibrary::kairo.unity.io.ConnectionException` | 2 | 9 | 2 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.io.Http` | 16 | 890 | 16 / 100.00% | 890 / 100.00% |
| `KairoLibrary::kairo.unity.io.Http+HttpThread` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.io.HttpObject` | 17 | 1728 | 15 / 88.24% | 499 / 28.88% |
| `KairoLibrary::kairo.unity.io.HttpObject+<>c__DisplayClass10_0` | 3 | 989 | 2 / 66.67% | 45 / 4.55% |
| `KairoLibrary::kairo.unity.io.HttpObject+<>c__DisplayClass10_1` | 2 | 110 | 2 / 100.00% | 110 / 100.00% |
| `KairoLibrary::kairo.unity.io.RecordStore` | 28 | 1592 | 23 / 82.14% | 992 / 62.31% |
| `KairoLibrary::kairo.unity.io.RecordStoreListener` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.io.ScratchPad` | 11 | 25 | 11 / 100.00% | 25 / 100.00% |
| `KairoLibrary::kairo.unity.io.Storage` | 18 | 1744 | 16 / 88.89% | 1389 / 79.64% |
| `KairoLibrary::kairo.unity.licensing.LicenseChecker` | 7 | 101 | 7 / 100.00% | 101 / 100.00% |
| `KairoLibrary::kairo.unity.m3d4.util3d.FastMath` | 16 | 325 | 15 / 93.75% | 312 / 96.00% |
| `KairoLibrary::kairo.unity.m3d4.util3d.Transform` | 27 | 1710 | 25 / 92.59% | 1446 / 84.56% |
| `KairoLibrary::kairo.unity.m3d4.util3d.Vector3D` | 24 | 775 | 24 / 100.00% | 775 / 100.00% |
| `KairoLibrary::kairo.unity.math.KairoMath` | 57 | 5195 | 53 / 92.98% | 4742 / 91.28% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin` | 14 | 566 | 14 / 100.00% | 566 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass10_0` | 2 | 24 | 2 / 100.00% | 24 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass12_0` | 2 | 24 | 2 / 100.00% | 24 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass14_0` | 2 | 47 | 2 / 100.00% | 47 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass15_0` | 2 | 35 | 2 / 100.00% | 35 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass16_0` | 2 | 30 | 2 / 100.00% | 30 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass7_0` | 2 | 80 | 2 / 100.00% | 80 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass8_0` | 2 | 70 | 2 / 100.00% | 70 / 100.00% |
| `KairoLibrary::kairo.unity.native.AdMobPlugin+<>c__DisplayClass9_0` | 2 | 45 | 2 / 100.00% | 45 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin` | 91 | 5107 | 90 / 98.90% | 4979 / 97.49% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c` | 33 | 993 | 33 / 100.00% | 993 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass103_0` | 2 | 58 | 2 / 100.00% | 58 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass105_0` | 2 | 58 | 2 / 100.00% | 58 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass108_0` | 2 | 53 | 2 / 100.00% | 53 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass109_0` | 2 | 65 | 2 / 100.00% | 65 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass10_0` | 2 | 81 | 2 / 100.00% | 81 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass111_0` | 2 | 90 | 2 / 100.00% | 90 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass114_0` | 2 | 75 | 2 / 100.00% | 75 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass115_0` | 2 | 80 | 2 / 100.00% | 80 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass37_0` | 2 | 69 | 2 / 100.00% | 69 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass53_0` | 2 | 30 | 2 / 100.00% | 30 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass64_0` | 2 | 235 | 2 / 100.00% | 235 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass65_0` | 2 | 134 | 2 / 100.00% | 134 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass66_0` | 2 | 72 | 2 / 100.00% | 72 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass71_0` | 2 | 209 | 2 / 100.00% | 209 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass72_0` | 2 | 108 | 2 / 100.00% | 108 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass75_0` | 2 | 62 | 2 / 100.00% | 62 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass76_0` | 2 | 89 | 1 / 50.00% | 7 / 7.87% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass77_0` | 2 | 53 | 2 / 100.00% | 53 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass79_0` | 2 | 79 | 2 / 100.00% | 79 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass81_0` | 2 | 352 | 2 / 100.00% | 352 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass82_0` | 2 | 304 | 1 / 50.00% | 7 / 2.30% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass85_0` | 2 | 53 | 2 / 100.00% | 53 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass87_0` | 2 | 72 | 2 / 100.00% | 72 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass88_0` | 2 | 95 | 2 / 100.00% | 95 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass89_0` | 2 | 183 | 2 / 100.00% | 183 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass90_0` | 2 | 310 | 1 / 50.00% | 7 / 2.26% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass91_0` | 2 | 158 | 2 / 100.00% | 158 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass97_0` | 2 | 84 | 2 / 100.00% | 84 / 100.00% |
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass98_0` | 2 | 72 | 2 / 100.00% | 72 / 100.00% |
| `KairoLibrary::kairo.unity.native.util.AsyncThread` | 1 | 115 | 1 / 100.00% | 115 / 100.00% |
| `KairoLibrary::kairo.unity.native.util.AsyncThread+<>c__DisplayClass0_0` | 3 | 126 | 3 / 100.00% | 126 / 100.00% |
| `KairoLibrary::kairo.unity.native.util.Dialog` | 1 | 42 | 1 / 100.00% | 42 / 100.00% |
| `KairoLibrary::kairo.unity.native.util.Dialog+<>c__DisplayClass0_0` | 2 | 573 | 2 / 100.00% | 573 / 100.00% |
| `KairoLibrary::kairo.unity.native.util.StringUtil` | 9 | 745 | 9 / 100.00% | 745 / 100.00% |
| `KairoLibrary::kairo.unity.panel.Button` | 5 | 33 | 5 / 100.00% | 33 / 100.00% |
| `KairoLibrary::kairo.unity.panel.Component` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.panel.FepPanel` | 15 | 591 | 14 / 93.33% | 545 / 92.22% |
| `KairoLibrary::kairo.unity.panel.FepPanel+Display` | 3 | 9 | 3 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.panel.Label` | 6 | 49 | 6 / 100.00% | 49 / 100.00% |
| `KairoLibrary::kairo.unity.panel.TextBox` | 7 | 76 | 7 / 100.00% | 76 / 100.00% |
| `KairoLibrary::kairo.unity.surface.MotionEvent` | 15 | 268 | 15 / 100.00% | 268 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase` | 76 | 7799 | 74 / 97.37% | 4175 / 53.53% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase+MotionEventRecord` | 19 | 882 | 19 / 100.00% | 882 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase+MotionEventRecord+MultiTouch` | 9 | 170 | 9 / 100.00% | 170 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase+TouchPoint` | 2 | 76 | 2 / 100.00% | 76 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceManagerBase` | 25 | 3793 | 23 / 92.00% | 2636 / 69.50% |
| `KairoLibrary::kairo.unity.surface.TouchComponent` | 11 | 461 | 11 / 100.00% | 461 / 100.00% |
| `KairoLibrary::kairo.unity.surface.TouchEvent` | 7 | 673 | 6 / 85.71% | 301 / 44.73% |
| `KairoLibrary::kairo.unity.surface.TouchOption` | 21 | 851 | 21 / 100.00% | 851 / 100.00% |
| `KairoLibrary::kairo.unity.surface.TouchValueAccessor` | 2 | 0 | 2 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.system.ErrorCode` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.system.GameDataRecord` | 32 | 2430 | 31 / 96.88% | 2141 / 88.11% |
| `KairoLibrary::kairo.unity.system.GameDataRecord+Data` | 5 | 287 | 5 / 100.00% | 287 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase` | 134 | 12704 | 127 / 94.78% | 7720 / 60.77% |
| `KairoLibrary::kairo.unity.system.KairoBase+<>c` | 4 | 216 | 1 / 25.00% | 16 / 7.41% |
| `KairoLibrary::kairo.unity.system.KairoBase+AccessCount` | 4 | 80 | 4 / 100.00% | 80 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+ConnectObject` | 2 | 69 | 2 / 100.00% | 69 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+ConnectTask` | 3 | 199 | 3 / 100.00% | 199 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+KairoDummy` | 5 | 11 | 5 / 100.00% | 11 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoClub` | 3 | 113 | 3 / 100.00% | 113 / 100.00% |
| `KairoLibrary::kairo.unity.system.appli.GameAppli` | 2 | 26 | 2 / 100.00% | 26 / 100.00% |
| `KairoLibrary::kairo.unity.system.lineup.ui.LineupData` | 9 | 492 | 8 / 88.89% | 300 / 60.98% |
| `KairoLibrary::kairo.unity.system.lineup.ui.LineupManager` | 14 | 2115 | 13 / 92.86% | 1056 / 49.93% |
| `KairoLibrary::kairo.unity.system.lobi.LobiManager` | 7 | 249 | 7 / 100.00% | 249 / 100.00% |
| `KairoLibrary::kairo.unity.system.notification.ui.NotifyManager` | 24 | 911 | 23 / 95.83% | 818 / 89.79% |
| `KairoLibrary::kairo.unity.system.payment.IllegalPurchaseException` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.system.payment.PaymentDataRecord` | 13 | 745 | 13 / 100.00% | 745 / 100.00% |
| `KairoLibrary::kairo.unity.system.tapjoy.ui.ITapjoy` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.AppEvent` | 1 | 41 | 1 / 100.00% | 41 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Balloon` | 27 | 4072 | 26 / 96.30% | 1060 / 26.03% |
| `KairoLibrary::kairo.unity.ui.Balloon+BalloonRender` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.Canvas` | 82 | 8325 | 76 / 92.68% | 5139 / 61.73% |
| `KairoLibrary::kairo.unity.ui.Canvas+<>c` | 3 | 398 | 3 / 100.00% | 398 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Canvas+<>c__DisplayClass107_0` | 2 | 75 | 2 / 100.00% | 75 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Canvas+<>c__DisplayClass152_0` | 2 | 172 | 1 / 50.00% | 7 / 4.07% |
| `KairoLibrary::kairo.unity.ui.DepthInfo` | 5 | 523 | 4 / 80.00% | 277 / 52.96% |
| `KairoLibrary::kairo.unity.ui.Dialog` | 7 | 287 | 7 / 100.00% | 287 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Font` | 45 | 1964 | 45 / 100.00% | 1964 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Font+<>c` | 3 | 24 | 3 / 100.00% | 24 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics` | 227 | 29937 | 209 / 92.07% | 16756 / 55.97% |
| `KairoLibrary::kairo.unity.ui.Graphics+GLText` | 2 | 173 | 2 / 100.00% | 173 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+GrayscaleImage` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+RenderCache` | 2 | 69 | 2 / 100.00% | 69 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+RenderPass` | 11 | 2121 | 8 / 72.73% | 1532 / 72.23% |
| `KairoLibrary::kairo.unity.ui.Graphics+StackState` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Handler` | 5 | 443 | 5 / 100.00% | 443 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Handler+<>c__DisplayClass3_0` | 2 | 19 | 2 / 100.00% | 19 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Handler+PostData` | 1 | 55 | 1 / 100.00% | 55 / 100.00% |
| `KairoLibrary::kairo.unity.ui.IApplication` | 77 | 4239 | 75 / 97.40% | 3927 / 92.64% |
| `KairoLibrary::kairo.unity.ui.IApplication+MethodTask` | 2 | 28 | 2 / 100.00% | 28 / 100.00% |
| `KairoLibrary::kairo.unity.ui.IApplication+UIMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.Image` | 92 | 8726 | 86 / 93.48% | 5411 / 62.01% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass155_0` | 2 | 380 | 1 / 50.00% | 7 / 1.84% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass156_0` | 2 | 42 | 2 / 100.00% | 42 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass157_0` | 2 | 48 | 2 / 100.00% | 48 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass82_0` | 2 | 111 | 2 / 100.00% | 111 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass85_0` | 2 | 86 | 2 / 100.00% | 86 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+BalloonRenderTask` | 1 | 35 | 1 / 100.00% | 35 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+TextRenderTask` | 1 | 46 | 1 / 100.00% | 46 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+TouchTask` | 1 | 161 | 1 / 100.00% | 161 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlas` | 21 | 5410 | 20 / 95.24% | 5270 / 97.41% |
| `KairoLibrary::kairo.unity.ui.ImageAtlas+<>c__DisplayClass15_0` | 2 | 56 | 2 / 100.00% | 56 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlas+AtlasRegion` | 4 | 77 | 4 / 100.00% | 77 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlas+HeightPoint` | 3 | 38 | 3 / 100.00% | 38 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlasManager` | 12 | 466 | 12 / 100.00% | 466 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlasManager+<>c__DisplayClass10_0` | 2 | 118 | 2 / 100.00% | 118 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlasManager+<>c__DisplayClass11_0` | 2 | 61 | 2 / 100.00% | 61 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlasManager+<>c__DisplayClass12_0` | 2 | 49 | 2 / 100.00% | 49 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlasManager+<>c__DisplayClass6_0` | 2 | 334 | 2 / 100.00% | 334 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlasManager+<>c__DisplayClass7_0` | 2 | 137 | 2 / 100.00% | 137 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageTransform` | 16 | 930 | 16 / 100.00% | 930 / 100.00% |
| `KairoLibrary::kairo.unity.ui.KeyEvent` | 4 | 37 | 4 / 100.00% | 37 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Matrix` | 34 | 2139 | 32 / 94.12% | 1841 / 86.07% |
| `KairoLibrary::kairo.unity.ui.Pictograph` | 16 | 199 | 16 / 100.00% | 199 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ResourceManager` | 49 | 3927 | 48 / 97.96% | 2582 / 65.75% |
| `KairoLibrary::kairo.unity.ui.ResourceManager+<>c__DisplayClass51_0` | 3 | 1995 | 2 / 66.67% | 67 / 3.36% |
| `KairoLibrary::kairo.unity.ui.ResourceManager+JarCache` | 1 | 22 | 1 / 100.00% | 22 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder` | 20 | 2773 | 17 / 85.00% | 1010 / 36.42% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+<>c__DisplayClass26_0` | 2 | 147 | 2 / 100.00% | 147 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PackMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PostPackMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PostSaveMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PrePackMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PreSaveMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.Seb` | 40 | 7805 | 34 / 85.00% | 2748 / 35.21% |
| `KairoLibrary::kairo.unity.ui.Seb+WorkStaticValue` | 1 | 90 | 1 / 100.00% | 90 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Seb+WorkValue` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundBase` | 20 | 331 | 20 / 100.00% | 331 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundOGG` | 10 | 345 | 10 / 100.00% | 345 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundOGG+<>c__DisplayClass3_0` | 2 | 26 | 2 / 100.00% | 26 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundOGG+<>c__DisplayClass4_0` | 2 | 390 | 1 / 50.00% | 7 / 1.79% |
| `KairoLibrary::kairo.unity.ui.SoundOGG+<>c__DisplayClass5_0` | 2 | 124 | 2 / 100.00% | 124 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundPlayer` | 30 | 1304 | 30 / 100.00% | 1304 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundPlayer+Port` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.TextFormat` | 9 | 500 | 8 / 88.89% | 222 / 44.40% |
| `KairoLibrary::kairo.unity.ui.TextLayout` | 60 | 6911 | 57 / 95.00% | 3313 / 47.94% |
| `KairoLibrary::kairo.unity.ui.TextLayout+TextBlock` | 3 | 1219 | 2 / 66.67% | 94 / 7.71% |
| `KairoLibrary::kairo.unity.ui.UIException` | 2 | 9 | 2 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.util.Base64` | 5 | 64 | 5 / 100.00% | 64 / 100.00% |
| `KairoLibrary::kairo.unity.util.BitUtil` | 21 | 316 | 21 / 100.00% | 316 / 100.00% |
| `KairoLibrary::kairo.unity.util.CRC64` | 5 | 152 | 4 / 80.00% | 74 / 48.68% |
| `KairoLibrary::kairo.unity.util.ClockTime` | 4 | 41 | 4 / 100.00% | 41 / 100.00% |
| `KairoLibrary::kairo.unity.util.Compressor` | 13 | 1663 | 10 / 76.92% | 598 / 35.96% |
| `KairoLibrary::kairo.unity.util.Compressor+Node` | 2 | 51 | 2 / 100.00% | 51 / 100.00% |
| `KairoLibrary::kairo.unity.util.DisplayMetrics` | 17 | 778 | 17 / 100.00% | 778 / 100.00% |
| `KairoLibrary::kairo.unity.util.EncodeUtil` | 25 | 719 | 25 / 100.00% | 719 / 100.00% |
| `KairoLibrary::kairo.unity.util.Encrypter` | 10 | 169 | 10 / 100.00% | 169 / 100.00% |
| `KairoLibrary::kairo.unity.util.FastVector` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.FastVector`1` | 39 | 1666 | 38 / 97.44% | 1585 / 95.14% |
| `KairoLibrary::kairo.unity.util.FastVector`1+<GetEnumerator>d__42` | 6 | 169 | 5 / 83.33% | 35 / 20.71% |
| `KairoLibrary::kairo.unity.util.ISerializable` | 2 | 0 | 2 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.util.IllegalFileException` | 2 | 15 | 2 / 100.00% | 15 / 100.00% |
| `KairoLibrary::kairo.unity.util.IntentUtil` | 7 | 312 | 7 / 100.00% | 312 / 100.00% |
| `KairoLibrary::kairo.unity.util.JarFormatException` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.JarInflater` | 15 | 1433 | 14 / 93.33% | 1244 / 86.81% |
| `KairoLibrary::kairo.unity.util.JarInflater+FileInfo` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.KairoException` | 2 | 23 | 2 / 100.00% | 23 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language` | 33 | 22129 | 27 / 81.82% | 16805 / 75.94% |
| `KairoLibrary::kairo.unity.util.Language+LangPackFormat` | 1 | 44 | 1 / 100.00% | 44 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+LangPackRecord` | 2 | 85 | 2 / 100.00% | 85 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+LanguagePackInfo` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+Position` | 1 | 15 | 1 / 100.00% | 15 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+RecordPosition` | 1 | 22 | 1 / 100.00% | 22 / 100.00% |
| `KairoLibrary::kairo.unity.util.Log` | 11 | 428 | 10 / 90.91% | 394 / 92.06% |
| `KairoLibrary::kairo.unity.util.LogListener` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.util.MD5` | 2 | 90 | 2 / 100.00% | 90 / 100.00% |
| `KairoLibrary::kairo.unity.util.NotPermitException` | 4 | 133 | 4 / 100.00% | 133 / 100.00% |
| `KairoLibrary::kairo.unity.util.Property` | 18 | 1436 | 17 / 94.44% | 1388 / 96.66% |
| `KairoLibrary::kairo.unity.util.SecureByte` | 15 | 307 | 15 / 100.00% | 307 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureByteArray` | 14 | 439 | 14 / 100.00% | 439 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureChar` | 13 | 277 | 13 / 100.00% | 277 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureInt` | 19 | 343 | 19 / 100.00% | 343 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureLong` | 21 | 367 | 21 / 100.00% | 367 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureNumber` | 12 | 215 | 12 / 100.00% | 215 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureProperty` | 22 | 1685 | 21 / 95.45% | 1637 / 97.15% |
| `KairoLibrary::kairo.unity.util.SecureShort` | 17 | 326 | 17 / 100.00% | 326 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureString` | 65 | 1365 | 65 / 100.00% | 1365 / 100.00% |
| `KairoLibrary::kairo.unity.util.StreamUtil` | 47 | 4196 | 46 / 97.87% | 2510 / 59.82% |
| `KairoLibrary::kairo.unity.util.StringUtil` | 20 | 2454 | 18 / 90.00% | 763 / 31.09% |
| `KairoLibrary::kfw.panel.MainPanel` | 11 | 1718 | 10 / 90.91% | 981 / 57.10% |
| `KairoLibrary::kfw.panel.MainPanel+OnFinishMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |

## Metadados dos tipos

466 de 466 tipos com metadados estruturais e ordinal verificados como idênticos. Inclui `<Module>` e tipos gerados; heaps e bytes integrais são uma camada separada.

## Arquivos externos e dados

1218 entradas: 2 alvo, 14 externas e 1202 dados.
Externos e dados não entram na porcentagem de código. DEX e plugins nativos não ampliam o escopo C#.

## Próximos tipos e métodos

Priorizar tipos com fonte compilado e menos pendências; ajustar um método por vez.

| Tipo | Pendentes | Primeiro método |
|---|---:|---|
| `Assembly-CSharp::cfg.MyConfig` | 1 | `Assembly-CSharp::cfg.MyConfig::.cctor()->System.Void` |
| `Assembly-CSharp::data.SponsorData` | 1 | `Assembly-CSharp::data.SponsorData::UpdateSatisFactPoint()->System.Void` |
| `Assembly-CSharp::data.SupervisorData` | 1 | `Assembly-CSharp::data.SupervisorData::SetResearchData(System.Int32,data.PlayerData)->System.Void` |
| `Assembly-CSharp::form.RankForm` | 1 | `Assembly-CSharp::form.RankForm::Init()->System.Boolean` |
| `Assembly-CSharp::game.DelayEvent` | 1 | `Assembly-CSharp::game.DelayEvent::Serialize(java.io.OutputStream)->System.Void` |
| `Assembly-CSharp::surface.SurfaceManager` | 1 | `Assembly-CSharp::surface.SurfaceManager::AdjustSurfaceLayout()->System.Void` |
| `Assembly-CSharp::surface.TouchEffect` | 1 | `Assembly-CSharp::surface.TouchEffect::Draw(kairo.unity.ui.Graphics)->System.Void` |
| `Assembly-CSharp::surface.TouchEffectManager` | 1 | `Assembly-CSharp::surface.TouchEffectManager::AddEffect(kairo.unity.surface.SurfaceBase,System.Int32,System.Single,System.Single,kairo.unity.surface.TouchComponent,System.Object)->surface.TouchEffect` |
| `Assembly-CSharp::system.KairoService` | 1 | `Assembly-CSharp::system.KairoService::OnStart()->System.Void` |
| `Assembly-CSharp::system.billing.ui.ApplyTask` | 1 | `Assembly-CSharp::system.billing.ui.ApplyTask::Init(system.billing.ui.BillingItem,System.String,System.Int32,System.Boolean)->System.Void` |

## Bloqueios

- Versão histórica exata dos compiladores: UNKNOWN; pacote Roslyn 1.1.0 é o melhor candidato testado para KairoLibrary.
- Assembly-CSharp: configuração stdlib default validada e chamada original de IndexOf restaurada; os corpos restantes ainda exigem recuperação.
- Todos os tipos e metadados de definição têm identidade estrutural comprovada; corpos e bytes brutos continuam divergentes.
- 57 membros têm campo gerado_por UNKNOWN; propriedades e caches compartilhados podem ter vínculo comprovado no relatório sem uma origem única representável nesse campo.
- Campos variáveis comprovados: quatro em Kairo e dois em App; nenhuma normalização de assembly foi aplicada.

Plano: [DECOMP_PLAN.md](DECOMP_PLAN.md). Ferramentas: [docs/toolchain.md](docs/toolchain.md).
