using System;
using UnityEngine;

public class CraftCondition : QuestCondition
{
    [SerializeField] private CardID targetCardId; // Specific card to craft

    private void Start() => CraftingManager.Instance.OnCraftComplete += OnCraftComplete;
    private void OnDestroy()
    {
        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
    }

    private void OnCraftComplete(int craftId, CardID outputCardID) => OnActionPerformed(outputCardID);

    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if(isCompleted)
            CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;

        return isCompleted;
    }

    public override void OnActionPerformed(object actionData)
    {
        if (actionData is CardID craftedCardId && craftedCardId == targetCardId)
        {
            quest.IncrementProgress();
        }
    }
}
