using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class TextWidget : Widget {

    string text;    

    public TextWidget(string text, int fontSize){
        this.text = text;
        SetFontSizeSize(text, fontSize);
        this.color = Color.White;
        this.DoMouseCollision = false;
    }

    protected override void OnDrawHud()
    {
        Raylib.DrawText(text, (int)GlobalPosition.X, (int)GlobalPosition.Y, fontSize, this.color);
    }

}   