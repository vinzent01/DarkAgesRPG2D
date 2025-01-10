
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
    public static RootWidget RootWidget = new();

}


public class Game : IUpdatable, ILoadable, IDrawable
{
    Object player;
    PackageManager packageManager;
    bool DrawDebug;

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

        // Player
        player = new Object();
        
        player.AddComponent(new Sprite("./content/human race/male-human/HumanMaleBody.png"));
        player.AddComponent(new PlayerControler());
        player.AddComponent(new EquipmentList());
        player.AddComponent(new Inventory());
        player.AddComponent(new InventoryHud());

        var playerInventory = player.GetComponent<Inventory>();

        var rockItem = new Object();
        rockItem.Name = "Pedra";
        rockItem.id = "pedra";

        rockItem.AddComponent(new Item());

        playerInventory.AddItem(rockItem);
        playerInventory.AddItem(rockItem);


        player.Name = "Player";
        player.id = "player";
        Globals.world.Add(player);

        // Chainmail
        var chainmailCover = new EquipmentObject(
            "Chain Mail",
            "chainmail-headcover",
            new Sprite("./content/chainmail armor/chainmail head cover/headCover_chainmain.png"),
            new Vector2(0, -26),
            3
        );

        Globals.world.Add(chainmailCover);

        // Chainmail torso
        var chainmailTorso = new EquipmentObject(
            "Chain Mail torso",
            "chainmail-torso",
            new Sprite("./content/chainmail armor/torso chainmail/torso_chainmail.png"),
            new Vector2(0, -18),
            2
        );

        chainmailTorso.CellPosition = new Vector2i(1,0);
        Globals.world.Add(chainmailTorso);

        // leather pants
        var leatherPants = new EquipmentObject(
            "Leather Pants",
            "leather-pants",
            new Sprite("./content/leather armor/leather pants/leather_pants.png"),
            new Vector2(0, -5),
            1
        );

        leatherPants.CellPosition = new Vector2i(2,0);
        Globals.world.Add(leatherPants);

        // leather boots
        var leatherBoots = new EquipmentObject(
            "Leather Boots",
            "leather-boots",
            new Sprite("./content/leather armor/leather boots/leather_boots.png"),
            new Vector2(0,-4),
            2
        );

        leatherBoots.CellPosition = new Vector2i(3,0);
        Globals.world.Add(leatherBoots);

        // sword

        var ironSword = new EquipmentObject(
            "Iron Sword",
            "iron-sword",
            new Sprite("./content/melee weapons/sword.png"),
            new Vector2(0,-23),
            new Vector2(0,23),
            4
        );

        ironSword.CellPosition = new Vector2i(4, 0);
        Globals.world.Add(ironSword);

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

        // World

        Globals.player = player;

        foreach (var obj in  Globals.world.Objects){
            obj.Load();
        }

        // Set sprite offsets
        var body = player.GetComponent<Sprite>();

        if (body != null)
            body.offset = new Vector2(0, -body.Texture.Height / 2);

        var chestMultiSprite = chest.GetComponent<MultiSprite>();
        if (chestMultiSprite != null && chestMultiSprite.CurrentSprite != null)
            chestMultiSprite.Offset = new Vector2(0, -chestMultiSprite.CurrentSprite.Texture.Height / 2);

    }

    public void LoadUI(){

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

        Globals.RootWidget.Update(delta);
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