# Referência e cadastros Mono

O inventário usa as evidências `inspect_managed_members` do REA como fonte principal. As DLLs e o APK permanecem somente leitura em `original/`. O conteúdo extraído já foi comparado com as entradas do APK, byte por byte.

## Evidências

| Assembly | Métodos | Corpos CIL presentes | Evidência REA |
|---|---:|---:|---|
| Assembly-CSharp | 1.958 | 1.952 | `ev_778d8b5a959e49d1174f10279c82263dcc1d68908fd0e7731d5d7439d69ab14d` |
| KairoLibrary | 3.287 | 3.211 | `ev_3288139fe486732ee984d8884bb8d1a6dfc31ad6b54b3675a60a422e5fed889b` |

O pacote completo está em `analysis/rea-evidence-2026-11-05.json`. Os 82 métodos sem corpo continuam cadastrados; isso não os caracteriza automaticamente como funções externas ou removidas.

## Identidade e ordem

O ID de método contém assembly, nome completo do tipo, nome do método, aridade genérica, parâmetros e retorno. O token original permanece em coluna separada. NestedClass resolve a cadeia dos tipos aninhados, que usam `+` no identificador; eles compartilham o futuro arquivo-fonte do tipo externo.

`types.csv` preserva a ordem das linhas de TypeDef. Os arquivos de `compile-order/` são uma ordem inicial derivada dessas linhas, não prova da linha de comando histórica. Nenhum tipo ou membro original foi renomeado. Os nomes abreviados dos arquivos de `ref/` servem apenas para respeitar os caminhos do Windows; as identidades completas ficam no conteúdo e nos CSVs.

Membros com `CompilerGeneratedAttribute` ou contidos em tipos gerados são registrados sem inventar uma origem. `gerado_por=UNKNOWN` precisa ser resolvido antes de conferir o grupo compilado. Não serão escritos fontes separados para closures ou iteradores gerados.

## Geração

`tools/rea_metadata.mjs` importa leitores da instalação existente do REA 6.5.0 para complementar as evidências retidas com TypeRef, NestedClass, TypeSpec, StandAloneSig e instruções completas. O adaptador verifica a versão e registra hashes dos módulos usados. Ele lê bytes; não carrega nem executa o jogo. Trata-se de uma API interna, cuja atualização exige revisar o adaptador.

Os complementos ficam em `analysis/metadata/`. Com esses arquivos e as evidências retidas, `python tools/extract` gera os cadastros e as listagens, recusando reiniciar um cadastro existente. A extração deliberada de um novo projeto exige primeiro prover suas referências REA correspondentes; não se deve apagar o cadastro para reiniciar progresso.

## Limites

As listagens são referência de análise e não um projeto ILAsm. Tokens que ainda não têm nome no mapa são preservados como coordenadas originais. O futuro verificador não poderá declarar correspondência enquanto houver resolução incompleta relevante.

O REA reportou leitura parcial de strings: 796 avisos em Assembly-CSharp e 2 em KairoLibrary, relativos ao byte terminal de entradas `#US`. O texto legível foi preservado. Os avisos não demonstram corrupção ou comportamento do runtime.

As categorias de DEX e bibliotecas nativas indicam componentes excluídos do escopo C#; não comprovam a origem de cada classe ou função. Componentes de assinatura também não entram na métrica de métodos.
