using System.Numerics;
using System.Reflection.Metadata;
using Raylib_cs;

namespace DarkAgesRPG;

interface IAssetInstance {
    public void LoadFromAsset(Asset asset);
}

public class Action {
    public string Name;

    public Action(string Name){
        this.Name = Name;
    }

    public virtual bool Execute(Object obj, Object target){
        return false;
    }   
}

public class TakeAction : Action {

    public TakeAction() : base("Take") {

    }

    public override bool Execute(Object obj, Object target)
    {
        return true;
    }
}

public class EquipAction : Action {
    public EquipAction() : base("Equip") {

    }

    public override bool Execute(Object obj, Object target)
    {
        Equipments? equips = target.GetComponent<Equipments>();

        if (equips == null)
            return false;

        equips.AddEquip(obj);
        return true;
    }
}


public class Object : IDrawable, ILoadable, IUpdatable, IAssetInstance{


    public Vector2 TotalPosition{
        get {
            if (Parent != null)
                return Parent.TotalPosition + RelativePosition;
            
            return RelativePosition;
        }
        set
        {
            if (Parent != null){
                RelativePosition = value - Parent.TotalPosition;
            }
            else{
                RelativePosition = value;
            }
        }
    }

    public Vector2i CellPosition{
        get {
            return new Vector2i(TotalPosition / Globals.TileSize);
        }
        set {
            Vector2i newPos = value * Globals.TileSize;
            TotalPosition = new Vector2(newPos.X, newPos.Y);
        }
    }

    public string Name;
    public string id;
    public Vector2 RelativePosition;
    public Object? Parent;
    public List<Object> Children;
    public List<Component> Components;

    public Object(){
        Name = "none";
        id = "none";
        Children = new();
        Components = new();
    }

    public bool HasComponent<T>() where T : Component{
        foreach (var c in Components){
            if (c is T){
                return true;
            }
        }
        return false;
    }

    public void AddComponent<T>(T component) where T : Component{
        if (!HasComponent<T>()){
            component.owner = this;
            Components.Add(component);
        }
    }

    public T? GetComponent<T>() where T : Component{
        foreach (var c in Components){
            if (c is T)
                return c as T;
        }
        return null;
    }

    public void RemoveComponent<T>() where T : Component{
        foreach (var c in Components.ToList()){
            if (c is T){
                c.owner = null;
                Components.Remove(c);
            }
        }
    }

    public virtual void LoadFromAsset(Asset asset){
        Name = asset.name;
        id = asset.id;
    }

    public bool HasChild(Object obj){
        foreach (var c in Children){
            if (obj == c){
                return true;
            }
        }
        return false;
    }

    public void AddChild(Object obj){
        if (HasChild(obj))
            return;
        
        obj.Parent = this;
        
        Children.Add(obj);
    }

    public void RemoveChild(Object obj){
        Children.Remove(obj);
        obj.Parent = null;
    }

    public virtual void Load(){

        foreach (var c in Components){
            c.Load();
        }

        foreach (var c in Children){
            c.Load();
        }
    }

    public virtual void UnLoad(){

        foreach (var c in Children){
            c.UnLoad();
        }
    }

    public virtual void Draw(){
        foreach (var c in Components){
            c.Draw();
        }

        foreach (var c in Children){
            c.Draw();
        }
    }


    public virtual void Update(float delta){
        foreach (var c in Components){
            c.Update(delta);
        }
    }
}