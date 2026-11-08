# Primeiro guard de continue e corpo positivo

Entrega privada READY_FOR_GAME_PROBE. Resultados dos alvos e regressões oficiais UNKNOWN. Zero compilações do jogo, verificadores oficiais, mudanças públicas ou de ledger. Pacotes anteriores imutáveis. Somente estes dois métodos foram estudados.

## Identidades e medidas

Base oficial B13 em `build/round12/runs/b13/commit/postverify01/Assembly-CSharp`. Retornos e assinaturas confirmados:

| ID | Token | IL original/atual | maxstack | Locais | EH |
|---|---|---:|---:|---:|---:|
|data.TenantData::StrengthTenant()->System.Void|0x060000e1|102/102|5|2|0|
|system.billing.ui.BillingManager::AddPurchaseLog(System.String,System.Int32,system.billing.util.Purchase)->System.Void|0x060005fe|164/164|16|4|0|

Total266IL, cabeçalhos fat12/flags19/initlocals=true. As primeiras diferenças brutas e causais são Tenant39 e Billing66, brtrue original contra brfalse reconstruído. Tamanho/cabeçalho/assinatura iguais não bastam: os corpos diferem na organização de guardas dentro de laços.

REA primário: Tenant `ev_dab38a5d53cf70ce45a459db377647395642b8372b5aa7f613fa4bb090205ecc`; Billing `ev_8554e98ebe240f580452bc153157548d1647ca1f3f33c4aa477c2fdb14b2a076`. Artefato Assembly-CSharp original SHA256 `3620ed91bd3a99cba9901cea111e6f6e489474e5c0b97b99676e5d4623817949`, MVID `48ce9f89-73d2-40be-bb24-22c807c04106`. Os bindings por método guardam hash dos bytes exatos de IL conferido no dump, artefato, fonte/manifest/receipt/dumps, CRLF e spansUTF16. Before de cada método ocorre uma vez no Main observado; futuras alterações nesses arquivos exigem rebase único por corpo e vínculo fresco.

## Tenant: guard de estado e teste positivo de identidade

Fonte atual: uma condição OR de falha, `state==0 || tenantDataId!=id_`, seguida de continue; depois StrenghTenant. Ela compartilha uma instrução br de continue entre os dois testes, mas produz brfalse39 para esse bloco e beq56 para a chamada.

Original: `ldfld state_@34; brtrue39→49; br44→72`. Portanto, o primeiro teste é um guard independente: `if(tenant.state_==0){continue;}`. O destino72 inicia o incremento do for (ldloc0,ldc1,add,stloc0), seguido da condição76 e blt89→20. Não é saída do método nem retorno.

Após esse guard, o original compara tenantDataId_ com id_ e faz `bne.un61→72`; a chamada StrenghTenant está em66–67. A forma correspondente é um corpo positivo `if(tenant.tenantDataId_==id_){tenant.StrenghTenant();}`. Um segundo guard de continue emitiria um br extra, que não existe no original. A composição mantém o guard lv_==5/ret12 e a chamada ChangeState(2)@96 depois do laço.

## Billing: guard de produto e corpo positivo de ordem

Fonte atual: `!product.Equals(productId) || (text!=null && !order.Equals(text))`, continue compartilhado, depois atribuição e break.

Original: `Equals@61; brtrue66→76; br71→106`. O destino106 decrementa num (ldloc2,ldc1,sub,stloc2), seguido da condição110/bge112→36. É continue do for descendente. A proposta separa somente o primeiro guard: `if(!purchaseRecord2.productId_.Equals(productId)){continue;}`.

Depois, `ldloc1@76; brfalse77→99` evita chamar order.Equals quando text é null; `Equals@89; brfalse94→106` evita a atribuição quando a ordem não coincide. A proposta envolve as folhas em `if(text==null || purchaseRecord2.orderId_.Equals(text)){purchaseRecord=purchaseRecord2;break;}`. O break@101→117 sai do for antes das verificações de criação do registro;117 não é ret. As condições posteriores de criação/ChangeState ficam intactas, com ret final163.

Os dois métodos compartilham o esqueleto primeiro guard de continue + corpo positivo restante. Os predicados e as folhas são diferentes: Tenant usa igualdade inteira e queda implícita no incremento; Billing usa OR com null e chamada Equals, atribuição e break do laço. Não se invertem todos os operadores nem se geram dois continues. Ordem e quantidade de avaliações, short-circuit de null e possíveis efeitos/exceções de Equals são preservados.

## Provas de emissão, modelos e cálculo

Mono fixado revisão `b6587926c0f146c8b12985744f33a13b39a22d57`, `analysis/tools/round5-mono-source/mcs/mcs`:

- expression.cs3418–3442, Binary.EmitBranchable, trata igualdade com zero/null por teste simples;3472–3481 implementa OR com caminhos de short-circuit. Isso explica os testes de state e de text sem materialização Boolean.
- statement.cs163–218 registra a inacessibilidade do fim de um braço com continue/break;225–267 emite o falso do if e não introduz else/br de junção desnecessário quando o corpo é a folha positiva final.
- statement.cs1117–1119 e1148–1150 emitem break para LoopEnd e continue para LoopBegin; os destinos reais106/117 no Billing e72 no Tenant foram conferidos no IL e vinculados ao laço correto.

NoEmit genuíno, helper existente/hash conferido: Tenant complete=true, `[Int32,game.Tenant]` exato; Billing complete=true, `[PurchaseRecord,String,Int32,PurchaseRecord]` com namespaces completos exatos. Before e after têm as mesmas assinaturas. Não há slot novo nem previsão completa errada relaxada. Todos os usos e âncoras constam dos slot-constraints por método.

A relocação diagnóstica de instruções reconstruídas, preservando os operandos resolvidos dos membros existentes, coincide em todos38objetos originais de Tenant e64 de Billing, tamanhos102/164. É cálculo analítico, não IL emitido no alvo nem aprovação do oráculo. Nenhum byte original foi copiado para um resultado.

## Fixtures e negativos

Dez compilações smcs2.6.5 de fixtures próprias, cinco por método, referências/flags fixadas, saída privada Fixture.dll e `-debug-`. Controles manuais completos com variáveis renomeadas coincidem integralmente com os candidatos em cabeçalho/locais/IL/EH. Os stubs são próprios, não assemblies do jogo.

| Grupo | Before | Composição/controle | Dois guards de continue | Somente corpo positivo composto |
|---|---:|---:|---:|---:|
|Tenant|102|102|107|97|
|Billing|164|164|169|159|

Todos os negativos diferem. Mesmo before e composição com tamanhos iguais têm IL diferente. As formas isoladas antigas foram mantidas como controles negativos próprios, não recompiladas como novas tentativas de alvo. Uma projeção adicional verifica offsets/opcodes/ramos/slots/constantes e nomes de folhas contra o original; exclui namespaces/assemblies dos stubs e não é o oráculo.

Três negativos do produtor rejeitam folha Tenant duplicada, break Billing trocado por continue e direção do for invertida. O primeiro ensaio puro revelou que o produtor aceitaria uma chamada Tenant extra fora da região textual; foi restringido para exigir uma ocorrência única. Esse erro do produtor foi corrigido antes do freeze. Os candidatos próprios já compilados e os after reais permaneceram byte idênticos, conferidos sem nova compilação. Total real permanece dez compilações próprias e zero do jogo.

## Histórico e entrega

Tenant:93receipts,11trials. R4 adotou loop-continue-guard, prefixo16→39. R4–R7 tentaram dois guards de continue, materialização de condição, empty-arm e positive-tail composto, sem avanço; R10C04 root-guard-positive-tail regrediu para7. Os dois guards isolados e o positive-tail composto têm os mesmos mecanismos dos negativos107 e97 próprios, não da composição atual.

Billing:93receipts,13trials. R4 adotou loop-continue-guard,32→66; R4–R7 tentaram dois guards, condition-local, agrupamentos, empty-arm e positive-tail composto, sem avanço. Os payloads históricos de dois guards e positivo composto foram lidos: não contêm o primeiro guard separado seguido apenas pelo corpo positivo restante. A proposta atual é essa combinação estreita, não repetição das variantes antigas. Não apareceu hash da proposta inteira; receipts sem payload/SHA não excluem universalmente toda forma histórica. Zero candidatos não é refutação.

APIs puras: `transform_strength(method_source)->str` e `transform_purchase_log(method_source)->str`, em calculate.py. Parâmetro opcional form serve aos negativos próprios. Bundles/regras são separados por método; ready-index.json aponta ambos e o freeze. A produção e a verificação completa com zero regressões pertencem ao executor. Nenhum método/byte foi contado como ganho nesta entrega.
