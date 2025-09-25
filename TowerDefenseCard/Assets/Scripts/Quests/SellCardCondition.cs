using UnityEngine;

public class SellCardCondition : QuestCondition
{
    [SerializeField] private CardID cardID;
    private void Start() => Reseller.OnResellCardID += OnResell;
    private void OnDestroy() => Reseller.OnResellCardID -= OnResell;
    private void OnResell(CardID cardID)
    {
       OnActionPerformed(cardID);
    }
    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            Reseller.OnResellCardID -= OnResell;
        return isCompleted;
    }
    public override void OnActionPerformed(object actionData)
    {
        if (actionData is CardID soldCardID && soldCardID != cardID)
            return;

        quest.IncrementProgress();
    }
}
