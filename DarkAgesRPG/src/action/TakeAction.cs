
namespace DarkAgesRPG;

public class TakeAction : Action {

    public TakeAction() : base("Take") {

    }

    public override bool OnStart()
    {
        Inventory? inventory = obj.GetComponent<Inventory>();
        ItemComponent item = target.GetComponent<ItemComponent>();

        if (inventory != null && item != null){
            
            // remove from inventory first
            if (item.inventory != null){
                item.inventory.RemoveItem(target);
            }

            bool succes = inventory.AddItem(target);

            if (succes){
                target.IsVisible = false;
                State.world.ToRemove(target);
            }
        }

        return true;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        Inventory? inventory = obj.GetComponent<Inventory>();
        EquipmentList? Equipments = obj.GetComponent<EquipmentList>();

        if (inventory != null && inventory.HasExactItem(target))
            return false;
        
        if (Equipments != null && Equipments.HasEquip(target))
            return false;


        return true;
    }
}