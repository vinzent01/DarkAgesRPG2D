namespace DarkAgesRPG;

public class World {
    public List<Object> Objects;
    private List<Object> ObjectsToRemove;

    public World(){
        Objects = new();
        ObjectsToRemove = new();
    }

    public void Add(Object obj){
        Objects.Add(obj);
    }

    public Object? Get(Vector2i position){
        foreach (var obj in Objects){
            if (obj.CellPosition == position){
                return obj;
            }
        }
        return null;
    }

    public void ToRemove(Vector2i position){
        Object? obj = Get(position);

        if (obj != null)
            ObjectsToRemove.Add(obj);
    }
    public void ToRemove(Object obj){
        ObjectsToRemove.Add(obj);
    }

    public void Remove(){
        foreach (var obj in ObjectsToRemove){
            Objects.Remove(obj);
        }
    }

}