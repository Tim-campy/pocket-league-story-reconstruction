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
