
namespace DarkAgesRPG;

public class ItemstackContainer : Container{

    public ItemstackContainer(string name, string id, List<Object> items) : 
        base(name, id, items, new Sprite("./content/ItemStack/ItemStack.png")) {
    }

}