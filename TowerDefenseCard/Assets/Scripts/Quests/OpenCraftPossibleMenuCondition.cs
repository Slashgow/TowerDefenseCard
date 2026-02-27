using UnityEngine;

public class OpenCraftPossibleMenuCondition : QuestCondition
{
    [SerializeField] private UIPanelCardCraftingRecipes craftingRecipesPanel;
    private void Start() => craftingRecipesPanel.OnShow += OnShowMenu;
    private void OnDestroy() => craftingRecipesPanel.OnShow -= OnShowMenu;

    protected void OnShowMenu()
    {
        OnActionPerformed(null);
    }
    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            craftingRecipesPanel.OnShow -= OnShowMenu;
        return isCompleted;
    }
    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}