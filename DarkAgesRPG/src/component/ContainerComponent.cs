using System.Diagnostics;
using System.Numerics;
using DarkAgesRPG.Gui;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class ContainerComponent : Component {
    public bool IsOpen;

    public ContainerComponent(){
        IsOpen = false;

    }

    public void Open(){
        IsOpen = true;

        var multisprite = owner?.GetComponent<MultiSprite>();

        if (multisprite != null){
            multisprite.SetCurrentSprite("open");
        }

    }

    public void Close(){
        IsOpen = false;

        var multisprite = owner?.GetComponent<MultiSprite>();

        if (multisprite != null){
            multisprite.SetCurrentSprite("closed");
        }
    }

    public static ContainerComponent Deserialize(Dictionary<string, object> parameters){
        return new ContainerComponent();
    }
}
