
using System.Diagnostics;
using System.Numerics;
using Raylib_cs;

namespace DarkAgesRPG;


public class EntitySpawner : Component {
    public Timer spawnTimer;
    public int spawnRadius;
    public Random random;
    public int maxCreatures;
    public int CreaturesCount;


    public EntitySpawner(int spawnRadius, float spawnRate = 60*5){
        this.spawnTimer = new Timer(spawnRate, true,  SpawnEntity);
        this.spawnRadius = spawnRadius;
        random = new Random();
        maxCreatures = 10;
    }

    public override void Load()
    {
        SpawnEntity();
    }

    public Asset GetRandomEntity(){
        var creatures = State.packageManager.GetAssetByTags("creature");

        Debug.Assert(creatures.Count > 0);

        var randomCreature = random.Next(0, creatures.Count-1);
        return creatures[randomCreature];
    }

    public void SpawnEntity(){
        var entityInstance = GetRandomEntity().Instanciate();
        var randomX = random.Next(owner.CellPosition.X - spawnRadius, owner.CellPosition.X + spawnRadius);
        var randomY = random.Next(owner.CellPosition.Y - spawnRadius, owner.CellPosition.Y + spawnRadius);

        entityInstance.CellPosition = new Vector2i(randomX, randomY);

        var entitySprite = entityInstance.GetComponent<Sprite>();
        entityInstance.AddComponent(new RandomStep());
        entityInstance.AddComponent(new FootPosition());
        entityInstance.Load();

        if (entitySprite != null){
            entityInstance.Offset = new Vector2((entitySprite.Width / 2) - State.Config.TileSize / 2, -(entitySprite.Height - 24));
            entityInstance.flipOffset = new Vector2((entitySprite.Width / 2) -State.Config.TileSize /2, -(entitySprite.Height - 24));
        }


        
        State.world.AddChild(entityInstance);

        if (State.world.Children.Count > maxCreatures){
            spawnTimer.Stop();
        }
    }

    public override void Draw()
    {
        if (State.Config.DrawDebug){
            Raylib.DrawCircleLines(
                owner.CellPosition.X * State.Config.TileSize,
                owner.CellPosition.Y * State.Config.TileSize, 
                spawnRadius * State.Config.TileSize, 
                Color.Green
            );
        }
    }

    public override void Update(float delta)
    {
        spawnTimer.Update(delta);
    }

}