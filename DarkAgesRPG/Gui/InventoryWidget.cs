using System.ComponentModel;
using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class InventoryItemWidget : Widget {

    public Object Item;

    public InventoryItemWidget(Object owner, Object Item){
        backgroundColor = new Color(10, 10, 100, 100);
        id = "Inventory Item Widget";

        this.Item = Item;
        
        margin = new Margin{
            bottom = 10, left = 10, right = 10, top = 10
        };

        this.Grow = new Grow{
            growX = true,
            growY = false
        };

        DoMouseCollision = true;
        CanDrag = true;

        var name = "";
        var count = 0;

        var equipment = owner.GetComponent<EquipmentList>();

        if (equipment != null){
            if (equipment.HasEquip(Item))
                name += "[EQUIPED] ";
        }

        if (Item.HasComponentType<ItemStackComponent>()){
            var itemStack = Item.GetComponent<ItemStackComponent>();

            if (itemStack != null){
                name += itemStack.Name;
                count = itemStack.stackCount ;
            }
        }
        else {
            name += Item.Name;
            count = 1;
        }

  
        var countText = new TextWidget(count + "x", 18);
        var nameText = new TextWidget(name, 18);

        countText.DoMouseCollision = false;
        nameText.DoMouseCollision = false;

        var HorizontalContainer = new HorizontalContainer();

        HorizontalContainer.DoMouseCollision = false;

        HorizontalContainer.AddChild(nameText);

        if (count > 1)
            HorizontalContainer.AddChild(countText);

        AddChild(HorizontalContainer);
    }

    protected override void OnClick()
    {
        var actionMenu = new ActionMenu(Item, Globals.player);

        Globals.RootWidget.SpawnWidget(actionMenu);
    }
}

public class InventoryWidget : Widget{

    public Object owner;

    public InventoryWidget(Object owner){
        this.owner = owner;
        id = "Inventory Widget";
        CanDrag = true;
        DoMouseCollision = true;
        IsContainer = true;

        this.TotalSize = new Vector2i(0, 200);

        this.margin = new Margin(){
            bottom = 10,left = 10, right = 10, top = 10
        };

        backgroundColor = new Color(25,25,25,100);

        var inventory = owner.GetComponent<Inventory>();
        var equipments = owner.GetComponent<EquipmentList>();

        if (inventory != null)
            inventory.OnChange = () => { UpdateInventoryItems(); };

        if (equipments != null)
            equipments.OnChange = () => {UpdateInventoryItems(); };

        UpdateInventoryItems();
    }


    public void UpdateInventoryItems(){
        RemoveAllChild();

        var horizontalContainer = new HorizontalContainer(
            new TextWidget(owner.Name + " Inventory", 22),
            new ButtonText("X", this.CloseWidget)
        );

        horizontalContainer.Grow = new Grow{
            growX = true, growY = false
        };

        horizontalContainer.DoMouseCollision = true;

        AddChild(horizontalContainer);

        var equipments = owner.GetComponent<EquipmentList>();
        var inventory = owner.GetComponent<Inventory>();

        if (equipments != null){
            // show equipments

            foreach (var equip in equipments.equips){
                AddChild(new InventoryItemWidget(owner, equip));
            }
        }

        if (inventory != null){
            // show inventory items

            foreach (var item in inventory.itemsOrStacks){
                /// Draw stack amount
                AddChild(new InventoryItemWidget(owner, item));
            }
        }

        FitContent();
    }

    protected override void OnSendDropWidget(Widget widget)
    {
        var inventory = owner.GetComponent<Inventory>();
        var equipments = owner.GetComponent<EquipmentList>();

        if (widget is InventoryItemWidget){
            var inventoryItemWidget = (InventoryItemWidget)widget;
            var widgetItem = inventoryItemWidget.Item;
            
            if (equipments != null){
                if (equipments.HasEquip(widgetItem)){
                    equipments.RemoveEquip(widgetItem);
                    widgetItem.IsVisible = false;
                    return;
                }
            }

            if (inventory != null){
                inventory.RemoveItem(widgetItem);
                return;
            }
            
        }
    }

    protected override bool OnDropOnGround(Widget widget) {

        if (widget is InventoryItemWidget itemWidget) {
            var item = itemWidget.Item;
            var dropAction = new DropItemAction();

            dropAction.Execute(owner, item);
            return true;
        }

        return false;
    }


    protected override bool OnRecieveDropWidget(Widget widget)
    {
        var inventory = owner.GetComponent<Inventory>();

        if (inventory == null)
            return false;

        if (widget is InventoryItemWidget){
            var InventoryItem = ( InventoryItemWidget )widget;
            var widgetItem = InventoryItem.Item;

            if (widgetItem is ItemStack){
                var itemStack = (ItemStack)widgetItem;
                inventory.AddItem(itemStack);
            }

            else if (widgetItem is Object){
                var obj = widgetItem;
                inventory.AddItem(obj);
            }
            
            return true;
        }

        return false;
    }

    protected override void OnClose(){
        
        // if is container
        if (owner.HasComponentType<ContainerComponent>()){
            var CloseAction = new CloseInventoryAction();

            if (CloseAction.MeetsCondition(Globals.player, owner)){
                CloseAction.Execute(Globals.player, owner);
            }
        }
    }
}