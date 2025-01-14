using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ColorButton : Widget {

    System.Action? OnClickAction;

    public ColorButton(Color color, System.Action? OnClick = null){
        this.backgroundColor = color;
        this.color = color;
        this.DoMouseCollision = true;
        this.margin = new Margin{
            bottom = 10, left = 10, right = 10, top = 10
        };
        this.TotalSize = new Vector2i(40,40);
        this.RoundBorders = 0.1f;

        if (OnClick != null)
            this.OnClickAction = OnClick;
    }

    protected override void OnClick()
    {
        OnClickAction?.Invoke();
    }
}