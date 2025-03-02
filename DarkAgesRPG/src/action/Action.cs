using System.Diagnostics;
using System.Geometry;
using System.Numerics;
using System.Runtime;
using CSScripting;
using DarkAgesRPG.Gui;

namespace DarkAgesRPG;
public class Action {
    public string? Name;
    public Actor? obj;
    public Object? target;
    public bool IsStarted = false;

    public Action(string Name){
        this.Name = Name;
        this.obj = obj;
        this.target = target;
    }

    public bool Update(float delta){

        if (IsStarted ){
            bool updatingResult = OnUpdate(delta);

            if (updatingResult == true){
                OnEnd();
                IsStarted = false;
                return true;
            }
        }
        return false;
    }

    public bool StartAction(Actor obj, Object target){
        this.obj = obj;
        this.target = target;
        IsStarted = true;
        return OnStart();
    } 

    public bool StartAction(Actor obj){
        this.obj = obj;
        IsStarted = true;
        return OnStart();
    } 

    public virtual bool OnStart(){
        return true;
    }

    public virtual bool OnUpdate(float delta){

        return true;
    }

    public virtual void OnEnd(){
    }

    public virtual bool MeetsCondition(Object obj, Object target){
        return true;
    }
}



