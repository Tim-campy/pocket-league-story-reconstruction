# Rodada 12: protocolo de execu??o

Base: a075dcc8ca7b08e2cb9fcdd0cbcb5ce1fe8cd236. A baseline pr?pria est? congelada em build/round12/baseline. Round11 permanece hist?rico somente leitura.

A Parte A tem contagem pr?pria, sem teto artificial de 40 ou parada de cinco zeros. Reconstr?i somente KairoLibrary, come?a pelos quatro candidatos congelados (251 + 396 bytes IL) e continua at? n?o haver corpo divergente nem auxiliar retido. Uma hip?tese sem avan?o exige diagn?stico novo: n?o se repetem fontes id?nticos. O verificador continua sendo o ?nico escritor de estados.

Depois desse fechamento sem?ntico, a produ??o para para o root comparar cada byte do PE congelado com o original. O gate B exige PASS num recibo hash-bound ao pr?prio PE, zero m?todos divergentes, zero retidos e zero bytes diferentes fora dos quatro campos comprovados. Igualdade estrutural ou contagem de m?todos n?o substitui esse teste. N?o h? normaliza??o nem edi??o de DLL. Se o bruto divergir, a causa fica em diagn?stico antes de declarar A conclu?da.

A Parte B s? come?a depois do PASS. Tem seu pr?prio limite de 40 ciclos e parada ap?s cinco ciclos consecutivos sem matched novo, com fila vazia antes e depois. A fila expl?cita ? UserTraining, SendSave, GameForm._draw, Draw_battlePic e Camera.Update_pos; depois a sele??o usa o maior peso residual por IL. No m?ximo dois ciclos consecutivos com o mesmo m?todo principal em cada parte.

Cada ciclo tem sonda isolada e passe principal em TODOS os corpos pendentes do assembly. Predi??o completa com tipos errados recusa; incompletude genu?na permite at? 12 variantes por m?todo/passe e 24/ciclo. Toda compila??o candidata ainda precisa da assinatura original exata e de aprova??o integral de cabe?alho, locais, IL e EH. S? a ado??o oficial e verifica??o completa podem gerar ganho. Ganho da sonda n?o se soma ao principal.

Nenhum auxiliar gerado ? escrito ? m?o. Nenhum byte original ? transplantado. Original/build/chaves/bin?rios n?o entram em Git. Falha de commit ap?s di?rio n?o autoriza repetir ciclo: preserva-se o resultado e refaz-se apenas a prova/commit. Ao parar, o operador entrega a propriedade ao root para QA e relat?rio final de oito se??es; n?o executa finish por conta pr?pria.

Doctor inicial executado pelo root: MCP gerenciado6.5 dispon?vel; exit1 por ferramentas opcionais Hopper/IDA/JADX ausentes e drift. N?o houve instala??o ou reparo. A refer?ncia de compila??o de Assembly-CSharp continua sendo a KairoLibrary ORIGINAL.
