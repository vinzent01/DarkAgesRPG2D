
using System.Numerics;
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
    public string contentDirectory;
    public string[]? tags;

    public AssetComponent[] components;
    public string type;

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

        if (this.type == "Actor"){
            Object = new Actor();
        }

        Object.id = this.id;
        Object.Name = this.Name;

        foreach (var component in components){
            var parameters = component.parameters;

            var DeserializedComponent = ComponentDeserializer.Deserialize(component.className, parameters);

            if (DeserializedComponent != null){
                Object.AddComponent(DeserializedComponent);
            }

        }
        return Object;
    }
}
