using System.Numerics;

namespace DarkAgesRPG;

public class EquipmentList : Component {

    public List<Object> equips;
    public System.Action? OnChange;

    public EquipmentList(){
        equips = new();
    }

    public bool HasEquip(Object equipment){
        foreach (var equip in equips){
            if (equip == equipment)
                return true;
        }
        return false;
    }

    public void AddEquip(Object equipment){
        if (HasEquip(equipment))
            return;

        owner?.Children.Add(equipment);

        // add offset
        var EquipSprite = equipment.GetComponent<Sprite>();
        var ObjectSprite = owner?.GetComponent<Sprite>();
        var EquipInteract = equipment.GetComponent<InteractComponent>();
        var equipEquipment = equipment.GetComponent<EquipmentComponent>();


        if (EquipSprite != null && ObjectSprite != null){

            if (equipEquipment != null){
                //EquipSprite.isFliped = ObjectSprite.isFliped;

                EquipSprite.offset = equipEquipment.EquipmentOffset;
            }
        }
        
        // Remove interact component
        equipment.RemoveComponent<InteractComponent>();
        equipment.Parent = owner;
        equipment.RelativePosition = new Vector2(0,0);
        equips.Add(equipment);

        if (OnChange != null)
            OnChange();
    }

    public void RemoveEquip(Object equipment){

        if (!HasEquip(equipment))
            return;

        var EquipSprite = equipment.GetComponent<Sprite>();
        var equipEquipment = equipment.GetComponent<EquipmentComponent>();

        if (EquipSprite != null ){

            if (equipEquipment != null){
                EquipSprite.offset = new Vector2(0,0);
            }
        }

        // add interact component
        equipment.AddComponent(new InteractComponent());

        owner?.Children.Remove(equipment);
        equips.Remove(equipment);
        equipment.Parent = null;

        if (owner != null)
            equipment.CellPosition = owner.CellPosition ;
        
        if (OnChange != null)
            OnChange();
    }

    public override void Load()
    {
        foreach (var obj in equips){
            obj.Load();
        }
    }

    public override void Draw()
    {
        foreach (var obj in equips){
            obj.Draw();
        }
    }

    public override void Update(float delta)
    {
        foreach (var obj in equips){
            obj.Update(delta);
        }
    }
}