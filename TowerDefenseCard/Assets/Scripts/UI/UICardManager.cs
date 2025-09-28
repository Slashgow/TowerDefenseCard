using TMPro;
using UnityEngine;

public class UICardManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI numberOfCardText;
    [SerializeField] private TextMeshProUGUI numberOfDefenseCardText;

    [SerializeField] private UIOutlineUnscaled uiOutlineUnscaledCard, uiOutlineUnscaledCardDefense;
    [SerializeField] private OutlineWidthEffect outlineWidthEffectCards, outlineWidthEffectCardDefense;

    private void Start()
    {
        CardManager.Instance.OnUpdateNumberOfCards += CardManager_OnUpdateNumberOfCards;
        CardManager.Instance.OnUpdateMaxNumberOfCards += CardManager_OnUpdateNumberOfCards;
        CardManager.Instance.OnUpdateNumberOfDefenseCards += CardManager_OnUpdateNumberOfDefenseCards;
        CardManager.Instance.OnUpdateMaxNumberOfDefenseCards += CardManager_OnUpdateNumberOfDefenseCards;


        Debug.Log("Initializing UICardManager with current card counts.");
        CardManager_OnUpdateNumberOfCards(CardManager.Instance.CurrentNumberOfCards, CardManager.Instance.MaxCardsAllowed);
        CardManager_OnUpdateNumberOfDefenseCards(CardManager.Instance.CurrentNumberOfDefenseCards, CardManager.Instance.MaxCardsDefenseAllowed);
    }

    private void OnDisable()
    {
        if (CardManager.HasInstance)
        {
            CardManager.Instance.OnUpdateNumberOfCards -= CardManager_OnUpdateNumberOfCards;
            CardManager.Instance.OnUpdateMaxNumberOfCards -= CardManager_OnUpdateNumberOfCards;
            CardManager.Instance.OnUpdateNumberOfDefenseCards -= CardManager_OnUpdateNumberOfDefenseCards;
            CardManager.Instance.OnUpdateMaxNumberOfDefenseCards -= CardManager_OnUpdateNumberOfDefenseCards;
        }
    }

    private void CardManager_OnUpdateNumberOfCards(int numberOfCards, int maxNumberOfCards)
    {
        UpdateText(numberOfCardText, numberOfCards, maxNumberOfCards);

        if (CardManager.Instance.IsMaxCardsReached)
        {
            outlineWidthEffectCards.DoEffect();
            uiOutlineUnscaledCard.enabled = true;
        }

        else
        {
            outlineWidthEffectCards.StopEffect();
            uiOutlineUnscaledCard.enabled = false;
        }
    }

    private void CardManager_OnUpdateNumberOfDefenseCards(int currentNumberOfDefenseCard, int maxNumberOfDefenseCard)
    {
        Debug.Log($"Updating Defense Cards UI: {currentNumberOfDefenseCard}/{maxNumberOfDefenseCard}");
        UpdateText(numberOfDefenseCardText, currentNumberOfDefenseCard, maxNumberOfDefenseCard);

        if (CardManager.Instance.IsMaxDefenseCardsReached)
        {
            outlineWidthEffectCardDefense.DoEffect();
            uiOutlineUnscaledCardDefense.enabled = true;
        }

        else
        {
            outlineWidthEffectCardDefense.StopEffect();
            uiOutlineUnscaledCardDefense.enabled = false;
        }
    }

    private void UpdateText(TextMeshProUGUI text, int numberOfCards, int maxNumberOfCards)
    {
        text.text = $"{numberOfCards}/{maxNumberOfCards}";
    }
}
