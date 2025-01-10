using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ButtonText : Widget {

    System.Action? OnClick;

    public ButtonText(string Text, System.Action? OnClick = null){
        backgroundColor = Color.Blue;
        this.color = Color.White;
        this.DoMouseCollision = true;
        this.margin = new Margin{
            bottom = 10, left = 10, right = 10, top = 10
        };

        id = "buttonText";

        if (OnClick != null)
            this.OnClick = OnClick;
            
        var textWidget = new TextWidget(Text, 22);
        textWidget.DoMouseCollision = false;

        AddChild(textWidget);
    }

    protected override void OnDrawHud()
    {
        if (GetRoot().GetWidgetOnMouse() == this ){
            backgroundColor = Color.DarkBlue;
        }
        else {
            backgroundColor = Color.Blue;
        }
    }

    protected override void OnUpdate(float delta)
    {
        if (IsClick()){
            OnClick();
        }
    }


}