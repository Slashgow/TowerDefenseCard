using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cardTitleText;
    [SerializeField] private TextMeshProUGUI cardCoinText;

    public void SetupCard(string cardTitle, string cardCoin)
    {
        cardTitleText.text = cardTitle;
        cardCoinText.text = cardCoin;
    }
}
