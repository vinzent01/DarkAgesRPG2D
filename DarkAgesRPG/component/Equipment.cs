using System.Numerics;

namespace DarkAgesRPG;

public class EquipmentComponent : Component {

    public Vector2 EquipmentOffset;
    public Vector2 EquipmentFlippedOffset;

    public EquipmentComponent(Vector2? equipmentOffset, Vector2? equipmentFlippedOffset = null)
    {
        if (equipmentOffset != null)
            EquipmentOffset = (Vector2)equipmentOffset;
        
        if (equipmentFlippedOffset != null)
            EquipmentFlippedOffset = (Vector2)equipmentFlippedOffset;
    }
}