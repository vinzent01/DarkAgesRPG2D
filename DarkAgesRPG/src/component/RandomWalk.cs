
using System.Diagnostics;
using System.Geometry;
using System.Numerics;
namespace DarkAgesRPG;

public class RandomStep : Component {
    public Random random;
    public Actor ownerActor;
    public Vector2i WalkTo;
    
    public RandomStep(){
        random = new();
    }

    public override void Load()
    {
        Debug.Assert(owner is Actor);
        ownerActor = owner as Actor;
    }


    public Vector2 RandomDirection(){
        int[] choices = {-1, 0, 1}; // Agora inclui 0
        var directionX = choices[random.Next(0, 3)];
        var directionY = choices[random.Next(0, 3)];

        if (directionX == 0 && directionY == 0) // Evita (0,0)
            return RandomDirection();

        return new Vector2(directionX, directionY);
    }

    public override void OnActorTurn()
    {
        bool[] choices = {true, true};

        var DoMove = choices[random.Next(0, 2)];
        
        if (DoMove == true){
            ownerActor.AppendAction(new StepAction(2.2f, RandomDirection()));
        }
    }
}