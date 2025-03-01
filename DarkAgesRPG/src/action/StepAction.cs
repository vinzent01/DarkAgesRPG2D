using System.Diagnostics;
using System.Geometry;
using System.Numerics;

namespace DarkAgesRPG;

public class StepAction : Action
{
    public float movementSpeed;
    public bool isMoving = false;
    private Bezier? movementCurve;
    private float movementT;
    private Vector2 Direction;

    public StepAction(float movementSpeed, Vector2 direction) : base("Step")
    {
        if (direction == Vector2.Zero)
            throw new ArgumentException("Direction não pode ser zero.");

        this.Direction = direction;
        this.movementSpeed = movementSpeed;
    }

    public StepAction() : base("Step") { }

    public bool Move(float delta)
    {
        if (movementCurve == null || obj == null)
            throw new InvalidOperationException("movementCurve ou obj não inicializado antes de Move ser chamado.");

        movementT += delta * movementSpeed;
        movementT = Math.Min(movementT, 1.0f); // Evita overshooting

        obj.RelativePosition = movementCurve.Position(movementT);

        if (movementT >= 1)
        {
            isMoving = false;
            movementT = 0;
            obj.RelativePosition = movementCurve.Position(1);
            return true;
        }
        return false;
    }

    public Bezier StartMove(Vector2 direction, Vector2 RelativePosition, int CellSize)
    {
        Debug.Assert(obj != null);

        // check collision to the next cell
        var nextCell = obj.CellPosition + new Vector2i(direction);
        var ObjNextCell = State.world.Get(nextCell);

        if (ObjNextCell.Count > 0){
            direction = new Vector2(0,0);
        }

        // bezier curve
        Vector2 p1 = RelativePosition;

        Vector2 p2 = new Vector2(
            p1.X + (direction.X * CellSize / 2),
            p1.Y + (direction.Y * CellSize / 2) - (CellSize * 1.5f)
        );

        Vector2 p3 = new Vector2(
            p1.X + (direction.X * CellSize), 
            p1.Y + (direction.Y * CellSize)
        );

        return new Bezier(p1, p2, p3);
    }

    public override bool OnStart()
    {
        if (!isMoving && Direction != Vector2.Zero && obj != null)
        {
            movementCurve = StartMove(Direction, obj.RelativePosition, State.Config.TileSize);
            isMoving = true;
            return true;
        }
        return false;
    }

    public override bool OnUpdate(float delta)
    {
        return isMoving && Move(delta);
    }

    public override void OnEnd()
    {
        isMoving = false;
        movementT = 0;
        movementCurve = null;

        var footPlacementComponent = obj.GetComponent<FootPosition>();

        if (footPlacementComponent != null){
            footPlacementComponent.UpdateCellPositions();
        }
    }
}
