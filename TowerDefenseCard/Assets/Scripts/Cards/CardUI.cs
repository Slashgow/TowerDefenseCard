using TMPro;
using UnityEngine;

public class CardUI : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI cardTitleText;
    [SerializeField] protected TextMeshProUGUI cardCoinText;

    public virtual void SetupCard(string cardTitle, string cardCoin)
    {
        cardTitleText.text = cardTitle;
        cardCoinText.text = cardCoin;
    }
}
