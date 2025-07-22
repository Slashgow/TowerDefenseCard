using UnityEngine;

public abstract class QuestCondition : MonoBehaviour, IQuestCondition
{
    protected Quest quest;
    public void Setup(Quest quest) => this.quest = quest;
    public abstract bool IsCompleted();
    public abstract void OnActionPerformed(object actionData);
}
