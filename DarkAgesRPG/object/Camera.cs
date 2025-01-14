using System.Numerics;
using Raylib_cs;

namespace DarkAgesRPG;

public class Camera : Object {

    private Camera2D camera;
    public Object? target;

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
    
    public override void Update(float delta)
    {
        if (target != null){
            camera.Target = target.TotalPosition;
        }

    }



}