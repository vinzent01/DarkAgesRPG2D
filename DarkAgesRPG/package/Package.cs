
using System.Diagnostics;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raylib_cs;

namespace DarkAgesRPG;

public class AssetComponent {
    public string className;
    public Dictionary<string, object> parameters;
}

public class Asset {

    public string Name;
    public string id;
    public string assetDirectory;
    public string[]? tags;

    public AssetComponent[] components;

    public AssetComponent? GetComponent(string className){
        foreach (var component in components){
            if (component.className == className){
                return component;
            }
        }

        return null;
    }

    public Object Instanciate(){

        var Object = new Object();

        Object.id = this.id;
        Object.Name = this.Name;

        foreach (var component in components){
            var parameters = component.parameters;


            switch (component.className){

                case  "Hair":
                    Object.AddComponent(
                        new Hair(
                            (parameters["racesId"] as JArray).ToObject<string[]>(),
                            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(),
                            new Sprite(Path.Join(assetDirectory, (string)parameters["spritePath"]))
                        )
                    );
                break;

                case "Beard":
                    Object.AddComponent(
                        new Beard(
                            (parameters["racesId"] as JArray).ToObject<string[]>(),
                            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(),
                            new Sprite(Path.Join(assetDirectory, (string)parameters["spritePath"]))
                        )
                    );
                break;

                case "RaceComponent" :

                    var hairColorsHex = (parameters["hairColors"] as JArray).ToObject<string[]>();
                    var hairColorsColor = new Color[hairColorsHex.Length];

                    for (var i = 0; i < hairColorsHex.Length; i++){
                        hairColorsColor[i] = Utils.HexToColor((string)hairColorsHex[i]);
                    }

                    var SkinColorsHex = (parameters["skinColors"] as JArray).ToObject<string[]>();
                    var skinColorsColor = new Color[SkinColorsHex.Length];

                    for (var i = 0; i < SkinColorsHex.Length; i++){
                        skinColorsColor[i] = Utils.HexToColor((string)SkinColorsHex[i]);
                    }
                    

                    Object.AddComponent(
                        new RaceComponent(
                            (string)parameters["raceId"],
                            new Sprite(Path.Join(assetDirectory, (string)parameters["spriteMasculine"])),
                            new Sprite(Path.Join(assetDirectory, (string)parameters["spriteFeminine"])),
                            hairColorsColor,
                            skinColorsColor
                        )
                    );

                break;

                case "Sprite":
                    Object.AddComponent(
                        new Sprite(Path.Join(assetDirectory, (string)parameters["path"]))
                    );
                break;
            }
        }
        return Object;
    }
}


public class Package {
    public PackageMeta Meta;
    public Dictionary<string, List<Asset>> Assets;
    public string DirectoryPath;

    public Package(string path){
        DirectoryPath = path;
        Assets = new();
    }

    public void LoadMeta(){
        var jsonContent = File.ReadAllText(Path.Join(DirectoryPath, "meta.json"));
        Meta = JsonConvert.DeserializeObject<PackageMeta>(jsonContent);
    }

    public Asset LoadAsset(string assetPath, string assetDirectory){
        var json = File.ReadAllText(assetPath);
        Asset? asset = JsonConvert.DeserializeObject<Asset>(json);
        asset.assetDirectory = assetDirectory;
        Debug.Assert(asset != null);

        return asset;
    }

    public Asset[] LoadAssetList(string assetListPath, string assetDirectory){
        var json = File.ReadAllText(assetListPath);

        Asset[]? assets = JsonConvert.DeserializeObject<Asset[]>(json);
        if (assets != null){
            foreach (var asset in assets){
                Console.WriteLine("Loading Asset " + asset.Name);
                asset.assetDirectory = assetDirectory;
            }

        }
        Debug.Assert(assets != null);

        return assets;
    }


    public void LoadAssets(string directoryPath = null)
    {
        if (directoryPath == null)
            directoryPath = DirectoryPath;

        foreach (var assetDirectory in Directory.EnumerateDirectories(directoryPath))
        {
            var singleAsset = Path.Join(assetDirectory, "asset.json");
            var assetList = Path.Join(assetDirectory, "assets.json");

            // Verifica se o arquivo asset.json existe no diretório
            if (File.Exists(singleAsset))
            {
                var asset = LoadAsset(singleAsset, assetDirectory);
                if (asset.tags != null)
                    AddAsset(asset.tags, asset);
            }

            // Carrega uma lista de assets
            if (File.Exists(assetList)){
                var assets = LoadAssetList(assetList, assetDirectory);

                foreach (var asset in assets){
                    if (asset.tags != null)
                        AddAsset(asset.tags, asset);
                }
            }

            // Chamada recursiva para processar subdiretórios
            LoadAssets(assetDirectory);
        }
    }

    public void AddAsset(string[] tags, Asset asset){

        foreach (var tag in tags){
            if (Assets.ContainsKey(tag)){
                Assets[tag].Add(asset);
            }
            else {
                Assets[tag] = new(){asset};
            }
        }
    }


    public Asset? GetAsset(string id){
        foreach (var assetGroup in Assets.Values){
            foreach (var asset in assetGroup){
                if (asset.id == id){
                    return asset;
                }
            }
        }
        return null;
    }

    public List<Asset>? GetAssetByTags(string tag){

        if (Assets.ContainsKey(tag)){
            return Assets[tag];
        }

        return null;
    }

    public List<Asset> GetAllAssets(){
        List<Asset> allAssets = new();

        foreach (var assetGroup in Assets.Values){
            foreach (var asset in assetGroup){
                allAssets.Add(asset);
            }
        }

        return allAssets;
    }
}