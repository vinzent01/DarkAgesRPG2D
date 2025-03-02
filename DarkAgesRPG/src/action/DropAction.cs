
namespace DarkAgesRPG;

public class DropItemAction : Action {
    public DropItemAction() : base("Drop Item"){

    }



    public override bool OnStart()
    {
        var inventory = obj.GetComponent<Inventory>();
        var equipment = obj.GetComponent<EquipmentList>();

        if (inventory != null)
            inventory.RemoveItem(target);

        if (equipment != null && equipment.HasEquip(target))
            equipment.RemoveEquip(target);

        var objectsOnGround = State.world.Get(obj.CellPosition);
        
        foreach (var objOnGround in objectsOnGround){
            if (objOnGround is ItemstackContainer itemStack){
                if (target is ItemstackContainer targetItemStack){
                    // add itemstack items
                    itemStack.AddItems(targetItemStack.GetItems());
                }
                else {
                    // add one item
                    itemStack.AddItem(target);
                }
                return true;
            }
        }

        // create an item stack countainer 
        if (target is ItemStack){
            List<Object> ItemsToAddStack = [target];
            
            foreach (var objOnGround in objectsOnGround){
                if (objOnGround.HasComponentType<ItemComponent>()){
                    ItemsToAddStack.Add(objOnGround);
                    State.world.ToRemove(objOnGround);
                }
            }

            var itemStackObject = new ItemstackContainer("item stack", "item-stack", ItemsToAddStack);

            itemStackObject.CellPosition = obj.CellPosition;
            itemStackObject.IsVisible = true;
            itemStackObject.Load();

            State.world.AddChild(itemStackObject);
            return true;
        }
        else {
            // Inicializa a lista com o alvo
            List<Object> ItemsToAddStack = new List<Object> { target };


            foreach (var objOnGround in objectsOnGround) {
                if (objOnGround.HasComponentType<ItemComponent>()) {
                    ItemsToAddStack.Add(objOnGround); // Adiciona o item à pilha
                    State.world.ToRemove(objOnGround);
                }
            }

            if (ItemsToAddStack.Count > 1) {
                // Cria uma nova pilha de itens
                var itemStackObject = new ItemstackContainer("item stack", "item-stack", ItemsToAddStack);

                itemStackObject.CellPosition = obj.CellPosition;
                itemStackObject.IsVisible = true;
                itemStackObject.Load();

                State.world.AddChild(itemStackObject);
                return true;
            }

            // Se houver apenas um item, reposiciona e torna visível
            target.CellPosition = obj.CellPosition;
            target.IsVisible = true;
            State.world.AddChild(target);
            return true;
        }
    }
}
