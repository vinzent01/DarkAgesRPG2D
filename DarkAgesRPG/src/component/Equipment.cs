using System.Numerics;
using Newtonsoft.Json.Linq;

namespace DarkAgesRPG;


public class EquipmentComponent : Component {

    public Vector2 EquipmentOffset;
    public Vector2 EquipmentFlippedOffset;
    public Dictionary<string, Vector2> offsetByRace;

    public EquipmentComponent(Dictionary<string, Vector2> offsetByRace, Vector2? equipmentFlippedOffset = null)
    {
        if (equipmentFlippedOffset != null){
            EquipmentFlippedOffset = (Vector2)equipmentFlippedOffset;
        }
        else {
            this.EquipmentFlippedOffset = new Vector2(0,0);
        }

        this.offsetByRace = offsetByRace;

    }
    public void SetOffsetByRace(){
        EquipmentOffset = offsetByRace.Values.ToList()[0];
    }

    public static EquipmentComponent Deserialize(Dictionary<string, object> parameters){
        return new EquipmentComponent(
            (parameters["racesOffsets"] as JObject).ToObject<Dictionary<string, Vector2>>(), 
            (parameters["flippedOffset"] as JObject).ToObject<Vector2>()
        );
    }
}