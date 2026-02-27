using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPanelCardCraftingRecipes : MonoSingleton<UIPanelCardCraftingRecipes>
{
    [SerializeField] private Vector2 sourceCardWorldSize = new Vector2(1.4f, 2f);

    [Header("Layout")]
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject uiCardMenuPrefab;   // same prefab used by UIPageCardsDiscovered
    [SerializeField] private Vector3 positionOffset;

    private List<CraftingRecipe> matchingRecipes = new List<CraftingRecipe>();
    public event Action<Card, CardWithRecipe> OnSelectCard;
    public event Action OnShow;
    private bool isVisible = false;
    protected override void Awake()
    {
        base.Awake();
        Hide();
    }

    public void RequestShow(Card sourceCard, Vector3 position)
    {
        if (isVisible)
        {
            Hide();
            return;
        }
            

        if (sourceCard == null) 
            return;

        matchingRecipes.Clear();
        matchingRecipes = GetRecipesContainingCard(sourceCard.CardData.CardID);

        if (matchingRecipes.Count == 0)
        {
            Hide();
            return;
        }

        Populate(matchingRecipes);
        gameObject.SetActive(true);

        UIUtility.PlaceNearAnchor(
           panel: this.transform as RectTransform,
           anchorWorldPosition: position,
           sourceWorldSize: sourceCardWorldSize,
           preferredOffset: positionOffset
       );

        OnShow?.Invoke();
        isVisible = true;
    }

    public void Hide()
    {
        isVisible = false;
        gameObject.SetActive(false);
    }


    private List<CraftingRecipe> GetRecipesContainingCard(CardID cardID)
    {
        // For FARMER / CARPENTER treat them as WORKER too (mirrors CraftingManager logic)
        bool isWorkerVariant = cardID == CardID.FARMER || cardID == CardID.CARPENTER;

        return CraftingManager.Instance.Recipes
            .Where(recipe => recipe.Ingredients.Any(ingredient =>
                ingredient.cardID == cardID ||
                (isWorkerVariant && ingredient.cardID == CardID.WORKER)))
            .ToList();
    }

    private void Populate(List<CraftingRecipe> recipes)
    {
        CardUtility.DestroyAllChildren(contentParent);

        foreach (CraftingRecipe recipe in recipes)
        {
            // A recipe can produce several outputs – show one button per output card.
            foreach (CraftingRecipe.OutputCard outputCard in recipe.OutputCards)
            {
                CardDiscoveryState discoveryState = CardManager.Instance.GetCardDiscoveryStateByCardID(outputCard.cardID);
                if(discoveryState != null && !discoveryState.isDiscovered)
                {
                    // If the card is undiscovered, skip it – we don't want to show recipes for cards the player hasn't found yet.
                    continue;
                }

                Card outputCardPrefab = CardManager.Instance.GetCardPrefabByCardID(outputCard.cardID);
                if (outputCardPrefab == null) 
                    continue;

                GameObject uiCardGO = Instantiate(uiCardMenuPrefab, contentParent);

                // Setup the visual (same pattern as UIPageCardsDiscovered)
                UICardMenuWithRecipe uICardMenu = uiCardGO.GetComponent<UICardMenuWithRecipe>();
                uICardMenu.SetupUICardMenu(outputCardPrefab.CardData, new CardWithRecipe(outputCard.cardID, recipe), false);

                // Disable selection outline – we manage selection ourselves
                UICardOulineSelectorCardAndRecipe outlineSelector = uiCardGO.transform.GetChild(0).GetComponent<UICardOulineSelectorCardAndRecipe>();
                //if (outlineSelector != null) outlineSelector.enabled = false;

                // Currency colour tint (mirrors UIPageCardsDiscovered)
                if (outputCardPrefab is Currency)
                {
                    UnityEngine.UI.Image bg = uiCardGO.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
                    if (bg != null) bg.color = new Color32(255, 213, 90, 255);
                }

                // Wire up selection: clicking this output card tells UICardMoreStat to show it
                uICardMenu.OnSelectEvent -= UICardMenu_OnSelectCard;
                uICardMenu.OnSelectEvent += UICardMenu_OnSelectCard;
            }
        }
    }

    private void UICardMenu_OnSelectCard(CardWithRecipe cardWithRecipe)
    {
        Debug.Log($"Selected card {cardWithRecipe.CardID} from crafting recipes panel");
        Card card = CardManager.Instance.GetCardPrefabByCardID(cardWithRecipe.CardID);
        OnSelectCard?.Invoke(card, cardWithRecipe);
    }
}
