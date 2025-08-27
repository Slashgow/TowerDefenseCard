using System;

[Serializable]
public class SuccessData
{
    public bool isDone;
    public Action OnComplete;

    public void Complete()
    {
        if(isDone) 
            return;

        isDone = true;
        OnComplete?.Invoke();
    }
}
