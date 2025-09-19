using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class ImageMoveSettings
{
    [Header("Movement Settings")]
    public Image image;
    public bool moveLeftToRight = true; 
    public float moveDistance = 500f; 

    [Header("Timing Settings")]
    public Vector2 moveDurationRange = new Vector2(2f, 5f); 
    public Vector2 startDelayRange = new Vector2(0f, 3f); 
    public Ease easeType = Ease.Linear;

    [Header("Loop Settings")]
    public bool loop = true;
    public LoopType loopType = LoopType.Restart;
}
