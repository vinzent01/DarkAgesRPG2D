using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DarkAgesRPG;

public class World : Object {
    private List<Object> ObjectsToRemove;
    private List<List<Action>> ActionsPools;

    public World(){
        ObjectsToRemove = new();
        ActionsPools = new();
    }

    protected override void OnUpdate(float delta){
        // update objects
        foreach (var obj in Children){
            obj.Update(delta);
        }
    }

    protected override void OnDraw()
    {
        foreach (var obj in  SortObjectsByPosition(Children)){
            obj.Draw();
        }
    }

    public void ConsumeActions(float delta)
    {
        if (ActionsPools.Count > 0){
            var currentActionPool = ActionsPools[0];

            if (currentActionPool.Count > 0){
                var currentAction = currentActionPool[0];
                var isEnded = currentAction.Update(delta);

                if (isEnded){
                    currentActionPool.Remove(currentAction);
                }
            }
        }
    }

    public List<Object> Get(Vector2i position){
        List<Object> returnObjects = new ();

        foreach (var obj in Children){
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
            Children.Remove(obj);
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
                    yPosition += obj.Offset.Y + sprite.Height;
                    yPosition += sprite.YsortOffset;
                }

                return yPosition + zOrder;
            })
            .ToList();
    }
}