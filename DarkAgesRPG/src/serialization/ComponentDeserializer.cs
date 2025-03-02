using System.Reflection;

namespace DarkAgesRPG;

public class ComponentDeserializer
{
    private static Dictionary<string, Type> _componentTypes = new();

    static ComponentDeserializer()
    {
        // Obtém todas as classes que herdam de Component automaticamente
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(Component).IsAssignableFrom(type) && !type.IsAbstract)
            {
                _componentTypes[type.Name] = type;
            }
        }
    }

    public static Component? Deserialize(string className, Dictionary<string, object> parameters)
    {
        if (_componentTypes.TryGetValue(className, out var type))
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