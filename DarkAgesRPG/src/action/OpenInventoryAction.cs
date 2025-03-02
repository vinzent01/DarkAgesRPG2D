namespace DarkAgesRPG;

public class OpenInventoryAction : Action {
    public OpenInventoryAction() : base("Open Inventory"){

    }

    public override bool OnStart()
    {
        string widgetId = target.id + " inventory";

        var container = target.GetComponent<ContainerComponent>();
        container?.Open();

        // spawn inventory widget
        if (!State.RootWidget.HasId(widgetId)){
            Gui.Widget inventoryWidget = new Gui.InventoryWidget(target);
            inventoryWidget.id = widgetId;    
            State.RootWidget.SpawnWidget(inventoryWidget);

            return true;
        }

        return false;
    }
}
