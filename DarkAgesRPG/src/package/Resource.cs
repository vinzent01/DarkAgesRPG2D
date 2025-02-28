using System.Dynamic;
using Raylib_cs;

public enum ResourceType {
    Texture,
    Sound,
    Video,
    Image
}

public abstract class Resource{
    public string Name;
    public ResourceType Type;
    public string Path;

    public Resource(string name, string path, ResourceType type){
        Name = name;
        Path = path;
        Type = type;

    }

    public virtual void Load(){

    }

    public virtual object? Get(){
        return default;
    }
}

public class TextureResource : Resource{
    Texture2D Texture;
    
    public TextureResource(string name, string path, ResourceType type) : base(name, path, type){
        Texture = new();
    }

    public override void Load()
    {
        Texture = Raylib.LoadTexture(Path);
    }

    public Texture2D Get()
    {
        return Texture;
    }

}

