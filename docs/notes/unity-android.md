# Unity Android 5.3.4f1: identidade comprovada

O módulo Android não estava instalado: `Editor/Data/PlaybackEngines/` continha somente WebPlayer. Conforme a solicitação explícita do usuário, o componente Android foi obtido no link Windows da [página oficial do Unity 5.3.4f1](https://unity.com/releases/editor/whats-new/5.3.4f1) e adicionado ao editor existente, sem instalar SDK Android, NDK, JDK ou outro editor.

Instalador: `UnitySetup-Android-Support-for-Editor-5.3.4f1.exe`, 64.934.904 bytes, SHA-256 `17ce1e474487363ddcf2b18191410f1216afba3885eb88e928310029bc1523e3`. A assinatura Authenticode foi verificada como **Valid**, emitida para Unity Technologies SF. Instalação silenciosa com `/S /D=<raiz do editor>`, seguindo as opções documentadas no [manual Unity 5.3](https://docs.unity3d.com/530/Documentation/Manual/InstallingUnity.html). Foram comparados por hash 140 arquivos existentes de Editor, Mono e WebPlayer antes e depois: nenhum mudou.

## UnityEngine.dll

A DLL oficial em `Editor/Data/PlaybackEngines/AndroidPlayer/Variations/mono/Managed/UnityEngine.dll` tem **970.752 bytes e é integralmente idêntica ao APK**, por comparação direta dos bytes, incluindo PE, MVID, metadados, recursos e todos os corpos de métodos. Nenhum campo foi normalizado.

SHA-256 de ambas: `7cc901371cc77e103d00b5845e8285f85fbdf36bb47ccf3d824f94c4ead32421`. MVID: `8ff1c57a-358d-4cd9-bfaa-aa0ef82c83ff`. O REA confirmou metadados completos, 871 TypeDefs e 10.459 MethodDefs, sem carregar nem executar a DLL. Evidências: `ev_cb07768bdd58b9844204a7f981194b68d1e0ac300d7737ba158d496208e2a0e6` e projeção `ev_8bd72c536f5eb55e128d95a5c4216199bb95758d4be460f573449a55c09ac187`.

A variante oficial IL2CPP também foi identificada, mas é outro arquivo: 968.704 bytes, SHA-256 `701dc097eae80232f12879dad76c2ea2a59ffbe04351e38a676a3de3736adc6d`. O backend do APK é Mono.

Nesta distribuição, `Managed/UnityEngine.dll` é um arquivo único por backend, ao lado dos diretórios `Release/` e `Development/`. Esses diretórios contêm as variantes nativas; não há uma segunda UnityEngine.dll dentro deles. A auditoria enumera todas as DLLs com esse nome no módulo, sem presumir caminhos adicionais.

Portanto, a diferença observada contra Editor e WebPlayer é explicada pela variante oficial Android. **UnityEngine.dll passa de externa não verificada a externa oficial com identidade integral comprovada.** Não há evidência de alteração dessa DLL no APK.

## Bibliotecas nativas

As seis bibliotecas nativas do APK também correspondem por SHA-256 a arquivos oficiais do módulo:

| Bibliotecas do APK | Correspondência oficial |
|---|---|
| `libunity.so`, ARMv7 e x86 | Mono **Release**, exclusivamente entre as variantes consultadas |
| `libmono.so`, ARMv7 e x86 | Mono Release e Development usam os mesmos arquivos |
| `libmain.so`, ARMv7 e x86 | Release dos dois backends usa os mesmos arquivos |

Os hashes de `libunity.so` comprovam a variante Mono Release nas duas arquiteturas. Essa conclusão usa identidade dos arquivos, sem engenharia reversa do código nativo.

Relatório completo: [UnityEngine-android-comparison.json](../../reports/UnityEngine-android-comparison.json). Reprodução da auditoria: `python tools/audit_android_unity.py`, após instalar o componente oficial e fornecer sua própria extração do APK. Downloads, instaladores e arquivos da instalação permanecem fora do Git.
