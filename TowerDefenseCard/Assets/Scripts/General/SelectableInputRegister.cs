using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableInputRegister : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public static event Action OnAnySelectableHover;
    public static event Action OnAnySelectablePressed;
    public void OnPointerClick(PointerEventData eventData) => OnAnySelectablePressed?.Invoke();
    public void OnPointerEnter(PointerEventData eventData) => OnAnySelectableHover?.Invoke();
}
