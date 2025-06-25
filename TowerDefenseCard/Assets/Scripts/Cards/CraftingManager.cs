using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

[Serializable]
public class CraftInfo
{
    private Transform stackParent;
    private CraftingRecipe craftingRecipe;
    private List<Card> stackCards;
    private int craftID;
    public List<Card> StackCards => stackCards;
    public Transform StackParent => stackParent;
    public CraftingRecipe CraftingRecipe => craftingRecipe;
    public int CraftID => craftID;
    public CraftInfo(Transform stackParent, CraftingRecipe craftingRecipe, List<Card> stackCards, int craftID)
    {
        this.stackParent = stackParent;
        this.craftingRecipe = craftingRecipe;
        this.stackCards = stackCards;
        this.craftID = craftID;
    }
}

public class CraftingManager : MonoSingleton<CraftingManager>
{
    [SerializeField, Range(0f, 500f)] private float timeCraftMode;
    [SerializeField] private List<CraftingRecipe> recipes;
    [SerializeField] private GameObject cooldownBarPrefab;
    [SerializeField, Range(0f,2f)] private float cooldownBarOffset = 0.3f;

    public event Action OnCraftCancel = delegate { };

    private Timer CraftingModeDurationTimer;
    public event Action<float> OnTickTimeCraftingMode;
    public float TimeCraftMode => timeCraftMode;
  
    private GameObject cooldownBar;
    private List<CraftInfo> currentCrafts = new List<CraftInfo>();
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
                currentCrafts.Add(new CraftInfo(stackParent, recipe, stackCards, CardUtility.GenerateUniqueID()));
                InitializeCooldownBar(stackParent, recipe.CraftingDelay, currentCrafts[currentCrafts.Count - 1].CraftID);
                return true;
            }
        }

        return false;
    }

    private void InitializeCooldownBar(Transform stackParent, float craftingDelay, int craftID)
    {
        cooldownBar = Instantiate(cooldownBarPrefab, stackParent.position, Quaternion.identity);
        cooldownBar.GetComponent<Canvas>().sortingOrder = 30;
        CooldownBarUI cooldownBarUI = cooldownBar.GetComponentInChildren<CooldownBarUI>();
        cooldownBarUI.OnCraftDelayEnd -= CooldownBarUI_OnCraftDelayEnd;
        cooldownBarUI.OnCraftDelayEnd += CooldownBarUI_OnCraftDelayEnd;
        cooldownBarUI.Init(stackParent, craftingDelay, cooldownBarOffset, craftID);
        cooldownBar.transform.position = stackParent.position + new Vector3(0, cooldownBarOffset, 0);
    }

    private void CooldownBarUI_OnCraftDelayEnd(int craftID)
    {
        CraftInfo craftInfo = currentCrafts.First(currentCraft => currentCraft.CraftID == craftID);
        Craft(craftInfo, out GameObject craftedCard);
    }

    private void Craft(CraftInfo craftInfo, out GameObject craftedCard)
    {
        // Destroy input cards
        foreach (var card in craftInfo.StackCards)
        {
            Destroy(card.gameObject);
        }

        // Instantiate output card at the stack's position
        craftedCard = Instantiate(craftInfo.CraftingRecipe.OutputCardPrefab, craftInfo.StackCards[0].transform.position, Quaternion.identity, craftInfo.StackParent.parent);
        currentCrafts.Remove(craftInfo);
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
        OnCraftCancel?.Invoke();
    }

    public void StartCraftingModeTimer()
    {
        CraftingModeDurationTimer = Timer.Register(timeCraftMode, 
            onComplete: GameManager.Instance.SwitchGameMode, 
            onUpdate: timeElapsed => OnTickTimeCraftingMode?.Invoke(timeElapsed));
    }
}