namespace DarkAgesRPG;

public class ComponentDeserializer{

    public static Component? Deserialize(string className, Dictionary<string, object> parameters){

        switch (className){
            case  "Hair":
                return Hair.Deserialize(parameters);

            case "Beard":
                return Beard.Deserialize(parameters);

            case "RaceComponent" :
                return RaceComponent.Deserialize(parameters);

            case "Sprite":
                return Sprite.Deserialize(parameters);

            case "Equipment":
                return EquipmentComponent.Deserialize(parameters);

            case "ItemComponent":
                return new Item();
            default:
                return default;
        }
    }
}