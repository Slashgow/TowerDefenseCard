using System;

public class OpenBoosterCondition : QuestCondition
{
    private void Start() => Booster.OnOpenBooster += OnOpenBooster;

    private void OnDestroy() => Booster.OnOpenBooster -= OnOpenBooster;

    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            Booster.OnOpenBooster -= OnOpenBooster;

        return isCompleted;
    }

    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }

    private void OnOpenBooster(CardID outputCardID)
    {
        OnActionPerformed(outputCardID);
    }
}
