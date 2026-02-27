using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UICardRecipeDisplay : MonoBehaviour
{
    [SerializeField] private UIPanelCardCraftingRecipes craftingRecipesPanel;

    [SerializeField] private Transform recipeParent;
    [SerializeField] private GameObject pairCardAndCostPrefab;

    private void OnEnable()
    {
        craftingRecipesPanel.OnSelectCard += DisplayRecipeForCard;
        craftingRecipesPanel.OnShow += Clear;
    }

    private void OnDisable()
    {
        craftingRecipesPanel.OnSelectCard -= DisplayRecipeForCard;
        craftingRecipesPanel.OnShow -= Clear;
    }

    /// <summary>
    /// Call this with any card to populate the ingredient list.
    /// Mirrors the recipe-display logic in UICardMoreStat.UIPageCardsDiscovered_OnSelectCard.
    /// </summary>
    public void DisplayRecipeForCard(Card card, CardWithRecipe cardWithRecipe)
    {
        CardUtility.DestroyAllChildren(recipeParent);

        if (card == null)
            return;

        if (card is CardExploitation)
        {
            CardExploitation cardExploitation = (CardExploitation)card;

            foreach (CraftingRecipe.OutputCard outputCard in cardExploitation.CraftingRecipe.OutputCards)
            {
                GameObject pairCardAndCost = Instantiate(pairCardAndCostPrefab, recipeParent);
                UICardRecipe uICardRecipe = pairCardAndCost.GetComponent<UICardRecipe>();
                string dropChance = $"{outputCard.dropChance} %";
                uICardRecipe.SetupUICardRecipe(CardManager.Instance.GetCardPrefabByCardID(outputCard.cardID).CardData, dropChance);
            }
        }
        else if (card is Merchant)
        {
            Merchant cardMerchant = (Merchant)card;

            foreach (ShopItem shopItem in cardMerchant.Shop.ShopItems)
            {
                GameObject pairCardAndCost = Instantiate(pairCardAndCostPrefab, recipeParent);
                UICardRecipe uICardRecipe = pairCardAndCost.GetComponent<UICardRecipe>();
                string dropChance = $"{shopItem.DropPercentage} %";
                uICardRecipe.SetupUICardRecipe(shopItem.CardPrefab.GetComponent<Card>().CardData, dropChance);
            }
        }
        else if (card is CardWorker)
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
            }

            if (craftingRecipe == null)
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
            CraftingRecipe craftingRecipe = cardWithRecipe.CraftingRecipe; // CraftingManager.Instance.GetRecipeByOutputCardID(card.CardData.CardID);

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

    /// <summary>Clears the displayed recipe without needing a card reference.</summary>
    public void Clear()
    {
        CardUtility.DestroyAllChildren(recipeParent);
    }
}