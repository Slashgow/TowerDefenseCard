using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardMenuWithRecipe : MonoBehaviour, IUISelectable<CardWithRecipe>
{
    [SerializeField] protected Image cardBackgroundImage;
    [SerializeField] protected Image cardImage;
    [SerializeField] protected TextMeshProUGUI cardTitle;
    [SerializeField] protected TextMeshProUGUI cardCost;
    [SerializeField] protected UINotificationCard uiNotificationCard;
    [SerializeField] private UILockCard uiLockCard;

    private CardData cardData;
    protected CardWithRecipe cardWithRecipe;
    public CardWithRecipe CardWithRecipe => cardWithRecipe;

    public event Action<CardWithRecipe> OnSelectEvent;
    public CardWithRecipe GetSelectableData() => CardWithRecipe;
    public void OnSelect(CardWithRecipe data) => OnSelectEvent?.Invoke(data);

    public virtual void SetupUICardMenu(CardData cardData, CardWithRecipe cardWithRecipe, bool disableNotification)
    {
        this.cardData = cardData;
        this.cardWithRecipe = cardWithRecipe;

#if UNITY_WEBGL
        cardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                cardTitle.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
        cardTitle.text = cardData.CardName.GetLocalizedString();
#endif


        cardCost.text = cardData.Cost.ToString();
        cardImage.sprite = cardData.CardSprite;
        cardBackgroundImage.sprite = cardData.CardBackgroundSprite;

        if (disableNotification)
        {
            uiNotificationCard.NotificationImage.gameObject.SetActive(false);
            uiNotificationCard.enabled = false;
        }
        else
            uiNotificationCard.Setup(CardWithRecipe.CardID);

        if (uiLockCard != null)
            uiLockCard.Setup(cardWithRecipe.CardID);
    }
}