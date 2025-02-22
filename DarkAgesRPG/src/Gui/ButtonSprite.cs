using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ButtonSprite : Widget {

    System.Action? OnClickAction;

    public ButtonSprite(SpriteRenderer spriteRenderer, System.Action? OnClick = null){
        backgroundColor = Color.Blue;
        this.color = Color.White;
        this.DoMouseCollision = true;
        this.margin = new Margin{
            bottom = 10, left = 10, right = 10, top = 10
        };
        
        id = "buttonText";

        if (OnClick != null)
            this.OnClickAction = OnClick;

        AddChild(spriteRenderer);
    }

    protected override void OnDrawHud()
    {
        var root = GetRoot();

        if (root != null && root.GetWidgetOnMouse() == this ){
            backgroundColor = Color.DarkBlue;
        }
        else {
            backgroundColor = Color.Blue;
        }

    }

    protected override void OnUpdate(float delta)
    {
        if (IsClick()){
            OnClickAction?.Invoke();
        }
    }
}