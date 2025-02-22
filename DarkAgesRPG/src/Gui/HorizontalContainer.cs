namespace DarkAgesRPG.Gui;

public class HorizontalContainer : Widget {

    public HorizontalContainer(params Widget[] children){
        flowDirection = FlowDirections.Horizontal;

        foreach (var child in children){
            AddChild(child);
        }

    }
}