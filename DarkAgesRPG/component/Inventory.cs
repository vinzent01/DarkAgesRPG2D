using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;

namespace DarkAgesRPG;

public class Inventory : Component {
    public List<Object> itemsOrStacks;
    public System.Action OnChange;
    public System.Action OnEmpty;

    public Inventory(){
        itemsOrStacks = new();
    }

    public override void Update(float delta)
    {
        base.Update(delta);
    }

    public bool HasExactItem(Object item){
        foreach (var itemOrStack in itemsOrStacks){
            if (itemOrStack == item){
                return true;
            }

            if (itemOrStack is ItemStack itemStack){
                foreach (var itemInStack in itemStack.GetItems()){
                    if (item == itemInStack )
                        return true;
                }
            }
        }
        return false;
    }

    public void AddItem(Object item) {

        var itemComponent = item.GetComponent<Item>();
        var itemStackComponent = item.GetComponent<ItemStackComponent>();

        if (itemComponent != null || itemStackComponent != null){
            // ok            
        }
        else {
            Debug.Fail("Item must have at least ItemComponent or ItemStackComponent");
        }


        // Verifica se já existe uma pilha para o mesmo item e adiciona, respeitando o limite
        foreach (var itemOrStack in itemsOrStacks) {
            if (itemOrStack is ItemStack itemStack && itemStack.id == item.id) {
                itemStack.Add(item);
                itemStack.IsVisible = false;
                
                if (itemComponent == null){
                    Debug.Fail("Item must have item component 1");
                }

                itemComponent.inventory = this;
                OnChange?.Invoke();
                return;
            }
        }

        // Verifica se o item já existe na lista individualmente
        for (int i = 0; i < itemsOrStacks.Count; i++) {
            if (itemsOrStacks[i] is Object obj && obj.id == item.id) {
                // Cria uma nova pilha e transfere o item existente para a pilha
                var newItemStack = new ItemStack();
                newItemStack.Add(obj);
                newItemStack.Add(item);

                if (itemComponent == null){
                    Debug.Fail("Item must have item component 2");
                }

                itemComponent.inventory = this;
                
                itemsOrStacks[i] = newItemStack; // Substitui o item pela nova pilha
                OnChange?.Invoke();
                return;
            }
        }

        // Caso contrário, adiciona o item como novo

        if (item is ItemStack itemStack1){
            // ok
        }
        else {
            if (itemComponent == null){
                Debug.Fail("Item must have item component 3");
            }

            itemComponent.inventory = this;
        }

        itemsOrStacks.Add(item);
        OnChange?.Invoke();
    }
    public void RemoveItem(Object obj) {
        // Tenta remover diretamente se o objeto está na lista principal
        if (itemsOrStacks.Remove(obj)) {
            obj.Parent = null;
            OnChange?.Invoke();

            Console.WriteLine("EMPTY IVNENTORY ITEM " + owner.GetType());
            if (itemsOrStacks.Count == 0){

                OnEmpty?.Invoke();
            }
            return;
        }

        // Procura dentro das pilhas
        foreach (var stack in itemsOrStacks) {
            if (stack is ItemStack itemStack) {
                var itemToRemove = itemStack.GetItems().FirstOrDefault(item => item.id == obj.id);
                if (itemToRemove != null) {
                    itemStack.Remove(itemToRemove);

                    // Remove a pilha se ela ficou vazia
                    if (itemStack.Count == 0) {
                        itemsOrStacks.Remove(itemStack);
                    }

                    if (itemsOrStacks.Count == 0){
                        OnEmpty?.Invoke();
                        Console.WriteLine("EMPTY IVNENTORY ITEM STACK");
                    }

                    OnChange?.Invoke();
                    return;
                }
            }
        }


        // Caso o item não tenha sido encontrado, nada é feito
    }
}