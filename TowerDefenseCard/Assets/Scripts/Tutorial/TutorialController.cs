using System;
using System.IO;
using UnityEngine;
using UnityTimer;

[Serializable]
public struct TutorialZoomData
{
    [SerializeField, Range(0, 50)] private int numberOfQuestCompletedToShow;
    public int NumberOfQuestCompletedToShow => numberOfQuestCompletedToShow;

    [SerializeField, Range(0f, 20f)] private float zoomOrthoSize;
    public float ZoomOrthoSize => zoomOrthoSize;
    [SerializeField, Range(0f, 5f)] private float timeToMove;
    public float TimeToMove => timeToMove;
    [SerializeField, Range(0f, 5f)] private float timeToZoom;
    public float TimeToZoom => timeToZoom;

    [SerializeField] private Transform cameraTransform;
    public Transform CameraTransform => cameraTransform;

    [SerializeField, Range(0f, 5f)] private float goBackToOriginalPositionTime;
    public float GoBackToOriginalPositionTime => goBackToOriginalPositionTime;

    [SerializeField, Range(0f, 5f)] private float timeBeforeGoingBackToOriginalPosition;
    public float TimeBeforeGoingBackToOriginalPosition => timeBeforeGoingBackToOriginalPosition;
}

public class TutorialController : MonoSingleton<TutorialController>
{
    [Header("References")]
    [SerializeField] private Logger logger;
    [SerializeField] private CameraMovement cameraMovement;
    [SerializeField] private QuestManager mainQuestManager;
    [SerializeField] private CardShop basePackCardShop;
    [SerializeField] private CardShop defensePackCardShop;
    [SerializeField] private CardShop engineeringkCardShop;
    [SerializeField] private CardShop foodPackCardShop;
    [SerializeField] private GameObject enableAnimationPrefab;

    [Header("Start Settings")]
    [SerializeField, Range(0f, 20f)] private float startZoom;
    [SerializeField] private Transform startTransform;
   
    [Header("Show Seller Settings")]
    [SerializeField] private TutorialZoomData showSellerSettings;

    [Header("Show First Pack Settings")]
    [SerializeField] private TutorialZoomData showBasePackSettings;

    [Header("Show Defense Pack Settings")]
    [SerializeField] private TutorialZoomData showDefensePackSettings;

    [Header("Show Engineering Pack Settings")]
    [SerializeField] private TutorialZoomData showEngineeringPackSettings;

    [Header("Show Food Pack Settings")]
    [SerializeField] private TutorialZoomData showFoodPackSettings;

    private bool firstTimePlaying;
    private bool isSellerShown = false;
    private bool isBasePackUnlocked = false;
    private bool isDefensePackUnlocked = false;
    private bool isEngineeringPackUnlocked = false;
    private bool isFoodPackUnlocked = false;

    protected override void Awake()
    {
        base.Awake();

        Load();

        cameraMovement.transform.position = startTransform.position;
        cameraMovement.transform.rotation = startTransform.rotation;
        cameraMovement.ZoomInstantTo(startZoom);

        mainQuestManager.OnQuestCompleted += OnQuestCompleted;
    }

    private void OnDestroy()
    {
        mainQuestManager.OnQuestCompleted -= OnQuestCompleted;
    }


    private void Start()
    {
        if (!firstTimePlaying)
        {
            StopTutorial();
            logger.Log("Tutorial already completed, skipping tutorial start.", this);
            return;
        }

        StartTutorial();
    }


    public void StartTutorial()
    {
        
    }
    public void StopTutorial() => Save();

    private void OnQuestCompleted(Quest quest)
    {
        if(!isSellerShown && mainQuestManager.CompletedQuestCount == showSellerSettings.NumberOfQuestCompletedToShow)
        {
            CameraMovement.Instance.LockMovement();
            CameraMovement.Instance.MoveAndZoomToSequentially(showSellerSettings.TimeToMove, showSellerSettings.TimeToZoom,
                showSellerSettings.ZoomOrthoSize, showSellerSettings.CameraTransform.position, onComplete: () =>
            {
                isSellerShown = true;
                Save();
                MoveCameraBack(showSellerSettings.TimeBeforeGoingBackToOriginalPosition, showSellerSettings.GoBackToOriginalPositionTime);
            });
        }
        else if(!isBasePackUnlocked && mainQuestManager.CompletedQuestCount == showBasePackSettings.NumberOfQuestCompletedToShow)
        {
            CameraMovement.Instance.LockMovement();
            CameraMovement.Instance.MoveAndZoomToSequentially(showBasePackSettings.TimeToMove, showBasePackSettings.TimeToZoom,
                showBasePackSettings.ZoomOrthoSize, showBasePackSettings.CameraTransform.position, () =>
            {
                isBasePackUnlocked = true;
                Instantiate(enableAnimationPrefab, basePackCardShop.transform.position, Quaternion.identity);
                basePackCardShop.Shop.Unlock();
                Save();
                MoveCameraBack(showBasePackSettings.TimeBeforeGoingBackToOriginalPosition, showBasePackSettings.GoBackToOriginalPositionTime);
            });
        }
        else if(!isDefensePackUnlocked && mainQuestManager.CompletedQuestCount == showDefensePackSettings.NumberOfQuestCompletedToShow)
        {
            CameraMovement.Instance.LockMovement();
            CameraMovement.Instance.MoveAndZoomToSequentially(showDefensePackSettings.TimeToMove, showDefensePackSettings.TimeToZoom,
                showDefensePackSettings.ZoomOrthoSize, showDefensePackSettings.CameraTransform.position, () =>
                {
                    isDefensePackUnlocked = true;
                    Instantiate(enableAnimationPrefab, defensePackCardShop.transform.position, Quaternion.identity);
                    defensePackCardShop.Shop.Unlock();
                    Save();
                    MoveCameraBack(showDefensePackSettings.TimeBeforeGoingBackToOriginalPosition, showDefensePackSettings.GoBackToOriginalPositionTime);
                });
        }
        else if (!isEngineeringPackUnlocked && mainQuestManager.CompletedQuestCount == showEngineeringPackSettings.NumberOfQuestCompletedToShow)
        {
            CameraMovement.Instance.LockMovement();
            CameraMovement.Instance.MoveAndZoomToSequentially(showEngineeringPackSettings.TimeToMove, showEngineeringPackSettings.TimeToZoom,
                showEngineeringPackSettings.ZoomOrthoSize, showEngineeringPackSettings.CameraTransform.position, () =>
                {
                    isEngineeringPackUnlocked = true;
                    Instantiate(enableAnimationPrefab, engineeringkCardShop.transform.position, Quaternion.identity);
                    engineeringkCardShop.Shop.Unlock();
                    Save();
                    MoveCameraBack(showEngineeringPackSettings.TimeBeforeGoingBackToOriginalPosition, showEngineeringPackSettings.GoBackToOriginalPositionTime);
                });
        }
        else if (!isFoodPackUnlocked && mainQuestManager.CompletedQuestCount == showFoodPackSettings.NumberOfQuestCompletedToShow)
        {
            CameraMovement.Instance.LockMovement();
            CameraMovement.Instance.MoveAndZoomToSequentially(showFoodPackSettings.TimeToMove, showFoodPackSettings.TimeToZoom,
                showFoodPackSettings.ZoomOrthoSize, showFoodPackSettings.CameraTransform.position, () =>
                {
                    isFoodPackUnlocked = true;
                    Instantiate(enableAnimationPrefab, foodPackCardShop.transform.position, Quaternion.identity);
                    foodPackCardShop.Shop.Unlock();
                    Save();
                    MoveCameraBack(showFoodPackSettings.TimeBeforeGoingBackToOriginalPosition, showFoodPackSettings.GoBackToOriginalPositionTime);
                });
        }
    }

    private void MoveCameraBack(float timeToWait, float timeToGoBack)
    {
        Timer.Register(timeToWait, onComplete: () =>
            CameraMovement.Instance.MoveAndZoomToSameTime(timeToGoBack, timeToGoBack, startZoom, startTransform.position,
                onCompleteMove: () =>
                {
                    CameraMovement.Instance.UnlockMovement();
                }, onCompleteZoom: null));
    }

    public void Save()
    {
        try
        {
            TutorialSaveData tutorialSaveData = new TutorialSaveData
            {
                firstTimePlaying = this.firstTimePlaying,
                isBasePackUnlocked = this.isBasePackUnlocked,
                isSellerShown = this.isSellerShown,
                isDefensePackUnlocked = this.isDefensePackUnlocked,
                isEngineeringPackUnlocked = this.isEngineeringPackUnlocked,
                isFoodPackUnlocked = this.isFoodPackUnlocked
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
                this.isBasePackUnlocked = saveData.isBasePackUnlocked;
                this.isSellerShown = saveData.isSellerShown;
                this.isDefensePackUnlocked = saveData.isDefensePackUnlocked;
                this.isEngineeringPackUnlocked = saveData.isEngineeringPackUnlocked;
                this.isFoodPackUnlocked = saveData.isFoodPackUnlocked;
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

        if(isBasePackUnlocked)
            basePackCardShop.Shop.Unlock();
        if(isDefensePackUnlocked)
            defensePackCardShop.Shop.Unlock();
        if(isEngineeringPackUnlocked)
            engineeringkCardShop.Shop.Unlock();
        if(isFoodPackUnlocked)
            foodPackCardShop.Shop.Unlock();
    }

    private void LoadDefault()
    {
        this.firstTimePlaying = true;
        this.isBasePackUnlocked = false;
        this.isSellerShown = false;
    }
}
