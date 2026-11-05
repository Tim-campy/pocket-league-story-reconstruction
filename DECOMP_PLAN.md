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
2. **Toolchain, candidatos validados.** Unity 5.3.4f1 e módulo Android instalados; todas as DLLs externas identificadas por hash oficial. Roslyn pacote 1.3.2 para KairoLibrary, Mono smcs 2.6.5.0 para Assembly-CSharp. Versões históricas exatas continuam UNKNOWN.
3. **Referência, gerada.** `ref/` contém uma listagem por método. Tipos aninhados foram resolvidos pela tabela NestedClass. O comparador Cecil resolve assinaturas de tipos/locais e todos os operandos de tokens, mantendo os demais bytes sob comparação.
4. **Decompilação em massa, realizada.** Ambos os assemblies recuperados; ILSpy com linguagem ajustada por DLL. Variantes de ILSpy e do decompilador independente Codemerx são geradas somente em analysis/.
5. **Build integral, realizado.** As duas DLLs compilam com seus candidatos selecionados, e todos os corpos são comparados. A configuração App stdlib default removeu a ambiguidade de IndexOf e permitiu restaurar a chamada original. As sondas não substituem as matrizes integrais.
6. **Ajustes, em execução.** Ordem expandida dos membros restaurada por regra comprovada, com dois construtores omitidos recuperados; 19 call/callvirt resolvidos. Variantes são selecionadas por método com verificação integral e rollback. Membros gerados seguem junto à origem; caches compartilhados exigem todas as lambdas correspondentes.
7. **Tipos e assembly, pendentes.** Conferir ordem de todos os membros, atributos, constantes, layout, tabelas, heaps, recursos e referências. Só então ajustar a ordem de arquivos no compilador.
8. **Remontagem, pendente.** Preservar ordem, compressão e alinhamento do ZIP. Documentar explicitamente quais diferenças vêm de assinatura ou de campos cuja não determinação tenha sido comprovada.

## Critérios de aprovação

`matched` dependerá de compilação real e verificação completa do método. Hash do IL original, listagem correta ou cópia de DLL não contam como reconstrução. Cabeçalhos, `maxstack`, locais, instruções, destinos de desvios, `switch` e cláusulas de exceção fazem parte da comparação.

A equivalência dos tokens usa a identidade dos membros referidos, não apenas seu número. Quatro campos voláteis Kairo foram comprovados por duas compilações idênticas e registrados em docs/toolchain.md; nenhum normalizador foi aplicado. Nenhum ajuste posterior da DLL compilada é permitido.

## Estado atualizado da linha de base

Os resultados atuais são gerados em [PROGRESS.md](PROGRESS.md), a partir do cadastro e dos recibos de compilação. As duas DLLs já têm verificação integral; a evolução de Kairo e a seleção de variantes estão registradas em [docs/notes/kairo-baseline.md](docs/notes/kairo-baseline.md), e a validação Mono em [docs/notes/assembly-csharp-baseline.md](docs/notes/assembly-csharp-baseline.md).

## Pendências para concluir a reconstrução

- Validar compilador, referências, perfil de API, versão C#, flags e símbolos por DLL.
- Diagnosticar os corpos que sobram após a seleção de variantes e a correção de opções; manter estados e regressões sob o verificador.
- Mapear o método de origem de 337 membros gerados; os casos ainda não comprovados ficam `UNKNOWN`.
- Implementar comparação completa dos tipos e assemblies; o comparador de métodos já promove automaticamente com recibos e proteção contra regressões.
- Ofuscação, criptografia e stripping: estado **UNKNOWN**. Nomes e corpos legíveis comprovam acessibilidade dos elementos observados, não ausência dessas transformações.
