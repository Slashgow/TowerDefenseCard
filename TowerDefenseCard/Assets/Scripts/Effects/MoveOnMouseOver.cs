using UnityEngine.EventSystems;
using UnityEngine.UI;


public class MoveOnMouseOver : MoveUIHorizontally, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveHorizontally();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetPosition();
    }
}
