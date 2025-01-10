using System.Diagnostics;
using System.Numerics;
using System.Xml.Schema;
using DarkAgesRPG.Gui;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace DarkAgesRPG;

public class Sprite : Component {
    public Texture2D Texture;
    public string TexturePath;
    public Color Color;
    public bool isFliped;
    public Vector2 offset;
    public Vector2 Origin;

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

        Vector2 totalOffset = offset;

        Rectangle rect = new(0,0, width, height);
        DrawTexturePro(
            Texture, 
            rect, 
            new Rectangle(
                owner.TotalPosition + totalOffset, 
                new Vector2(Texture.Width, Texture.Height)
            ), 
            Origin, 
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
