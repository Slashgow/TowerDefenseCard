public class SellAnyCardCondition : QuestCondition
{
    private void Start() => Reseller.OnResell += OnResell;
    private void OnDestroy() => Reseller.OnResell -= OnResell;
    private void OnResell(int obj)
    {
        OnActionPerformed(null);
    }

    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;

        if (isCompleted)
            Reseller.OnResell -= OnResell;

        return isCompleted;
    }
    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}
