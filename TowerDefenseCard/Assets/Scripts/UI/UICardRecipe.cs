using TMPro;
using UnityEngine;

public class UICardRecipe : UICardMenu
{
    [SerializeField] private TextMeshProUGUI cardAmount;

    public void SetupUICardRecipe(CardData cardData , int cardAmount)
    {
        base.SetupUICardMenu(cardData);
        this.cardAmount.text = cardAmount.ToString();
    }
}
