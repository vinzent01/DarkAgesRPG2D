using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ButtonText : Widget {
    public string Text;
    public ButtonText(Vector2 pos, string Text){
        this.Position = pos;
        this.Text = Text;
    }

    public override void DrawHUD()
    {
        this.Size.X = Raylib.MeasureText(Text, 22);
        this.Size.Y = 22;

        Color color;

        if (MouseInWidget()){
            color = Color.DarkBlue;
        }
        else {
            color = Color.Blue;
        }

        Raylib.DrawRectangle((int)Position.X, (int)Position.Y, (int)this.Size.X  + 10, 22 + 10, color);
        Raylib.DrawText(Text, (int)Position.X + 5, (int)Position.Y + 5, 22, Color.White);
    }

    public bool IsClick(){
        if (MouseInWidget() && Raylib.IsMouseButtonPressed(MouseButton.Left)){
            return true;
        }
        return false;
    }
}