using TMPro;
using UnityEngine;

public class UICardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberOfCardText;

    private void Start()
    {
        CardManager.Instance.OnUpdateNumberOfCards += CardManager_OnUpdateNumberOfCards;
        CardManager.Instance.OnUpdateMaxNumberOfCards += CardManager_OnUpdateNumberOfCards;

        UpdateText(CardManager.Instance.CurrentNumberOfCards, CardManager.Instance.MaxCardsAllowed);
    }

    private void OnDisable()
    {
        if (CardManager.HasInstance)
        {
            CardManager.Instance.OnUpdateNumberOfCards -= CardManager_OnUpdateNumberOfCards;
            CardManager.Instance.OnUpdateMaxNumberOfCards -= CardManager_OnUpdateNumberOfCards;
        }
    }

    private void CardManager_OnUpdateNumberOfCards(int numberOfCards, int maxNumberOfCards) => UpdateText(numberOfCards, maxNumberOfCards);

    private void UpdateText(int numberOfCards, int maxNumberOfCards)
    {
        numberOfCardText.text = $"{numberOfCards}/{maxNumberOfCards}";
    }
}
