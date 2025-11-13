using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] private GameObject recipeHighestParent;
    [SerializeField] private Transform recipeParent;
    [SerializeField] private GameObject pairCardAndCostPrefab;

    [Header("Success")]
    [SerializeField] private UIPageSuccess uIPageSuccess;
    [SerializeField] private TextMeshProUGUI successDescriptionText;

    public static readonly string[] BOOSTER_OPEN_STEAM_SUCCESS_IDS = { "SUCCESS_BOOSTER_ADDICT_1", "SUCCESS_BOOSTER_ADDICT_2", "SUCCESS_BOOSTER_ADDICT_3", "SUCCESS_BOOSTER_ADDICT_4" };
    public static readonly string[] CRAFTED_CARDS_STEAM_SUCCESS_IDS = { "SUCCESS_CRAFTER_2", "SUCCESS_CRAFTER_3", "SUCCESS_CRAFTER_4" };
    public static readonly string[] SOLD_CARDS_STEAM_SUCCESS_IDS = { "SUCCESS_SELLER_1", "SUCCESS_SELLER_2", "SUCCESS_SELLER_3", "SUCCESS_SELLER_4" };

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
        recipeHighestParent.SetActive(false);
        recipeParent.gameObject.SetActive(false);
        descriptionValue.text = string.Empty;
    }
    private void DisplaySuccessInfo(SuccessData successData)
    {
        if (successData == null) 
            return;

#if UNITY_WEBGL
        successData.Description.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                successDescriptionText.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
           successDescriptionText.text = successData.Description.GetLocalizedString();
#endif


        TryDisplaySuccessStat(successData.SteamId);
    }

    private void TryDisplaySuccessStat(string steamID)
    {
        SuccessStatData successStatData = SuccessManager.Instance.SuccessStatData;

        if (BOOSTER_OPEN_STEAM_SUCCESS_IDS.Contains(steamID))
        {
            successDescriptionText.text += $"\n\n {successStatData.boosterOpenedCounterAllTime} /";
            switch (steamID)
            {
                case "SUCCESS_BOOSTER_ADDICT_1":
                    successDescriptionText.text += " 50";
                    break;
                case "SUCCESS_BOOSTER_ADDICT_2":
                    successDescriptionText.text += " 250";
                    break;
                case "SUCCESS_BOOSTER_ADDICT_3":
                    successDescriptionText.text += " 500";
                    break;
                case "SUCCESS_BOOSTER_ADDICT_4":
                    successDescriptionText.text += " 1000";
                    break;
                default:
                    break;
            }
        }
        else if (CRAFTED_CARDS_STEAM_SUCCESS_IDS.Contains(steamID))
        {
            successDescriptionText.text += $"\n\n {successStatData.craftedCardCounterAllTime} /";
            switch (steamID)
            {
                case "SUCCESS_CRAFTER_2":
                    successDescriptionText.text += " 250";
                    break;
                case "SUCCESS_CRAFTER_3":
                    successDescriptionText.text += " 1000";
                    break;
                case "SUCCESS_CRAFTER_4":
                    successDescriptionText.text += " 10000";
                    break;
                default:
                    break;
            }
        }
        else if (SOLD_CARDS_STEAM_SUCCESS_IDS.Contains(steamID))
        {
            successDescriptionText.text += $"\n\n {successStatData.soldCardCounterAllTime} /";
            switch (steamID)
            {
                case "SUCCESS_SELLER_1":
                    successDescriptionText.text += " 100";
                    break;
                case "SUCCESS_SELLER_2":
                    successDescriptionText.text += " 500";
                    break;
                case "SUCCESS_SELLER_3":
                    successDescriptionText.text += " 2500";
                    break;
                case "SUCCESS_SELLER_4":
                    successDescriptionText.text += " 5000";
                    break;
                default:
                    break;
            }
        }
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


#if UNITY_WEBGL
        card.CardData.CardDescription.GetLocalizedStringAsync().Completed += (handle) =>
        {
            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                descriptionValue.text = handle.Result;
            }
        };
#endif

#if !UNITY_WEBGL
       descriptionValue.text = card.CardData.CardDescription.GetLocalizedString();
#endif



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

#if UNITY_WEBGL
            cardUpgrade.UpgradeData.UpgradeLocalizedDescription.GetLocalizedStringAsync().Completed += (handle) =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    upgradeDescription.text = handle.Result;
                }
            };
#endif

#if !UNITY_WEBGL
            upgradeDescription.text = cardUpgrade.UpgradeData.UpgradeLocalizedDescription.GetLocalizedString();
#endif
        }

        recipeHighestParent.SetActive(true);
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
        else if(card is CardWorker)
        {
            List<CraftingRecipe> craftingRecipes = CraftingManager.Instance.GetRecipesByOutputCardID(CardID.CURRENCY);

            CraftingRecipe craftingRecipe = null;

            switch (card.CardData.CardID)
            {
                case CardID.WORKER:
                    craftingRecipe = craftingRecipes.FirstOrDefault(recipe => recipe.Ingredients.Any(ingredient => ingredient.cardID == CardID.RICE));
                    break;
                case CardID.FARMER:
                    craftingRecipe = craftingRecipes.FirstOrDefault(recipe => recipe.Ingredients.Any(ingredient => ingredient.cardID == CardID.SICKLE));
                    break;
                case CardID.CARPENTER:
                    craftingRecipe = craftingRecipes.FirstOrDefault(recipe => recipe.Ingredients.Any(ingredient => ingredient.cardID == CardID.HAMMER));
                    break;
                default:
                    break;
            }

            if(craftingRecipe == null)
                return;

            foreach (CraftingRecipe.Ingredient ingredient in craftingRecipe.Ingredients)
            {
                GameObject pairCardAndCost = Instantiate(pairCardAndCostPrefab, recipeParent);
                UICardRecipe uICardRecipe = pairCardAndCost.GetComponent<UICardRecipe>();
                uICardRecipe.SetupUICardRecipe(CardManager.Instance.GetCardPrefabByCardID(ingredient.cardID).CardData, ingredient.quantity, !ingredient.isNotDestroyedOnCraft);
            }
        }
        else
        {
            CraftingRecipe craftingRecipe = CraftingManager.Instance.GetRecipeByOutputCardID(card.CardData.CardID);

            if (craftingRecipe == null)
                return;

            foreach (CraftingRecipe.Ingredient ingredient in craftingRecipe.Ingredients)
            {
                GameObject pairCardAndCost = Instantiate(pairCardAndCostPrefab, recipeParent);
                UICardRecipe uICardRecipe = pairCardAndCost.GetComponent<UICardRecipe>();
                uICardRecipe.SetupUICardRecipe(CardManager.Instance.GetCardPrefabByCardID(ingredient.cardID).CardData, ingredient.quantity, !ingredient.isNotDestroyedOnCraft);
            }
        }
    }


}
