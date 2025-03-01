namespace DarkAgesRPG;

public class EquipAction : Action {
    public EquipAction() : base("Equip") {

    }

    public override bool OnStart()
    {
        EquipmentList? equips = obj.GetComponent<EquipmentList>();
        var itemComponent = target.GetComponent<Item>();

        if (itemComponent == null)
            return false;

        if (equips == null)
            return false;

        if (itemComponent.inventory != null){
            itemComponent.inventory.RemoveItem(obj);
        }

        State.world.ToRemove(target);
        equips.AddEquip(target);
        target.IsVisible = true;
        
        
        return true;
    }
}