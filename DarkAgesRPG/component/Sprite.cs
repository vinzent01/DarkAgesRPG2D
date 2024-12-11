using System.Diagnostics;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class Sprite : Component {
    public Texture2D Texture;
    public string TexturePath;
    public Color Color;
    public bool isFliped;
    public Vector2 offset;

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
        isFliped = false;
        TexturePath = texturePath;
    }

    public override void Load(){
        Debug.Assert(File.Exists(TexturePath));
        Texture = LoadTexture(TexturePath);
        SetTextureFilter(Texture, TextureFilter.Point);
    }

    public override void UnLoad(){
        UnloadTexture(Texture);
    }

    public override void Draw(){
        int width = isFliped? -Texture.Width : Texture.Width;
        int height = Texture.Height;
        
        Rectangle rect = new(0,0, width, height);
        DrawTextureRec(Texture, rect, owner.TotalPosition + offset, Color );
    }
}
