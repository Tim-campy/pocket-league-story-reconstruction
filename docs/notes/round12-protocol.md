# Rodada 12: protocolo de execução

Base: a075dcc8ca7b08e2cb9fcdd0cbcb5ce1fe8cd236. Baseline própria: build/round12/baseline. Round11 permanece histórico somente leitura.

A Parte A tem contagem própria, sem teto de 40 nem parada de cinco zeros. Reconstrói somente KairoLibrary, começa pelos quatro candidatos congelados (251 + 396 bytes IL) e continua até não haver corpo divergente nem auxiliar retido. Hipótese sem avanço exige diagnóstico novo, sem repetir fontes idênticos. Somente o verificador escreve os estados.

Após o fechamento semântico, root compara cada byte do PE congelado com o original. O gate B exige recibo PASS ligado por SHA ao próprio PE, zero métodos divergentes, zero retidos e zero bytes diferentes fora dos quatro campos comprovados. Contagens e igualdade estrutural não substituem esse teste. Não há normalização nem edição de DLL. Se o bruto divergir, diagnosticar a causa antes de declarar A concluída.

B só começa depois do PASS. Tem limite próprio de 40 ciclos e parada após cinco ciclos consecutivos sem matched novo, com fila vazia antes e depois. Fila explícita: UserTraining, SendSave, GameForm._draw, Draw_battlePic e Camera.Update_pos; depois selecionar o maior peso residual por IL. No máximo dois ciclos consecutivos com o mesmo principal em cada parte.

Cada ciclo tem sonda e passe principal em TODOS os corpos pendentes do assembly. Predição completa errada recusa; incompletude genuína permite 12 variantes por método/passe e 24/ciclo. Toda compilação candidata exige assinatura original exata e aprovação integral de cabeçalho, locais, IL e EH. Apenas adoção oficial e verificação completa geram ganho. Não somar ganho da sonda ao principal.

Nenhuma classe gerada é escrita manualmente; nenhum byte original é transplantado. Original/build/chaves/binários ficam fora de Git. Falha de commit após diário exige reparar só prova/commit, nunca duplicar ciclo. Ao parar, entregar propriedade ao root para QA e relatório final de oito seções; não executar finish por conta própria.

Doctor inicial pelo root: MCP gerenciado 6.5 disponível; exit1 por Hopper/IDA/JADX opcionais ausentes e drift. Sem instalação ou reparo. Assembly-CSharp continua referenciando KairoLibrary ORIGINAL.

Errata documental após A01: a pipe PowerShell corrompeu acentos da nota inicial. Este texto foi regravado com OutputEncoding UTF8 explícito. Nenhum C# de jogo, produtor congelado ou corpo medido foi alterado pela correção.
