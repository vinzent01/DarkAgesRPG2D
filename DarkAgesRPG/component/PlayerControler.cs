
using System.Geometry;
using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class PlayerControler : Component{
    Bezier? movementCurve;
    public float movementSpeed;
    bool isMoving = false;
    private float movementT;

    public PlayerControler(){
        movementSpeed = 2.5f;
    }

    public Bezier StartMove(Vector2 direction, Vector2 RelativePosition, int CellSize){
        Vector2 p1 = RelativePosition;

        Vector2 p2 = new Vector2(
            RelativePosition.X + (direction.X * CellSize / 2), 
            direction.Y < 0 ?  RelativePosition.Y + (-1 * CellSize  * 2) : RelativePosition.Y + (-1 * CellSize / 2)
        );

        Vector2 p3 = new Vector2(
            RelativePosition.X + (direction.X * CellSize ), 
            RelativePosition.Y + (direction.Y * CellSize)
        );

        return new Bezier(p1, p2, p3);
    }

    public void Move(float delta){
        movementT += delta * movementSpeed;

        if (movementCurve != null && owner != null){
            owner.RelativePosition = movementCurve.Position(movementT);

            if (movementT >= 1){
                isMoving = false;
                movementT = 0;
                owner.RelativePosition = movementCurve.Position(1);
            }
        }
    }

    public Vector2 GetInputDirection(){
        Vector2 InputDirection = Vector2.Zero;


        // flip all children
        void FlipRecursive(Object currentObj, bool flipped){

            // Flip the current object's sprite if it exists
            Sprite? sprite = currentObj.GetComponent<Sprite>();
            if (sprite != null)
            {
                sprite.isFliped = flipped;
            }

            // Iterate over the children and call the method recursively
            foreach (var child in currentObj.Children.ToList())
            {
                FlipRecursive(child, flipped);
            }
        }

        if (Input.IsKeyDown(KeyboardKey.A)){
            InputDirection.X = -1;
            FlipRecursive(owner, true);
        }
        else if (Input.IsKeyDown(KeyboardKey.D)){
            InputDirection.X = 1;

            FlipRecursive(owner, false);
        }
        if (Input.IsKeyDown(KeyboardKey.W)){
            InputDirection.Y = -1;
        }
        else if (Input.IsKeyDown(KeyboardKey.S)){
            InputDirection.Y = 1;
        }

        return InputDirection;
    }

    public override void Update(float delta)
    {
        Vector2 InputDirection = GetInputDirection();

        if (isMoving == false && InputDirection != Vector2.Zero && owner != null){
            movementCurve = StartMove(InputDirection, owner.RelativePosition, Globals.TileSize);
            isMoving = true;
        }

        if (isMoving){
            Move(delta);
        }
    }
}

