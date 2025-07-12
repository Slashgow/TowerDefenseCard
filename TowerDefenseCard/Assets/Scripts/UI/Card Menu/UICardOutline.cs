using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICardOutline : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private UICardMenu uICardMenu;
    [SerializeField] private UIOutline uiOutline;
    [SerializeField] private Color hoverColor, selectedColor;

    private static UICardOutline currentSelected;
    public event Action<CardID> OnSelectCard;

    private void OnEnable() => HideOutline();
    private void ShowOutline(Color color)
    {
        uiOutline.enabled = true;
        uiOutline.color = color;
    }

    private void HideOutline() => uiOutline.enabled = false;
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentSelected != this) 
        {
            HideOutline();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentSelected != this) 
        {
            ShowOutline(hoverColor);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentSelected != null && currentSelected != this)
        {
            currentSelected.HideOutline();
        }
        currentSelected = this; 
        ShowOutline(selectedColor);
        OnSelectCard?.Invoke(uICardMenu.CardID);
    }
}
