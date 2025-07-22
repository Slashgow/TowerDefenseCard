using System;
using UnityEngine;

public class CraftCondition : QuestCondition
{
    [SerializeField] private CardID targetCardId; // Specific card to craft

    private void Start()
    {
        CraftingManager.Instance.OnCraftComplete += OnCraftComplete;
    }

    private void OnCraftComplete(int arg1, CardID iD)
    {
        //OnActionPerformed
    }

    public override bool IsCompleted(Quest quest)
    {
        CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
        return quest.CurrentProgress >= quest.GoalCount;
    }

    public override void OnActionPerformed(Quest quest, object actionData)
    {
        if (actionData is CardID craftedCardId && craftedCardId == targetCardId)
        {
            quest.IncrementProgress();
        }
    }
}
