public class ApplyUpgradeCondition : QuestCondition
{
    private void Start() => CardUpgrade.OnAppliedAnyUpgrade += OnUpgradeApplied;
    private void OnDestroy() => CardUpgrade.OnAppliedAnyUpgrade -= OnUpgradeApplied;
    private void OnUpgradeApplied() => OnActionPerformed(null);
    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if(isCompleted)
            CardUpgrade.OnAppliedAnyUpgrade -= OnUpgradeApplied;
        return isCompleted;
    }
    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}
