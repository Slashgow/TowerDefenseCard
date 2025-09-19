using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityTimer;

public class HorizontalImageMover : MonoBehaviour
{
    [Header("Image List")]
    [SerializeField] private List<ImageMoveSettings> imageSettings = new List<ImageMoveSettings>();
    [SerializeField] private List<Sprite> imageSprites = new List<Sprite>();

    [Header("Global Settings")]
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private bool useCanvasBounds = true;

    private Canvas parentCanvas;
    private RectTransform canvasRect;
    private List<Timer> startTimers = new List<Timer>();

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();

        if (parentCanvas != null)
            canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (startOnAwake)
            StartMovement();
    }

    public void StartMovement()
    {
        StopAllMovement();

        foreach (var setting in imageSettings)
        {
            if (setting.image != null)
                StartImageMovement(setting);
        }
    }

    public void StopAllMovement()
    {
        foreach (var timer in startTimers)
        {
            if (timer != null)
                timer.Cancel();
        }
        startTimers.Clear();

        foreach (var setting in imageSettings)
        {
            if (setting.image != null)
                setting.image.rectTransform.DOKill();
        }
    }

    private void StartImageMovement(ImageMoveSettings setting)
    {
        setting.image.sprite = imageSprites[Random.Range(0, imageSprites.Count)];
        float startDelay = Random.Range(setting.startDelayRange.x, setting.startDelayRange.y);
        var timer = Timer.Register(startDelay, onComplete: () => MoveImage(setting));
        startTimers.Add(timer);
    }

    private void MoveImage(ImageMoveSettings setting)
    {
        RectTransform rectTransform = setting.image.rectTransform;
        Vector3 originalPosition = rectTransform.anchoredPosition;
        float distance = useCanvasBounds && canvasRect != null ? canvasRect.rect.width + 400f: setting.moveDistance;

        Vector3 startPos, endPos;

        if (setting.moveLeftToRight)
        {
            startPos = new Vector3(-distance / 2f, originalPosition.y, originalPosition.z);
            endPos = new Vector3(distance / 2f, originalPosition.y, originalPosition.z);
        }
        else
        {
            startPos = new Vector3(distance / 2f, originalPosition.y, originalPosition.z);
            endPos = new Vector3(-distance / 2f, originalPosition.y, originalPosition.z);
        }

        rectTransform.anchoredPosition = startPos;
        float moveDuration = Random.Range(setting.moveDurationRange.x, setting.moveDurationRange.y);
        Tween moveTween = rectTransform.DOAnchorPos(endPos, moveDuration).SetEase(setting.easeType);

        if (setting.loop)
        {
            moveTween.SetLoops(-1, setting.loopType);
            moveTween.OnStepComplete(() => setting.image.sprite = imageSprites[Random.Range(0, imageSprites.Count)]);
        }
           

        moveTween.Play();
    }

    public void AddImage(Image image, bool leftToRight = true, float moveDistance = 500f)
    {
        ImageMoveSettings newSetting = new ImageMoveSettings
        {
            image = image,
            moveLeftToRight = leftToRight,
            moveDistance = moveDistance
        };

        imageSettings.Add(newSetting);

        // Start movement for this image if the system is already running
        if (Application.isPlaying)
        {
            StartImageMovement(newSetting);
        }
    }

    public void RemoveImage(Image image)
    {
        for (int i = imageSettings.Count - 1; i >= 0; i--)
        {
            if (imageSettings[i].image == image)
            {
                // Stop movement for this image
                if (image != null)
                {
                    image.rectTransform.DOKill();
                }

                imageSettings.RemoveAt(i);
                break;
            }
        }
    }

    public void PauseAllMovement()
    {
        foreach (var setting in imageSettings)
        {
            if (setting.image != null)
            {
                setting.image.rectTransform.DOPause();
            }
        }
    }

    public void ResumeAllMovement()
    {
        foreach (var setting in imageSettings)
        {
            if (setting.image != null)
            {
                setting.image.rectTransform.DOPlay();
            }
        }
    }

    private void OnDestroy()
    {
        StopAllMovement();
    }
}