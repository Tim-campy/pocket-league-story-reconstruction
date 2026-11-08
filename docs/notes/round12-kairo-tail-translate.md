# R12 — trio final privado,207bytes de IL original

Três propostas independentes; nenhuma compilação do jogo, verifier, estado
ou arquivo público foi alterado. As hipóteses foram confirmadas em fixtures
próprias, mas os três resultados reais permanecem **UNKNOWN**.

Original comum: artifact SHA256
`bb553d341eecd9b900f772486bf4a5c289aa2f09566d9d402a3eb70268f4b3ee`,
MVID `b564fa31-554d-4b48-a8c0-a0b41bea2550`. Página REA dos112métodos
selecionados: `ev_9fc2e95fbf6ed5952abaf7c7c9fc3bcc5a71554641bf9ab5d7bd20f0223fb04b`.
Os hashes CIL individuais abaixo não são hashes de assembly.

## RecordStore.RemoveAllRecord —94bytes

ID `kairo.unity.io.RecordStore::RemoveAllRecord()->System.Void`,
token0x06000892, CIL SHA256
`cef6ce08f6f752c3258142c4b6c9c373f09dc7f7277d381cf0b3d4839b828eb7`.
Atual99bytes, cinco locais; original94bytes, três: String[],Int32,String.
O primeiro ramo foreach já corresponde integralmente. No segundo, o atual
for usa array nomeado3/índice4; o original reutiliza array0/índice1 e passa
a String diretamente na pilha à chamada estática RemovePreference.

Proposta: trocar **somente o segundo for** por
`foreach(string key in KairoPlugin.GetPreferenceKeys()){KairoPlugin.RemovePreference(key);}`.
O primeiro foreach/filename e seus calls ficam iguais. O primeiro filename
continua materializado, pois a chamada de instância exige carregar o
receiver storage antes do argumento. A chave do segundo foreach é consumida
diretamente pela chamada estática; os temporários sintetizados de array e
índice podem reutilizar os slots liberados no outro ramo.

Ownfixture: before99/5locais; candidato/controle94/3locais,maxstack2.
Negativo materializando a chave via chamada ref tem103bytes/quatro locais
e não bate. O corpo completo candidato=controle e before difere.
Modelo12 é genuinamente incompleto: prevê seis locais antes, quatro depois,
motivos foreach_collection_local_stack_scheduler/foreach_iteration_stack_scheduler.
A assinatura compilada da fixture tem três; não se tratou previsão
incompleta como uma assinatura real ou veto.

API: `transform_recordstore_preference_foreach`.
Regra: `r12-recordstore-preference-array-foreach-reused-temps`.
Histórico55registros/um trial de guard positivo de raiz rejeitado; não era
esta conversão do segundo laço. Nenhum candidato completo igual encontrado.

## Transform.Translate(Single,Single,Single) —67bytes

Token0x06000810, CIL SHA256
`1e29b9c4bae09d2e2faa92d7fc46bbd88f9de2497365ceb2db572be47b419463`.
Atual69bytes, dois locais,maxstack3. Original67bytes, um Transform,maxstack5.
O original carrega o receiver em22 antes da criação do array, inicializa a
parte constante, faz dup/stores3,7,11 com x,y,z e chama Set diretamente.
O atual guarda o array em local, faz os três stores e só depois carrega
receiver/array para Set.

Proposta: manter o guard/construtor/Multiply e passar a matriz mista direto:

```csharp
transform.Set(new float[16] {
    1f,0f,0f,x, 0f,1f,0f,y, 0f,0f,1f,z, 0f,0f,0f,1f
});
```

Não se copiou qualquer blob binário. Os valores constantes vêm do fonte
recuperado e o csc regenera por conta própria o campo de dados
`40126E70D1873C3C3306B219AAFE18F9DAF7580B`, observado na fixture candidata.
As leituras x,y,z continuam uma vez cada e na mesma ordem; o guard de
argumentos e Multiply não mudam.

Ownfixture: before69/2locais/max3; candidato/controle67/1local/max5.
Negativo trocando x/y tem o mesmo67/1/max5 e falha no corpo, demonstrando
que tamanho/assinatura não bastam. NoEmit12 continua incompleto por uso
múltiplo de transform, mas prevê o único tipo Transform; não se mudou modelo.

Rebase obrigatório documentado: arquivo baseline SHA256
`28571cb81ad1a62048af0ff55c9b7fef86e78b85c040b1cbe38a2313a29df738`,
arquivo atual capturado SHA256
`bb4346d0debdfa73abc39d99b1d09a75e220654ecf283873e97e60f9c429d4bf`.
O arquivo inteiro difere por Transpose já adotado. **O método Translate antes
coincide byte a byte com a baseline**, inclusive início UTF16 3986. O patch
usa o arquivo atual capturado em source-snapshot, preserva Transpose e não
aplica um SHA antigo como se fosse Main. Alterações posteriores de vizinhos
exigem rebase único do before e prova fresca pelo operador.

API: `transform_translate_inline_array`.
Regra: `r12-translate-inline-mixed-array-set-argument`.
Histórico55registros/um trial antigo de ordem de declaração rejeitado;
nenhum payload integral igual à matriz mista no argumento.

## KairoMath.Sqrt(Int64) —46bytes

Token0x060007d7, CIL SHA256
`ee8e08e8491c17d94f39110e94c3a8af944767690b9380996afa8eaceb9a2833`.
Ambos46bytes/maxstack3. Atual: Int32,Int64,Int32; original: Int64,Int32,Int32,
com limit10 gravado no slot2 antes do acumulador0 e índice1 inicializado
somente depois do do/while.

Proposta após o guard existente:

```csharp
long num2; int i; int num=10; num2=0L;
// do/while existente, sem mudança
for(i=0;i<num;i++) { /* corpo existente */ }
```

As declarações definem os slots0/1/2, mas a ordem das atribuições continua
limit10, acumulador0, depois índice0. Não se antecipa a atribuição do índice,
não se muda condição, divisão, literal, checked ou retorno.

Ownfixture: candidato=controle completo46bytes,3locais/max3. Before difere;
negativo com a mesma assinatura Long,Int,Int mas limit/índice trocados
também difere. Modelo12 é **completo**, prevê Long,Int,Int exatamente como
o original; não há relaxamento de previsão completa errada.

API: `transform_sqrt_root_local_order`.
Regra: `r12-sqrt-root-local-order-preserved-initialization`.
Histórico55registros/dois trials de mover declaração depois, rejeitados;
não eram a composição de declarações sem inicializador+atribuições mantidas.
Nenhum texto/hash completo igual foi encontrado.

## Ferramentas, controles e limites

Doze compilações próprias: quatro por método, com before/candidato/controle
manual independente/negativo. Três positivos de corpo completo e seis
negativos cumpriram o resultado esperado. csc1.1.0.51109 SHA256
`53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3`,
flags/referências fixadas. `/out` e `/pdb` individuais privados; não se
escreveu no PDB histórico da produção. Nenhum target foi compilado.

Evento de preparação preservado: a primeira chamada de fixture passou a
classe inteira para a API de método e encontrou o parâmetro key do stub
externo, recusando antes de qualquer compilação. O driver foi corrigido
para extrair Run. Não houve build falhado nem hipótese do alvo refutada.

Fonte Roslyn `4cb203dab89e1542cdf7d88cb6d32767bc0e5460`, git-blobs em
`pinned/provenance.json`:

- `LocalRewriter_ForEachStatement.cs:524–616` sintetiza array e índice;
  `LocalSlotManager.cs:121–158` reutiliza slot livre por assinatura.
- `SynthesizedLocalKind.cs:242–260` exclui locais do usuário da reutilização,
  mantendo os temporários sintetizados elegíveis em Release.
- `EmitArrayInitializer.cs:41–62` separa o bloco constante da parte mista,
  emitindo depois os stores dinâmicos.
- `EmitStatement.cs:594–630` define locais do bloco em ordem antes dos
  comandos. Isso permite Sqrt declarar primeiro e atribuir depois.

Os diagnósticos simbólicos calculam94/67/46bytes e zero diferenças de todos
os opcodes, operandos resolvidos e destinos. Isso **não é oráculo**, não
produz bytes para um resultado e não substitui build completo/regressões.
Headers/locais/EH só foram comprovados nos próprios controles compilados.

Índice comum `ready-index.json`; bundles independentes nas três pastas e
freeze `ready-freeze.json`. Ganhos medidos do jogo:0métodos/0bytes. Não há
nova família além destes três; o executor decide a seleção e adoção.
