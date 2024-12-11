namespace DarkAgesRPG;

public class Equipments : Component {

    List<Object> equips;

    public Equipments(){
        equips = new();
    }

    public void AddEquip(Object Equipment){
        owner.Children.Add(Equipment);
        Equipment.Parent = owner;
        equips.Add(Equipment);
    }

    public override void Load()
    {
        foreach (var obj in equips){
            obj.Load();
        }
    }

    public override void Draw()
    {
        foreach (var obj in equips){
            obj.Draw();
        }
    }

    public override void Update(float delta)
    {
        foreach (var obj in equips){
            obj.Update(delta);
        }
    }
}