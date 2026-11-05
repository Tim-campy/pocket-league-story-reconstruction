# Dois casos de ordem: construtores vazios omitidos

O planejamento da ordem dos membros não encontrava solução para `kairo.unity.ui.IApplication` e `kairo.unity.system.KairoBase` porque faltava um membro no conjunto de declarações recuperadas.

Na KairoLibrary original, o construtor `.ctor()` é o **primeiro MethodDef** de ambos os tipos. No fonte ILSpy atual ele não está declarado; na recompilação ele aparece como construtor implícito no fim dos métodos, antes de `.cctor()`. Os dois tipos mantêm o mesmo atributo `BeforeFieldInit`; não é necessário declarar um construtor estático explícito.

Isso sustenta a hipótese de que o fonte original declarava um construtor vazio e o decompilador o omitiu. A posição histórica exata dessa declaração não é recuperável apenas pelos metadados, mas é possível criar uma declaração C# legítima que satisfaça simultaneamente a ordem dos métodos e as restrições dos nomes gerados.

`tools/plan_kairo_order_extra.py` introduz um membro de peso 1 para o construtor, com a visibilidade pública observada no MethodDef, e resolve novamente a intercalação das ordens de campos, métodos e tipos aninhados. Escreve candidatos somente em `analysis/variants/kairo-order-extra/`, sem mudar os fontes principais. Todos os membros existentes são preservados como fatias exatas do fonte, usando os offsets UTF-16 capturados pelo parser.

| Método | Slot atual | Slot exigido pelo nome original | Slot do candidato |
|---|---:|---:|---:|
| `IApplication.SetResolutionScaleRatio(float)` | 107 | 108 | 108 |
| `KairoBase.Start()` | 65 | 55 | 55 |
| `KairoBase.GetTapjoy()` | 187 | 188 | 188 |

O construtor continua sendo C# compilável com os inicializadores dos campos já recuperados; nenhum byte de IL é copiado do original. Satisfazer as restrições de slots ainda não comprova correspondência: os candidatos devem passar pela compilação integral, comparação dos nomes gerados e verificação de regressões antes de qualquer aplicação ao fonte principal.

Plano e metadados observados: [KairoLibrary-order-unresolved.json](../../reports/KairoLibrary-order-unresolved.json). Mapa dos candidatos: `analysis/kairo-order-extra-source-map.json`, fora do Git.
