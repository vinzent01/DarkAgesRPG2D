using DarkAgesRPG.Gui;

namespace DarkAgesRPG;
public class Action {
    public string Name;

    public Action(string Name){
        this.Name = Name;
    }

    public virtual bool Execute(Object obj, Object target){
        return false;
    } 

    public virtual bool MeetsCondition(Object obj, Object target){
        return true;
    }
}

public class TakeAction : Action {

    public TakeAction() : base("Take") {

    }

    public override bool Execute(Object obj, Object target)
    {
        Inventory? inventory = target.GetComponent<Inventory>();

        if (inventory != null){
            inventory.AddItem(obj);
        }

        obj.IsVisible = false;

        return true;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        Inventory? inventory = target.GetComponent<Inventory>();
        EquipmentList? Equipments = target.GetComponent<EquipmentList>();

        if (inventory != null && inventory.HasExactItem(obj))
            return false;
        
        if (Equipments != null && Equipments.HasEquip(obj))
            return false;

        return true;
    }
}

public class EquipAction : Action {
    public EquipAction() : base("Equip") {

    }

    public override bool Execute(Object obj, Object target)
    {
        EquipmentList? equips = target.GetComponent<EquipmentList>();
        var itemComponent = obj.GetComponent<Item>();

        if (itemComponent == null)
            return false;

        if (equips == null)
            return false;

        if (itemComponent.inventory != null){
            itemComponent.inventory.RemoveItem(obj);
        }

        equips.AddEquip(obj);
        obj.IsVisible = true;
        
        return true;
    }
}

public class OpenInventoryAction : Action {
    public OpenInventoryAction() : base("Open Inventory"){

    }

    public override bool Execute(Object obj, Object target)
    {
        string widgetId = obj.id + " inventory";

        var container = obj.GetComponent<ContainerComponent>();
        container?.Open();

        // spawn inventory widget
        if (!Globals.RootWidget.HasId(widgetId)){
            Gui.Widget inventoryWidget = new Gui.InventoryWidget(obj);
            inventoryWidget.id = widgetId;
            
            Globals.RootWidget.SpawnWidget(inventoryWidget);
            return true;
        }

        return false;
    }
}

public class CloseInventoryAction : Action {
    public CloseInventoryAction() : base("Close Inventory"){

    }

    public override bool Execute(Object obj, Object target)
    {
        string widgetId = obj.id + " inventory";

        if (target.HasComponentType<ContainerComponent>()){
            var container = target.GetComponent<ContainerComponent>();

            container?.Close();
        }

        return false;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        if (obj.HasComponentType<ContainerComponent>()){
            var container = obj.GetComponent<ContainerComponent>();

            if (container != null)
                return container.IsOpen;
            return false;
        }

        return true;
    }
}

public class DropItemAction : Action {
    public DropItemAction() : base("Close Inventory"){

    }

    public override bool Execute(Object obj, Object target)
    {
        var inventory = obj.GetComponent<Inventory>();
        var equipment = obj.GetComponent<EquipmentList>();

        if (inventory != null)
            inventory.RemoveItem(target);

        if (equipment != null && equipment.HasEquip(target))
            equipment.RemoveEquip(target);


        // Drop on ground

        // create an item stack countainer 
        if (target is ItemStack){
            var container = new Object();
            container.Name = "Item Stack";
            container.AddComponent(new InteractComponent());
            container.AddComponent(new Inventory());
            container.AddComponent(new Sprite("./content/ItemStack/ItemStack.png"));
            container.AddComponent(new ActionsToPerform(
                new List<Action>(){
                    new OpenInventoryAction()
                }
            ));

            var containerInventory = container.GetComponent<Inventory>();

            if (containerInventory != null){
                containerInventory.AddItem(target);            
                containerInventory.OnEmpty = () => {
                    Globals.world.ToRemove(container); 
                };
            }

            container.CellPosition = obj.CellPosition;
            container.IsVisible = true;
            container.Load();
            Globals.world.Add(container);
            return true;
        }
        else {
            target.CellPosition = obj.CellPosition;
            target.IsVisible = true;
            Globals.world.Add(target);
            return true;
        }
    }
}