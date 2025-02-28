namespace DarkAgesRPG;

public class TurnSystem
{
    private Queue<Actor> turnQueue = new Queue<Actor>();
    public System.Action? OnTurnChanged; // Evento chamado ao mudar o turno
    public Actor? currentTurn;
    private bool isTurnActive = false;
    private bool isTurnStarted = false;

    public void AddToTurnQueue(Actor entity)
    {
        turnQueue.Enqueue(entity);
    }

    public void StartTurns(){

        if (State.player != null){
            turnQueue.Enqueue(State.player);
        }

        foreach (var obj in State.world.Children){
            if (obj is Actor actor && actor != State.player){
                turnQueue.Enqueue(actor);
            }
        }

        NextTurn();
    }

    private void NextTurn()
    {
        if (turnQueue.Count == 0) return;

        if (currentTurn != null)
            currentTurn.EndTurn(); // Finaliza o turno anterior

        currentTurn = turnQueue.Dequeue();

        OnTurnChanged?.Invoke();

        currentTurn.StartTurn();
        isTurnActive = true;
    }

    public void UpdateTurn(float deltaTime)
    {
        if (turnQueue.Count == 0 && isTurnActive == false){
            Console.WriteLine("Iniciando turnos");
            StartTurns();
        }

        if (currentTurn == State.player && isTurnStarted == false){
            // wait for new player action

            bool IsAction = State.player.HandleActions();

            Console.WriteLine("WAITING FOR ACTION");

            if (IsAction){
                currentTurn.CurrentAction = currentTurn.ActionQueue.Dequeue();
                Console.WriteLine("ACTION ACTIVE! " + currentTurn.CurrentAction.Name);

                isTurnStarted = true;
       
                if (isTurnActive && currentTurn != null)
                {
                    bool isEnded = currentTurn.UpdateTurn(deltaTime); // Atualiza o turno atual

                    if (isEnded) // Verifica se terminou
                    {
                        isTurnActive = false;
                        isTurnStarted = false;
                        NextTurn();
                        return;
                    }
                }         
            }
        }
        else if (isTurnActive && currentTurn != null)
        {
            bool isEnded = currentTurn.UpdateTurn(deltaTime); // Atualiza o turno atual
            isTurnStarted = true;

            if (isEnded) // Verifica se terminou
            {
                isTurnActive = false;
                isTurnStarted = false;
                NextTurn();
                return;
            }
        }
    }
}

public class NoTurnSystem{
    public float turntime;


    public void UpdateTurn(float delta){

        foreach (var actor in State.world.Children.OfType<Actor>() ){
            if (actor.CurrentAction == null){
                if (turntime > State.Config.NPCTurnTime){
                    turntime = 0;
                }

                if (turntime == 0){
                    actor.StartTurn();
                }

                turntime += delta;
                
            }
            if (actor == State.player && actor.CurrentAction == null){
                var IsAction = actor.HandleActions();

                if (IsAction){
                    actor.CurrentAction = actor.ActionQueue.Dequeue();
                }
            }

            actor.UpdateTurn(delta);
        }

    }
}