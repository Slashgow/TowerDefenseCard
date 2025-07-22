public interface IQuestCondition
{
    bool IsCompleted(Quest quest);
    void OnActionPerformed(Quest quest, object actionData);
}
