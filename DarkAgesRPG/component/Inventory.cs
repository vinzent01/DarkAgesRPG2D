namespace DarkAgesRPG;

public class Inventory : Component {
    public List<ItemStack> items;

    public Inventory(){
        items = new();
    }

    public void AddItem(Object item){
        // if has stack
        foreach (var stack in items){
            if (stack.object_id == item.id){
                stack.Add(item);
                return;
            }
        }

        // create new stack
        ItemStack newStack = new();
        newStack.object_id = item.id;
        newStack.name = item.Name;
        newStack.Add(item);

        items.Add(newStack);
    }

    public void RemoveItem(string id){
        foreach (var stack in items){
            if (stack.object_id == id){

                if (stack.stackCount > 1)
                    stack.stackCount--;
                else {
                    items.Remove(stack);
                }

                return;
            }
        }
    }
}