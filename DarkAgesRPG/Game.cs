
using System.Numerics;
using DarkAgesRPG.Gui;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace  DarkAgesRPG;

public static class Globals{
    public static Camera Camera;
    public static int TileSize;
    public static World world;
    public static Object player;
    public static RootWidget RootWidget = new();
    public static PackageManager packageManager = new();

}


public class Game : IUpdatable, ILoadable, IDrawable
{
    Object player;
    bool DrawDebug;

    public Game(){
        // Initialization
        int screenWidth = GetScreenWidth();
        int screenHeight = GetScreenHeight();

        InitWindow(screenWidth, screenHeight, "DarkAgesRPG");
        SetTraceLogLevel(TraceLogLevel.Error);

        Globals.Camera = new();
        Globals.TileSize = 32;
        Globals.world = new World();
    }

    ~Game(){
        // De-Initialization
        UnLoad();
        CloseWindow();
    }

    public void ChangeFullScreen(){
        if (IsKeyPressed(KeyboardKey.F11)){
            ToggleFullscreen();
        }
    }

    public void Load(){
        Globals.packageManager.LoadPackages("./content/");
        Globals.packageManager.PrintAllAssets();

        // chest
        var chest = new Object(
            "chest", 
            "chest",
            new MultiSprite(
                new Dictionary<string, Sprite>{
                    {"closed", new Sprite("./content/chest/chest-closed.png")},
                    {"open", new Sprite("./content/chest/chest-open.png")}
                }
            ),
            new Inventory(),
            new ActionsToPerform( new List<Action>() {
                new OpenInventoryAction()
            }),
            new InteractComponent(),
            new ContainerComponent()
        );

        chest.CellPosition = new Vector2i(3,3);
        Globals.world.Add(chest);

        foreach (var obj in  Globals.world.Objects){
            obj.Load();
        }

        var chestMultiSprite = chest.GetComponent<MultiSprite>();
        if (chestMultiSprite != null && chestMultiSprite.CurrentSprite != null)
            chestMultiSprite.Offset = new Vector2(0, -chestMultiSprite.CurrentSprite.Texture.Height / 2);

    }

    public void LoadUI(){

        var CharacterCreationMenu  = new CharacterCreationMenu();
        CharacterCreationMenu.CenterScreen();
        Globals.RootWidget.AddChild(CharacterCreationMenu);
        
    }

    public void UnLoad(){

        foreach (var obj in Globals.world.Objects){
            obj.UnLoad();
        }
    }

    public void Draw(){
        // Draw cells
        for (var x = 0; x < 100; x++){
            for (var y = 0; y < 100; y++){
                DrawRectangleLines(x * 32,y * 32, 32, 32, new Color(150,150,150,255));
            }
        }
        
        foreach (var obj in  World.SortObjectsByPosition(Globals.world.Objects)){
            obj.Draw();
        }
    }

    public void DrawHUD(){
        Globals.RootWidget.DrawHud();
    }

    public void Update(float delta){
        ChangeFullScreen();

        foreach (var obj in  Globals.world.Objects){
            obj.Update(delta);
        }

        if (Raylib.IsKeyPressed(KeyboardKey.H)){
            DrawDebug = ! DrawDebug;
        }

        Globals.Camera.Update(delta);
        Globals.RootWidget.Update(delta);
        Globals.world.Remove();
        //Globals.camera.Target = player.TotalPosition;
    }

    public int Main()
    {
        Load();
        LoadUI();

        ChangeFullScreen();
        SetTargetFPS(60);

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            float delta = GetFrameTime();
            Update(delta);

            BeginDrawing();
                ClearBackground(new Color(255,255,255,255));

                // Draw fps
                BeginMode2D(Globals.Camera.GetRaylibCamera());
                    Draw();
                EndMode2D();

                DrawText("CURRENT FPS: " + 1.0f/delta, GetScreenWidth() - 220, 40, 20, new Color(0,255,0,255));

                DrawHUD();

            EndDrawing();
        }

        return 0;
    }
}