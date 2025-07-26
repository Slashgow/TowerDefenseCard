using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIQuestTab : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UIQuestManager uiQuestManager;

    public UnityEvent OnEnter;

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();
    }
}
