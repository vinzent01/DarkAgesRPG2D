using System.Numerics;

namespace DarkAgesRPG;


public class Actor : Object {
    public Queue<Action> ActionQueue { get; } = new Queue<Action>();
    public Action? CurrentAction;
    private float elapsedTime = 0.0f;
    private bool actionComplete = false;
    public Actor(){
    }

    public Actor(string name, string id, params Component[] components) : base(name, id, components){
    }

    public void AppendAction(Action action, Object target){
        Console.WriteLine("Adding new action " +  action.Name + " To " + this.Name);
        action.StartAction(this, target);
        ActionQueue.Enqueue(action);
    }

    public void AppendAction(Action action){
        Console.WriteLine("Adding new action " +  action.Name + " To " + this.Name);
        action.StartAction(this);
        ActionQueue.Enqueue(action);
    }

    public bool HandleActions(){
        bool result = false;

        foreach (var component in Components){
            var componentResult = component.HandleActions();

            if (componentResult == true)
                result = true;
        }

        return result;
    }

    public void NextAction(){
        if (ActionQueue.Count > 0){
            CurrentAction = ActionQueue.Dequeue();
        }
    }


    protected override void OnDraw()
    {
    }

    public void StartTurn(){
        Console.WriteLine($"{Name} começou seu turno!");
        elapsedTime = 0.0f;
        actionComplete = false;

        foreach (var component in Components){
            component.OnActorTurn();
        }
    }

    public bool UpdateTurn(float delta)
    {
        if (CurrentAction == null){
            NextAction();
        }

        if (CurrentAction != null){
            var isEnded = CurrentAction.Update(delta);

            if (isEnded){
                CurrentAction = null;
                NextAction();
            }
        }

        return CurrentAction == null && ActionQueue.Count == 0;
    }

    public void EndTurn()
    {
        Console.WriteLine($"{Name} terminou seu turno.");
    }

    public bool IsTurnComplete => actionComplete;
}
