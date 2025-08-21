using System;
using UnityEngine;

public class StorageCountCondition : QuestCondition
{
    [SerializeField, Range(0,200)] private int maxNumberOfCardCountCondition;

    private void Start() => CardManager.Instance.OnUpdateMaxNumberOfCards += OnUpdateMaxNumberOfCards;

    private void OnDestroy()
    {
        if(CardManager.HasInstance)
            CardManager.Instance.OnUpdateMaxNumberOfCards -= OnUpdateMaxNumberOfCards;
    }

    private void OnUpdateMaxNumberOfCards(int currentNumberOfCard, int maxNumberOfCard) => OnActionPerformed(maxNumberOfCard);

    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            CardManager.Instance.OnUpdateMaxNumberOfCards -= OnUpdateMaxNumberOfCards;

        return isCompleted;
    }

    public override void OnActionPerformed(object actionData)
    {
        if (maxNumberOfCardCountCondition <= (int)actionData)
        {
            quest.IncrementProgress();
        }
    }
}
