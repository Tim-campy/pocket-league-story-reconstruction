"""Pinned compatible candidate; historical compiler identity remains UNKNOWN."""
PACKAGE_VERSION='1.1.0'
COMPILER_VERSION='1.1.0.51109'
COMPILER_SHA256='53b693536e8a86f35d05d2165ff9b3e0aeaa499ecfd8135a9f99ab4404ca86c3'
PACKAGE_SHA256='cd84fb2c71cafe03cdae0add609fa12c75f61c89ea87b1422fd108ac7e570745'
LABEL='roslyn-'+PACKAGE_VERSION
BUILD_FOLDER='build/KairoLibrary-'+LABEL
COMPILER_PATH='analysis/tools/'+LABEL+'/tools/csc.exe'
CORELIB_PATH='C:/Windows/Microsoft.NET/Framework/v2.0.50727/mscorlib.dll'
CORELIB_SHA256='c4729c6fc2279933fa9638f60775ab2f7eb731d5b04f9e04c15187afbdf94558'
COMPONENT_SHA256={
    'csc.exe':COMPILER_SHA256,
    'csc.exe.config':'0501cae075d0ab6002dbb47eeb92e61935b5e85fb40d9a7c3bf6e6f36ab53bfa',
    'Microsoft.CodeAnalysis.dll':'4d9c797aa6a62f850d8417f20bfc213844a4777c3cd7e75e744225b5812fb781',
    'Microsoft.CodeAnalysis.CSharp.dll':'1800cb4d49698d4f908338d4bb443c10f7549b9be907c7c70a2e4e6b1e17693b',
    'System.Collections.Immutable.dll':'51d4f7c607d5a2f636671d4ad1dc756a272018898267145dbfffc5bdb09df1a1',
    'System.Reflection.Metadata.dll':'e538275e53148ad564061dd9566d6e13bd820a50645ea415d58784857a625852',
}
