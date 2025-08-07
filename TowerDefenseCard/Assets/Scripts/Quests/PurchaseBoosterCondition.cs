public class PurchaseBoosterCondition : QuestCondition
{


    private void Start() => ShopManager.OnPurchaseBooster += OnPurchaseBooster;
    private void OnDestroy() => ShopManager.OnPurchaseBooster -= OnPurchaseBooster;
    private void OnPurchaseBooster()
    {
        OnActionPerformed(null);
    }

    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            ShopManager.OnPurchaseBooster -= OnPurchaseBooster;

        return isCompleted;
    }

    public  override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();   
    }
}
