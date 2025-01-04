
using System.Numerics;
using DarkAgesRPG.Gui;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace  DarkAgesRPG;

public static class Globals{
    public static Camera2D camera;
    public static int TileSize;
    public static World world;
    public static Object player;
    public static Widgets Widgets = new();
}


public class Game : IUpdatable, ILoadable, IDrawable
{
    Object player;
    PackageManager packageManager;

    public Game(){
        // Initialization
        int screenWidth = GetScreenWidth();
        int screenHeight = GetScreenHeight();

        InitWindow(screenWidth, screenHeight, "DarkAgesRPG");
        SetTraceLogLevel(TraceLogLevel.Error);

        packageManager = new();        
        Globals.camera = new();
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
        int screenWidth = GetScreenWidth();
        int screenHeight = GetScreenHeight();

        Globals.camera.Target = Vector2.Zero;
        Globals.camera.Offset = new Vector2(screenWidth / 2, screenHeight / 2);
        Globals.camera.Zoom = 3f;

        packageManager.LoadPackages("./content/");

        player = new Object();

        player.AddComponent(new Sprite("./content/human race/male-human/HumanMaleBody.png"));
        player.AddComponent(new PlayerControler());
        player.AddComponent(new Equipments());
        player.AddComponent(new Inventory());

        var equip = player.GetComponent<Equipments>();
        var chainmail = new Object();

        chainmail.AddComponent(new Sprite("./content/chainmail armor/chainmail head cover/headCover_chainmain.png"));
        chainmail.AddComponent(new InteractComponent());
        chainmail.AddComponent(new ActionsToPerform(new List<Action>(){
            new TakeAction(),
            new EquipAction()
        }));

        chainmail.Name = "Chainmail head cover";
        chainmail.id = "chainmail-head-cover";

        Globals.world.Add(chainmail);
        Globals.world.Add(player);
        Globals.player = player;

        foreach (var obj in  Globals.world.Objects){
            obj.Load();
        }

        var body = player.GetComponent<Sprite>();
        body.offset = new Vector2(0, -body.Texture.Height / 2);
    }

    public void LoadUI(){
        var playerInventory = player.GetComponent<Inventory>();        
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

        foreach (var obj in  Globals.world.Objects){
            obj.Draw();
        }
    }

    public void DrawHUD(){

        foreach (var widget in Globals.Widgets.ToList()){
            widget.DrawHUD();
        }
    }

    public void Update(float delta){
        ChangeFullScreen();

        foreach (var obj in  Globals.world.Objects){
            obj.Update(delta);
        }

        foreach (var widget in Globals.Widgets){
            widget.Update(delta);
        }

        Globals.world.Remove();
        Globals.camera.Target = player.TotalPosition;
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
                BeginMode2D(Globals.camera);
                    Draw();
                EndMode2D();

                DrawText("CURRENT FPS: " + 1.0f/delta, GetScreenWidth() - 220, 40, 20, new Color(0,255,0,255));

                DrawHUD();

            EndDrawing();
        }

        return 0;
    }
}