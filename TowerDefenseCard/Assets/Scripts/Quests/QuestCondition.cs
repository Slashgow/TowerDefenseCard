using UnityEngine;

public abstract class QuestCondition : MonoBehaviour, IQuestCondition
{
    public abstract bool IsCompleted(Quest quest);
    public abstract void OnActionPerformed(Quest quest, object actionData);
}
