public delegate void CallBack();

public class Delay {

    public float DelayTime;
    float DelayDelta;

    public Delay(float delayTime){
        DelayTime = delayTime;
        DelayDelta = 0;
    }

    public bool Update(float delta){
        DelayDelta -= delta;

        if (DelayDelta <= 0){
            DelayDelta = DelayTime;
            return true;
        }
        return false;
    }

}