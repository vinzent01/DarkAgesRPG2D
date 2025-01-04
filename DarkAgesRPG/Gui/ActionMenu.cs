using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ActionMenu : Widget{

    public Object Object;
    public Object Target;

    public ActionMenu(Object obj, Object target, Vector2 pos){
        this.Object = obj;
        this.Target = target;
        this.Position = pos;
        this.Size.X = 250;
        this.Size.Y = 400;
        CanDrag = true;
    }

    public override void DrawHUD()
    {
        var ActionsToPerform = Object.GetComponent<ActionsToPerform>();

        Raylib.DrawRectangle((int)Position.X,(int)Position.Y,(int)Size.X,(int)Size.Y, new Color(25,25,25,100));
        Raylib.DrawText(Object.Name,(int)Position.X, (int)Position.Y, 22, Color.White);

        int PaddingY = 22;

        if (ActionsToPerform != null){            
            // Buttons
            foreach (var action in ActionsToPerform.Actions){
                int gap = 5;
                int margin = 10;

                ButtonText button = new (new Vector2(Position.X, Position.Y + PaddingY), action.Name);

                button.DrawHUD();

                if (button.IsClick()){
                    action.Execute(Object, Target);

                    Globals.Widgets.Remove(this);
                }

                PaddingY += 22 + gap + margin;
            }
        }
        else {
            Raylib.DrawText("This Object has no actions to perform", (int)Position.X, (int)Position.Y, 22, Color.White);
        }
    }
}