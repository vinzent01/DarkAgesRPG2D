using System.Diagnostics;
using System.Geometry;
using System.Numerics;
using System.Runtime;
using CSScripting;
using DarkAgesRPG.Gui;

namespace DarkAgesRPG;
public class Action {
    public string? Name;
    public Actor? obj;
    public Object? target;
    public bool IsStarted = false;

    public Action(string Name){
        this.Name = Name;
        this.obj = obj;
        this.target = target;
    }

    public bool Update(float delta){

        if (IsStarted ){
            bool updatingResult = OnUpdate(delta);

            if (updatingResult == true){
                OnEnd();
                IsStarted = false;
                return true;
            }
        }
        return false;
    }

    public bool StartAction(Actor obj, Object target){
        this.obj = obj;
        this.target = target;
        IsStarted = true;
        return OnStart();
    } 

    public bool StartAction(Actor obj){
        this.obj = obj;
        IsStarted = true;
        return OnStart();
    } 

    public virtual bool OnStart(){
        return true;
    }

    public virtual bool OnUpdate(float delta){

        return true;
    }

    public virtual void OnEnd(){
    }

    public virtual bool MeetsCondition(Object obj, Object target){
        return true;
    }
}

public class TakeAction : Action {

    public TakeAction() : base("Take") {

    }

    public override bool OnStart()
    {
        Inventory? inventory = target.GetComponent<Inventory>();

        if (inventory != null){
            inventory.AddItem(obj);
        }

        obj.IsVisible = false;
        State.world.ToRemove(obj);


        return true;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        Inventory? inventory = target.GetComponent<Inventory>();
        EquipmentList? Equipments = target.GetComponent<EquipmentList>();

        if (inventory != null && inventory.HasExactItem(obj))
            return false;
        
        if (Equipments != null && Equipments.HasEquip(obj))
            return false;


        return true;
    }
}

public class EquipAction : Action {
    public EquipAction() : base("Equip") {

    }

    public override bool OnStart()
    {
        EquipmentList? equips = target.GetComponent<EquipmentList>();
        var itemComponent = obj.GetComponent<Item>();

        if (itemComponent == null)
            return false;

        if (equips == null)
            return false;

        if (itemComponent.inventory != null){
            itemComponent.inventory.RemoveItem(obj);
        }

        State.world.ToRemove(obj);
        equips.AddEquip(obj);
        obj.IsVisible = true;
        
        
        return true;
    }
}

public class OpenInventoryAction : Action {
    public OpenInventoryAction() : base("Open Inventory"){

    }

    public override bool OnStart()
    {
        string widgetId = target.id + " inventory";

        var container = target.GetComponent<ContainerComponent>();
        container?.Open();

        // spawn inventory widget
        if (!State.RootWidget.HasId(widgetId)){
            Gui.Widget inventoryWidget = new Gui.InventoryWidget(target);
            inventoryWidget.id = widgetId;    
            State.RootWidget.SpawnWidget(inventoryWidget);

            return true;
        }

        return false;
    }
}

public class CloseInventoryAction : Action {
    public CloseInventoryAction() : base("Close Inventory"){

    }

    public override bool OnStart()
    {
        string widgetId = obj.id + " inventory";

        if (target.HasComponentType<ContainerComponent>()){
            var container = target.GetComponent<ContainerComponent>();

            container?.Close();
            return true;
        }

        return false;
    }

    public override bool MeetsCondition(Object obj, Object target)
    {
        if (obj.HasComponentType<ContainerComponent>()){
            var container = obj.GetComponent<ContainerComponent>();

            if (container != null)
                return container.IsOpen;
            return false;
        }

        return true;
    }
}
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
