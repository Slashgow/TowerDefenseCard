using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class Merchant : CardShop
{
    [SerializeField] private UnityEvent OnPurchaseBoosterSuccess;
    [SerializeField, Range(0f, 2f)] private float destroyDelayAfterPurchase = 0.5f;

    private Timer destroyTimer;

    public override bool TryPurchaseBooster(bool bypassLocked)
    {
        bool success = base.TryPurchaseBooster(bypassLocked);

        if (success)
        {
            OnPurchaseBoosterSuccess?.Invoke();
            destroyTimer = Timer.Register(destroyDelayAfterPurchase, () =>
            {
                Destroy(this.gameObject);
            });
        }

        return success;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        destroyTimer?.Cancel();
    }
}
