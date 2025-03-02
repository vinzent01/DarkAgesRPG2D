using System.Numerics;
using Newtonsoft.Json.Linq;

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

    public static MultiSprite Deserialize(Dictionary<string, object> parameters){
        var spritesPaths = (parameters["sprites"] as JObject).ToObject<Dictionary<string, string>>();
        var sprites = new Dictionary<string, Sprite>();

        foreach (var spritePath in spritesPaths){
            var newSprite = new Sprite(spritePath.Value);

            sprites.Add(spritePath.Key, newSprite);
        }

        return new MultiSprite(sprites);
    }
}