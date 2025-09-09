using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager>
{
    [SerializeField] private UIInput uiInput;
    [SerializeField] private Logger logger;
    [SerializeField] private CanvasGroup tutorialCanvasGroup;
    [SerializeField] private ScrollTextWithVoice scrollTextWithVoice;
    [SerializeField] private Image tanukiImage;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TutorialStep[] tutorialSteps;

    public event Action OnStartTutorial;
    public event Action OnStopTutorial;

    private bool firstTimePlaying;
    private int currentStepIndex;

    protected override void Awake()
    {
        base.Awake();
        Load();
    }

    private void Start()
    {
        if(!firstTimePlaying)
        {
            StopTutorial();
            logger.Log("Tutorial already completed, skipping tutorial start.", this);
            return;
        }

        StartTutorial();
    }

    public void StartTutorial()
    {
        GameSettingsManager.Instance.AllowTemporaryPause();
        GameManager.Instance.Pause();
        uiInput.StopRecevingInput();
        CameraMovement.Instance.StopAllMovement();
        currentStepIndex = 0;
        tutorialCanvasGroup.alpha = 1f;
        tutorialCanvasGroup.interactable = true;
        tutorialCanvasGroup.blocksRaycasts = true;
        foreach (var step in tutorialSteps)
        {
            step.Hide(scrollTextWithVoice);
        }
        tutorialSteps[currentStepIndex].Show(scrollTextWithVoice, tanukiImage, descriptionText);
    }

    public void StopTutorial()
    {
        tutorialCanvasGroup.alpha = 0f;
        tutorialCanvasGroup.interactable = false;
        tutorialCanvasGroup.blocksRaycasts = false;
        firstTimePlaying = false;
        Save();
        OnStopTutorial?.Invoke();
        CameraMovement.Instance.ResumeAllMovement();
        uiInput.StartReceivingInput();
        GameSettingsManager.Instance.BackToPreviousPauseSetting();
        GameManager.Instance.Resume();
    }

    public void ShowNextStep()
    {
        tutorialSteps[currentStepIndex].Hide(scrollTextWithVoice);
        currentStepIndex = Mathf.Clamp(currentStepIndex + 1, 0, tutorialSteps.Length);

        if (currentStepIndex == tutorialSteps.Length)
        {
            StopTutorial();
            return;
        }

        tutorialSteps[currentStepIndex].Show(scrollTextWithVoice, tanukiImage, descriptionText);
    }

    public void ShowPreviousStep()
    {
        tutorialSteps[currentStepIndex].Hide(scrollTextWithVoice);
        currentStepIndex = Mathf.Clamp(currentStepIndex - 1, 0, tutorialSteps.Length - 1);
        tutorialSteps[currentStepIndex].Show(scrollTextWithVoice, tanukiImage, descriptionText);
    }
    public void Save()
    {
        try
        {
            TutorialSaveData tutorialSaveData = new TutorialSaveData
            {
                firstTimePlaying = this.firstTimePlaying,
                currentStepIndex = this.currentStepIndex
            };

            string json = JsonUtility.ToJson(tutorialSaveData, true);
            File.WriteAllText(SavePath.SavePathTutorial, json);
            logger.Log($"Game saved to {SavePath.SavePathTutorial}", this);
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to save game: {e.Message}", this);
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(SavePath.SavePathTutorial))
            {
                string json = File.ReadAllText(SavePath.SavePathTutorial);
                TutorialSaveData saveData = JsonUtility.FromJson<TutorialSaveData>(json);
                this.firstTimePlaying = saveData.firstTimePlaying;
                this.currentStepIndex = saveData.currentStepIndex;
                logger.Log($"Game loaded from {SavePath.SavePathTutorial}", this);
            }
            else
            {
                logger.Log("No save file found, using default GameMode", this);
                LoadDefault();
            }
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to load game: {e.Message}", this);
            LoadDefault();
        }
    }

    private void LoadDefault()
    {
        this.firstTimePlaying = true;
        this.currentStepIndex = 0;
    }
}
