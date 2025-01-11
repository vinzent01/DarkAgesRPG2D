using Microsoft.CodeAnalysis.CSharp.Syntax;

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

    public List<Object> Get(Vector2i position){
        List<Object> returnObjects = new ();

        foreach (var obj in Objects){
            if (obj.CellPosition == position){
                returnObjects.Add(obj);
            }
        }

        return returnObjects;
    }

    public void ToRemove(Vector2i position){
        List<Object> objs = Get(position);

        foreach (var obj in objs)
            ObjectsToRemove.Add(obj);
    }
    public void ToRemove(Object obj){
        ObjectsToRemove.Add(obj);
    }

    public void Remove(){
        foreach (var obj in ObjectsToRemove.ToList()){
            Objects.Remove(obj);
            ObjectsToRemove.Remove(obj);
        }
    }

    public static List<Object> SortObjectsByPosition(List<Object> objects) {

        return objects
            .OrderBy(obj =>  { 
                var sprite = obj.GetComponent<Sprite>(); 
                float yPosition = obj.TotalPosition.Y;
                float zOrder = obj.TotalZ;

                // Se o objeto tiver um sprite, considere a altura para calcular a ordenação.
                if (sprite != null)
                {
                    yPosition += sprite.Height;
                }

                return yPosition + zOrder;
            })
            .ToList();
    }

}