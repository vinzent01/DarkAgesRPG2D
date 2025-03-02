using System.Numerics;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;

namespace DarkAgesRPG;

public class Hair : Component{

    public string[ ] RacesId;
    public Dictionary<string, Vector2> racesOffsets;
    public Sprite sprite;


    public Hair(string[] racesId, Dictionary<string, Vector2> racesOffset, Sprite sprite){
        this.RacesId = racesId;
        this.sprite = sprite;
        this.racesOffsets = racesOffset;
    }

    public Vector2 GetRaceOffset(string raceId){
        Vector2 raceOffset = new Vector2(0,0);

        // get race offset
        if (racesOffsets.ContainsKey(raceId)){
            raceOffset = racesOffsets[raceId];
        }
        else if (racesOffsets.Count > 0) {
            raceOffset = racesOffsets.Values.ToList()[0];
        }

        return raceOffset;
    }

    public override void Load()
    {
        sprite.owner = owner;
        sprite.Load();
    }

    public override void Draw()
    {
        sprite.Draw();
    }

    public static void Serialize()
    {

    }

    public static Hair Deserialize(Dictionary<string, object> parameters)
    {
        return new Hair(
            (parameters["racesId"] as JArray).ToObject<string[]>(),
            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(),
            new Sprite((string)parameters["spritePath"])
        );
    }

}