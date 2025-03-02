using System.Diagnostics;
using System.Geometry;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class Component :ILoadable, IDrawable, IUpdatable{
    public Object? owner;

    public Component(){

    }

    public virtual void Load(){

    }

    public virtual void UnLoad(){

    }

    public virtual void Update(float delta){

    }

    public virtual void Draw(){

    }

    public virtual void OnActorTurn(){
        
    }

    public virtual bool HandleActions(){
        return false;
    }
}

