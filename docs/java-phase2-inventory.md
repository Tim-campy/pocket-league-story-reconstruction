# Inventário Java para decisão de segunda fase

São **101 classes candidatas**, incluindo classes anônimas/internas e cinco classes R geradas. Não são 101 fontes escritos à mão. Nenhuma foi decompilada; nenhum corpo Java foi examinado nesta rodada.

Os nomes, superclasses, índices class_def e source_file são fatos da tabela DEX já inventariada. A divisão funcional abaixo é uma inferência por pacote/nome/superclasse, não uma confirmação de comportamento. Autoria histórica e função exata dos auxiliares permanecem **UNKNOWN**. Licenciamento não foi presumido como cobrança; Utility e Referrer ficam em Outros por falta de evidência mais específica.

DEX SHA-256: `4b5d18e92e9364ffd22cdaed6d47b470a4881120cd5b8ab35b3c223ed5753f94`. Fonte: [inventário original](../reports/DEX-package-inventory.json); [lista estruturada com índices e superclasses](../reports/java-phase2-inventory.json).

| Grupo | Classes |
|---|---:|
| Cobrança | 5 |
| Anúncios | 11 |
| Interface Android | 47 |
| Outros | 38 |
| **Total** | **101** |

Para contexto: o DEX contém 3.232 classes: 79 nomes correspondem ao classes.jar oficial Unity; 3.052 foram classificados como SDKs de terceiros por namespace; 101 são estes candidatos. Versões exatas dos SDKs e autoria de cada candidato continuam UNKNOWN.

## Cobrança — 5 classes

Pacote plugin.billing e nome PurchaseLogView.

| Classe binária completa | class_def | Superclasse |
|---|---:|---|
| `plugin.billing.PurchaseLogView$1$1` | 3221 | `Ljava/lang/Object;` |
| `plugin.billing.PurchaseLogView$1$2$1` | 3222 | `Ljava/lang/Object;` |
| `plugin.billing.PurchaseLogView$1$2` | 3223 | `Ljava/lang/Object;` |
| `plugin.billing.PurchaseLogView$1` | 3224 | `Ljava/lang/Object;` |
| `plugin.billing.PurchaseLogView` | 3225 | `Ljava/lang/Object;` |

## Anúncios — 11 classes

Nomes AdMobView/Interstitial; superclasses AdListener nos auxiliares indicados.

| Classe binária completa | class_def | Superclasse |
|---|---:|---|
| `kairo.android.plugin.ad.AdMobView` | 3139 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ad.Interstitial` | 3140 | `Ljava/lang/Object;` |
| `kairo.android.plugin.gms.AdMobView$1` | 3141 | `Ljava/lang/Object;` |
| `kairo.android.plugin.gms.AdMobView$2` | 3142 | `Ljava/lang/Object;` |
| `kairo.android.plugin.gms.AdMobView$3$1` | 3143 | `Lcom/google/android/gms/ads/AdListener;` |
| `kairo.android.plugin.gms.AdMobView$3` | 3144 | `Ljava/lang/Object;` |
| `kairo.android.plugin.gms.AdMobView` | 3145 | `Landroid/view/ViewGroup;` |
| `kairo.android.plugin.gms.Interstitial$1$1` | 3146 | `Lcom/google/android/gms/ads/AdListener;` |
| `kairo.android.plugin.gms.Interstitial$1` | 3147 | `Ljava/lang/Object;` |
| `kairo.android.plugin.gms.Interstitial$2` | 3148 | `Ljava/lang/Object;` |
| `kairo.android.plugin.gms.Interstitial` | 3149 | `Ljava/lang/Object;` |

## Interface Android — 47 classes

Pacote/nome de painel, diálogo, notificação, WebBox ou Activity principal.

| Classe binária completa | class_def | Superclasse |
|---|---:|---|
| `kairo.android.plugin.notification.GCMReceiver` | 3158 | `Lcom/tapjoy/GCMReceiver;` |
| `kairo.android.plugin.notification.NotificationActivity$1` | 3159 | `Ljava/lang/Object;` |
| `kairo.android.plugin.notification.NotificationActivity$2` | 3160 | `Ljava/lang/Object;` |
| `kairo.android.plugin.notification.NotificationActivity$3` | 3161 | `Ljava/lang/Object;` |
| `kairo.android.plugin.notification.NotificationActivity` | 3162 | `Landroid/app/Activity;` |
| `kairo.android.plugin.notification.NotificationContents` | 3163 | `Ljava/lang/Object;` |
| `kairo.android.plugin.notification.NotificationReceiver` | 3164 | `Landroid/content/BroadcastReceiver;` |
| `kairo.android.plugin.notification.util.TextLayout$TextBlock` | 3165 | `Ljava/lang/Object;` |
| `kairo.android.plugin.notification.util.TextLayout` | 3166 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.EditTextWatcher` | 3167 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.FepPanel$1$1` | 3168 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.FepPanel$1$2` | 3169 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.FepPanel$1$3` | 3170 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.FepPanel$1$4` | 3171 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.FepPanel$1` | 3172 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.FepPanel` | 3173 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$1$1` | 3174 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$1$2` | 3175 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$1$3` | 3176 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$1$4` | 3177 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$1$5` | 3178 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$1` | 3179 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$2` | 3180 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$3$1` | 3181 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$3$2` | 3182 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$3$3` | 3183 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$3` | 3184 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$4` | 3185 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$5` | 3186 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$6` | 3187 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$7` | 3188 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel$8` | 3189 | `Ljava/lang/Object;` |
| `kairo.android.plugin.panel.LanguagePanel` | 3190 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ui.Dialog$1$1` | 3191 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ui.Dialog$1$2` | 3192 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ui.Dialog$1$3` | 3193 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ui.Dialog$1$4` | 3194 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ui.Dialog$1` | 3195 | `Ljava/lang/Object;` |
| `kairo.android.plugin.ui.Dialog` | 3196 | `Ljava/lang/Object;` |
| `main.Main` | 3209 | `Lcom/unity3d/player/UnityPlayerNativeActivity;` |
| `plugin.WebBox` | 3220 | `Ljava/lang/Object;` |
| `plugin.ui.WebBox$1` | 3226 | `Ljava/lang/Object;` |
| `plugin.ui.WebBox$2` | 3227 | `Ljava/lang/Object;` |
| `plugin.ui.WebBox$3` | 3228 | `Ljava/lang/Object;` |
| `plugin.ui.WebBox$4` | 3229 | `Ljava/lang/Object;` |
| `plugin.ui.WebBox$5` | 3230 | `Ljava/lang/Object;` |
| `plugin.ui.WebBox` | 3231 | `Ljava/lang/Object;` |

## Outros — 38 classes

Configuração, utilitários, persistência, licenciamento ou R gerado; função detalhada UNKNOWN.

| Classe binária completa | class_def | Superclasse |
|---|---:|---|
| `kairo.android.plugin.Config` | 3129 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$1` | 3130 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$2` | 3131 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$3$1` | 3132 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$3` | 3133 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$4` | 3134 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$5` | 3135 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility$6` | 3137 | `Ljava/lang/Object;` |
| `kairo.android.plugin.Utility` | 3138 | `Ljava/lang/Object;` |
| `kairo.android.plugin.io.Preference$Base64$1` | 3150 | `Ljava/io/ObjectInputStream;` |
| `kairo.android.plugin.io.Preference$Base64$InputStream` | 3151 | `Ljava/io/FilterInputStream;` |
| `kairo.android.plugin.io.Preference$Base64$OutputStream` | 3152 | `Ljava/io/FilterOutputStream;` |
| `kairo.android.plugin.io.Preference$Base64` | 3153 | `Ljava/lang/Object;` |
| `kairo.android.plugin.io.Preference` | 3154 | `Ljava/lang/Object;` |
| `kairo.android.plugin.licensing.LicenseChecker$1` | 3155 | `Ljava/lang/Object;` |
| `kairo.android.plugin.licensing.LicenseChecker$MyLicenseCheckerCallback` | 3156 | `Ljava/lang/Object;` |
| `kairo.android.plugin.licensing.LicenseChecker` | 3157 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.AsyncRunnable` | 3136 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.AsyncThread` | 3197 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.Battery$1` | 3198 | `Landroid/content/BroadcastReceiver;` |
| `kairo.android.plugin.util.Battery` | 3199 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.ClockTime$1` | 3200 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.ClockTime$ClockReceiver` | 3201 | `Landroid/content/BroadcastReceiver;` |
| `kairo.android.plugin.util.ClockTime$ClockTask` | 3202 | `Ljava/util/TimerTask;` |
| `kairo.android.plugin.util.ClockTime` | 3203 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.Language` | 3204 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.Log` | 3205 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.Referrer` | 3206 | `Landroid/content/BroadcastReceiver;` |
| `kairo.android.plugin.util.SoftkeyboardMonitor` | 3207 | `Ljava/lang/Object;` |
| `kairo.android.plugin.util.StringUtil` | 3208 | `Ljava/lang/Object;` |
| `net.kairosoft.android.soccer_en.R$attr` | 3210 | `Ljava/lang/Object;` |
| `net.kairosoft.android.soccer_en.R$drawable` | 3211 | `Ljava/lang/Object;` |
| `net.kairosoft.android.soccer_en.R$string` | 3212 | `Ljava/lang/Object;` |
| `net.kairosoft.android.soccer_en.R$style` | 3213 | `Ljava/lang/Object;` |
| `net.kairosoft.android.soccer_en.R` | 3214 | `Ljava/lang/Object;` |
| `plugin.Config` | 3217 | `Ljava/lang/Object;` |
| `plugin.Utility$1` | 3218 | `Ljava/lang/Object;` |
| `plugin.Utility` | 3219 | `Ljava/lang/Object;` |

A inclusão de qualquer grupo na segunda fase depende da decisão do usuário. Este inventário não amplia o escopo concluído de C# nem altera classes.dex.
