using System.Reflection;

namespace DarkAgesRPG;

public class ComponentDeserializer
{
    private static Dictionary<string, Type> ComponentTypes = new();

    static ComponentDeserializer()
    {
        // Obtém todas as classes que herdam de Component automaticamente
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(Component).IsAssignableFrom(type) && !type.IsAbstract)
            {
                ComponentTypes[type.Name] = type;
            }
        }
    }

    public static Component? Deserialize(string className, Dictionary<string, object> parameters)
    {
        if (ComponentTypes.TryGetValue(className, out var type))
        {
            var method = type.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
            {
                return method.Invoke(null, new object[] { parameters }) as Component;
            }

            // Caso a classe não tenha um método Deserialize, tenta criar uma instância
            return Activator.CreateInstance(type) as Component;
        }

        return null;
    }
}