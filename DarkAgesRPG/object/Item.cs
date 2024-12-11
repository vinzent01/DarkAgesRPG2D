namespace DarkAgesRPG;


public class ItemStack {
    public int MaxStack = 1;
    public int stackCount = 0;
    public List<Object> objects;
    public string? object_id;
    public string? name;

    public ItemStack(){
        objects = new();
    }

    public void Add(Object obj){
        if (object_id == null){
            objects.Add(obj);
            object_id = obj.id;
            name = obj.Name;
            stackCount ++;
        }
        else {
            if (obj.id == object_id){
                objects.Add(obj);
                stackCount ++;
            }
        }
    }

    public void Remove(Object obj){
        objects.Remove(obj);
    }
}