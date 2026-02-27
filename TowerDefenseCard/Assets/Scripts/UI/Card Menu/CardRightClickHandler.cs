using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card))]
public class CardRightClickHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    [SerializeField] private Card card;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UIPanelCardCraftingRecipes.Instance.RequestShow(card, this.transform.position);
        }
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            UIPanelCardCraftingRecipes.Instance.Hide();
        }
    }
}
