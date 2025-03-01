
namespace DarkAgesRPG;

public class TakeAction : Action {

    public TakeAction() : base("Take") {

    }

    public override bool OnStart()
    {
        Inventory? inventory = obj.GetComponent<Inventory>();

        if (inventory != null){
            inventory.AddItem(target);
        }

        target.IsVisible = false;
        State.world.ToRemove(target);


        return true;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        Inventory? inventory = obj.GetComponent<Inventory>();
        EquipmentList? Equipments = obj.GetComponent<EquipmentList>();

        if (inventory != null && inventory.HasExactItem(obj))
            return false;
        
        if (Equipments != null && Equipments.HasEquip(obj))
            return false;


        return true;
    }
}