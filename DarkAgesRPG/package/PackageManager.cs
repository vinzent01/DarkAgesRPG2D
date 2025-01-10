using System.Diagnostics;

namespace DarkAgesRPG;

public class PackageManager {
    public List<Package> Packages;

    public Package LoadPackage(string directory){
        Package package = new(directory);

        package.LoadMeta();
        package.LoadAssets();

        return package;
    }

    public void LoadPackages(string directory){
        Debug.Assert(Directory.Exists(directory));
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

    public Object? GetAsset(string id){

        foreach (var package in Packages){
            Object? asset = package.GetAsset(id);

            if (asset != null)
                return asset;
        }
        
        return null;
    }

    public PackageManager(){
        Packages = new();
    }
}