using DarkAgesRPG.Gui;
using Raylib_cs;

namespace DarkAgesRPG;

public class InventoryHud : Component {

    public override void Update(float delta)
    {

        if (Input.IsKeyPressed(Raylib_cs.KeyboardKey.I)){

            if (!State.RootWidget.HasId("player inventory") && owner != null){
                var  playerInventory = new InventoryWidget(owner);
                playerInventory.id = "player inventory";
                
                State.RootWidget.SpawnWidget(playerInventory);
            }
        }
    }

}