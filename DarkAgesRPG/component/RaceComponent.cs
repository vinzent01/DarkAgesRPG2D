using System.Collections.Specialized;
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

}

