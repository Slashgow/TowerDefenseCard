using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverCameraZoomDisabler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData) => CameraMovement.Instance.IsZoomingEnable = false;
    public void OnPointerExit(PointerEventData eventData) => CameraMovement.Instance.IsZoomingEnable = true;
}