using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class RootWidget : Widget {

    public bool DebugDraw;

    public List<Widget> allWidgets;
    public Widget? currentDraggingWidget;

    public RootWidget(){
        id = "root";
        DoFlow = false;
        DebugDraw = true;
    }

    public void SpawnWidget(Widget widget){
        Random rnd = new Random();

        widget.GlobalPosition = new Vector2i(
            (Raylib.GetScreenWidth() / 2)  - (widget.TotalSize.X / 2) + rnd.Next(-100, 100), 
            (Raylib.GetScreenHeight() / 2) - (widget.TotalSize.Y / 2) + rnd.Next(-100, 100)
        );

        AddChild(widget);
    }

    protected override void OnDrawHud()
    {
        if (DebugDraw){
            var WidgetOnMouse = GetWidgetOnMouse();
            if (WidgetOnMouse != null){
                Raylib.DrawRectangleLines(
                    WidgetOnMouse.GlobalPosition.X, 
                    WidgetOnMouse.GlobalPosition.Y, 
                    (int)WidgetOnMouse.TotalSize.X, 
                    (int)WidgetOnMouse.TotalSize.Y, 
                    Color.Violet
                );
            }

            var containerOnMouse = GetContainerWidgetOnMouse();
            
            if (containerOnMouse != null){
                Raylib.DrawRectangleLines(
                    containerOnMouse.GlobalPosition.X, 
                    containerOnMouse.GlobalPosition.Y, 
                    (int)containerOnMouse.TotalSize.X, 
                    (int)containerOnMouse.TotalSize.Y, 
                    Color.Green
                );
            }

        }
    }

    public Widget? GetWidgetOnMouse(Widget? currentWidget = null)
    {
        var widget = currentWidget ?? this;
        var mousePos = Raylib.GetMousePosition();

        if (widget.Childrens.Count > 0)
        {
            var childrenReversed = widget.Childrens.ToList();
            childrenReversed.Reverse();
            foreach (var child in childrenReversed)
            {
                if (child.DoMouseCollision == false)
                    continue;
                
                if (child.Contains(new Vector2i(mousePos)) )
                {
                    return GetWidgetOnMouse(child);
                }
            }
        }

        if (widget.Contains(new Vector2i(mousePos)))
        {
            return widget;
        }

        return null;
    }

    public Widget? GetContainerWidgetOnMouse(Widget? currentWidget = null)
    {
        var widget = currentWidget ?? this;
        var mousePos = Raylib.GetMousePosition();

        if (widget.Childrens.Count > 0)
        {
            var childrenReversed = widget.Childrens.ToList();
            childrenReversed.Reverse();
            foreach (var child in childrenReversed)
            {
                if (child.DoMouseCollision == false)
                    continue;

                if (child.IsContainer == false)
                    continue;
                
                if (child.Contains(new Vector2i(mousePos)) )
                {
                    return GetContainerWidgetOnMouse(child);
                }
            }
        }

        if (widget.Contains(new Vector2i(mousePos)))
        {
            if (widget.DoMouseCollision == false)
                return null;

            if (widget.IsContainer == false)
                return null;
            
            return widget;
        }

        return null;
    }

}