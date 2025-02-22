
using System.Diagnostics;
using System.Geometry;
using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class PlayerControler : Component{

    public float movementSpeed;
    public Actor ownerActor;

    public PlayerControler(){
        movementSpeed = 2.5f;
        
    }

    public override void Load()
    {
        Debug.Assert(owner is Actor);
        ownerActor = owner as Actor;
    }

    public override bool HandleActions(){
        Vector2 direction = new Vector2(0,0);

        if (Input.IsKeyDown(KeyboardKey.A)){
            direction.X = -1;
            owner.Flip(true);
        }
        else if (Input.IsKeyDown(KeyboardKey.D)){
            direction.X = 1;
            owner.Flip(false);
        }
        if (Input.IsKeyDown(KeyboardKey.W)){
            direction.Y = -1;
        }
        else if (Input.IsKeyDown(KeyboardKey.S)){
            direction.Y = 1;
        }

        if (direction != Vector2.Zero){
            ownerActor.AppendAction(new StepAction(movementSpeed, direction));
            return true;
        }
        return false;
    }
}

