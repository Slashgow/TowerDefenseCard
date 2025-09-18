using UnityEngine;


public class OpenMenuCondition : QuestCondition
{
    [SerializeField] private UIPage menuToOpen;

    private void Start() => menuToOpen.OnShow += OnShowMenu;
    private void OnDestroy() => menuToOpen.OnShow -= OnShowMenu;
    private void OnShowMenu()
    {
        OnActionPerformed(null);
    }
    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            menuToOpen.OnShow -= OnShowMenu;
        return isCompleted;
    }
    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}
