using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

public class CraftingManager : MonoSingleton<CraftingManager>
{
    [SerializeField] private List<CraftingRecipe> recipes;
    [SerializeField] private GameObject cooldownBarPrefab;
    [SerializeField, Range(0f,1f)] private float cooldownBarOffset = 0.3f;

    public event Action<float> OnCooldownCraftTick = delegate { };
    public event Action OnCooldownCraftCancel = delegate { };

    private Timer craftingTimer;
    private GameObject cooldownBar;
    public bool TryCraft(Transform stackParent, out GameObject craftedCard)
    {
        craftedCard = null;
        Card parentCard = stackParent.GetComponent<Card>();
        if (parentCard == null) return false;

        // Get all cards in the stack (parent + children)
        List<Card> stackCards = new List<Card> { parentCard };
        stackCards.AddRange(stackParent.GetComponentsInChildren<Card>().Where(card => card != parentCard));

        // Count cards by cardID
        Dictionary<CardID, int> cardCounts = new Dictionary<CardID, int>();
        foreach (var card in stackCards)
        {
            cardCounts[card.CardData.CardID] = cardCounts.GetValueOrDefault(card.CardData.CardID, 0) + 1;
        }

        // Check each recipe
        foreach (var recipe in recipes)
        {
            if (IsRecipeMatch(recipe, cardCounts))
            {
                InitializeCooldownBar(stackParent, recipe.CraftingDelay);

                // Register a timer for the crafting delay
                craftingTimer = Timer.Register(
                    duration: recipe.CraftingDelay,
                    onUpdate: secondsElapsed => OnCooldownCraftTick?.Invoke(secondsElapsed),
                    onComplete: () => Craft(stackCards, recipe, stackParent.parent, out GameObject craftedCard, cooldownBar)
                );
                return true;
            }
        }

        return false;
    }

    private void InitializeCooldownBar(Transform stackParent, float craftingDelay)
    {
        cooldownBar = Instantiate(cooldownBarPrefab, stackParent.position, Quaternion.identity);
        CooldownBarUI cooldownBarUI = cooldownBar.GetComponentInChildren<CooldownBarUI>();
        cooldownBarUI.Init(stackParent, craftingDelay, cooldownBarOffset);
        cooldownBar.transform.position = stackParent.position + new Vector3(0, cooldownBarOffset, 0);
    }

    private void Craft(List<Card> stackCards, CraftingRecipe recipe, Transform newParent, out GameObject craftedCard, GameObject cooldownBar)
    {
        // Destroy input cards
        foreach (var card in stackCards)
        {
            Destroy(card.gameObject);
        }

        if (cooldownBar != null)
            Destroy(cooldownBar);

        // Instantiate output card at the stack's position
        craftedCard = Instantiate(recipe.OutputCardPrefab, stackCards[0].transform.position, Quaternion.identity, newParent);
    }

    private bool IsRecipeMatch(CraftingRecipe recipe, Dictionary<CardID, int> cardCounts)
    {
        if(recipe.Ingredients.Count != cardCounts.Count)
            return false;

        foreach (var ingredient in recipe.Ingredients)
        {
            if (!cardCounts.ContainsKey(ingredient.cardID) || cardCounts[ingredient.cardID] < ingredient.quantity)
            {
                return false;
            }
        }
        return true;
    }

    public void CancelCraft()
    {
        OnCooldownCraftCancel?.Invoke();
        Timer.Cancel(craftingTimer);
     
        if (cooldownBar != null)
            Destroy(cooldownBar);
    }
}