
namespace DarkAgesRPG;

public class Container : Object{

    public Container(string name, string id, List<Object> items, Sprite sprite) {
        this.Name = name;
        this.id = id;
        this.AddComponent(new InteractComponent());
        this.AddComponent(new Inventory());
        this.AddComponent(sprite);
        this.AddComponent(new ActionsToPerform(
            new List<Action>(){
                new OpenInventoryAction()
            }
        ));

        var containerInventory = this.GetComponent<Inventory>();
        
        foreach (var obj in items){
            if (containerInventory != null){
                containerInventory.AddItem(obj);
            }
        }

        if (containerInventory != null)
            containerInventory.OnEmpty = () => {
                State.world.ToRemove(this); 
            };
    }

    public List<Object> GetItems(){
        var containerInventory = this.GetComponent<Inventory>();

        if (containerInventory != null)
            return containerInventory.itemsOrStacks;
        else 
            return [];
    }

    public bool AddItems(List<Object> objects){
        var containerInventory = this.GetComponent<Inventory>();

        if (containerInventory != null){
            foreach (var item in objects){
                containerInventory.AddItem(item);
            }
            return true;
        }

        return false;
    }

    public bool AddItem(Object obj){
        var containerInventory = this.GetComponent<Inventory>();

        if (containerInventory != null){
            containerInventory.AddItem(obj);
            return true;
        }

        return false;
    }
}