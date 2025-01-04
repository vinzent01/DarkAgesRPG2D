namespace DarkAgesRPG;

public class Equipments : Component {

    List<Object> equips;

    public Equipments(){
        equips = new();
    }

    public void AddEquip(Object Equipment){
        owner.Children.Add(Equipment);

        // add offset
        var EquipSprite = Equipment.GetComponent<Sprite>();
        var ObjectSprite = owner.GetComponent<Sprite>();
        var EquipInteract = Equipment.GetComponent<InteractComponent>();

        if (EquipSprite != null && ObjectSprite != null){
            EquipSprite.offset = ObjectSprite.offset;
        }
        
        // Remove interact component
        Equipment.RemoveComponent<InteractComponent>();

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