# Round 11: comparação de operandos numéricos

O repertório `r11-c5-mirror-primitive-operands` admite identificadores de locais ou parâmetros por valor e constantes numéricas primitivas, quando os `ConvertedType` são iguais. Exige pelo menos um identificador; rejeita campos, chamadas, parâmetros ref, dois identificadores de tipos diferentes, Boolean e comparações de duas constantes. Inverte somente a ordem dos operandos e o operador espelhado; conserva a negação externa e não complementa uma comparação de ponto flutuante.

O helper aceita a regra primária C5 ou sua presença no repertório retido. O filtro de primeiro passo do motor continua exigindo a família selecionada pelo ciclo. Assim, reter o molde não permite adotar uma hipótese antiga antes de testar a nova família.

`reports/round11-c5-literal-mirror-fixtures.json` registra oito métodos positivos e 48 requisições de análise: regra primária, regra retida e desabilitada, para ambos os compiladores. Seis negativos foram recusados. Quatro compilações de bibliotecas próprias produziram corpos candidatos idênticos aos controles manuais nos dois compiladores; as verificações de execução incluíram NaN, infinidades e zero com sinal. Nenhum assembly alvo foi compilado nesses controles.

O primeiro passe C5 do Assembly-CSharp ainda usava o molde restrito a identificadores. Sua auditoria separada está em `reports/round11-c5-app-initial-mirror-audit.json`: um mirror gerado em Player.Collision, vetado pela assinatura completa de locais, zero mirrors compilados e zero ganhos desse molde. Map.SetCoatLine fechou por oito subtrações Int32 recuperadas; esse ganho não prova o molde de comparações. A ampliação com literais exige novos passes C5 nos dois assemblies e permanece hipótese para o código do jogo até esses testes.
