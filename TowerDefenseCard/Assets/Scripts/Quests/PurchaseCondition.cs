public class PurchaseCondition : QuestCondition
{
    public override bool IsCompleted(Quest quest)
    {
        return quest.CurrentProgress >= quest.GoalCount;
    }

    public  override void OnActionPerformed(Quest quest, object actionData)
    {
        if (actionData is Shop) // Any shop purchase
        {
            quest.IncrementProgress();
        }
    }
}
