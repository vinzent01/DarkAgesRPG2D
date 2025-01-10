
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSScriptLib;
using Newtonsoft.Json;
using System.IO;
using System.Numerics;

namespace DarkAgesRPG;

public class Package {
    public PackageMeta Meta;
    public List<Object> Assets;
    public string DirectoryPath;

    public Package(string path){
        DirectoryPath = path;
        Assets = new();
    }

    public void LoadMeta(){
        var jsonContent = File.ReadAllText(Path.Join(DirectoryPath, "meta.json"));
        Meta = JsonConvert.DeserializeObject<PackageMeta>(jsonContent);
    }

    public Object LoadAsset(string Directory){
        var json = File.ReadAllText(Directory + "/asset.json");
        Object? asset = JsonConvert.DeserializeObject<Object>(json);
        Debug.Assert(asset != null);

        Console.WriteLine("LOADED " + asset.id.ToString());
        return asset;
    }

    public void LoadAssets(){
        foreach (var asset in Directory.EnumerateDirectories(DirectoryPath)){
            if (File.Exists(asset+"/asset.json"))
                Assets.Add(LoadAsset(asset));
        }
    }

    public Object? GetAsset(string id){
        foreach (var asset in Assets){
            if (asset.id == id){
                return asset;
            }
        }
        return null;
    }
}