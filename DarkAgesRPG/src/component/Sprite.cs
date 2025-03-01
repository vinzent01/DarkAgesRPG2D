using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

[JsonConverter(typeof(StringEnumConverter))]
public enum SpriteOrigin {
    TopLeft,
    Center,
    BottomCenter,
    Custom
}

public class Sprite : Component {
    public Texture2D Texture;
    public string ResourcePath;
    public Color Color;
    public Vector2 offset;
    public SpriteOrigin Origin;
    public Vector2 OriginOffset;
    public float YsortOffset;


    public int Width {
        get {
            return Texture.Width;
        }
    }

    public int Height {
        get {
            return Texture.Height;
        }
    }

    public Sprite(string texturePath){
        Color = new Color(255,255,255,255);
        ResourcePath = texturePath;
        this.Origin = SpriteOrigin.TopLeft;
    }

    public Sprite(string texturePath, Vector2 Offset){
        Color = new Color(255,255,255,255);
        ResourcePath = texturePath;
        this.offset = Offset;
        this.Origin = SpriteOrigin.TopLeft;
    }

    public Sprite(string texturePath, Vector2 Offset, float YsortOffset){
        Color = new Color(255,255,255,255);
        ResourcePath = texturePath;
        this.offset = Offset;
        this.YsortOffset = YsortOffset;
        this.Origin = SpriteOrigin.TopLeft;
    }

    public Sprite(
        string texturePath, 
        Vector2 Scale, 
        Vector2 Offset, 
        float YsortOffset,
        SpriteOrigin Origin)
    {
        Color = new Color(255,255,255,255);
        ResourcePath = texturePath;

        this.offset = Offset;
        this.YsortOffset = YsortOffset;
        this.Origin = Origin;
    }

    public override void Load(){
        var resource = State.packageManager.GetResource<TextureResource>(ResourcePath);
        Debug.Assert(resource != null);
        
        if (resource != null){

            Texture = resource.Get();
        }

        SetTextureFilter(Texture, TextureFilter.Point);
    }

    public override void UnLoad(){
        UnloadTexture(Texture);
    }

    public Vector2 GetOrigin(){
        switch (Origin){
            case SpriteOrigin.Center:
                return new Vector2(Width /2, Height / 2);

            case SpriteOrigin.TopLeft:
                return new Vector2(0,0);
            
            case SpriteOrigin.BottomCenter:
                return new Vector2(Width / 2, Height);
        }

        return OriginOffset;
    }

    public override void Draw(){
        int width = owner != null ? owner.IsFlipped? -Texture.Width : Texture.Width : Texture.Width;
        int height = Texture.Height;

        Vector2 totalOffset;

        if (owner != null){
            totalOffset = offset + owner.Offset;
        }
        else {
            totalOffset = offset;
        }


        Rectangle rect = new(0,0, width, height);

        if (owner != null){

            DrawTexturePro(
                Texture, 
                rect, 
                new Rectangle(
                    owner.TotalPosition + totalOffset * owner.Scale, 
                    new Vector2(owner.Scale.X * ( Texture.Width), owner.Scale.Y * ( Texture.Height))
                ),
                GetOrigin(),
                0, 
                Color
            );

            if (State.Config.DrawDebug && owner.DoDrawDebug){
                DrawRectangleLines(
                    (int)(owner.TotalPosition.X + (int)totalOffset.X * owner.Scale.X), 
                    (int)(owner.TotalPosition.Y + (int)totalOffset.Y * owner.Scale.Y), 
                    (int)(owner.Scale.X * Texture.Width), 
                    (int)(owner.Scale.Y * Texture.Height), Color.Purple);
            }
    
        }
        else {
            DrawTexturePro(
                Texture, 
                rect, 
                new Rectangle(
                    totalOffset,
                    new Vector2(Texture.Width, Texture.Height)
                ), 
                GetOrigin(),
                0, 
                Color
            );

            if (State.Config.DrawDebug){
                DrawRectangleLines((int)totalOffset.X, (int)totalOffset.Y, Texture.Width, Texture.Height, Color.Purple);
            }
        }
    }

    public void DrawHud(Vector2 position){
        int width = owner != null ? owner.IsFlipped? -Texture.Width : Texture.Width : Texture.Width;
        int height = Texture.Height;

        Rectangle rect = new(0,0, width, height);

        DrawTexturePro(
            Texture, 
            rect, 
            new Rectangle(
                position, 
                new Vector2(Texture.Width, Texture.Height)
            ), 
            GetOrigin(), 
            0, 
            Color
        );
    }

    public bool Contains(Vector2 position){

        if (owner != null){
            if (position.X >= owner.TotalPosition.X && position.X <= owner.TotalPosition.X + Width ){

                if (position.Y >= owner.TotalPosition.Y && position.Y <= owner.TotalPosition.Y + Height){
                    return true;
                }
            }
        }
        
        return false;

    }
}
