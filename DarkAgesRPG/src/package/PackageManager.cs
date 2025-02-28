
namespace DarkAgesRPG;

public class PackageManager {
    public List<Package> Packages;

    public Package LoadPackage(string directory){
        Package package = new(directory);

        package.LoadMeta();
        package.LoadPackage();

        return package;
    }

    public void LoadPackages(string directory){
        
        
        Console.WriteLine("Loading Packages from " + directory + "...");

        foreach (var folder in Directory.EnumerateDirectories(directory)){
            var meta = Path.Join(folder, "meta.json");
            
            if (File.Exists(meta)){
                try{
                    Packages.Add(LoadPackage(folder));
                }
                catch (Exception e){
                    Console.WriteLine(e + " Could Not load package " + folder);
                }
            }
        }
    }

    public T? GetResource<T>(string path) where T : Resource{

        var totalPath = path.Replace("res://", "");

        var paths = totalPath.Split("/", 2);

        if (paths.Length > 1){
            var packagePath = paths[0];
            var resourcePath = paths[1];

            var package = GetPackage(packagePath);

            if (package != null){
                var result = package.GetResource(resourcePath);

                if (result == null){
                    Console.WriteLine($"Resource {resourcePath} in {packagePath} not found.");
                }

                return result as T;
            }
            else {
                Console.WriteLine($"Package {packagePath } not found in {totalPath}.");
                return null;
            }
        }
        else {
            Console.WriteLine($"Could Not load Resource {path}");
            return null;
        }
    }

    public void PrintAllAssets(){
        Console.WriteLine("PRINTING ALL GAME ASSETS");
        foreach (var package in Packages){
            Console.WriteLine("=== Package : " + package.Meta.Name);

            var packageAssets = package.GetAllAssets();

            foreach (var asset in packageAssets){
                Console.WriteLine("- Asset : " + asset.Name);

                foreach (var component in asset.components){
                    Console.WriteLine(" - Component : " + component.className);
                }
            }
        }
    }

    public Asset? GetAsset(string id){

        foreach (var package in Packages){
            Asset? asset = package.GetAsset(id);

            if (asset != null)
                return asset;
        }
        
        return null;
    }

    public List<Asset> GetAllAssets(){
        List<Asset> assets = new();

        foreach (var package in Packages){
            foreach (var asset in package.GetAllAssets()){
                assets.Add(asset);
            }
        }

        return assets;
    }

    public Package? GetPackage(string name){
        foreach (var package in Packages){
            if (package.Meta.Name == name)
                return package;
        }
        return null;
    }

    public List<Asset> GetAssetByTags(string tag){
        var assets = new List<Asset>();

        foreach (var package in Packages){
            var packageAssets = package.GetAssetByTags(tag);

            if (packageAssets != null){
                foreach (var asset in packageAssets){
                    assets.Add(asset);
                }
            }
        }

        return assets;
    }

    public PackageManager(){
        Packages = new();
    }
}