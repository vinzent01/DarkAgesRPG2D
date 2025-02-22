using System.Numerics;

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

        this.EquipmentFlippedOffset = new Vector2(0,0);
        this.offsetByRace = offsetByRace;

    }
    public void SetOffsetByRace(){
        EquipmentOffset = offsetByRace.Values.ToList()[0];
    }
}