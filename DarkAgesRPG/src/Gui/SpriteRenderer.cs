using System.Numerics;

namespace DarkAgesRPG.Gui;
public class SpriteRenderer : Widget{
    Sprite sprite;

    public SpriteRenderer(Sprite sprite, Vector2 scale){
        this.sprite = sprite;
        this.sprite.Load();
        this.TotalSize = new Vector2i((int)(sprite.Width), (int)(sprite.Height));
        this.DoMouseCollision = false;
    }

    public SpriteRenderer(Sprite sprite){
        this.sprite = sprite;
        this.sprite.Load();
        this.DoMouseCollision = false;
        this.TotalSize = new Vector2i((int)(sprite.Width ), (int)(sprite.Height));
    }

    protected override void OnDrawHud()
    {
        sprite.DrawHud(new Vector2(this.GlobalPosition.X, this.GlobalPosition.Y));
    }

}