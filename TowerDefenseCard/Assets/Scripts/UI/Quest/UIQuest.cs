using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    [SerializeField] private Toggle toggleIsDone;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private OutlineWidthEffect outlineEffect;

    private Quest quest;
    public Quest Quest => quest;
    public bool IsHighlighted { get; private set; }

    public void Setup(string questDescription, bool isQuestDone, Quest quest)
    {
        questDescriptionText.text = questDescription;  
        toggleIsDone.isOn = isQuestDone;
        this.quest = quest;
        toggleIsDone.interactable = false;

        if (outlineEffect != null)
        {
            outlineEffect.StopEffect();
            IsHighlighted = false;
        }
    }

    public void SetQuestAsCompleted()
    {
        toggleIsDone.isOn = true;

        if (IsHighlighted)
            StopHighlight();
    }
    public void StartHighlight()
    {
        if (outlineEffect != null && !quest.IsCompleted)
        {
            outlineEffect.DoEffect();
            IsHighlighted = true;
        }
    }

    public void StopHighlight()
    {
        if (outlineEffect != null)
        {
            outlineEffect.StopEffect();
            IsHighlighted = false;
        }
    }

    private void OnDestroy()
    {
        if (outlineEffect != null)
            outlineEffect.StopEffect();
    }
}
