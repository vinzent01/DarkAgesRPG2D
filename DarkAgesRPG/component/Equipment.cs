using System.Numerics;

namespace DarkAgesRPG;


public class EquipmentComponent : Component {

    public Vector2 EquipmentOffset;
    public Vector2 EquipmentFlippedOffset;

    public EquipmentComponent(Dictionary<string, Vector2> offsetByRace, Vector2? equipmentFlippedOffset = null)
    {
        var raceComponent = owner.GetComponent<RaceComponent>();

        if (offsetByRace != null && raceComponent != null)
            EquipmentOffset = offsetByRace.Values.ToList()[0];
        
        if (equipmentFlippedOffset != null)
            EquipmentFlippedOffset = (Vector2)equipmentFlippedOffset;
    }
}