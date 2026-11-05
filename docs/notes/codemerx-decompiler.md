# Segundo decompilador: Codemerx / JustDecompileEngine

Foi obtido o [JustDecompileEngine mantido pela equipe Codemerx](https://github.com/codemerx/JustDecompileEngine), independente do ILSpy, no commit fixado `35f93d8fae9f6220bc8c976d1dbf376e57d6321d`. O README oficial oferece os runners Framework 4.7.2 e .NET 6; foi usado o primeiro, compilado localmente com o MSBuild/Roslyn já presentes no Visual Studio 18. Nenhum fonte do decompilador foi modificado.

Faltavam apenas os assemblies de referência 4.7.2 para o build da ferramenta. O pacote oficial [Microsoft.NETFramework.ReferenceAssemblies.net472 1.0.3](https://www.nuget.org/packages/Microsoft.NETFramework.ReferenceAssemblies.net472/1.0.3) foi baixado e extraído em `analysis/`: 20.790.130 bytes, SHA-256 `ffa0a5570a39f911399164d0581ffddef99b5e3dfbaa5f220e5ce22969bcf57c`. Isso não instalou SDK, pacote de desenvolvimento ou runtime no sistema, nem alterou as referências usadas para recompilar o jogo.

O runner foi compilado na configuração **Debug**, com os projetos do motor em **Debug EngineOnly**, `LangVersion=latest`, e `TargetFrameworkRootPath` apontando para o pacote portátil. As tarefas COM de pré/pós-build foram desativadas por opções MSBuild. A configuração Release do upstream solicita acesso entre assemblies assinado com uma chave privada indisponível; a configuração Debug já define o acesso sem assinatura no próprio fonte, dispensando alterações. A versão interna da DLL do motor é `2019.1.118.0`, distinta da data do commit.

## Recuperação do fonte alternativo

O runner executou com:

```text
/target:<KairoLibrary.dll original>
/out:<analysis/variants/kairo-codemerx>
/lang:csharp /vs:2015 /nodoc /norename /nohex
```

Foram gerados **163 arquivos C#** em 4,4 segundos. A opção que decompila recursos perigosos não foi usada; a KairoLibrary não contém recursos embutidos no inventário. Nenhum código do jogo foi carregado ou executado. O CLI apenas lê IL e produz fonte.

As variantes permanecem em `analysis/variants/kairo-codemerx`, fora do Git e separadas dos fontes confirmados. Recuperar o C# não declara que ele compila ou corresponde: cada corpo escolhido ainda precisa passar pelo build integral e pelo verificador do projeto. Exemplo de diferença útil já observada: `MotionEvent.GetDeltaX(int)` preserva a expressão `pointerDelta[index * 3 + 0]`, que o ILSpy havia simplificado.

Reprodução: `python tools/prepare_codemerx.py --decompile`. O CLI upstream usa cores do console e requer um console real no Windows; por isso, no Codex, usar `tty:true`, sem redirecionar sua saída. O helper de build e o relatório são distribuídos; os fontes e binários externos do decompilador, sob licença AGPL-3.0, continuam em `analysis/`, fora do repositório.

Commit, hashes dos executáveis e hashes dos 163 fontes gerados: [Codemerx-decompiler.json](../../reports/Codemerx-decompiler.json).
