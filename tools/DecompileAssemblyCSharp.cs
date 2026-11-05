// Offline ILSpy driver for Unity's C# 3 / 2.0.5.0 profile. Never executes input.
using System;
using System.IO;
using System.Text.Json;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

public static class DecompileAssemblyCSharp {
    public static int Main(string[] args) {
        if(args.Length==1 && args[0]=="--settings") {
            var settingsToInspect=new DecompilerSettings(LanguageVersion.CSharp3);
            foreach(var p in typeof(DecompilerSettings).GetProperties())
                if(p.Name.IndexOf("Ref",StringComparison.OrdinalIgnoreCase)>=0 || p.Name.IndexOf("Optional",StringComparison.OrdinalIgnoreCase)>=0)
                    Console.WriteLine(p.Name+"="+p.GetValue(settingsToInspect));
            return 0;
        }
        if(args.Length!=5) throw new ArgumentException("input manifest unity-root language assembly-info-output");
        string input=Path.GetFullPath(args[0]),unity=args[2];
        var settings=new DecompilerSettings((LanguageVersion)Enum.Parse(typeof(LanguageVersion),args[3]));
        settings.UseDebugSymbols=false;settings.ShowXmlDocumentation=false;settings.ThrowOnAssemblyResolveErrors=true;
        var resolver=new UniversalAssemblyResolver(input,true,".NETFramework,Version=v3.5",null,
            PEStreamOptions.PrefetchEntireImage,MetadataReaderOptions.Default);
        foreach(var directory in resolver.GetSearchDirectories()) resolver.RemoveSearchDirectory(directory);
        resolver.AddSearchDirectory(Path.Combine(unity,"Editor/Data/Mono/lib/mono/unity"));
        resolver.AddSearchDirectory(Path.GetDirectoryName(input));
        resolver.AddSearchDirectory(Path.Combine(unity,"Editor/Data/Mono/lib/mono/2.0"));
        var decompiler=new CSharpDecompiler(input,resolver,settings);
        var manifest=JsonDocument.Parse(File.ReadAllText(args[1])).RootElement;
        int count=0;
        foreach(var entry in manifest.EnumerateArray()) {
            string type=entry.GetProperty("type").GetString(),output=entry.GetProperty("output").GetString();
            if(File.Exists(output)) throw new IOException("Refusing to overwrite source: "+output);
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            File.WriteAllText(output,decompiler.DecompileTypeAsString(new FullTypeName(type)));
            count++;
        }
        if(File.Exists(args[4])) throw new IOException("Refusing to overwrite assembly attributes");
        File.WriteAllText(args[4],decompiler.DecompileModuleAndAssemblyAttributesToString());
        Console.WriteLine("Decompiled top-level types: "+count);
        return 0;
    }
}
