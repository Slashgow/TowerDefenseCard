using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardRecipe : UICardMenu
{
    [SerializeField] private TextMeshProUGUI cardAmount;
    [SerializeField] private Image isDestroyedOnCraftImage;

    public void SetupUICardRecipe(CardData cardData , int cardAmount, bool isDestroyedOnCraft)
    {
        this.cardData = cardData;

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
        this.cardAmount.text = cardAmount.ToString();
        cardBackgroundImage.sprite = cardData.CardBackgroundSprite;

        if(isDestroyedOnCraft)
            isDestroyedOnCraftImage.gameObject.SetActive(true);
        else
            isDestroyedOnCraftImage.gameObject.SetActive(false);
    }

    public void SetupUICardRecipe(CardData cardData, string dropChance)
    {
        this.cardData = cardData;

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
        this.cardAmount.text = dropChance;
        cardBackgroundImage.sprite = cardData.CardBackgroundSprite;
        isDestroyedOnCraftImage.gameObject.SetActive(false);
    }
}
