
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private Color cardNamesColor;
    [SerializeField] private GameObject tooltipGameObject;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private CardShop cardShop;

    private List<string> cardNames = new List<string>();
    private List<string> cardIdeaNames = new List<string>();
    private string hexaCardNamesColor = string.Empty;

    private void Awake()
    {
        hexaCardNamesColor = "#"+ColorUtility.ToHtmlStringRGBA(cardNamesColor);
        SetupTooltipText();
        tooltipGameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        tooltipGameObject.SetActive(true);
    }

    private void SetupTooltipText()
    {
        cardNames.Clear();
        cardIdeaNames.Clear();
        foreach (ShopItem shopItem in cardShop.Shop.ShopItems)
        {
            cardNames.Add(shopItem.CardPrefab.GetComponent<Card>().CardData.CardName.GetLocalizedString());
        }
        foreach(ShopCardIdea shopCardIdea in cardShop.Shop.ShopCardIdeas)
        {
            cardIdeaNames.Add(shopCardIdea.CardIdeaPrefab.CardData.CardName.GetLocalizedString());
        }

        tooltipText.text = $"<color={hexaCardNamesColor}> Can provide : ";
        for (int i = 0; i < cardNames.Count; i++)
        {
            string cardName = cardNames[i];
            
            if (i == cardNames.Count - 1)
            {
                tooltipText.text += $"{cardName}.</color> \n";
                break;
            }

            tooltipText.text += $"{cardName}, ";
        }

        tooltipText.text += "Ideas : ";
        for(int i = 0;i < cardIdeaNames.Count; i++)
        {
            string cardIdeaName = cardIdeaNames[i];

            if (i == cardIdeaNames.Count - 1)
            {
                tooltipText.text += $"{cardIdeaName}.";
                break;
            }

            tooltipText.text += $"{cardIdeaName}, ";
        }
    }

    private void OnMouseExit()
    {
        tooltipGameObject.SetActive(false);
    }
}
