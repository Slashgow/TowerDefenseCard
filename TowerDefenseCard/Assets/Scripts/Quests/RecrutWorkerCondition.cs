

public class RecrutWorkerCondition : QuestCondition
{
    private void Start() => CardRecruter.OnAnyRecruitmentComplete += OnRecruitmentComplete;
    private void OnDestroy() => CardRecruter.OnAnyRecruitmentComplete -= OnRecruitmentComplete;
    private void OnRecruitmentComplete() => OnActionPerformed(null);
    public override bool IsCompleted()
    {
        bool isCompleted = quest.CurrentProgress >= quest.GoalCount;
        if(isCompleted)
            CardRecruter.OnAnyRecruitmentComplete -= OnRecruitmentComplete;
        return isCompleted;
    }
    public override void OnActionPerformed(object actionData)
    {
        quest.IncrementProgress();
    }
}
