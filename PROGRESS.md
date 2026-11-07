# Progresso dos métodos C#

Gerado por `python tools/progress` a partir de `config/methods.csv` e `config/types.csv`.

**5,109 de 5,245 métodos matched (97.41%).** 871,497 bytes de IL nos assemblies alvo.
743,560 bytes IL matched (85.32%).
Cabeçalhos, locais e exceções também precisam corresponder; o denominador acima mede somente bytes IL.

Ambos os assemblies foram recuperados e compilam integralmente. KairoLibrary usa o candidato Roslyn 1.1.0; Assembly-CSharp usa smcs 2.6.5.0.

KairoLibrary: 3171 corpos correspondem com tokens resolvidos; 2 ficam retidos por dependências de membros gerados. Declarações sem CIL só entram em matched com prova completa dos metadados do método e do tipo.

## Estados

| Estado | Métodos |
|---|---:|
| todo | 1 |
| decompiled | 135 |
| equivalent | 0 |
| matched | 5109 |
| external | 0 |

## Assemblies

| Grupo | Métodos | Bytes IL | Métodos matched | Bytes IL matched |
|---|---:|---:|---:|---:|
| `Assembly-CSharp` | 1958 | 601603 | 1864 / 95.20% | 490362 / 81.51% |
| `KairoLibrary` | 3287 | 269894 | 3245 / 98.72% | 253198 / 93.81% |

## Namespaces

| Grupo | Métodos | Bytes IL | Métodos matched | Bytes IL matched |
|---|---:|---:|---:|---:|
| `(global)` | 2 | 14 | 1 / 50.00% | 7 / 50.00% |
| `BuildInfos` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `UnityEngine.Purchasing` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `analytics` | 4 | 93 | 4 / 100.00% | 93 / 100.00% |
| `cfg` | 2 | 348 | 2 / 100.00% | 348 / 100.00% |
| `data` | 255 | 26353 | 252 / 98.82% | 23108 / 87.69% |
| `form` | 371 | 250649 | 343 / 92.45% | 195128 / 77.85% |
| `game` | 441 | 202949 | 408 / 92.52% | 162836 / 80.23% |
| `java.io` | 89 | 2223 | 89 / 100.00% | 2223 / 100.00% |
| `java.lang` | 171 | 4928 | 171 / 100.00% | 4928 / 100.00% |
| `java.native` | 34 | 13621 | 33 / 97.06% | 13481 / 98.97% |
| `java.net` | 5 | 502 | 5 / 100.00% | 502 / 100.00% |
| `java.util` | 107 | 3643 | 107 / 100.00% | 3643 / 100.00% |
| `kairo.common.cfg` | 7 | 2180 | 7 / 100.00% | 2180 / 100.00% |
| `kairo.unity.ad` | 15 | 837 | 15 / 100.00% | 837 / 100.00% |
| `kairo.unity.analytics` | 15 | 1284 | 15 / 100.00% | 1284 / 100.00% |
| `kairo.unity.form` | 88 | 6284 | 87 / 98.86% | 5600 / 89.12% |
| `kairo.unity.graph` | 7 | 680 | 7 / 100.00% | 680 / 100.00% |
| `kairo.unity.graphics` | 195 | 21048 | 191 / 97.95% | 19914 / 94.61% |
| `kairo.unity.iOSsupport` | 4 | 18 | 4 / 100.00% | 18 / 100.00% |
| `kairo.unity.io` | 149 | 9866 | 145 / 97.32% | 8631 / 87.48% |
| `kairo.unity.licensing` | 7 | 101 | 7 / 100.00% | 101 / 100.00% |
| `kairo.unity.m3d4.util3d` | 67 | 2810 | 65 / 97.01% | 2546 / 90.60% |
| `kairo.unity.math` | 57 | 5195 | 56 / 98.25% | 5149 / 99.11% |
| `kairo.unity.native` | 212 | 10404 | 209 / 98.58% | 9676 / 93.00% |
| `kairo.unity.native.util` | 16 | 1601 | 16 / 100.00% | 1601 / 100.00% |
| `kairo.unity.panel` | 37 | 765 | 37 / 100.00% | 765 / 100.00% |
| `kairo.unity.surface` | 187 | 14973 | 186 / 99.47% | 14601 / 97.52% |
| `kairo.unity.system` | 193 | 16116 | 190 / 98.45% | 15916 / 98.76% |
| `kairo.unity.system.appli` | 2 | 26 | 2 / 100.00% | 26 / 100.00% |
| `kairo.unity.system.lineup.ui` | 23 | 2607 | 22 / 95.65% | 2415 / 92.64% |
| `kairo.unity.system.lobi` | 7 | 249 | 7 / 100.00% | 249 / 100.00% |
| `kairo.unity.system.notification.ui` | 24 | 911 | 24 / 100.00% | 911 / 100.00% |
| `kairo.unity.system.payment` | 14 | 752 | 14 / 100.00% | 752 / 100.00% |
| `kairo.unity.system.tapjoy.ui` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `kairo.unity.ui` | 1021 | 100597 | 1006 / 98.53% | 91784 / 91.24% |
| `kairo.unity.util` | 518 | 43955 | 512 / 98.84% | 41067 / 93.43% |
| `kfw.panel` | 15 | 1718 | 15 / 100.00% | 1718 / 100.00% |
| `main` | 223 | 35448 | 216 / 96.86% | 31403 / 88.59% |
| `native` | 26 | 896 | 26 / 100.00% | 896 / 100.00% |
| `panel` | 5 | 49 | 5 / 100.00% | 49 / 100.00% |
| `surface` | 122 | 26518 | 112 / 91.80% | 21306 / 80.35% |
| `system` | 20 | 1636 | 20 / 100.00% | 1636 / 100.00% |
| `system.billing` | 8 | 956 | 8 / 100.00% | 956 / 100.00% |
| `system.billing.ui` | 153 | 13782 | 148 / 96.73% | 11788 / 85.53% |
| `system.billing.util` | 45 | 1788 | 44 / 97.78% | 1715 / 95.92% |
| `system.form` | 192 | 33269 | 188 / 97.92% | 32915 / 98.94% |
| `system.friend` | 4 | 570 | 3 / 75.00% | 284 / 49.82% |
| `system.friend.ui` | 47 | 4407 | 47 / 100.00% | 4407 / 100.00% |
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
| `Assembly-CSharp::cfg.MyConfig` | 2 | 348 | 2 / 100.00% | 348 / 100.00% |
| `Assembly-CSharp::data.BaseData` | 2 | 22 | 2 / 100.00% | 22 / 100.00% |
| `Assembly-CSharp::data.EventData` | 7 | 342 | 7 / 100.00% | 342 / 100.00% |
| `Assembly-CSharp::data.FanActData` | 23 | 2141 | 23 / 100.00% | 2141 / 100.00% |
| `Assembly-CSharp::data.FanBonusData` | 15 | 4141 | 14 / 93.33% | 1536 / 37.09% |
| `Assembly-CSharp::data.FormationData` | 12 | 796 | 12 / 100.00% | 796 / 100.00% |
| `Assembly-CSharp::data.LeagueData` | 17 | 1388 | 17 / 100.00% | 1388 / 100.00% |
| `Assembly-CSharp::data.MapchipMain` | 3 | 179 | 3 / 100.00% | 179 / 100.00% |
| `Assembly-CSharp::data.MapchipMatch` | 3 | 161 | 3 / 100.00% | 161 / 100.00% |
| `Assembly-CSharp::data.PlayerData` | 55 | 6370 | 55 / 100.00% | 6370 / 100.00% |
| `Assembly-CSharp::data.QuestMatchData` | 16 | 1254 | 16 / 100.00% | 1254 / 100.00% |
| `Assembly-CSharp::data.SponsorData` | 16 | 1288 | 16 / 100.00% | 1288 / 100.00% |
| `Assembly-CSharp::data.StadiumData` | 6 | 177 | 6 / 100.00% | 177 / 100.00% |
| `Assembly-CSharp::data.SupervisorData` | 15 | 1869 | 15 / 100.00% | 1869 / 100.00% |
| `Assembly-CSharp::data.TeamData` | 12 | 1963 | 11 / 91.67% | 1425 / 72.59% |
| `Assembly-CSharp::data.TenantData` | 26 | 2743 | 25 / 96.15% | 2641 / 96.28% |
| `Assembly-CSharp::data.TradeData` | 13 | 757 | 13 / 100.00% | 757 / 100.00% |
| `Assembly-CSharp::data.TrainingData` | 14 | 762 | 14 / 100.00% | 762 / 100.00% |
| `Assembly-CSharp::form.BootForm` | 21 | 7712 | 18 / 85.71% | 3707 / 48.07% |
| `Assembly-CSharp::form.DebugForm` | 21 | 45497 | 21 / 100.00% | 45497 / 100.00% |
| `Assembly-CSharp::form.FormManager` | 13 | 577 | 13 / 100.00% | 577 / 100.00% |
| `Assembly-CSharp::form.GameForm` | 51 | 16474 | 47 / 92.16% | 12327 / 74.83% |
| `Assembly-CSharp::form.MyFormBase` | 48 | 3906 | 46 / 95.83% | 2178 / 55.76% |
| `Assembly-CSharp::form.RankForm` | 14 | 1516 | 14 / 100.00% | 1516 / 100.00% |
| `Assembly-CSharp::form.SoccerForm` | 62 | 33639 | 54 / 87.10% | 23404 / 69.57% |
| `Assembly-CSharp::form.SpForm` | 19 | 7256 | 16 / 84.21% | 4576 / 63.07% |
| `Assembly-CSharp::form.SubForm` | 95 | 125345 | 89 / 93.68% | 93067 / 74.25% |
| `Assembly-CSharp::form.SubForm+<Update2>c__AnonStorey0` | 3 | 227 | 2 / 66.67% | 31 / 13.66% |
| `Assembly-CSharp::form.TitleForm` | 24 | 8500 | 23 / 95.83% | 8248 / 97.04% |
| `Assembly-CSharp::game.Ball` | 18 | 15652 | 16 / 88.89% | 13195 / 84.30% |
| `Assembly-CSharp::game.Camera` | 31 | 7399 | 29 / 93.55% | 4254 / 57.49% |
| `Assembly-CSharp::game.DelayEvent` | 4 | 456 | 4 / 100.00% | 456 / 100.00% |
| `Assembly-CSharp::game.GameManager` | 20 | 11549 | 17 / 85.00% | 6747 / 58.42% |
| `Assembly-CSharp::game.GameUtil` | 47 | 3570 | 46 / 97.87% | 3404 / 95.35% |
| `Assembly-CSharp::game.Map` | 18 | 13729 | 15 / 83.33% | 10504 / 76.51% |
| `Assembly-CSharp::game.MapMain` | 14 | 7415 | 13 / 92.86% | 6792 / 91.60% |
| `Assembly-CSharp::game.MapObject` | 2 | 46 | 2 / 100.00% | 46 / 100.00% |
| `Assembly-CSharp::game.Player` | 63 | 32346 | 56 / 88.89% | 25051 / 77.45% |
| `Assembly-CSharp::game.TArea` | 14 | 4070 | 14 / 100.00% | 4070 / 100.00% |
| `Assembly-CSharp::game.TBall` | 14 | 5087 | 14 / 100.00% | 5087 / 100.00% |
| `Assembly-CSharp::game.TGroupArea` | 14 | 7524 | 14 / 100.00% | 7524 / 100.00% |
| `Assembly-CSharp::game.TPlayer` | 34 | 13693 | 33 / 97.06% | 13109 / 95.74% |
| `Assembly-CSharp::game.Team` | 42 | 22251 | 40 / 95.24% | 20671 / 92.90% |
| `Assembly-CSharp::game.Tenant` | 14 | 2692 | 13 / 92.86% | 2200 / 81.72% |
| `Assembly-CSharp::game.UserData` | 92 | 55470 | 82 / 89.13% | 39726 / 71.62% |
| `Assembly-CSharp::main.AppData` | 201 | 33806 | 194 / 96.52% | 29761 / 88.03% |
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
| `Assembly-CSharp::surface.GamePad` | 29 | 9686 | 27 / 93.10% | 6659 / 68.75% |
| `Assembly-CSharp::surface.GameView` | 66 | 13578 | 58 / 87.88% | 11393 / 83.91% |
| `Assembly-CSharp::surface.SurfaceManager` | 8 | 1729 | 8 / 100.00% | 1729 / 100.00% |
| `Assembly-CSharp::surface.TouchEffect` | 5 | 821 | 5 / 100.00% | 821 / 100.00% |
| `Assembly-CSharp::surface.TouchEffectManager` | 14 | 704 | 14 / 100.00% | 704 / 100.00% |
| `Assembly-CSharp::system.KairoService` | 15 | 1353 | 15 / 100.00% | 1353 / 100.00% |
| `Assembly-CSharp::system.KairoText` | 5 | 283 | 5 / 100.00% | 283 / 100.00% |
| `Assembly-CSharp::system.billing.MyBilling` | 8 | 956 | 8 / 100.00% | 956 / 100.00% |
| `Assembly-CSharp::system.billing.ui.ApplyTask` | 12 | 1507 | 12 / 100.00% | 1507 / 100.00% |
| `Assembly-CSharp::system.billing.ui.BillingItem` | 26 | 1007 | 26 / 100.00% | 1007 / 100.00% |
| `Assembly-CSharp::system.billing.ui.BillingManager` | 44 | 6863 | 40 / 90.91% | 4968 / 72.39% |
| `Assembly-CSharp::system.billing.ui.PurchaseLogView` | 3 | 130 | 3 / 100.00% | 130 / 100.00% |
| `Assembly-CSharp::system.billing.ui.PurchaseRecord` | 12 | 1155 | 12 / 100.00% | 1155 / 100.00% |
| `Assembly-CSharp::system.billing.ui.Receipt` | 26 | 932 | 26 / 100.00% | 932 / 100.00% |
| `Assembly-CSharp::system.billing.ui.StoreBase` | 28 | 2159 | 27 / 96.43% | 2060 / 95.41% |
| `Assembly-CSharp::system.billing.ui.StoreBase+<SetRestoreRequest>c__AnonStorey7` | 2 | 29 | 2 / 100.00% | 29 / 100.00% |
| `Assembly-CSharp::system.billing.util.IabHelper` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::system.billing.util.Inventory` | 11 | 288 | 11 / 100.00% | 288 / 100.00% |
| `Assembly-CSharp::system.billing.util.Json` | 7 | 672 | 6 / 85.71% | 599 / 89.14% |
| `Assembly-CSharp::system.billing.util.Purchase` | 15 | 527 | 15 / 100.00% | 527 / 100.00% |
| `Assembly-CSharp::system.billing.util.SkuDetails` | 11 | 294 | 11 / 100.00% | 294 / 100.00% |
| `Assembly-CSharp::system.form.BillingForm` | 42 | 7424 | 41 / 97.62% | 7313 / 98.50% |
| `Assembly-CSharp::system.form.ConnectForm` | 11 | 400 | 11 / 100.00% | 400 / 100.00% |
| `Assembly-CSharp::system.form.FriendForm` | 53 | 17764 | 53 / 100.00% | 17764 / 100.00% |
| `Assembly-CSharp::system.form.HttpForm` | 16 | 894 | 16 / 100.00% | 894 / 100.00% |
| `Assembly-CSharp::system.form.HttpForm+HttpMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `Assembly-CSharp::system.form.HttpForm+ResultData` | 11 | 132 | 11 / 100.00% | 132 / 100.00% |
| `Assembly-CSharp::system.form.LanguageForm` | 13 | 1452 | 11 / 84.62% | 1265 / 87.12% |
| `Assembly-CSharp::system.form.LineupForm` | 15 | 1711 | 14 / 93.33% | 1655 / 96.73% |
| `Assembly-CSharp::system.form.MatchForm` | 19 | 3263 | 19 / 100.00% | 3263 / 100.00% |
| `Assembly-CSharp::system.form.MatchForm+UserRecord` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `Assembly-CSharp::system.form.RestoreForm` | 7 | 222 | 7 / 100.00% | 222 / 100.00% |
| `Assembly-CSharp::system.friend.MyFriend` | 4 | 570 | 3 / 75.00% | 284 / 49.82% |
| `Assembly-CSharp::system.friend.ui.Friend` | 6 | 661 | 6 / 100.00% | 661 / 100.00% |
| `Assembly-CSharp::system.friend.ui.FriendData` | 37 | 3548 | 37 / 100.00% | 3548 / 100.00% |
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
| `KairoLibrary::java.lang.JMath` | 20 | 199 | 20 / 100.00% | 199 / 100.00% |
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
| `KairoLibrary::kairo.unity.ad.InterstitialAd` | 8 | 658 | 8 / 100.00% | 658 / 100.00% |
| `KairoLibrary::kairo.unity.analytics.AnalyticsTracker` | 10 | 1045 | 10 / 100.00% | 1045 / 100.00% |
| `KairoLibrary::kairo.unity.analytics.AnalyticsTracker+<>c` | 3 | 200 | 3 / 100.00% | 200 / 100.00% |
| `KairoLibrary::kairo.unity.analytics.AnalyticsTracker+AnalyticsData` | 2 | 39 | 2 / 100.00% | 39 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormBase` | 24 | 267 | 24 / 100.00% | 267 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase` | 53 | 4432 | 53 / 100.00% | 4432 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+<>c__DisplayClass85_0` | 2 | 691 | 1 / 50.00% | 7 / 1.01% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+<>c__DisplayClass85_1` | 4 | 887 | 4 / 100.00% | 887 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+<>c__DisplayClass85_2` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.form.FormManagerBase+OnCaptureCompleted` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graph.Graph` | 7 | 680 | 7 / 100.00% | 680 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Material` | 13 | 322 | 13 / 100.00% | 322 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MaterialManager` | 11 | 1735 | 9 / 81.82% | 1209 / 69.68% |
| `KairoLibrary::kairo.unity.graphics.MeshBuilder` | 31 | 4626 | 31 / 100.00% | 4626 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshBuilder+Group` | 3 | 910 | 3 / 100.00% | 910 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshBuilder+Layer` | 3 | 106 | 3 / 100.00% | 106 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshManager` | 24 | 3389 | 23 / 95.83% | 3248 / 95.84% |
| `KairoLibrary::kairo.unity.graphics.MeshManager+LayerMesh` | 2 | 52 | 2 / 100.00% | 52 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.MeshRecord` | 6 | 300 | 6 / 100.00% | 300 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen` | 29 | 3347 | 29 / 100.00% | 3347 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+<>c__DisplayClass33_0` | 2 | 209 | 2 / 100.00% | 209 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+<>c__DisplayClass34_0` | 2 | 617 | 2 / 100.00% | 617 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+<>c__DisplayClass35_0` | 2 | 638 | 2 / 100.00% | 638 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+GetLayoutMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+MultiRenderMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graphics.Offscreen+RenderMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.graphics.Point` | 7 | 137 | 7 / 100.00% | 137 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.Rectangle` | 10 | 269 | 10 / 100.00% | 269 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.RectangleF` | 12 | 332 | 12 / 100.00% | 332 / 100.00% |
| `KairoLibrary::kairo.unity.graphics.ShaderBuilder` | 14 | 3664 | 13 / 92.86% | 3197 / 87.25% |
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
| `KairoLibrary::kairo.unity.io.HttpObject` | 17 | 1728 | 17 / 100.00% | 1728 / 100.00% |
| `KairoLibrary::kairo.unity.io.HttpObject+<>c__DisplayClass10_0` | 3 | 989 | 2 / 66.67% | 45 / 4.55% |
| `KairoLibrary::kairo.unity.io.HttpObject+<>c__DisplayClass10_1` | 2 | 110 | 2 / 100.00% | 110 / 100.00% |
| `KairoLibrary::kairo.unity.io.RecordStore` | 28 | 1592 | 27 / 96.43% | 1498 / 94.10% |
| `KairoLibrary::kairo.unity.io.RecordStoreListener` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.io.ScratchPad` | 11 | 25 | 11 / 100.00% | 25 / 100.00% |
| `KairoLibrary::kairo.unity.io.Storage` | 18 | 1744 | 17 / 94.44% | 1580 / 90.60% |
| `KairoLibrary::kairo.unity.licensing.LicenseChecker` | 7 | 101 | 7 / 100.00% | 101 / 100.00% |
| `KairoLibrary::kairo.unity.m3d4.util3d.FastMath` | 16 | 325 | 16 / 100.00% | 325 / 100.00% |
| `KairoLibrary::kairo.unity.m3d4.util3d.Transform` | 27 | 1710 | 25 / 92.59% | 1446 / 84.56% |
| `KairoLibrary::kairo.unity.m3d4.util3d.Vector3D` | 24 | 775 | 24 / 100.00% | 775 / 100.00% |
| `KairoLibrary::kairo.unity.math.KairoMath` | 57 | 5195 | 56 / 98.25% | 5149 / 99.11% |
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
| `KairoLibrary::kairo.unity.native.KairoPlugin+<>c__DisplayClass76_0` | 2 | 89 | 2 / 100.00% | 89 / 100.00% |
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
| `KairoLibrary::kairo.unity.panel.FepPanel` | 15 | 591 | 15 / 100.00% | 591 / 100.00% |
| `KairoLibrary::kairo.unity.panel.FepPanel+Display` | 3 | 9 | 3 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.panel.Label` | 6 | 49 | 6 / 100.00% | 49 / 100.00% |
| `KairoLibrary::kairo.unity.panel.TextBox` | 7 | 76 | 7 / 100.00% | 76 / 100.00% |
| `KairoLibrary::kairo.unity.surface.MotionEvent` | 15 | 268 | 15 / 100.00% | 268 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase` | 76 | 7799 | 76 / 100.00% | 7799 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase+MotionEventRecord` | 19 | 882 | 19 / 100.00% | 882 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase+MotionEventRecord+MultiTouch` | 9 | 170 | 9 / 100.00% | 170 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceBase+TouchPoint` | 2 | 76 | 2 / 100.00% | 76 / 100.00% |
| `KairoLibrary::kairo.unity.surface.SurfaceManagerBase` | 25 | 3793 | 25 / 100.00% | 3793 / 100.00% |
| `KairoLibrary::kairo.unity.surface.TouchComponent` | 11 | 461 | 11 / 100.00% | 461 / 100.00% |
| `KairoLibrary::kairo.unity.surface.TouchEvent` | 7 | 673 | 6 / 85.71% | 301 / 44.73% |
| `KairoLibrary::kairo.unity.surface.TouchOption` | 21 | 851 | 21 / 100.00% | 851 / 100.00% |
| `KairoLibrary::kairo.unity.surface.TouchValueAccessor` | 2 | 0 | 2 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.system.ErrorCode` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.system.GameDataRecord` | 32 | 2430 | 32 / 100.00% | 2430 / 100.00% |
| `KairoLibrary::kairo.unity.system.GameDataRecord+Data` | 5 | 287 | 5 / 100.00% | 287 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase` | 134 | 12704 | 134 / 100.00% | 12704 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+<>c` | 4 | 216 | 1 / 25.00% | 16 / 7.41% |
| `KairoLibrary::kairo.unity.system.KairoBase+AccessCount` | 4 | 80 | 4 / 100.00% | 80 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+ConnectObject` | 2 | 69 | 2 / 100.00% | 69 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+ConnectTask` | 3 | 199 | 3 / 100.00% | 199 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoBase+KairoDummy` | 5 | 11 | 5 / 100.00% | 11 / 100.00% |
| `KairoLibrary::kairo.unity.system.KairoClub` | 3 | 113 | 3 / 100.00% | 113 / 100.00% |
| `KairoLibrary::kairo.unity.system.appli.GameAppli` | 2 | 26 | 2 / 100.00% | 26 / 100.00% |
| `KairoLibrary::kairo.unity.system.lineup.ui.LineupData` | 9 | 492 | 8 / 88.89% | 300 / 60.98% |
| `KairoLibrary::kairo.unity.system.lineup.ui.LineupManager` | 14 | 2115 | 14 / 100.00% | 2115 / 100.00% |
| `KairoLibrary::kairo.unity.system.lobi.LobiManager` | 7 | 249 | 7 / 100.00% | 249 / 100.00% |
| `KairoLibrary::kairo.unity.system.notification.ui.NotifyManager` | 24 | 911 | 24 / 100.00% | 911 / 100.00% |
| `KairoLibrary::kairo.unity.system.payment.IllegalPurchaseException` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.system.payment.PaymentDataRecord` | 13 | 745 | 13 / 100.00% | 745 / 100.00% |
| `KairoLibrary::kairo.unity.system.tapjoy.ui.ITapjoy` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.AppEvent` | 1 | 41 | 1 / 100.00% | 41 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Balloon` | 27 | 4072 | 27 / 100.00% | 4072 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Balloon+BalloonRender` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.Canvas` | 82 | 8325 | 82 / 100.00% | 8325 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Canvas+<>c` | 3 | 398 | 3 / 100.00% | 398 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Canvas+<>c__DisplayClass107_0` | 2 | 75 | 2 / 100.00% | 75 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Canvas+<>c__DisplayClass152_0` | 2 | 172 | 2 / 100.00% | 172 / 100.00% |
| `KairoLibrary::kairo.unity.ui.DepthInfo` | 5 | 523 | 5 / 100.00% | 523 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Dialog` | 7 | 287 | 7 / 100.00% | 287 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Font` | 45 | 1964 | 45 / 100.00% | 1964 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Font+<>c` | 3 | 24 | 3 / 100.00% | 24 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics` | 227 | 29937 | 224 / 98.68% | 29150 / 97.37% |
| `KairoLibrary::kairo.unity.ui.Graphics+GLText` | 2 | 173 | 2 / 100.00% | 173 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+GrayscaleImage` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+RenderCache` | 2 | 69 | 2 / 100.00% | 69 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+RenderPass` | 11 | 2121 | 11 / 100.00% | 2121 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Graphics+StackState` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Handler` | 5 | 443 | 5 / 100.00% | 443 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Handler+<>c__DisplayClass3_0` | 2 | 19 | 2 / 100.00% | 19 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Handler+PostData` | 1 | 55 | 1 / 100.00% | 55 / 100.00% |
| `KairoLibrary::kairo.unity.ui.IApplication` | 77 | 4239 | 76 / 98.70% | 4040 / 95.31% |
| `KairoLibrary::kairo.unity.ui.IApplication+MethodTask` | 2 | 28 | 2 / 100.00% | 28 / 100.00% |
| `KairoLibrary::kairo.unity.ui.IApplication+UIMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.Image` | 92 | 8726 | 89 / 96.74% | 7461 / 85.50% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass155_0` | 2 | 380 | 2 / 100.00% | 380 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass156_0` | 2 | 42 | 2 / 100.00% | 42 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass157_0` | 2 | 48 | 2 / 100.00% | 48 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass82_0` | 2 | 111 | 2 / 100.00% | 111 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+<>c__DisplayClass85_0` | 2 | 86 | 2 / 100.00% | 86 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+BalloonRenderTask` | 1 | 35 | 1 / 100.00% | 35 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+TextRenderTask` | 1 | 46 | 1 / 100.00% | 46 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Image+TouchTask` | 1 | 161 | 1 / 100.00% | 161 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ImageAtlas` | 21 | 5410 | 21 / 100.00% | 5410 / 100.00% |
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
| `KairoLibrary::kairo.unity.ui.Matrix` | 34 | 2139 | 33 / 97.06% | 1990 / 93.03% |
| `KairoLibrary::kairo.unity.ui.Pictograph` | 16 | 199 | 16 / 100.00% | 199 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ResourceManager` | 49 | 3927 | 49 / 100.00% | 3927 / 100.00% |
| `KairoLibrary::kairo.unity.ui.ResourceManager+<>c__DisplayClass51_0` | 3 | 1995 | 2 / 66.67% | 67 / 3.36% |
| `KairoLibrary::kairo.unity.ui.ResourceManager+JarCache` | 1 | 22 | 1 / 100.00% | 22 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder` | 20 | 2773 | 19 / 95.00% | 2236 / 80.63% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+<>c__DisplayClass26_0` | 2 | 147 | 2 / 100.00% | 147 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PackMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PostPackMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PostSaveMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PrePackMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.SaveRecorder+PreSaveMethod` | 4 | 0 | 4 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.ui.Seb` | 40 | 7805 | 38 / 95.00% | 4741 / 60.74% |
| `KairoLibrary::kairo.unity.ui.Seb+WorkStaticValue` | 1 | 90 | 1 / 100.00% | 90 / 100.00% |
| `KairoLibrary::kairo.unity.ui.Seb+WorkValue` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundBase` | 20 | 331 | 20 / 100.00% | 331 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundOGG` | 10 | 345 | 10 / 100.00% | 345 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundOGG+<>c__DisplayClass3_0` | 2 | 26 | 2 / 100.00% | 26 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundOGG+<>c__DisplayClass4_0` | 2 | 390 | 1 / 50.00% | 7 / 1.79% |
| `KairoLibrary::kairo.unity.ui.SoundOGG+<>c__DisplayClass5_0` | 2 | 124 | 2 / 100.00% | 124 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundPlayer` | 30 | 1304 | 30 / 100.00% | 1304 / 100.00% |
| `KairoLibrary::kairo.unity.ui.SoundPlayer+Port` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.ui.TextFormat` | 9 | 500 | 9 / 100.00% | 500 / 100.00% |
| `KairoLibrary::kairo.unity.ui.TextLayout` | 60 | 6911 | 58 / 96.67% | 6410 / 92.75% |
| `KairoLibrary::kairo.unity.ui.TextLayout+TextBlock` | 3 | 1219 | 3 / 100.00% | 1219 / 100.00% |
| `KairoLibrary::kairo.unity.ui.UIException` | 2 | 9 | 2 / 100.00% | 9 / 100.00% |
| `KairoLibrary::kairo.unity.util.Base64` | 5 | 64 | 5 / 100.00% | 64 / 100.00% |
| `KairoLibrary::kairo.unity.util.BitUtil` | 21 | 316 | 21 / 100.00% | 316 / 100.00% |
| `KairoLibrary::kairo.unity.util.CRC64` | 5 | 152 | 5 / 100.00% | 152 / 100.00% |
| `KairoLibrary::kairo.unity.util.ClockTime` | 4 | 41 | 4 / 100.00% | 41 / 100.00% |
| `KairoLibrary::kairo.unity.util.Compressor` | 13 | 1663 | 11 / 84.62% | 788 / 47.38% |
| `KairoLibrary::kairo.unity.util.Compressor+Node` | 2 | 51 | 2 / 100.00% | 51 / 100.00% |
| `KairoLibrary::kairo.unity.util.DisplayMetrics` | 17 | 778 | 17 / 100.00% | 778 / 100.00% |
| `KairoLibrary::kairo.unity.util.EncodeUtil` | 25 | 719 | 25 / 100.00% | 719 / 100.00% |
| `KairoLibrary::kairo.unity.util.Encrypter` | 10 | 169 | 10 / 100.00% | 169 / 100.00% |
| `KairoLibrary::kairo.unity.util.FastVector` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.FastVector`1` | 39 | 1666 | 39 / 100.00% | 1666 / 100.00% |
| `KairoLibrary::kairo.unity.util.FastVector`1+<GetEnumerator>d__42` | 6 | 169 | 6 / 100.00% | 169 / 100.00% |
| `KairoLibrary::kairo.unity.util.ISerializable` | 2 | 0 | 2 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.util.IllegalFileException` | 2 | 15 | 2 / 100.00% | 15 / 100.00% |
| `KairoLibrary::kairo.unity.util.IntentUtil` | 7 | 312 | 7 / 100.00% | 312 / 100.00% |
| `KairoLibrary::kairo.unity.util.JarFormatException` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.JarInflater` | 15 | 1433 | 15 / 100.00% | 1433 / 100.00% |
| `KairoLibrary::kairo.unity.util.JarInflater+FileInfo` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.KairoException` | 2 | 23 | 2 / 100.00% | 23 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language` | 33 | 22129 | 30 / 90.91% | 20481 / 92.55% |
| `KairoLibrary::kairo.unity.util.Language+LangPackFormat` | 1 | 44 | 1 / 100.00% | 44 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+LangPackRecord` | 2 | 85 | 2 / 100.00% | 85 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+LanguagePackInfo` | 1 | 7 | 1 / 100.00% | 7 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+Position` | 1 | 15 | 1 / 100.00% | 15 / 100.00% |
| `KairoLibrary::kairo.unity.util.Language+RecordPosition` | 1 | 22 | 1 / 100.00% | 22 / 100.00% |
| `KairoLibrary::kairo.unity.util.Log` | 11 | 428 | 11 / 100.00% | 428 / 100.00% |
| `KairoLibrary::kairo.unity.util.LogListener` | 1 | 0 | 1 / 100.00% | 0 / 0.00% |
| `KairoLibrary::kairo.unity.util.MD5` | 2 | 90 | 2 / 100.00% | 90 / 100.00% |
| `KairoLibrary::kairo.unity.util.NotPermitException` | 4 | 133 | 4 / 100.00% | 133 / 100.00% |
| `KairoLibrary::kairo.unity.util.Property` | 18 | 1436 | 18 / 100.00% | 1436 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureByte` | 15 | 307 | 15 / 100.00% | 307 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureByteArray` | 14 | 439 | 14 / 100.00% | 439 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureChar` | 13 | 277 | 13 / 100.00% | 277 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureInt` | 19 | 343 | 19 / 100.00% | 343 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureLong` | 21 | 367 | 21 / 100.00% | 367 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureNumber` | 12 | 215 | 12 / 100.00% | 215 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureProperty` | 22 | 1685 | 22 / 100.00% | 1685 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureShort` | 17 | 326 | 17 / 100.00% | 326 / 100.00% |
| `KairoLibrary::kairo.unity.util.SecureString` | 65 | 1365 | 65 / 100.00% | 1365 / 100.00% |
| `KairoLibrary::kairo.unity.util.StreamUtil` | 47 | 4196 | 47 / 100.00% | 4196 / 100.00% |
| `KairoLibrary::kairo.unity.util.StringUtil` | 20 | 2454 | 19 / 95.00% | 2089 / 85.13% |
| `KairoLibrary::kfw.panel.MainPanel` | 11 | 1718 | 11 / 100.00% | 1718 / 100.00% |
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
| `Assembly-CSharp::data.FanBonusData` | 1 | `Assembly-CSharp::data.FanBonusData::ChBonusEvent()->System.Void` |
| `Assembly-CSharp::data.TeamData` | 1 | `Assembly-CSharp::data.TeamData::SetScoreRanking(data.LeagueData,java.util.Vector,java.util.Vector)->System.Void` |
| `Assembly-CSharp::data.TenantData` | 1 | `Assembly-CSharp::data.TenantData::StrengthTenant()->System.Void` |
| `Assembly-CSharp::form.TitleForm` | 1 | `Assembly-CSharp::form.TitleForm::OnTouchEvent(kairo.unity.surface.TouchEvent)->System.Boolean` |
| `Assembly-CSharp::game.GameUtil` | 1 | `Assembly-CSharp::game.GameUtil::.ctor()->System.Void` |
| `Assembly-CSharp::game.MapMain` | 1 | `Assembly-CSharp::game.MapMain::Draw_coat(kairo.unity.ui.Graphics,System.Boolean)->System.Void` |
| `Assembly-CSharp::game.TPlayer` | 1 | `Assembly-CSharp::game.TPlayer::CollisionSaving(game.TBall)->System.Void` |
| `Assembly-CSharp::game.Tenant` | 1 | `Assembly-CSharp::game.Tenant::Draw(kairo.unity.ui.Graphics)->System.Void` |
| `Assembly-CSharp::system.billing.ui.StoreBase` | 1 | `Assembly-CSharp::system.billing.ui.StoreBase::CheckPaymentMaintenance()->System.Boolean` |
| `Assembly-CSharp::system.billing.util.Json` | 1 | `Assembly-CSharp::system.billing.util.Json::MakeJson(System.String[])->System.String` |

## Bloqueios

- Versão histórica exata dos compiladores: UNKNOWN; pacote Roslyn 1.1.0 é o melhor candidato testado para KairoLibrary.
- Assembly-CSharp: configuração stdlib default validada e chamada original de IndexOf restaurada; os corpos restantes ainda exigem recuperação.
- Todos os tipos e metadados de definição têm identidade estrutural comprovada; corpos e bytes brutos continuam divergentes.
- 57 membros têm campo gerado_por UNKNOWN; propriedades e caches compartilhados podem ter vínculo comprovado no relatório sem uma origem única representável nesse campo.
- Campos variáveis comprovados: quatro em Kairo e dois em App; nenhuma normalização de assembly foi aplicada.

Plano: [DECOMP_PLAN.md](DECOMP_PLAN.md). Ferramentas: [docs/toolchain.md](docs/toolchain.md).
