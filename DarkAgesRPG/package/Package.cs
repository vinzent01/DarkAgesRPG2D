
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSScriptLib;
using Newtonsoft.Json;

namespace DarkAgesRPG;

public enum AssetType {
    Equipment,
    Object,
    Actor,
    Race
}

public class Asset {
    public AssetType type;
    public string for_race;
    public string name;
    public string id;
    public string texture;
    public string path;

    public T Instance<T>() where T : Object, new(){
        T instance = new();
        instance.LoadFromAsset(this);
        return instance;
    }
}

public class Package {
    public PackageMeta Meta;
    public List<Asset> Assets;
    public string Directory;

    public Package(string path){
        Directory = path;
        Assets = new();
    }

    public void LoadMeta(){
        var jsonContent = File.ReadAllText(Path.Join(Directory, "meta.json"));
        Meta = JsonConvert.DeserializeObject<PackageMeta>(jsonContent);
    }

    public Asset LoadAsset(string Directory){
        var json = File.ReadAllText(Directory + "/asset.json");
        Asset? asset = JsonConvert.DeserializeObject<Asset>(json);
        Debug.Assert(asset != null);
        asset.path = Directory;

        return asset;
    }

    public void LoadAssets(){
        foreach (var asset in System.IO.Directory.EnumerateDirectories(Directory)){
            Assets.Add(LoadAsset(asset));
        }
    }

    public Asset? GetAsset(string id){
        foreach (var asset in Assets){
            if (asset.id == id){
                return asset;
            }
        }
        return null;
    }
}