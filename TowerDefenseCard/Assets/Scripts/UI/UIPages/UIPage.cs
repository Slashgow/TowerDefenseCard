using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class UIPage : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    protected virtual void Awake() => canvasGroup = GetComponent<CanvasGroup>();
    public virtual void Show() 
    { 
        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
    public virtual void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
