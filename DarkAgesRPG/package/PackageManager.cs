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
            try{
                Packages.Add(LoadPackage(folder));
            }
            catch (Exception e){
                Console.WriteLine(e + " Could Not load package " + folder);
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

    public PackageManager(){
        Packages = new();
    }
}