using System.Security.Cryptography;

namespace DarkAgesRPG;

public class ActionsToPerform : Component {
    public List<Action> Actions;

    public ActionsToPerform(
        List<Action> actions
    ){
        Actions = actions;
    }

    public void AddAction(Action action){
        Actions.Add(action);
    }

    public void RemoveAction(Action action){
        Actions.Remove(action);
    }
}