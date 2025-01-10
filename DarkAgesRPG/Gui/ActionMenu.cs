using System.Numerics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Raylib_cs;

namespace DarkAgesRPG.Gui;

public class ActionButton : ButtonText {
    Action action;
    private Object obj;
    private Object target;

    public ActionButton(string text, Action action, Object obj, Object target) : base(text){
        this.action = action;
        this.obj = obj;
        this.target = target;
    }

    protected override void OnUpdate(float delta)
    {
        if (IsClick()){
            action.Execute(obj, target);
        }
    }
}

public class ActionMenu : Widget{

    public Object Object;
    public Object Target;

    public void ExecuteAction(Action action){
        action.Execute(Object, Target);
    }

    public ActionMenu(Object obj, Object target){
        this.Object = obj;
        this.Target = target;
        this.margin = new Margin{
            bottom = 10, left= 10, right= 10, top = 10
        };

        backgroundColor = new Color(25,25,25,100);
        CanDrag = true;

        var ActionsToPerform = Object.GetComponent<ActionsToPerform>();
        var ActionsText = new TextWidget(obj.Name + " Actions", 22);

        ActionsText.DoMouseCollision =false;

        AddChild(
            new HorizontalContainer(
                ActionsText,
                new ButtonText("X", this.CloseWidget)
            )
        );

        if (ActionsToPerform != null){            
            // Buttons
            foreach (var action in ActionsToPerform.Actions){
                if (action.MeetsCondition(Object, Target)){
                    System.Action performAction =  () =>{action.Execute(Object, Target); this.CloseWidget();};

                    AddChild(
                        new ButtonText(action.Name, performAction)
                    );
                }
            }
        }
        else {
            AddChild(
                new TextWidget("This Object has no actions to perform", 18)
            );
        }
    }
}