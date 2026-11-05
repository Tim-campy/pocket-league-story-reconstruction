# Plano de reconstrução Mono

O alvo é recuperar fontes C# que recompilam os métodos e metadados das DLLs próprias do jogo. O escopo segue o `AGENTS.md` atualizado: Unity e bibliotecas padrão são externos; recursos são dados; ambos ficam fora da porcentagem de código.

## Escopo e referência

- Alvos presentes: `Assembly-CSharp.dll` e `KairoLibrary.dll`.
- Não foi encontrado `Assembly-CSharp-firstpass.dll` no inventário deste APK.
- Referência: `original/app.apk`, SHA-256 `d557af6b4bc7748605a39ed3548821f6763c5773a12432a3fed6e8c413b211e8`.
- Unity **5.3.4f1**, observado no offset `0x14` de `globalgamemanagers` e `unity_builtin_extra`; hashes em `config/project.json`.
- DEX e plugins nativos permanecem fora da reconstrução. Sua composição interna é UNKNOWN; se código próprio for identificado, consultar o usuário antes de incluí-lo.

## Etapas

1. **Inventário e organização, preparados.** `inventory.csv` registra 2 arquivos alvo, 14 externos e 1.202 dados. `methods.csv` contém 5.245 declarações e `types.csv` 466 linhas de TypeDef, incluindo `<Module>` e tipos gerados.
2. **Toolchain, em identificação.** Instalar a distribuição oficial do Unity 5.3.4f1, registrar hashes e versões do compilador e investigar cada DLL separadamente. Padrões de nomes gerados são pistas, não prova do compilador exato.
3. **Referência, gerada.** `ref/` contém uma listagem por método. Tipos aninhados foram resolvidos pela tabela NestedClass. As assinaturas dos IDs resolvem TypeDef/TypeRef/TypeSpec; o comparador futuro ainda precisa resolver todos os operandos de IL e todas as assinaturas locais.
4. **Decompilação em massa, pendente.** Recuperar cada assembly inteiro para a versão C# aceita pelo compilador validado. Nenhum fonte de jogo foi gerado nesta preparação.
5. **Build integral e três ou quatro sondas, pendentes.** Compilar todos os tipos e comparar métodos simples de tipos diferentes. Não validar KairoLibrary com base apenas no resultado de Assembly-CSharp.
6. **Ajustes, pendentes.** Fechar tipos com menos pendências, ajustando um método por vez e tratando membros gerados junto com o método de origem.
7. **Tipos e assembly, pendentes.** Conferir ordem de todos os membros, atributos, constantes, layout, tabelas, heaps, recursos e referências. Só então ajustar a ordem de arquivos no compilador.
8. **Remontagem, pendente.** Preservar ordem, compressão e alinhamento do ZIP. Documentar explicitamente quais diferenças vêm de assinatura ou de campos cuja não determinação tenha sido comprovada.

## Critérios de aprovação

`matched` dependerá de compilação real e verificação completa do método. Hash do IL original, listagem correta ou cópia de DLL não contam como reconstrução. Cabeçalhos, `maxstack`, locais, instruções, destinos de desvios, `switch` e cláusulas de exceção fazem parte da comparação.

A equivalência dos tokens deve usar a identidade dos membros referidos, e não apenas seu número. MVID e timestamp não serão normalizados sem duas compilações consecutivas do mesmo fonte que provem a diferença. **A lista atual de exceções está vazia.** Nenhum ajuste posterior da DLL compilada é permitido.

## Estado atualizado da linha de base

Unity 5.3.4f1 instalado e sondas compatíveis em ambas as DLLs. KairoLibrary recuperada e compilada integralmente com dois Roslyn; pacote 1.3.2 escolhido como melhor candidato experimental, sem afirmar identidade histórica. Comparador de corpos e vínculo mecânico de membros gerados implementados: 2.783 métodos matched; quatro outros corpos iguais ainda retidos. As 424 falhas de corpos foram agrupadas antes de iniciar ajustes. Detalhes em [docs/notes/kairo-baseline.md](docs/notes/kairo-baseline.md). As etapas acima registram o plano inicial; decompilação/build Kairo e comparação por método já foram executados.

## Pendências para concluir a reconstrução

- Validar compilador, referências, perfil de API, versão C#, flags e símbolos por DLL.
- Recuperar e compilar Assembly-CSharp com o Mono validado pelas sondas; ILSpy já disponível.
- Mapear o método de origem de 337 membros gerados; os casos ainda não comprovados ficam `UNKNOWN`.
- Implementar comparação completa dos tipos e assemblies; o comparador de métodos já promove automaticamente com recibos e proteção contra regressões.
- Ofuscação, criptografia e stripping: estado **UNKNOWN**. Nomes e corpos legíveis comprovam acessibilidade dos elementos observados, não ausência dessas transformações.
