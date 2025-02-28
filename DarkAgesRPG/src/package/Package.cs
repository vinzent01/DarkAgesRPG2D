
using System.Diagnostics;
using Newtonsoft.Json;

namespace DarkAgesRPG;


public class Package {
    public PackageMeta Meta;
    public Dictionary<string, List<Asset>> Assets;
    public Dictionary<string, Resource> Resources;
    public string DirectoryPath;

    public Package(string path){
        DirectoryPath = path;
        Assets = new();
        Resources = new();
    }

    public void LoadMeta(){
        var jsonContent = File.ReadAllText(Path.Join(DirectoryPath, "meta.json"));
        Meta = JsonConvert.DeserializeObject<PackageMeta>(jsonContent);
    }

    public Asset LoadAsset(string assetPath, string assetDirectory){
        var json = File.ReadAllText(assetPath);
        Asset? asset = JsonConvert.DeserializeObject<Asset>(json);
        asset.assetDirectory = assetDirectory;
        asset.contentDirectory = Path.Join(assetDirectory, "../");
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

    public Resource? LoadResource(string file){
        var filename = Path.GetFileName(file);

        if (file.EndsWith(".png")){

            filename = filename.Replace(".png", "");

            return new TextureResource(filename, file, ResourceType.Texture);
        }

        Console.WriteLine($"Could Not load Resource {file}");

        return null;
    }

    public List<Resource> LoadResources(string path){
        List<Resource> Resources = new();
        var files = Directory.GetFiles(path);

        foreach (var file in files){
            Resource? resource = LoadResource(file);

            if (resource != null){
                resource.Load();
                Resources.Add(resource);
            }
        }

        return Resources;
    }


    public void LoadPackage(string directoryPath = null, int recursivity =0)
    {
        if (directoryPath == null)
            directoryPath = DirectoryPath;

        // load assets outside folders
        var singleAsset = Path.Join(directoryPath, "asset.json");
        var assetList = Path.Join(directoryPath, "assets.json");

        if (recursivity == 0){
            var resourcesPath = Path.Join(directoryPath, "resources");

            // carregar recursos
            if (Directory.Exists(resourcesPath)){
                var resourcesList = LoadResources(resourcesPath);

                foreach (var resource in resourcesList){
                    AddResource(resource);
                }
            }
        }


        // Verifica se o arquivo asset.json existe no diretório
        if (File.Exists(singleAsset))
        {
            var asset = LoadAsset(singleAsset, directoryPath);
            if (asset.tags != null)
                AddAsset(asset.tags, asset);
        }

        // Carrega uma lista de assets
        if (File.Exists(assetList)){
            var assets = LoadAssetList(assetList, directoryPath);

            foreach (var asset in assets){
                if (asset.tags != null)
                    AddAsset(asset.tags, asset);
            }
        }

        foreach (var assetDirectory in Directory.EnumerateDirectories(directoryPath))
        {
            var singleAsset2 = Path.Join(assetDirectory, "asset.json");
            var assetList2 = Path.Join(assetDirectory, "assets.json");

            // Verifica se o arquivo asset.json existe no diretório
            if (File.Exists(singleAsset2))
            {
                var asset = LoadAsset(singleAsset2, assetDirectory);
                if (asset.tags != null)
                    AddAsset(asset.tags, asset);
            }

            // Carrega uma lista de assets
            if (File.Exists(assetList2)){
                var assets = LoadAssetList(assetList2, assetDirectory);

                foreach (var asset in assets){
                    if (asset.tags != null)
                        AddAsset(asset.tags, asset);
                }
            }

            // Chamada recursiva para processar subdiretórios
            LoadPackage(assetDirectory, recursivity + 1);
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

    public void AddResource(Resource resource){
        var localResourcePath = resource.Path.Replace($"{DirectoryPath}/resources/", "");
        Resources.TryAdd(localResourcePath, resource);
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

    public Resource? GetResource(string path){
        Resources.TryGetValue(path, out var res);

        return res;
    }
}