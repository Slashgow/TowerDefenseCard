using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardMenu : MonoBehaviour
{
    [SerializeField] private Image cardBackgroundImage;
    [SerializeField] private Image cardImage;
    [SerializeField] private TextMeshProUGUI cardTitle;
    [SerializeField] private TextMeshProUGUI cardCost;

    private CardData cardData;
    public CardID CardID => cardData.CardID;
    public virtual void SetupUICardMenu(CardData cardData)
    { 
        this.cardData = cardData;
        cardTitle.text = cardData.CardName;
        cardCost.text = cardData.Cost.ToString();
        cardImage.sprite = cardData.CardSprite;
        cardBackgroundImage.sprite = cardData.CardBackgroundSprite;
    }
}
