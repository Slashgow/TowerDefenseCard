using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIQuestTab : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UIQuestManager uiQuestManager;
    [SerializeField] private bool autoShowOnStart = true;

    public UnityEvent OnEnter;

    private void Start()
    {
        if(autoShowOnStart)
            OnEnter?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }
}
