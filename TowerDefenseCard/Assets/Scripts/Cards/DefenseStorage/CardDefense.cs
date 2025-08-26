using System;

public class CardDefense : Card
{
    public static event Action OnCreateAnyCardDefense;
    public static event Action OnDestroyAnyCardDefense;

    protected override void Start()
    {
        base.Start();
        OnCreateAnyCardDefense?.Invoke();
    }

    private void OnDestroy()
    {
        OnDestroyAnyCardDefense?.Invoke();
    }
}
