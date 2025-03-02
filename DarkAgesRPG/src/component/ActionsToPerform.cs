using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

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

    public static ActionsToPerform Deserialize(Dictionary<string, object> parameters){
        string[] ActionsClass = (parameters["actions"] as JArray).ToObject<string[]>();
        List<Action> actions = new();

        if (ActionsClass == null){
            return new ActionsToPerform([]);    
        }
        
        foreach (var action in ActionsClass){
            var result = ActionDeserialzer.Deserialize(action);

            if (result != null)
                actions.Add(result);
        }

        return new ActionsToPerform(actions);
    }
}