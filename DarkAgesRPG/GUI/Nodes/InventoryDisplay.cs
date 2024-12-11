namespace DarkAgesRPG.GUI;

using System.Data;
using Raylib_cs;

public class InventoryDisplay : Panel {

    Widget ItemsContainer; 
    Inventory Inventory;

    Color backgroundColor2 = new Color(68, 68, 68, 200);
    Color TextColor = new Color(234,234,234,255);

    public InventoryDisplay(ref Inventory inventory){
        Inventory = inventory;
        ItemsContainer = CreateItemsContainer();

        PushWidget(ItemsContainer);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        RemoveWidget(ItemsContainer);
        ItemsContainer.RemoveWidgets();

        foreach (var item in Inventory.items){
            ItemsContainer.PushWidget(CreateItem(item.stackCount,item.name, item.object_id));
        }

        PushWidget(ItemsContainer);
        UpdateStyles();
    }

    public Widget CreateItem(int count,string name, string id){
        Widget item = new();
        
        item.styles.Set("Grow", 1);
        item.styles.Set("PaddingDirection", paddingDirecton.horizontal);
        item.styles.Set("Color", backgroundColor2);
        item.styles.Set("PaddingIncrease", 5);


        Text ItemName = new();
        ItemName.TextString = name;
        ItemName.styles.Set("Color", TextColor);

        ButtonText Action = new("Action", 24);
        Action.styles.Set("Margin", new Vector2i(5,5));

        item.PushWidget(Action);


        if (count > 1){
            Text ItemCount = new();
            ItemCount.TextString = count.ToString( ) + "x";
            ItemCount.styles.Set("Color", TextColor);
            item.PushWidget(ItemCount);
        }
        
        item.PushWidget(ItemName);
        return item;
    }

    public Widget CreateItemsContainer(){
        Widget ItemsContainer = new Widget();

        ItemsContainer.styles.Set("PaddingDirection", paddingDirecton.vertical);
        ItemsContainer.styles.Set("Color", backgroundColor2);
        ItemsContainer.styles.Set("PaddingIncrease", 10);

        return ItemsContainer;
    }

}