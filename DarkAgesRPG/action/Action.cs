using CSScripting;
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
        Globals.world.ToRemove(obj);


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

        Globals.world.ToRemove(obj);
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

        // if has item stack add item to stack
        // Obtém objetos no chão na posição do objeto
        var objectsOnGround = Globals.world.Get(obj.CellPosition);
        
        foreach (var objOnGround in objectsOnGround){
            if (objOnGround is ItemstackContainer itemStack){
                if (target is ItemstackContainer targetItemStack){
                    // add itemstack items
                    itemStack.AddItems(targetItemStack.GetItems());
                }
                else {
                    // add one item
                    itemStack.AddItem(target);
                }
                return true;
            }
        }

        // create an item stack countainer 
        if (target is ItemStack){


            List<Object> ItemsToAddStack = [target];
            
            foreach (var objOnGround in objectsOnGround){
                if (objOnGround.HasComponentType<Item>()){
                    ItemsToAddStack.Add(objOnGround);
                    Globals.world.ToRemove(objOnGround);
                }
            }

            var itemStackObject = new ItemstackContainer("item stack", "item-stack", ItemsToAddStack);

            itemStackObject.CellPosition = obj.CellPosition;
            itemStackObject.IsVisible = true;
            itemStackObject.Load();

            Globals.world.Add(itemStackObject);
            return true;
        }
        else {
 

            // Inicializa a lista com o alvo
            List<Object> ItemsToAddStack = new List<Object> { target };


            foreach (var objOnGround in objectsOnGround) {
                if (objOnGround.HasComponentType<Item>()) {
                    ItemsToAddStack.Add(objOnGround); // Adiciona o item à pilha
                    Globals.world.ToRemove(objOnGround);
                }
            }

            if (ItemsToAddStack.Count > 1) {
                // Cria uma nova pilha de itens
                var itemStackObject = new ItemstackContainer("item stack", "item-stack", ItemsToAddStack);

                itemStackObject.CellPosition = obj.CellPosition;
                itemStackObject.IsVisible = true;
                itemStackObject.Load();

                Globals.world.Add(itemStackObject);
                return true;
            }

            // Se houver apenas um item, reposiciona e torna visível
            target.CellPosition = obj.CellPosition;
            target.IsVisible = true;
            Globals.world.Add(target);
            return true;
        }
    }
}