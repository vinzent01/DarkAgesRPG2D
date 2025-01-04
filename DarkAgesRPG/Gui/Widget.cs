using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class Widget {

    public Vector2 Position;
    public Vector2 Size;
    public bool CanDrag = false;
    public string id;


    public virtual void DrawHUD(){

    }

    public virtual void Update(float delta){
        if (CanDrag && IsDragging()){
            Position += Raylib.GetMouseDelta();
        }
    }

    public bool Contains(Vector2 Position){
        if (Position.X >= this.Position.X && Position.X <= this.Position.X + this.Size.X){
            if (Position.Y >= this.Position.Y && Position.Y <= this.Position.Y + this.Size.Y){
                return true;
            }   
        }
        return false;
    }

    public bool MouseInWidget(){
        var mousePos = Raylib.GetMousePosition();
        return Contains(mousePos);
    }

    public bool IsDragging(){
        if (MouseInWidget() && Raylib.IsMouseButtonDown(MouseButton.Left)){
            return true;
        }
        return false;
    }
}