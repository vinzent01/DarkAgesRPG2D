namespace DarkAgesRPG;

public class ItemStack : Object {

    public ItemStack(){
        Name = "Item Stack";
        AddComponent(new ItemStackComponent());
    }

    public int Count {
        get {
            var ItemStackComponent = GetComponent<ItemStackComponent>();

            if (ItemStackComponent != null)
                return ItemStackComponent.stackCount;

            return 0;
        }
    }

    public void Add(Object obj){
        var ItemStackComponent = GetComponent<ItemStackComponent>();

        if (ItemStackComponent != null){
            ItemStackComponent.Add(obj);
            id = ItemStackComponent.stack_id;
        }
        
    }

    public void Add(List<Object> objs){
        foreach (var item in objs){
            Add(item);
        }
    }

    public void Remove(Object obj){
        var ItemStackComponent = GetComponent<ItemStackComponent>();

        if (ItemStackComponent != null)
            ItemStackComponent.Remove(obj);
    }

    public void RemoveFromWorld(){

        State.world.ToRemove(this);

        
    }

    public List<Object> GetItems(){
        var ItemStackComponent = GetComponent<ItemStackComponent>();

        if (ItemStackComponent != null)
            return ItemStackComponent.objects;

        return [];
    }
}