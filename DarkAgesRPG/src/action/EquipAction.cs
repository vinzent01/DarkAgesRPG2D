namespace DarkAgesRPG;

public class EquipAction : Action {
    public EquipAction() : base("Equip") {

    }

    public override bool OnStart()
    {
        EquipmentList? equips = obj.GetComponent<EquipmentList>();
        var itemComponent = target.GetComponent<ItemComponent>();

        if (itemComponent == null)
            return false;

        if (equips == null)
            return false;

        if (itemComponent.inventory != null){
            itemComponent.inventory.RemoveItem(target);
        }

        State.world.ToRemove(target);
        equips.AddEquip(target);
        target.IsVisible = true;
        
        return true;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        EquipmentList? Equipments = obj.GetComponent<EquipmentList>();
        
        if (Equipments != null && Equipments.HasEquip(target))
            return false;


        return true;
    }
}