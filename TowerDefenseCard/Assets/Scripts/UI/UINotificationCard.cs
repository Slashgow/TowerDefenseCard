using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UINotificationCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image notificationImage;
    public Image NotificationImage => notificationImage;

    private CardID cardID;

    public void Setup(CardID cardID)
    {
        this.cardID = cardID;

        CardDiscoveryState cardDiscoveryState = CardManager.Instance.GetCardDiscoveryStateByCardID(cardID);

        if (!cardDiscoveryState.isClickedAfterNotification)
            notificationImage.gameObject.SetActive(true);
        else
            notificationImage.gameObject.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardDiscoveryState cardDiscoveryState = CardManager.Instance.GetCardDiscoveryStateByCardID(cardID);
        if (!cardDiscoveryState.isClickedAfterNotification)
        {
            CardManager.Instance.SetNotificationStatusByCardID(cardID, true);
            notificationImage.gameObject.SetActive(false);
        }
    }
}
