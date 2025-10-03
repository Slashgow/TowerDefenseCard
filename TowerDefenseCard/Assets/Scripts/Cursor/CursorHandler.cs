using System;
using UnityEngine;

[Serializable]
public struct CursorData
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotSpot;
    [SerializeField] private CursorMode cursorMode;
    public Texture2D CursorTexture => cursorTexture;
    public Vector2 HotSpot => hotSpot;
    public CursorMode CursorMode => cursorMode;
}

public class CursorHandler : MonoBehaviour
{
    [SerializeField] private CursorData defaultCursor;
    [SerializeField] private CursorData hoverCardCursor;
    [SerializeField] private CursorData grabCursor;

    private void Start()
    {
        CardMover.OnGrabCard += CardMover_OnGrabCard;
        CardMover.OnReleaseCard += CardMover_OnReleaseCard;
        CardMover.OnHoverEnterCard += CardMover_OnHoverEnterCard;
        CardMover.OnHoverExitCard += CardMover_OnHoverExitCard;
    }

    private void OnDestroy()
    {
        CardMover.OnGrabCard -= CardMover_OnGrabCard;
        CardMover.OnReleaseCard -= CardMover_OnReleaseCard;
        CardMover.OnHoverEnterCard -= CardMover_OnHoverEnterCard;
        CardMover.OnHoverExitCard -= CardMover_OnHoverExitCard;
    }

    public void SetCursor(CursorData cursorData) => Cursor.SetCursor(cursorData.CursorTexture, cursorData.HotSpot, cursorData.CursorMode);
    private void CardMover_OnHoverExitCard() => SetCursor(defaultCursor);
    private void CardMover_OnHoverEnterCard() => SetCursor(hoverCardCursor);
    private void CardMover_OnReleaseCard() => SetCursor(defaultCursor);
    private void CardMover_OnGrabCard() => SetCursor(grabCursor);
}
