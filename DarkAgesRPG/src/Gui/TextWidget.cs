using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class TextWidget : Widget {

    private string text;

    public string Text {
        get {
            return text;
        }
        set {
            text = value;
            SetWidgetSize(text, this.fontSize);
        }
    }

    public TextWidget(string text, int fontSize){
        this.text = text;
        this.fontSize = fontSize;
        SetWidgetSize(text, fontSize);
        this.color = Color.White;
        this.DoMouseCollision = false;
    }

    protected override void OnDrawHud()
    {
        Raylib.DrawText(text, (int)GlobalPosition.X, (int)GlobalPosition.Y, fontSize, this.color);
    }

}   