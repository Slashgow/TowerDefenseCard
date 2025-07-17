using TMPro;
using UnityEngine;

public class UICardMoreStat : MonoBehaviour
{
    [SerializeField] private UIPageCardsDiscovered uIPageCardsDiscovered;
    [SerializeField] private GameObject cardPictureParent, cardStatsParents;
    [SerializeField] private TextMeshProUGUI damageValue, attackSpeedValue, attackRangeValue, AttackAreaValue, DotDamageValue, DotDurationValue;
    [SerializeField] private TextMeshProUGUI descriptionValue;
    [SerializeField] private Transform recipeParent;
    [SerializeField] private GameObject pairCardAndCostPrefab;

    private void OnEnable()
    {
        descriptionValue.text = string.Empty;
        cardStatsParents.SetActive(false);
        uIPageCardsDiscovered.OnSelectCard += UIPageCardsDiscovered_OnSelectCard;
    }

    private void OnDisable() => uIPageCardsDiscovered.OnSelectCard -= UIPageCardsDiscovered_OnSelectCard;

    private void UIPageCardsDiscovered_OnSelectCard(Card card)
    {
        CardUtility.DestroyAllChildren(cardPictureParent.transform);

        GameObject uiCardMenuGameObject = Instantiate(uIPageCardsDiscovered.UICardMenuPrefab, cardPictureParent.transform);

        RectTransform rectTransform = uiCardMenuGameObject.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;

        UICardMenu uICardMenu = uiCardMenuGameObject.GetComponent<UICardMenu>();
        uICardMenu.SetupUICardMenu(card.CardData);
        uiCardMenuGameObject.GetComponent<UIOutline>().enabled = false;
        uiCardMenuGameObject.transform.GetChild(0).GetComponent<UICardOutline>().enabled = false;

        if(!card.GetComponent<BaseDamageor>())
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

        
        CraftingRecipe craftingRecipe = CraftingManager.Instance.GetRecipeByOuputCardID(card.CardData.CardID);

        CardUtility.DestroyAllChildren(recipeParent);

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
