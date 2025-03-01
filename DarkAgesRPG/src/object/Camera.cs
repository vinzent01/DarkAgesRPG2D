using System.Numerics;
using Raylib_cs;

namespace DarkAgesRPG;

public class Camera : Object {

    private Camera2D camera;
    public Vector2 Position;
    public Object? Follow;

    public Camera(){
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        camera = new();
        camera.Offset = new Vector2(screenWidth / 2, screenHeight / 2);
        camera.Zoom = 3f;
    }

    public Camera2D GetRaylibCamera(){
        return camera;
    }
    
    protected override void OnUpdate(float delta)
    {
        if (Follow != null){
            camera.Target = Follow.TotalPosition;
        }
        else {
            camera.Target = Position;
        }
    }

    public bool IsInsideCameraView(Object obj)
    {
        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        Vector2 cameraPosition = camera.Target - new Vector2(screenWidth / (2 * camera.Zoom), screenHeight / (2 * camera.Zoom));

        var sprite = obj.GetComponent<Sprite>();
        if (sprite == null) return false;

        Vector2 objPos = obj.TotalPosition;
        float objWidth = sprite.Width;
        float objHeight = sprite.Height;

        // Verifica se qualquer parte do sprite está dentro da área visível
        bool isInside =
            objPos.X + objWidth > cameraPosition.X &&
            objPos.X < cameraPosition.X + (screenWidth / camera.Zoom) &&
            objPos.Y + objHeight > cameraPosition.Y &&
            objPos.Y < cameraPosition.Y + (screenHeight / camera.Zoom);

        return isInside;
    }




}