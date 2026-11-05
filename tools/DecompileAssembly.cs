// Uses the ILSpy engine already shipped with Visual Studio. Does not execute
// the input assembly. Output is reconstructed C# for the requested language.
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

public static class DecompileAssembly {
    public static int Main(string[] args) {
        if(args.Length<5) { Console.Error.WriteLine("input manifest unity-root language assembly-info-output");return 2; }
        string input=Path.GetFullPath(args[0]),unity=args[2];
        var settings=new DecompilerSettings((LanguageVersion)Enum.Parse(typeof(LanguageVersion),args[3]));
        settings.UseDebugSymbols=false;
        settings.ShowXmlDocumentation=false;
        settings.ThrowOnAssemblyResolveErrors=true;
        var resolver=new UniversalAssemblyResolver(input,true,".NETFramework,Version=v3.5",null,
            PEStreamOptions.PrefetchEntireImage,MetadataReaderOptions.Default);
        foreach(var directory in resolver.GetSearchDirectories()) resolver.RemoveSearchDirectory(directory);
        resolver.AddSearchDirectory(Path.Combine(unity,"Editor/Data/Mono/lib/mono/2.0"));
        resolver.AddSearchDirectory(Path.GetDirectoryName(input));
        resolver.AddSearchDirectory(Path.Combine(unity,"Editor/Data/Mono/lib/mono/unity"));
        var decompiler=new CSharpDecompiler(input,resolver,settings);
        var manifest=JsonDocument.Parse(File.ReadAllText(args[1])).RootElement;
        int count=0;
        foreach(var entry in manifest.EnumerateArray()) {
            string type=entry.GetProperty("type").GetString(),output=entry.GetProperty("output").GetString();
            if(File.Exists(output)) throw new IOException("Refusing to overwrite source: "+output);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output,decompiler.DecompileTypeAsString(new FullTypeName(type)));
            Console.WriteLine(type);count++;
        }
        if(File.Exists(args[4])) throw new IOException("Refusing to overwrite assembly attributes");
        File.WriteAllText(args[4],decompiler.DecompileModuleAndAssemblyAttributesToString());
        Console.WriteLine("Decompiled top-level types: "+count);
        return 0;
    }
}
