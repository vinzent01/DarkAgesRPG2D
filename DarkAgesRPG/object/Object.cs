using System.Numerics;

namespace DarkAgesRPG;


public class Object : IDrawable, ILoadable, IUpdatable{

    public bool IsVisible;
    protected int zIndex = 0;
    private Vector2 scale;
    private Vector2 offset;
    public Vector2 flipOffset;
    public Vector2 Offset{
        get {
            if (isFlipped){
                if (Parent != null){
                    return Parent.flipOffset + flipOffset;
                }
                return flipOffset;
            }
            else {
                if (Parent != null){
                    return Parent.Offset + offset;
                }
                return offset;
            }
        }
        set {
            offset = value;
        }
    }


    private bool isFlipped;
    public bool IsFlipped {
        get {
            return isFlipped;
        }
    }

    public Vector2 Scale {
        get {
            if (Parent != null)
                return Parent.scale * scale;
            return scale;
        }
        set {
            scale = value;
        }
    }

    public int TotalZ {
        get {
            if (Parent != null)
                return Parent.zIndex + zIndex;
            return this.zIndex;
        }
        set {
            zIndex = value;
        }
    }


    public Vector2 TotalPosition {
        get {
            if (Parent != null)
                return Parent.TotalPosition + RelativePosition;

            return RelativePosition;
        }
        set {
            if (Parent != null) {
                RelativePosition = value - Parent.TotalPosition;
            } else {
                RelativePosition = value;
            }
        }
    }

    public Vector2i CellPosition {
        get {
            return new Vector2i(
                (int)(TotalPosition.X / Globals.TileSize),
                (int)(TotalPosition.Y / Globals.TileSize)
            );
        }
        set {
            Vector2 newTotalPosition = new Vector2(value.X * Globals.TileSize, value.Y * Globals.TileSize);

            if (Parent != null) {
                RelativePosition = newTotalPosition - Parent.TotalPosition;
            } else {
                RelativePosition = newTotalPosition;
            }
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
        IsVisible = true;
        Scale = new Vector2(1,1);
    }

    public Object(string name, string id, params Component[] components){
        this.Name = name;
        this.id = id;

        Components = new();
        Children = new();
        IsVisible = true;
        Scale = new Vector2(1,1);

        foreach (var component in components){
            AddComponent(component);
        }
    }

    public bool HasComponent(Component component){
        foreach (var c in Components){
            if (c == component){
                return true;
            }
        }
        return false;
    }

    public bool HasComponentType<T>(){
        foreach (var c in Components){
            if (c is T){
                return true;
            }
        }
        return false;
    }

    public void AddComponent<T>(T component) where T : Component{
        if (!HasComponent(component)){
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


    public bool HasChild(Object obj){
        foreach (var c in Children){
            if (obj == c){
                return true;
            }
        }
        return false;
    }
    public bool HasChildId(string id){
        foreach (var c in Children){
            if (id == c.id){
                return true;
            }
        }
        return false;
    }

    public Object? GetChildId(string id){
        foreach (var c in Children){
            if (c.id == id){
                return c;
            }
        }
        return null;
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
        if (!IsVisible)
            return;

        foreach (var c in Components){
            c.Draw();
        }

        foreach (var c in World.SortObjectsByPosition(Children)){
            c.Draw();
        }
    }


    public virtual void Update(float delta){
        foreach (var c in Components){
            c.Update(delta);
        }
    }

    public void Flip(bool direction)
    {
        HashSet<Object> visited = new HashSet<Object>();

        void FlipRecursive(Object currentObj, bool flipped)
        {
            if (visited.Contains(currentObj)) return; // Evita ciclos

            visited.Add(currentObj);

            currentObj.isFlipped = flipped;

            // Recursively flip all children
            foreach (var child in currentObj.Children)
            {
                FlipRecursive(child, flipped);
            }
        }

        isFlipped = direction;
        FlipRecursive(this, direction);
    }
}