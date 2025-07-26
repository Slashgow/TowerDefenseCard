using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    [SerializeField] private Toggle toggleIsDone;
    [SerializeField] private TextMeshProUGUI questDescriptionText;

    private Quest quest;
    public Quest Quest => quest;

    public void Setup(string questDescription, bool isQuestDone, Quest quest)
    {
        questDescriptionText.text = questDescription;  
        toggleIsDone.isOn = isQuestDone;
        this.quest = quest;
    }

    public void SetQuestAsCompleted() => toggleIsDone.isOn = true;
}
