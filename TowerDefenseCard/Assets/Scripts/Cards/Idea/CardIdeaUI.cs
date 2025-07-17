using TMPro;
using UnityEngine;

public class CardIdeaUI : CardUI
{
    [SerializeField] private TextMeshProUGUI cardIdeaText;
    public void SetupCard(string cardTitle, string cardCoin, string cardIdea)
    {
        cardTitleText.text = cardTitle;
        cardCoinText.text = cardCoin;
        cardIdeaText.text = cardIdea;
    }
}
