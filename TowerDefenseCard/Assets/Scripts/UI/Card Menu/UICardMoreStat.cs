using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardMoreStat : MonoBehaviour
{
    [Header("Cards")]
    [SerializeField] private UIPageCardsDiscovered uIPageCardsDiscovered;
    [SerializeField] private GameObject cardPictureParent, cardStatsParents, cardPictureAndStat;
    [SerializeField] private TextMeshProUGUI upgradeDescription;
    [SerializeField] private TextMeshProUGUI damageValue, attackSpeedValue, attackRangeValue, AttackAreaValue, DotDamageValue, DotDurationValue;
    [SerializeField] private TextMeshProUGUI descriptionValue;
    [SerializeField] private Transform recipeParent;
    [SerializeField] private GameObject pairCardAndCostPrefab;

    [Header("Success")]
    [SerializeField] private UIPageSuccess uIPageSuccess;
    [SerializeField] private TextMeshProUGUI successDescriptionText;

    private void OnEnable()
    {
        descriptionValue.text = string.Empty;
        cardStatsParents.SetActive(false);
        uIPageCardsDiscovered.OnSelectCard += UIPageCardsDiscovered_OnSelectCard;

        successDescriptionText.text = string.Empty;
        uIPageSuccess.OnSelectSuccess += UIPageSuccess_OnSelectSuccess;
    }

    private void OnDisable()
    {
        uIPageCardsDiscovered.OnSelectCard -= UIPageCardsDiscovered_OnSelectCard;
        uIPageSuccess.OnSelectSuccess -= UIPageSuccess_OnSelectSuccess;
    }

    private void UIPageSuccess_OnSelectSuccess(SuccessData successData)
    {
        ClearCardContent();
        DisplaySuccessInfo(successData);
    }
    private void ClearCardContent()
    {
        CardUtility.DestroyAllChildren(cardPictureParent.transform);
        cardStatsParents.SetActive(false);
        cardPictureAndStat.SetActive(false);
        if (upgradeDescription != null)
            upgradeDescription.gameObject.SetActive(false);

        CardUtility.DestroyAllChildren(recipeParent);
        recipeParent.gameObject.SetActive(false);
        descriptionValue.text = string.Empty;
    }
    private void DisplaySuccessInfo(SuccessData successData)
    {
        if (successData == null) 
            return;

        successDescriptionText.text = successData.Description.GetLocalizedString();
    }

    private void UIPageCardsDiscovered_OnSelectCard(Card card)
    {
        cardPictureAndStat.SetActive(true);
        successDescriptionText.text = string.Empty;

        CardUtility.DestroyAllChildren(cardPictureParent.transform);

        GameObject uiCardMenuGameObject = Instantiate(uIPageCardsDiscovered.UICardMenuPrefab, cardPictureParent.transform);

        RectTransform rectTransform = uiCardMenuGameObject.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;

        descriptionValue.text = card.CardData.CardDescription.GetLocalizedString();

        UICardMenu uICardMenu = uiCardMenuGameObject.GetComponent<UICardMenu>();
        uICardMenu.SetupUICardMenu(card.CardData, true);
        uiCardMenuGameObject.transform.GetChild(0).GetChild(3).GetComponent<UIOutline>().enabled = false;
        uiCardMenuGameObject.transform.GetChild(0).GetComponent<UICardOutlineSelector>().enabled = false;

        if (card is Currency)
        {
            uiCardMenuGameObject.transform.GetChild(0).GetComponent<Image>().color = new Color32(255, 213, 90, 255);
            RectTransform rectTransformImage = uiCardMenuGameObject.transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
            Vector2 sizeDelta = rectTransformImage.sizeDelta;
            rectTransformImage.sizeDelta = sizeDelta * 0.5f;
            //uiCardMenuGameObject.transform.GetChild(3).gameObject.SetActive(false);
        }

        if (!card.GetComponent<BaseDamageor>())
            cardStatsParents.SetActive(false);

        else
        {
            cardStatsParents.SetActive(true);

            BaseDamageor cardBaseDamageor = card.GetComponent<BaseDamageor>();
            damageValue.text = cardBaseDamageor.CardDamageorData.Damage.ToString();
            attackSpeedValue.text = cardBaseDamageor.CardDamageorData.AttackSpeed.ToString();
            attackRangeValue.text = cardBaseDamageor.CardDamageorData.AttackRange.ToString();
            AttackAreaValue.text = cardBaseDamageor.CardDamageorData.AttackArea.ToString();
            DotDamageValue.text = cardBaseDamageor.CardDamageorData.DoT.ToString();
            DotDurationValue.text = cardBaseDamageor.CardDamageorData.DoTDuration.ToString();
        }

        CardUpgrade cardUpgrade = card.GetComponent<CardUpgrade>();
        if (cardUpgrade == null)
            upgradeDescription.gameObject.SetActive(false);
        else
        {
            upgradeDescription.gameObject.SetActive(true);
            upgradeDescription.text = cardUpgrade.UpgradeData.UpgradeLocalizedDescription.GetLocalizedString();
        }


        recipeParent.gameObject.SetActive(true);   
        CardUtility.DestroyAllChildren(recipeParent);

        if (card is CardExploitation)
        {
            CardExploitation cardExploitation =(CardExploitation)card;

            foreach (CraftingRecipe.OutputCard outputCard in cardExploitation.CraftingRecipe.OutputCards)
            {
                GameObject pairCardAndCost = Instantiate(pairCardAndCostPrefab, recipeParent);
                UICardRecipe uICardRecipe = pairCardAndCost.GetComponent<UICardRecipe>();
                string dropChance = $"{outputCard.dropChance} %";
                uICardRecipe.SetupUICardRecipe(CardManager.Instance.GetCardPrefabByCardID(outputCard.cardID).CardData, dropChance);
            }
        }
        else
        {
            CraftingRecipe craftingRecipe = CraftingManager.Instance.GetRecipeByOuputCardID(card.CardData.CardID);

            if (craftingRecipe == null)
                return;

            foreach (CraftingRecipe.Ingredient ingredient in craftingRecipe.Ingredients)
            {
                GameObject pairCardAndCost = Instantiate(pairCardAndCostPrefab, recipeParent);
                UICardRecipe uICardRecipe = pairCardAndCost.GetComponent<UICardRecipe>();
                uICardRecipe.SetupUICardRecipe(CardManager.Instance.GetCardPrefabByCardID(ingredient.cardID).CardData, ingredient.quantity);
            }
        }
    }


}
