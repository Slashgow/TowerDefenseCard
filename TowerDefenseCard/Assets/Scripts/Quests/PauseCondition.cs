

public class PauseCondition : QuestCondition
{
    private void Start() => GameManager.Instance.OnPause += GameManager_OnPause;
    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnPause -= GameManager_OnPause;
    }

    private void GameManager_OnPause() => OnActionPerformed(null);

    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            GameManager.Instance.OnPause -= GameManager_OnPause;
        return isCompleted;

    }
    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}
