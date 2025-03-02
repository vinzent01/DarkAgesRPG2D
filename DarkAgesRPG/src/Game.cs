
using System.Numerics;
using DarkAgesRPG.Gui;
using Newtonsoft.Json;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace  DarkAgesRPG;

public class Config {
    public bool isTurnEnabled;
    public float NPCTurnTime = 1f; // npcs acts every 1 second
    public int TileSize;
    public bool DrawDebug;
}

public static class State{
    public static Camera Camera;
    public static World world;
    public static Actor player;
    public static RootWidget RootWidget = new();
    public static PackageManager packageManager = new();
    public static TurnSystem turnSystem = new();
    public static NoTurnSystem NoTurnSystem = new();
    public static Config Config = new();
}


public class Game : IUpdatable, ILoadable, IDrawable
{

    public Game(){
        // Initialization
        int screenWidth = GetScreenWidth();
        int screenHeight = GetScreenHeight();

        InitWindow(screenWidth, screenHeight, "DarkAgesRPG");
        SetTraceLogLevel(TraceLogLevel.Error);

        State.Camera = new();
        State.Config.TileSize = 32;
        State.world = new World();
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
    public void LoadUI(){

        var CharacterCreationMenu  = new CharacterCreationMenu();
        CharacterCreationMenu.CenterScreen();
        State.RootWidget.AddChild(CharacterCreationMenu);
    }

    public void Load(){
        State.packageManager.LoadPackages("./content/");
        State.packageManager.PrintAllAssets();

        State.Config.isTurnEnabled = false;
        State.Camera.Position = new Vector2(50 * State.Config.TileSize,50 * State.Config.TileSize);

        // test horse
        var horse = new Actor(
            "Horse",
            "horse",
            new Sprite("res://creatures/horse.png"),
            new FootPosition(),
            new RandomStep()
        );

        var axeAsset = State.packageManager.GetAsset("axe");
        var chestAsset = State.packageManager.GetAsset("chest");

        if (chestAsset != null){
            var chestObject = chestAsset.Instanciate();
            chestObject.CellPosition = new Vector2i(51, 50);
            State.world.AddChild(chestObject);

            if (axeAsset != null){
                var axeObject = axeAsset.Instanciate();
                axeObject.CellPosition = new Vector2i(50, 50);
                axeObject.IsVisible = false;

                State.world.AddChild(axeObject);
                var chestInventory = chestObject.GetComponent<Inventory>();

                if (chestInventory != null){
                    chestInventory.AddItem(axeObject);
                }
            }
        }

        horse.Flip(true);
        horse.CellPosition = new Vector2i(2,2);

        State.world.AddChild(horse);

        State.world.AddComponent(
            new TerrainGenerator(100,100)
        );

        State.world.Load();
    }



    public void UnLoad(){
        State.world.UnLoad();
    }

    public void Draw(){
        // Draw cells
        
        for (var x = 0; x < 100; x++){
            for (var y = 0; y < 100; y++){
                DrawRectangleLines(x * 32,y * 32, 32, 32, new Color(150,150,150,255));
            }
        }

        State.world.Draw();
    }

    public void DrawHUD(){
        State.RootWidget.DrawHud();

        if (State.Config.DrawDebug){
            
            var turnText = "None Turn";

            if (State.turnSystem.currentTurn != null){
                turnText = State.turnSystem.currentTurn.Name + " Turn";
            }

            DrawText(turnText, Raylib.GetScreenWidth()/ 2, 0, 22, Color.Green);
        }
    }

    public void Update(float delta){
        ChangeFullScreen();

        if (Raylib.IsKeyPressed(KeyboardKey.H)){
            State.Config.DrawDebug = ! State.Config.DrawDebug;
        }

        State.world.Update(delta);

        if (State.Config.isTurnEnabled){
            State.turnSystem.UpdateTurn(delta);
        }
        else {
            State.NoTurnSystem.UpdateTurn(delta);
        }

        State.Camera.Update(delta);
        State.RootWidget.Update(delta);
        State.world.Remove();
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
                BeginMode2D(State.Camera.GetRaylibCamera());
                    Draw();
                EndMode2D();

                DrawText("CURRENT FPS: " + 1.0f/delta, GetScreenWidth() - 220, 40, 20, new Color(0,255,0,255));

                DrawHUD();

            EndDrawing();
        }

        return 0;
    }
}