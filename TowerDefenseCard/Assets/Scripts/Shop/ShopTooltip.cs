
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class ShopTooltip : MonoBehaviour
{
    [SerializeField] private Color cardNamesColor;
    [SerializeField] private GameObject tooltipGameObject;
    [SerializeField] private TextMeshProUGUI tooltipText;
    [SerializeField] private CardShop cardShop;
    [SerializeField] private LocalizedString startCardTooltip, startIdeaTooltip;

    private List<string> cardNames = new List<string>();
    private List<string> cardIdeaNames = new List<string>();
    private string hexaCardNamesColor = string.Empty;

    private void Awake()
    {
        hexaCardNamesColor = "#"+ColorUtility.ToHtmlStringRGBA(cardNamesColor);
        SetupTooltipText();
        tooltipGameObject.SetActive(false);
    }

    private void Start()
    {
        LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    private void LocalizationSettings_SelectedLocaleChanged(Locale locale) => SetupTooltipText();

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
#if UNITY_WEBGL
            shopItem.CardPrefab.GetComponent<Card>().CardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    cardNames.Add(handle.Result);
                }
            };
#endif

#if !UNITY_WEBGL
            cardNames.Add(shopItem.CardPrefab.GetComponent<Card>().CardData.CardName.GetLocalizedString());
#endif


        }
        foreach (ShopCardIdea shopCardIdea in cardShop.Shop.ShopCardIdeas)
        {
#if UNITY_WEBGL
            shopCardIdea.CardIdeaPrefab.CardData.CardName.GetLocalizedStringAsync().Completed += (handle) =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    cardIdeaNames.Add(handle.Result);
                }
            };
#endif

#if !UNITY_WEBGL
           cardIdeaNames.Add(shopCardIdea.CardIdeaPrefab.CardData.CardName.GetLocalizedString());
#endif

        }


#if UNITY_WEBGL
        startCardTooltip.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                tooltipText.text = $"<color={hexaCardNamesColor}> {handle.Result} ";
            }
        };
#endif

#if !UNITY_WEBGL
        tooltipText.text = $"<color={hexaCardNamesColor}> {startCardTooltip.GetLocalizedString()} ";
#endif


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

#if UNITY_WEBGL
        startCardTooltip.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                tooltipText.text += $"{handle.Result} ";
            }
        };
#endif

#if !UNITY_WEBGL
        tooltipText.text += $"{startIdeaTooltip.GetLocalizedString()} ";
#endif



        for (int i = 0;i < cardIdeaNames.Count; i++)
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
