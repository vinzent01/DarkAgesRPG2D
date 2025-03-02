using System.Numerics;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace DarkAgesRPG;

public class Beard : Hair {
    public Beard(string[] racesId, Dictionary<string, Vector2> racesOffset, Sprite sprite) : base(racesId, racesOffset, sprite){
        this.RacesId = racesId;
        this.sprite = sprite;
        this.racesOffsets = racesOffset;
    }

    public static new Beard Deserialize(Dictionary<string, object> parameters){
        return new Beard(
            (parameters["racesId"] as JArray).ToObject<string[]>(),
            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(),
            new Sprite((string)parameters["spritePath"])
        );
    }
}