
public class Timer{
    float currentSecond = 0;
    float MaxSeconds = 0;
    bool repeat = false;
    bool isActive = true;
    public System.Action? Ontime;

    public Timer(float seconds, bool repeat, System.Action Callback){
        this.MaxSeconds = seconds;
        this.repeat = repeat;
        this.Ontime = Callback;
    }

    public void reset(){
        currentSecond = 0;

        if (repeat == false){
            isActive = false;
        }
    }

    public void Update(float delta){
        if (isActive){
            currentSecond += delta;

            if (currentSecond >= MaxSeconds){
                reset();
                Ontime?.Invoke();
            }
        }
    }

    public void Stop(){
        repeat= false;
        reset();
    }
}