using System.Diagnostics;
using System.Numerics;
using CSScripting;
using DarkAgesRPG.Gui;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class InteractComponent : Component {

    public InteractComponent(){

    }

    public override void Update(float delta)
    {
        if (owner == null)
            return;

        if (owner.IsVisible == false)
            return;

        if (Globals.player == null){
            return;
        }

        var SpriteComponent = owner.GetComponent<Sprite>();
        var multispriteComponent = owner.GetComponent<MultiSprite>();

        // Must have an spritecomponent to get dimensions
        Debug.Assert(
            SpriteComponent != null || (multispriteComponent != null && multispriteComponent.CurrentSprite != null)
        );

        Vector2 mouse =Raylib.GetMousePosition();
        Vector2 mouseWorld = Raylib.GetScreenToWorld2D(mouse, Globals.Camera.GetRaylibCamera());

        Sprite CollisionSprite;

        if (SpriteComponent != null)
            CollisionSprite = SpriteComponent;
        else
            CollisionSprite = multispriteComponent.CurrentSprite;

        if (
            CollisionSprite != null &&
            CollisionSprite.Contains(mouseWorld) && 
            IsMouseButtonPressed(MouseButton.Left) && 
            Globals.RootWidget.GetContainerWidgetOnMouse() == null
        ){
            var ActionsToPerform = owner.GetComponent<ActionsToPerform>();

            if (ActionsToPerform != null){

                var actions = ActionsToPerform.Actions;

                if (actions.Count == 0)
                    return;

                if (actions.Count> 1){
                    // create action menu
                    var actionMenuId = owner.id + " action menu";


                    if (!Globals.RootWidget.HasId(actionMenuId)){
                        ActionMenu actionMenu = new ActionMenu(owner, Globals.player);
                        actionMenu.id = actionMenuId;

                        Globals.RootWidget.SpawnWidget(actionMenu);
                    }
                }
                else {
                    // execute first action
                    var firstAction = actions[0];

                    firstAction.Execute(owner, Globals.player);
                }
            }            
        }
    }
}
