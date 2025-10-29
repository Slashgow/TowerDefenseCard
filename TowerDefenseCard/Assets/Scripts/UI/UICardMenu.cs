using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardMenu : MonoBehaviour, IUISelectable<CardID>
{
    [SerializeField] protected Image cardBackgroundImage;
    [SerializeField] protected Image cardImage;
    [SerializeField] protected TextMeshProUGUI cardTitle;
    [SerializeField] protected TextMeshProUGUI cardCost;
    [SerializeField] protected UINotificationCard uiNotificationCard;
    [SerializeField] private UILockCard uiLockCard;

    protected CardData cardData;
    public CardID CardID => cardData.CardID;

    public event Action<CardID> OnSelectEvent;
    public CardID GetSelectableData() => CardID;
    public void OnSelect(CardID data) => OnSelectEvent?.Invoke(data);

    public virtual void SetupUICardMenu(CardData cardData, bool disableNotification)
    { 
        this.cardData = cardData;
        cardTitle.text = cardData.CardName.GetLocalizedString();
        cardCost.text = cardData.Cost.ToString();
        cardImage.sprite = cardData.CardSprite;
        cardBackgroundImage.sprite = cardData.CardBackgroundSprite;

        if (disableNotification)
        {
            uiNotificationCard.NotificationImage.gameObject.SetActive(false);
            uiNotificationCard.enabled = false;
        }
        else
            uiNotificationCard.Setup(CardID);

        uiLockCard.Setup(cardData.CardID);
    }
}
