using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DarkAgesRPG;

public class ActionDeserialzer {

    private static Dictionary<string, Type> ActionTypes = new();

    static ActionDeserialzer(){
        var assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes()){
            if (typeof(Action).IsAssignableFrom(type) && !type.IsAbstract){
                ActionTypes.Add(type.Name, type);
            }
        }
    }

    public static Action? Deserialize(string className, Dictionary<string, object> parameters){

        if (ActionTypes.TryGetValue(className, out Type? type))
        {
            return Activator.CreateInstance(type) as Action;
        }

        return null;
    }
}