using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using Raylib_cs;

namespace DarkAgesRPG;

public enum Genre{
    Masculine,
    Feminine
}

public class RaceComponent : Component{

    public Sprite SpriteMasculine;
    public Sprite SpriteFeminine;
    public Sprite currentSprite;
    public string raceId;
    public Color[] hairColors;
    public Color[] skinColors;


    public RaceComponent(
        string raceId, 
        Sprite SpriteMasculine, 
        Sprite SpriteFemine, 
        Color[] hairColors, 
        Color[] SkinColors
    ){
        this.raceId = raceId;
        this.SpriteMasculine = SpriteMasculine;
        this.SpriteFeminine = SpriteFemine;
        this.currentSprite = SpriteMasculine;
        this.hairColors = hairColors;
        this.skinColors = SkinColors;
    }

    public override void Load()
    {
        currentSprite.Load();
    }

    public override void Draw()
    {
        currentSprite.owner = owner;
        currentSprite.Draw();
    }

    public static RaceComponent Deserialize(Dictionary<string, object> parameters){
        var hairColorsHex = (parameters["hairColors"] as JArray).ToObject<string[]>();
        var hairColorsColor = new Color[hairColorsHex.Length];

        for (var i = 0; i < hairColorsHex.Length; i++){
            hairColorsColor[i] = Utils.HexToColor((string)hairColorsHex[i]);
        }

        var SkinColorsHex = (parameters["skinColors"] as JArray).ToObject<string[]>();
        var skinColorsColor = new Color[SkinColorsHex.Length];

        for (var i = 0; i < SkinColorsHex.Length; i++){
            skinColorsColor[i] = Utils.HexToColor((string)SkinColorsHex[i]);
        }

        return new RaceComponent(
            (string)parameters["raceId"],
            new Sprite((string)parameters["spriteMasculine"]),
            new Sprite((string)parameters["spriteFeminine"]),
            hairColorsColor,
            skinColorsColor
        );
    }

}

