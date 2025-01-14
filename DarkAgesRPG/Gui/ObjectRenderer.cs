using System.Numerics;

namespace DarkAgesRPG.Gui;

public class ObjectRendererWidget : Widget {

    public Object obj;

    public ObjectRendererWidget(Object obj){
        SetObject(obj);
    }


    public void SetObject(Object obj){
        this.obj =obj;

        var SizeComponent = obj.GetComponent<SizeComponent>();

        if (SizeComponent != null)
            this.TotalSize = SizeComponent.CalculateTotalSize(obj);
    }

    protected override void OnDrawHud()
    {

        obj.TotalPosition = new Vector2(this.GlobalPosition.X, this.GlobalPosition.Y);
        obj.Draw();
    }

}