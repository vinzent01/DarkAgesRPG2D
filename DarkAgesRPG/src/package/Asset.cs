
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


            switch (component.className){

                case  "Hair":
                    Object.AddComponent(
                        new Hair(
                            (parameters["racesId"] as JArray).ToObject<string[]>(),
                            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(),
                            new Sprite(Path.Join(contentDirectory, (string)parameters["spritePath"]))
                        )
                    );
                break;

                case "Beard":
                    Object.AddComponent(
                        new Beard(
                            (parameters["racesId"] as JArray).ToObject<string[]>(),
                            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(),
                            new Sprite(Path.Join(contentDirectory, (string)parameters["spritePath"]))
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
                            new Sprite(Path.Join(contentDirectory, (string)parameters["spriteMasculine"])),
                            new Sprite(Path.Join(contentDirectory, (string)parameters["spriteFeminine"])),
                            hairColorsColor,
                            skinColorsColor
                        )
                    );

                break;

                case "Sprite":

                    parameters.TryGetValue("Offset", out var offsetparameter );
                    parameters.TryGetValue("ysortOffset", out var ysortparameter);
                    parameters.TryGetValue("origin", out var originparameter);

                    Vector2 offset;
                    float ysort;
                    SpriteOrigin origin;

                    if (offsetparameter == null){
                        offset = new Vector2(0,0);
                    }
                    else {
                        offset = (offsetparameter as JObject).ToObject<Vector2>();
                    }

                    if (originparameter == null){
                        origin = SpriteOrigin.TopLeft;
                    }
                    else {
                        origin = new JValue(originparameter).ToObject<SpriteOrigin>();
                    }

                    if (ysortparameter == null){
                        ysort = 0.0f;
                    }
                    else {
                        ysort = (float)(double)ysortparameter;
                    }

                    Object.AddComponent(
                        new Sprite(
                            Path.Join(contentDirectory, (string)parameters["path"]), 
                            new Vector2(1,1),
                            offset,
                            ysort,
                            origin
                        )
                    );

                break;

                case "Equipment":
                    Object.AddComponent(
                        new EquipmentComponent(
                            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(), 
                            (parameters["flippedOffset"] as JObject).ToObject<Vector2>()
                        )
                    );
                break;

                case "ItemComponent":
                    Object.AddComponent(
                        new Item()
                    );
                break;
            }
        }
        return Object;
    }
}
