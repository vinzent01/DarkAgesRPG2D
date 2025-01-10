
using System.Numerics;

namespace DarkAgesRPG;

public class EquipmentObject : Object{


    public EquipmentObject(string Name, string Id, Sprite sprite, Vector2? equipmentOffset= null, Vector2? flippedOffset = null, int? LocalZIndex = null){

        this.Name = Name;
        this.id = Id;

        var offset = equipmentOffset != null ? equipmentOffset : new Vector2(0,0);

        if (LocalZIndex != null)
            this.TotalZ = (int)LocalZIndex;

        AddComponents(sprite, offset, flippedOffset);
    }

    public EquipmentObject(string Name, string Id, Sprite sprite, Vector2? equipmentOffset= null, int? LocalZIndex = null){

        this.Name = Name;
        this.id = Id;

        var offset = equipmentOffset != null ? equipmentOffset : new Vector2(0,0);

        if (LocalZIndex != null)
            this.TotalZ = (int)LocalZIndex;

        AddComponents(sprite, offset, null);
    }

    private void AddComponents(Sprite sprite, Vector2? offset, Vector2? flippedOffset){
        
        AddComponent(new Item());
        AddComponent(sprite);
        if (flippedOffset != null){
            AddComponent(new EquipmentComponent(offset, flippedOffset));
        }
        else {
            AddComponent(new EquipmentComponent(offset));
        }
        AddComponent(new ActionsToPerform(new List<Action>(){
            new TakeAction(),
            new EquipAction()
        }));
        AddComponent(new InteractComponent());
    }


}