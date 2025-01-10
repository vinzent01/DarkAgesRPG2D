namespace DarkAgesRPG;

public class ItemStackComponent : Component {
    public int MaxStack = 10;
    public int stackCount = 0;
    public List<Object> objects;
    public string? stack_id;
    public string? Name;
    public Inventory? inventory;

    public ItemStackComponent(){
        objects = new();
    }

    public void Add(Object obj){
        if (stackCount >= MaxStack) {
            throw new InvalidOperationException("Max stack limit reached");
        }
        
        if (stack_id == null){
            objects.Add(obj);
            stack_id = obj.id;
            Name = obj.Name;
            stackCount ++;
        }
        else {
            if (obj.id == stack_id){
                objects.Add(obj);
                stackCount ++;
            }
        }
    }

    public void Remove(Object obj){
        objects.Remove(obj);
        stackCount --;
    }
    public void RemoveAll(){

        foreach (var obj in objects.ToList()){
            objects.Remove(obj);
            stackCount --;
        }
    }
}