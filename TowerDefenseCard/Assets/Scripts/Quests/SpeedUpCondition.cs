public class SpeedUpCondition : QuestCondition
{
    private void Start() => GameManager.Instance.OnSpeedUp += GameManager_OnSpeedUp;
    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnSpeedUp -= GameManager_OnSpeedUp;
    }

    private void GameManager_OnSpeedUp() => OnActionPerformed(null);
    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if (isCompleted)
            GameManager.Instance.OnPause -= GameManager_OnSpeedUp;
        return isCompleted;
    }

    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}
