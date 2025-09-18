using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    [SerializeField] private Toggle toggleIsDone;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private OutlineWidthEffect outlineEffect;
    [SerializeField] private ParticleSystem completeParticleSystem;

    private Quest quest;
    public Quest Quest => quest;
    public bool IsHighlighted { get; private set; }

    private const string LOCKED_QUEST_DESCRIPTION = "????";
    public void Setup(string questDescription, bool isQuestDone, Quest quest)
    {
        toggleIsDone.isOn = isQuestDone;

        if (isQuestDone || !quest.IsLocked )
            questDescriptionText.text = questDescription;
        else
            questDescriptionText.text = LOCKED_QUEST_DESCRIPTION;

        this.quest = quest;
        toggleIsDone.interactable = false;
        completeParticleSystem.gameObject.SetActive(false);

        if (outlineEffect != null)
        {
            outlineEffect.StopEffect();
            IsHighlighted = false;
        }
    }

    public void UnlockQuest(string questDescription)
    {
        Debug.Log("Update UI Quest Description");
        questDescriptionText.text = questDescription;
    }

    public void SetQuestAsCompleted()
    {
        toggleIsDone.isOn = true;
        completeParticleSystem.gameObject.SetActive(true);
        completeParticleSystem.Play();

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
