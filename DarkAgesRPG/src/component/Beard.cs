using System.Numerics;

namespace DarkAgesRPG;

public class Beard : Hair {
    public Beard(string[] racesId, Dictionary<string, Vector2> racesOffset, Sprite sprite) : base(racesId, racesOffset, sprite){
        this.RacesId = racesId;
        this.sprite = sprite;
        this.racesOffsets = racesOffset;
    }
}