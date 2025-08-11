using TMPro;
using UnityEngine;

public class UICardRecipe : UICardMenu
{
    [SerializeField] private TextMeshProUGUI cardAmount;

    public void SetupUICardRecipe(CardData cardData , int cardAmount)
    {
        this.cardData = cardData;
        cardTitle.text = cardData.CardName.GetLocalizedString();
        cardCost.text = cardData.Cost.ToString();
        cardImage.sprite = cardData.CardSprite;
        this.cardAmount.text = cardAmount.ToString();
        cardBackgroundImage.sprite = cardData.CardBackgroundSprite;
    }
}
