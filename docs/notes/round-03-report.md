## 1. PROGRESSO

Base: commit `88519635c00d72a7d1dd31f55901d73a643cf226`. Percentuais em frações exatas.

| Assembly | Total | Matched anterior | Percentual anterior | Matched atual | Percentual atual | Ganho |
|---|---:|---:|---|---:|---|---:|
| KairoLibrary | 3.287 | 3.123 | (3123/3287) × 100% | 3.144 | (3144/3287) × 100% | 21 |
| Assembly-CSharp | 1.958 | 1.572 | (1572/1958) × 100% | 1.611 | (1611/1958) × 100% | 39 |
| Total | 5.245 | 4.695 | (4695/5245) × 100% | 4.755 | (4755/5245) × 100% | 60 |

Zero regressões adotadas de corpos/tipos e zero retiradas de estado. Ganhos: 58 novos corpos e dois auxiliares liberados automaticamente.

Kairo: 3.070 corpos iguais, 76 declarações sem CIL aprovadas, dois auxiliares iguais retidos, 141 corpos divergentes. App: 1.606 corpos iguais, seis declarações aprovadas, um auxiliar igual retido, 346 divergentes. Restam 490 métodos fora de matched: 487 divergentes e três retidos.

Metadados de definição: **460/466 → 466/466 tipos**, com igualdade das dimensões globais comparadas. Bytes IL matched: **352.867 → 363.493**, de871.497. Igualdade estrutural não significa igualdade bruta dos arquivos.

## 2. O QUE FOI FEITO NESTA RODADA

Ganhos/perdas abaixo são adotados. Tentativas rejeitadas estão identificadas.

| Frente | Tentativa | Ganhou matched | Perdeu matched |
|---|---|---:|---:|
| A isolado | GetItemType proposto bate; sonda sem promoção. | 0 | 0 |
| Ternários smcs corrigidos | 95 alvos, 13 aprovados. | 13 | 0 |
| Ternários Roslyn corrigidos | 39 alvos, 18 corpos novos e dois auxiliares liberados. | 20 | 0 |
| If/else smcs | 152 alvos, zero ganhos; sete perdas potenciais, variante rejeitada. | 0 | 0 |
| If/else Roslyn | 38 alvos, zero ganhos; duas perdas potenciais, variante rejeitada. | 0 | 0 |
| B / if vazio | Composto e aninhado batem; adotado composto em GetStatusBarHeight. | 1 | 0 |
| Busca pop | Dois métodos encontrados; segundo candidato não bateu integralmente. | 0 | 0 |
| Guarda antecipada | 77 alvos smcs, 23 aprovados, incluindo Tapjoy.Quit. | 23 | 0 |
| Guarda reversa/com valor | Orientações implementadas; nenhum alvo elegível nos filtros usados. | 0 | 0 |
| Temporário de condição | Sete alvos com slot Boolean original, três aprovados, incluindo WebBox.Create. | 3 | 0 |
| Ordem dos blocos smcs | Aplicada nas regras acima, sem dupla contagem; TPlayer.Draw_svisor confirma guardas em sequência. | 0 | 0 |
| LanguageForm | Oito nomes de parâmetros; +1tipo, sem mudar corpos. | 0 | 0 |
| Lambda vs delegate | Troca isolada não resolveu os três caches. | 0 | 0 |
| Escopo das closures | Escopos de laço; +3 tipos, sem perda de corpos. | 0 | 0 |
| Referência mscorlib | Microsoft .NET 2.0; +2 tipos, zero outputs de métodos alterados. | 0 | 0 |
| Revisões Roslyn | 1.1.1 empata; 1.2.1/1.2.2 perderiam18 cada; mantido1.1.0. | 0 | 0 |
| Repetibilidade e bytes brutos | Campos variáveis provados e raw medido; sem normalização/correção. | 0 | 0 |
| Total | | 60 | 0 |

Negativos preservados: nove perdas potenciais de if/else foram rejeitadas; o segundo método com pop não passou; quatro candidatos de temporário Boolean não passaram; guardas com valor não tiveram alvo elegível; apenas trocar sintaxe lambda/delegate não mudou os caches. Inversão por método usa ocorrências não sobrepostas; zero ganhos não esgota a seleção de ramos individuais.

Passaram **65 controles automatizados**, mais fixtures próprias de ifs vazios, escopos de cache, interfaces e três controles da matriz. A proteção de recibos rejeita prova antiga após fonte ou binário mudar, inclusive quando retorno1 pode significar falha de compilação.

A verificação completa final compilou ambos os assemblies, conferiu1.218 entradas originais e5.163 corpos de referência. Retorno1 corresponde às diferenças pendentes; não houve erro de compilação.

## 3. HIPÓTESES

**A — CONFIRMADO: falha de seleção.** Receipt.GetItemType não entrou nos 39testes anteriores. Um filtro excluía o grupo I puro; outro enxergava só brtrue/brfalse e retornava listas vazias para beq/bne.un. A troca !=→== já existia. Compilar a expressão proposta isoladamente reproduziu o original sem perdas; após corrigir filtros, a regra geral aprovou o método.

**B — CONFIRMADO; operador histórico UNKNOWN.** As duas formas vazias reproduzem GetStatusBarHeight integralmente:102 bytes/29instruções. O compilador elimina a comparação que aponta para a instrução seguinte e mantém as cargas/descartes. Oito fixtures, com quatro relações e duas formas, produziram corpos iguais. Portanto >,>=,<,<= são indistinguíveis nessa evidência. A nota do método registra UNKNOWN; > é apenas um representante comprovado.

| Estado | Hipótese e evidência |
|---|---|
| CONFIRMADA | Negação integral e De Morgan: controles de eq/neq, relações, &&/|| passam; float/double/nullable mantêm negação explícita. |
| INCONCLUSIVA | Inversão conjunta de if/else recupera os grupos: zero ganhos em 152/38 alvos; ramos isolados não foram esgotados. |
| CONFIRMADA em casos medidos | Guardas recuperam ordem de blocos:23 métodos, incluindo múltiplas guardas em TPlayer.Draw_svisor. |
| CONFIRMADA em três casos | Temporário Boolean resolve WebBox.Create, GamePad.GetSideViewRatio e TouchEffectManager.Frame. |
| INCONCLUSIVA universalmente | Ordem lexical smcs é útil nos casos aprovados; não foi provada para todo lowering/EH ou todo tipo de temporário. |
| REFUTADA | Havia variáveis capturadas com nomes errados nas três closures: campos capturados já eram iguais; faltavam caches gerados. |
| REFUTADA como causa suficiente | Lambda versus anonymous delegate: nenhuma alteração dos caches. |
| CONFIRMADA | Escopo de laço gera os quatro caches ausentes e aprova três tipos. |
| CONFIRMADA | Referência mscorlib causava ordem errada das interfaces: troca única preserva todos os métodos e aprova os dois iteradores. |
| CONFIRMADA no conjunto medido | Dois pares Roslyn indistinguíveis; série 1.2 perde18 matches contra1.1. |
| CONFIRMADA para os candidatos | Quatro campos Kairo e dois App variam nos pares de builds, sem bytes inexplicados. |
| INCONCLUSIVA | Executável/comando/referências históricos exatos e determinismo dos originais permanecem UNKNOWN. |

A heurística de escopo/cache também é sustentada pelo fonte oficial Roslyn 1.1:
https://raw.githubusercontent.com/dotnet/roslyn/version-1.1.0/src/Compilers/CSharp/Portable/Lowering/LambdaRewriter/LambdaRewriter.cs .
As aprovações vieram da compilação e do verificador.

## 4. PENDENTES POR GRUPO

H=cabeçalho; L=locais; I=instruções/operandos/desvios; E=EH/seções adicionais. Grupos exclusivos, sem presumir causas.

| Grupo | Kairo | App | Total |
|---|---:|---:|---:|
| H+I | 51 | 155 | 206 |
| H+L+I | 25 | 121 | 146 |
| H+I+E | 17 | 28 | 45 |
| H+L+I+E | 17 | 20 | 37 |
| L+I | 17 | 10 | 27 |
| I | 13 | 11 | 24 |
| L | 0 | 1 | 1 |
| L+I+E | 1 | 0 | 1 |
| Total | 141 | 346 | 487 |

Os três retidos não entram nessa tabela.

**H+I:** `KairoLibrary::kairo.unity.m3d4.util3d.FastMath::Abs(System.Single)->System.Single`.
Tiny/MaxStack 8/sem locais; 13 versus18 bytes.

| Original | Recompilado |
|---|---|
| 0000 ldarg.0 | 0000 ldarg.0 |
| 0001 ldc.r4 0 | 0001 ldc.r4 0 |
| 0006 blt.s 000a | 0006 blt.s 000a |
| 0008 ldarg.0 | 0008 ldarg.0 |
| 0009 ret | 0009 ret |
| 000a ldarg.0 | 000a ldc.r4 0 |
| 000b neg | 000f ldarg.0 |
| 000c ret | 0010 sub |
| — | 0011 ret |

**H+L+I:** `KairoLibrary::kairo.unity.ui.Seb::GetWorkValue()->kairo.unity.ui.Seb+WorkValue`.
Fat/MaxStack 3/InitLocals; um local WorkValue contra dois; 25 versus27 bytes.

| Original | Recompilado |
|---|---|
| 0000 ldarg.0 | 0000 ldarg.0 |
| 0001 ldfld Seb::threadLocal_ | 0001 ldfld Seb::threadLocal_ |
| 0006 stloc.0 | 0006 stloc.0 |
| 0007 ldloc.0 | 0007 ldloc.0 |
| 0008 brtrue.s 0017 | 0008 brtrue.s 0019 |
| 000a ldarg.0 | 000a ldarg.0 |
| 000b newobj WorkValue::.ctor() | 000b newobj WorkValue::.ctor() |
| 0010 dup | 0010 dup |
| 0011 stloc.0 | 0011 stloc.1 |
| 0012 stfld Seb::threadLocal_ | 0012 stfld Seb::threadLocal_ |
| 0017 ldloc.0 | 0017 ldloc.1 |
| 0018 ret | 0018 stloc.0 |
| — | 0019 ldloc.0 |
| — | 001a ret |

**H+I+E:** ``Assembly-CSharp::system.billing.ui.StoreBase::GetIncompletePayments()->System.Collections.Generic.List`1<system.billing.ui.Receipt>``.
Fat/MaxStack 13/InitLocals; mesmos três locais; 101 versus91 bytes. Trecho central:

| Original | Recompilado |
|---|---|
| 001f ldloc.1 | 001f ldloc.1 |
| 0020 ldfld Receipt::paymentComplete_ | 0020 ldfld Receipt::paymentComplete_ |
| 0025 brfalse 002f | 0025 brtrue 003c |
| 002a br 0046 | 002a ldloc.1 |
| 002f ldloc.1 | 002b callvirt Receipt::IsTicketProduct() |
| 0030 callvirt Receipt::IsTicketProduct() | 0030 brfalse 003c |
| 0035 brtrue 003f | 0035 ldloc.0 |
| 003a br 0046 | 0036 ldloc.1 |
| 003f ldloc.0 | 0037 callvirt List<Receipt>::Add() |
| 0040 ldloc.1 | 003c ldloca.s V_2 |
| 0041 callvirt List<Receipt>::Add() | 003e call Enumerator::MoveNext() |
| 0046 ldloca.s V_2 | 0043 brtrue 0017 |
| 0048 call Enumerator::MoveNext() | 0048 leave 0059 |
| 004d brtrue 0017 | — |
| 0052 leave 0063 | — |

| EH, limites exclusivos | Original | Recompilado |
|---|---|---|
| try | [0012, 0057) | [0012, 004d) |
| finally | [0057, 0063) | [004d, 0059) |

Offsets hexadecimais e operandos abreviados só na apresentação. Listagens completas estão no resumo JSON.

## 5. TOOLCHAIN

Kairo mantém **Microsoft.Net.Compilers 1.1.0/csc 1.1.0.51109**. SHA pacote:
`cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745`;
SHA csc:
`53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`.

```text
/nologo /noconfig /nostdlib+ /target:library /platform:anycpu
/optimize+ /debug:pdbonly /deterministic- /langversion:6
/unsafe- /checked- /codepage:65001 /utf8output
```

Mudou apenas a primeira referência Kairo: Microsoft .NET Framework 2.0,
C:/Windows/Microsoft.NET/Framework/v2.0.50727/mscorlib.dll,
SHA`c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558`.
Ordem:mscorlib, System.Core3.5, UnityEngineAPK, System2.0. System.Core/System continuam do Unity clássico. Framework64 tem a mesma identidade/ordem de interfaces, mas arquivo diferente e não foi selecionado.

App mantém **smcs 2.6.5.0/Unity 5.3.4f1**, SHA
`26b4f7cde44cb44fe79c4981f17235de6c477df20c8daef4509903b69cc56fc0`.

```text
-nologo -noconfig -target:library -platform:anycpu -optimize+
-debug- -langversion:Default -unsafe- -checked- -codepage:65001
```

Sem -nostdlib; ambiente Mono Unity local ao subprocesso; referências APK mscorlib, KairoLibrary, UnityEngine, System.Core, System. Sem defines extras; noconfig fora do response file nos dois. Nenhuma instalação global nova.

Os três pacotes pedidos existem, foram adquiridos portáteis e tiveram seis componentes fixados por hash:

| Pacote | Executável | Corpos iguais | Ganhos/perdas vs1.1.0 | Exclusivos individuais |
|---|---|---:|---|---:|
| 1.1.0 | 1.1.0.51109 | 3.070 | referência | 0 |
| 1.1.1 | 1.1.0.51202 | 3.070 | +0/-0 | 0 |
| 1.2.1 | 1.2.0.60325 | 3.052 | +0/-18 | 0 |
| 1.2.2 | 1.2.0.60425 | 3.052 | +0/-18 | 0 |

Dois pares idênticos nos métodos inspecionados; 18 matches exclusivos do par 1.1 contra o par 1.2; 34 outputs diferentes entre séries. Todos preservam337/337 tipos, 76 declarações, zero IDs ausentes/adicionais. Mantido1.1.0.

UNKNOWN: compilador histórico entre candidatos indistinguíveis, comando original completo, hashes históricos das referências e determinismo original.

## 6. COMPONENTES EXTERNOS E METADADOS

As sete DLLs gerenciadas e seis bibliotecas nativas continuam identificadas contra os componentes oficiais Unity 5.3.4f1. UnityEngine Android/Mono tem970.752 bytes eSHA
`7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`.
Nenhum componente do APK mudou. DEX/Java ficou excluído, sem análise nesta rodada.

**466/466 tipos aprovados estruturalmente, sem falhas de definição restantes.**
LanguageForm recebeu oito nomes de parâmetros(form, data). As closures já tinham capturas corretas; faltavam:
- ResourceManager/<>c__DisplayClass51_0: <>9__0.
- HttpObject/<>c__DisplayClass10_1: <>9__1.
- FormManagerBase/<>c__DisplayClass85_1: <>9__1 e <>9__2.

Caches gerados pelo escopo do C# de origem, sem escrever classes geradas.

Métodos com yield:
- ``kairo.unity.util.FastVector`1::GetEnumerator()->System.Collections.IEnumerator``.
- ``java.util.Vector`1::GetEnumerator()->System.Collections.IEnumerator``.

Ambos retornam IEnumerator não genérico, não têm implementação explícita delegadora. Interfaces dos tipos gerados:
- Original: IEnumerator<object>, IDisposable, IEnumerator.
- Referência Unity anterior: IEnumerator<object>, IEnumerator, IDisposable.
- Referência Microsoft atual: IEnumerator<object>, IDisposable, IEnumerator.

Ordem de tipos/membros, parâmetros, atributos, layout, constantes e recursos passam. Ambos têm zero recursos embutidos e zero diferenças globais nas dimensões estruturais comparadas.

**Campos variáveis:**

| Assembly | Campo | Tamanho | Bytes alterados no par de prova |
|---|---|---:|---:|
| Kairo | Module.Mvid | 16 | 16 |
| Kairo | COFF.TimeDateStamp | 4 | 1 |
| Kairo | IMAGE_DEBUG_DIRECTORY.TimeDateStamp | 4 | 1 |
| Kairo | CodeView.RSDS.PdbGuid | 16 | 16 |
| App | Module.Mvid | 16 | 16 |
| App | COFF.TimeDateStamp | 4 | 1 |

Kairo34 bytes; App17; zero fora dos intervalos. App debug- não emitiu diretório de depuração/CodeView. Prova App:97 fontes congelados do início da rodada. Prova Kairo:163 fontes finais/referência Microsoft. Nenhuma normalização, inclusive na medição bruta.

**Bytes diferentes ou sem par por região PE:**

| Região | Kairo | App |
|---|---:|---:|
| Cabeçalhos | 10 | 18 |
| .text | 482.986 | 812.508 |
| .rsrc | 0 | 1 |
| .reloc | 1 | 0 |

**Streams/heaps:**

| Stream | Kairo | App |
|---|---:|---:|
| #~ | 16.245 | 6.661 |
| #Strings | 0 | 0 |
| #US | 299 | 12.466 |
| #GUID | 16 | 16 |
| #Blob | 20.877 | 11.981 |

#Blob Kairo inclui84 bytes novos sem par. App:8 originais sem par em #~, 4 novos em #US e40 novos em #Blob. Demais streams não têm caudas sem par.

**Tabelas brutas com diferenças:**

| Tabela | Kairo | App |
|---|---:|---:|
| TypeRef | 225 | 0 |
| TypeDef | 3 | 0 |
| Field | 2982 | 680 |
| MethodDef | 6204 | 4343 |
| MemberRef | 2397 | 1 |
| Constant | 1120 | 0 |
| CustomAttribute | 242 | 1 |
| StandAloneSig | 621 | 625 |
| Property | 66 | 5 |
| TypeSpec | 179 | 1 |
| FieldRVA | 168 | 434 |
| AssemblyRef | 3 | 0 |
| MethodSpec | 53 | 1 |

Demais tabelas presentes:zero diferenças brutas. Contagem=bytes diferentes no prefixo comum mais caudas sem par, sem preencher com zeros. Seções, streams e tabelas são camadas sobrepostas; não somar. Índices/RVAs/campos variáveis entram na medição; não são contagens de erros semânticos. Não houve tentativa de correção motivada por raw.

Arquivo inteiro, posicional:Kairo569.856→569.856 bytes, 482.997diferentes; App895.488→893.440 bytes, 811.973diferentes no prefixo comum+2.048 originais sem par=814.021. Snapshots e builds verificados têm os mesmos hashes.

## 7. BLOQUEIOS

Nenhuma instalação, arquivo ou decisão do usuário é necessária. Restam487 corpos divergentes e três vínculos retidos:KairoBase/<>c .ctor/.cctor e App <PrivateImplementationDetails> .ctor.

Não há tipos estruturais pendentes. Os bytes brutos foram medidos, mas não corrigidos; a interpretação semântica de todas as colunas brutas não foi feita. Identidade histórica exata continua UNKNOWN. DEX/Java permanece excluído até concluir C#.

## 8. SUA RECOMENDAÇÃO

1. Guardas com continue em foreach/try, como StoreBase.GetIncompletePayments:45 alvos H+I+E, teto45 corpos, ganho efetivo UNKNOWN. O exemplo mostra duas guardas fundidas e10 bytes de fluxo eliminados.
2. Reutilização de locais/atribuições encadeadas, como Seb.GetWorkValue:146 alvos H+L+I, teto146, ganho efetivo UNKNOWN.
3. Diferenças aritméticas simples, começando por neg versus0-sub em FastMath.Abs:206 alvos H+I, teto206, ganho efetivo UNKNOWN. Preservar comparações float; cada transformação precisa do verificador. Não procurar versões indistinguíveis sem falha nova que justifique.
