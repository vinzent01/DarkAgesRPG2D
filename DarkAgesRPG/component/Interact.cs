using System.Diagnostics;
using System.Numerics;
using DarkAgesRPG.Gui;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class InteractComponent : Component {

    public InteractComponent(){

    }

    public override void Update(float delta)
    {
        if (owner == null)
            return;

        var SpriteComponent = owner.GetComponent<Sprite>();

        // Must have an spritecomponent to get dimensions
        Debug.Assert(SpriteComponent != null);


        Vector2 mouse =Raylib.GetMousePosition();
        Vector2 mouseWorld = Raylib.GetScreenToWorld2D(mouse, Globals.camera);

        if (Globals.Widgets.IsMouseOnWidget())
            return;

        if (SpriteComponent.Contains(mouseWorld) && Raylib.IsMouseButtonReleased(MouseButton.Left)){

            Globals.Widgets.RemoveType<ActionMenu>();
            Globals.Widgets.Add(new ActionMenu(owner, Globals.player, new Vector2(mouse.X, mouse.Y)));
        }
    }
}
