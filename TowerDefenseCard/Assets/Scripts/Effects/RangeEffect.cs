using FirstGearGames.Utilities.Structures;
using UnityEngine;

public abstract class RangeEffect: MonoBehaviour
{
    [SerializeField] private LineRenderer rangeIndicator;
    [SerializeField] private CardMover cardMover;
    [SerializeField] private bool showDuringMovement = true;

    protected float range; 
    public float Range => range;
    
    private void Start() => rangeIndicator.enabled = false;

    private void OnEnable()
    {
        cardMover.OnPointerDownEvent += CardMover_OnPointerDownEvent;
        cardMover.OnPointerUpEvent += CardMover_OnPointerUpEvent;
    }

    private void OnDisable()
    {
        cardMover.OnPointerDownEvent -= CardMover_OnPointerDownEvent;
        cardMover.OnPointerUpEvent -= CardMover_OnPointerUpEvent;
    }

    private void CardMover_OnPointerUpEvent() => ShowRange(false);
    private void CardMover_OnPointerDownEvent() => ShowRange(true);


    public void ShowRange(bool show)
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.enabled = show;
            if (show)
                DrawRangeCircle();
        } 
    }

    private void DrawRangeCircle()
    {
        if (rangeIndicator == null) 
            return;

        float range = Range;
        const int segments = 32; // Number of points for a smooth circle
        rangeIndicator.positionCount = segments + 1;

        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * range;
            float y = Mathf.Sin(angle) * range;
            rangeIndicator.SetPosition(i, new Vector3(x, y, 0));
        }
    }

}
