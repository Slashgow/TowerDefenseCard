
using UnityEngine;
using UnityEngine.EventSystems;

public class CardShop : Card, IPointerUpHandler, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("On Pointer down shop");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("On Pointer up shop");
        ShopManager.Instance.TryPurchaseWeightedCard();
    }
}
