using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class UIOutlineSelector<T> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] private UIOutline uiOutline;
    [SerializeField] private Color hoverColor = Color.yellow;
    [SerializeField] private Color selectedColor = Color.white;

    private static UIOutlineSelector<T> currentSelected;
    private IUISelectable<T> selectableComponent;

    public event Action<T> OnSelect;
    public bool IsSelected => currentSelected == this;

    private void Awake()
    {
        selectableComponent = GetComponentInParent<IUISelectable<T>>();
        if (selectableComponent == null)
        {
            Debug.LogError($"UIOutlineSelector requires a component implementing ISelectable<{typeof(T).Name}>", this);
        }
    }

    private void OnEnable() => HideOutline();

    private void ShowOutline(Color color)
    {
        if (uiOutline != null)
        {
            uiOutline.enabled = true;
            uiOutline.color = color;
        }
    }

    private void HideOutline()
    {
        if (uiOutline != null)
            uiOutline.enabled = false;
    }

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

        Select();
    }

    public void Select()
    {
        currentSelected = this;
        ShowOutline(selectedColor);

        if (selectableComponent != null)
        {
            T data = selectableComponent.GetSelectableData();
            OnSelect?.Invoke(data);
            selectableComponent.OnSelect(data);
        }
    }

    public void Deselect()
    {
        if (currentSelected == this)
        {
            currentSelected = null;
            HideOutline();
        }
    }
    
}
