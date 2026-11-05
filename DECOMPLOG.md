# Histórico da preparação

## 2026 11 05 — Referência inicial

O REA abriu o APK, inventariou e extraiu 1.218 entradas. A comparação direta dos conteúdos descompactados com a extração conferiu 170.556.304 bytes sem diferenças. As evidências registraram 5.245 métodos, sendo 5.163 com corpo CIL, em Assembly-CSharp e KairoLibrary. Isso estabeleceu a referência; nenhuma correspondência de reconstrução foi declarada.

## 2026 11 05 — Instruções Mono atualizadas

Relido o AGENTS.md refeito. O cadastro genérico `functions.csv` foi substituído pelo cadastro `methods.csv`, com IDs por assinatura completa. Foram gerados `types.csv`, inventário por categoria, ordens iniciais de compilação e referências IL. A tabela NestedClass resolveu 204 relações de tipos aninhados nas duas DLLs; a ordem original de TypeDef foi preservada.

Estado preparado: 5.245 métodos, 466 linhas de TypeDef, 871.497 bytes de IL, todos os métodos em `todo`. 337 membros gerados têm origem UNKNOWN. Componentes externos e dados ficaram fora da métrica. Os artefatos e scripts genéricos anteriores foram arquivados em `analysis/legacy-generic-structure/`.

O REA 6.5.0 respondeu pelo MCP e pelo diagnóstico CLI. O diagnóstico relatou providers opcionais e registros desatualizados; a leitura .NET já verificada continua disponível. O adaptador local usa o parser instalado e fixa sua versão, sem instalar dependências ou executar o jogo.

O usuário autorizou instalar Unity 5.3.4f1. O instalador de 264.190.104 bytes foi baixado do domínio oficial e sua assinatura foi validada pelo Windows. Após a confirmação UAC, a instalação foi concluída em `C:\Users\l_abn\AppData\Local\Programs\Unity\5.3.4f1`. gmcs e smcs responderam como 2.6.5.0, e o mcs do MonoBleedingEdge como 3.4.0.0. Uma DLL sintética foi compilada com sucesso pelo gmcs. Mono.Cecil 0.9.6.0 também está disponível. Nenhum método do jogo foi compilado nesse teste.

Pendências: validar compilador e opções por DLL, disponibilizar decompilador C#, recuperar e compilar assemblies inteiros e implementar o comparador completo antes de promover qualquer método. A verificação atual é somente da referência e retorna bloqueio para a reconstrução.

Conferências da estrutura: 5.245 IDs de métodos e 466 IDs de tipos únicos, 204 tipos aninhados resolvidos, 5.245 leituras de layout compatíveis com as referências armazenadas, proteção contra reiniciar o cadastro e rejeição de promoção manual a matched. `tools/verify --all` conferiu as 1.218 entradas e 5.163 corpos presentes; retornou 2, bloqueado para reconstrução, conforme esperado. O progresso permanece 0% matched.

## 2026 11 05 — Compilação cruzada e linha de base Kairo

Por solicitação explícita, criado repositório público https://github.com/Tim-campy/pocket-league-story-reconstruction. Autenticação pelo Git Credential Manager existente, sem gravar credenciais. Somente fontes, cadastros, scripts e relatórios vão para o Git; original, assets, binários, pacotes e build permanecem ignorados.

REA auditou nove DLLs gerenciadas. Seis das sete externas correspondem por SHA-256 à instalação oficial do Unity; UnityEngine.dll permanece externa não verificada. A comparação estrutural contra Editor encontrou todos os 10.459 métodos do APK, mais 691 no Editor, com 28 corpos comuns diferentes. Contra WebPlayer há também 133 métodos e oito tipos exclusivos do APK. Apenas stripping das variantes disponíveis não explica a diferença. Evidências em config/dll-audit.json e reports/UnityEngine-structural-comparison.json.

Quatro sondas Assembly-CSharp passaram com Mono smcs/gmcs 2.6.5.0, perfil Unity 2.0.5.0; quatro Kairo passaram com Roslyn 5.8, perfil clássico mscorlib 2.0.0.0/System.Core 3.5.0.0. ILSpy 9.1.0.7988 e runtime .NET 10.0.10 foram encontrados no VS já instalado. Driver com C# 6 e resolução estrita recuperou 162 tipos superiores mais AssemblyInfo.cs, 163 fontes e 48.652 linhas, sem decompilar bibliotecas externas.

O usuário orientou compilar toda KairoLibrary e testar Microsoft.Net.Compilers 1.3.2. Pacote confirmado na API NuGet oficial e obtido em analysis/, com hash fixado. Executável informa 1.3.1.60621; ambas as numerações e o hash do csc constam em docs/toolchain.md. Nenhuma nova instalação global. As instruções explícitas de experimentar esses candidatos substituíram a pergunta anterior sobre aceitar UNKNOWN; a identificação histórica continua UNKNOWN.

Mesmos fontes, ordem, opções e referências compilam sem erros nos dois compiladores. Dos 3.211 corpos originais, 2.736 correspondem em ambos, 51 apenas no pacote 1.3.2 e zero apenas no 5.8; portanto não houve perda na troca. Há 330 saídas iguais entre os candidatos, divergentes do original, e 94 diferentes/ausentes sem correspondência em nenhum. As 76 declarações sem CIL ficam separadas. Nenhuma cópia de IL ou edição de DLL foi usada.

Comparador Cecil verifica todos os bytes de cabeçalhos, IL, padding e exceções, substituindo apenas operandos de tokens por identidades resolvidas. Primeiro resultado conservador: 2.473 matched e 314 corpos iguais retidos. Verificação mecânica de nomes, metadados, criação de closures e ponteiros de delegates nos dois lados liberou 310; quatro construtores/inicializadores de caches compartilhados continuam retidos. Referências genéricas locais são resolvidas para as definições. A propagação das origens é por ponto fixo, independente da ordem de visita e sem presumir origem em ciclos sem raiz. Resultado: **2.783 matched, 504 decompiled**, 84,7% da KairoLibrary; projeto inteiro 53,06% dos métodos. Tipos e assembly completo não estão verificados.

As 424 falhas de corpos foram agrupadas por edições reais, sem ajustar métodos: desvios/retornos 106; locais/ordem 61; identidades geradas ausentes 55; outras instruções/operandos 43; exceções/using/finally 37; aritmética/conversões 37; nomes gerados/destinos de delegates 31; switch 20; call/callvirt 19; closures/delegates 8; concatenação 4; lock 3. Categorias são diagnósticas, não causas comprovadas. Aguardamos a escolha do grupo pelo usuário antes dos ajustes; versões vizinhas só se o diagnóstico justificar.

PE original Kairo: linker 48.0, timestamp 2016-11-04T08:59:39Z, CodeView RSDS tipo 2, PDB basename KairoLibrary.pdb, age 1, sem REPRO. A ausência de REPRO e dos atributos modernos não prova a versão exata nem o determinismo. Caminho absoluto do PDB não publicado. Duas compilações completas idênticas com 1.3.2/debug:pdbonly diferiram em 34 bytes, restritos a MVID, timestamp COFF, timestamp debug e GUID CodeView PDB. Nenhuma exceção de normalização foi aplicada.

Testes negativos cobrem troca de identidade mantendo token, alteração de desvios/cabeçalhos/exceções, fonte alterado após verificação, promoção manual sem recibo e nomes gerados iguais sem referência IL. A verificação integral recompila Kairo e protege estados; diferenças conhecidas retornam 1. Global retorna 2 porque Assembly-CSharp e comparação completa dos metadados ainda estão pendentes. Esta linha de base é trabalho parcial verificável, não reconstrução concluída.
