using DarkAgesRPG;
using Raylib_cs;

public static class Input{

    public static bool IsKeyDown(KeyboardKey key) {
        return Raylib.IsKeyDown(key);
    }

    public static bool IsKeyUp(KeyboardKey key){
        return Raylib.IsKeyUp(key);
    }

    public static bool IsKeyPressed(KeyboardKey key){
        return Raylib.IsKeyPressed(key);
    }


    public static bool IsMouseButtonDown(MouseButton button){
        return Raylib.IsMouseButtonDown(button);
    }
}