using System.Numerics;

namespace DarkAgesRPG;

public class MultiSprite : Component{

    public Dictionary<string, Sprite> Sprites;
    private Sprite? currentSprite;
    public Sprite? CurrentSprite {get { return currentSprite;} }
    public Vector2 Offset;

    public MultiSprite(Dictionary<string, Sprite> sprites) {
        this.Sprites = sprites;

        var spriteList = sprites.Values.ToList();

        if (spriteList.Count > 0){
            currentSprite = spriteList[0];
        }

        Offset = new Vector2(0,0);
    }

    public bool SetCurrentSprite(string key){
        
        var sprite = Sprites[key];

        if (sprite != null){
            currentSprite = sprite;
            return true;
        }

        return false;
    }

    public override void Draw()
    {
        if (currentSprite != null){
            currentSprite.offset = Offset;
            currentSprite.owner = owner;
            currentSprite.Draw();
        }
    }

    public override void Load()
    {
        var spriteList = Sprites.Values.ToList();

        foreach (var sprite in spriteList){
            sprite.Load();
        }
    }
}