using System.Numerics;

namespace DarkAgesRPG;

interface ILoadable{
    void Load();

    void UnLoad();
}

interface ILoadablePath{
    void Load(string path);

    void UnLoad();
}

interface IUpdatable{
    void Update(float delta);
}

interface IDrawable{

    void Draw();
}

interface IDrawableAt{
    void Draw(Vector2 position);
}

interface IDrawableHUD{

    void DrawHUD();
}