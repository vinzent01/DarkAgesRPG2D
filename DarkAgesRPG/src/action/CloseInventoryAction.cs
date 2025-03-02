namespace DarkAgesRPG;

public class CloseInventoryAction : Action {
    public CloseInventoryAction() : base("Close Inventory"){

    }

    public override bool OnStart()
    {
        string widgetId = obj.id + " inventory";

        if (target.HasComponentType<ContainerComponent>()){
            var container = target.GetComponent<ContainerComponent>();

            container?.Close();
            return true;
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