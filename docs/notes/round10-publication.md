# Publicação das propostas da rodada 10

Os pacotes `round10-*-patches.json` publicados nesta etapa são hipóteses C#
congeladas com fixtures próprias. `UNKNOWN` nesses pacotes descreve o momento
da publicação. Só o motor, a compilação real e o verificador podem comprovar o
resultado no jogo; os recibos de adoção e o diário registram esse resultado.

Os produtores puros foram copiados com os mesmos bytes. As provas privadas
ficam em `build/round10`, com hashes conferidos pelo motor; fontes das fixtures
próprias e seus recibos foram publicados. Não foram publicados APKs, DLLs,
chaves, capturas do aparelho ou logs privados.

Na cópia pública de uma fixture de `smcs-guard-slot-compositions`, o preflight
detectou espaços ao fim das linhas 17, 21 e 28. A primeira checagem do pequeno
formatador supôs que as linhas eram vazias e recusou a operação antes de
escrever qualquer arquivo. A inspeção mostrou três linhas de código com um
espaço final. O formatador passou a exigir exatamente esse caso, protegeu os
literais e comprovou que todos os tokens C# eram iguais antes e depois.

O recibo `round10-smcs-guard-slot-fixture-format.json` registra os dois hashes;
o pacote publicado registra o hash da cópia formatada. A fixture privada que
foi compilada e suas provas ficaram intactas. Essa recuperação não compilou
o jogo, não alterou fonte alvo e não mudou estados de métodos.

As três propostas Roslyn de escopo, local em corpo de laço e dispatch têm
primeiras divergências diferentes. A fila as separa para testes em ciclos
distintos; não é inferida uma única regra de emissão a partir das três.
