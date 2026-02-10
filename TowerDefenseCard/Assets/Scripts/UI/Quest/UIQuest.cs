using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    [SerializeField] private Toggle toggleIsDone;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private OutlineWidthEffect outlineEffect;
    [SerializeField] private ParticleSystem completeParticleSystem;
    [SerializeField] private GameObject demoLockedImage;
    [SerializeField] private Image backgroundDemoLockedImage;
    [SerializeField] private LocalizedString demoLockedLocalizedString;

    private Quest quest;
    public Quest Quest => quest;
    public bool IsHighlighted { get; private set; }

    private const string LOCKED_QUEST_DESCRIPTION = "????";

    public void Setup(string questDescription, bool isQuestDone, Quest quest)
    {
        this.quest = quest;

        if (DemoManager.HasInstance && DemoManager.Instance.UseDemoMode && quest.IsDemoLocked)
        {
            toggleIsDone.isOn = false;

#if UNITY_WEBGL

        demoLockedLocalizedString.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                 questDescriptionText.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
            questDescriptionText.text = demoLockedLocalizedString.GetLocalizedString();
#endif
            toggleIsDone.interactable = false;
            completeParticleSystem.gameObject.SetActive(false);
            backgroundDemoLockedImage.enabled = true;

            if (demoLockedImage != null)
                demoLockedImage.SetActive(true);

            if (outlineEffect != null)
            {
                outlineEffect.StopEffect();
                IsHighlighted = false;
            }

            return;
        }

        if (demoLockedImage != null)
            demoLockedImage.SetActive(false);

        toggleIsDone.isOn = isQuestDone;

        if (isQuestDone || !quest.IsLocked )
            questDescriptionText.text = questDescription;
        else
            questDescriptionText.text = LOCKED_QUEST_DESCRIPTION;

    
        toggleIsDone.interactable = false;
        completeParticleSystem.gameObject.SetActive(false);
        backgroundDemoLockedImage.enabled = false;

        if (outlineEffect != null)
        {
            outlineEffect.StopEffect();
            IsHighlighted = false;
        }
    }

    public void UnlockQuest(string questDescription)
    {
        if (DemoManager.HasInstance && DemoManager.Instance.UseDemoMode &&  quest.IsDemoLocked)
        {
            Debug.LogWarning("Cannot unlock a demo locked quest");
            return;
        }

        Debug.Log("Update UI Quest Description");
        questDescriptionText.text = questDescription;
    }

    public void SetQuestAsCompleted()
    {
        if (DemoManager.HasInstance && DemoManager.Instance.UseDemoMode && quest.IsDemoLocked)
        {
            Debug.LogWarning("Cannot complete a demo locked quest");
            return;
        }

        toggleIsDone.isOn = true;
        completeParticleSystem.gameObject.SetActive(true);
        completeParticleSystem.Play();

        if (IsHighlighted)
            StopHighlight();
    }
    public void StartHighlight()
    {
        if (DemoManager.HasInstance && DemoManager.Instance.UseDemoMode && quest.IsDemoLocked)
            return;

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
