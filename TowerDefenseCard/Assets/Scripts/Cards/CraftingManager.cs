using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

public class CraftingManager : MonoSingleton<CraftingManager>
{
    [SerializeField, Range(0f, 500f)] private float timeCraftMode;
    [SerializeField] private List<CraftingRecipe> recipes;
    [SerializeField] private GameObject cooldownBarPrefab;
    [SerializeField, Range(0f,2f)] private float cooldownBarOffset = 0.3f;

    public event Action<int> OnCraftCancel = delegate { };
    public event Action<int, CardID> OnCraftComplete = delegate { };

    private Timer CraftingModeDurationTimer;
    public event Action<float> OnTickTimeCraftingMode;
    public float TimeCraftMode => timeCraftMode;
  
    private GameObject cooldownBar;
    private List<CraftInfo> currentCrafts = new List<CraftInfo>();

    private void OnEnable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCraftMode += GameManager_OnStartCraftMode;
    }

    private void OnDisable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCraftMode -= GameManager_OnStartCraftMode;
    }

    private void GameManager_OnStartCraftMode() => StartCraftingModeTimer();

    public bool TryCraft(Transform stackParent, Card movedCard)
    {
        Card parentCard = stackParent.GetComponent<Card>();
        if (parentCard == null) return false;

        // Get all cards in the stack (parent + children)
        List<Card> stackCards = new List<Card> { parentCard };
        stackCards.AddRange(stackParent.GetComponentsInChildren<Card>().Where(card => card != parentCard));

        if (IsCardsInOnGoingCraft(stackCards))
        {
            movedCard.transform.SetParent(null);
            return false;
        }
            

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
                // remove cards from stack if too much card per ingredients
                foreach (var ingredient in recipe.Ingredients)
                {
                    if (cardCounts.ContainsKey(ingredient.cardID) && cardCounts[ingredient.cardID] > ingredient.quantity)
                    {
                        Card cardToRemove = stackCards.Where(card => card.CardData.CardID == ingredient.cardID).First();
                        cardToRemove.transform.SetParent(null);
                        stackCards.Remove(cardToRemove);
                    }
                }
                currentCrafts.Add(new CraftInfo(stackParent, recipe, stackCards, CardUtility.GenerateUniqueID()));
                InitializeCooldownBar(stackParent, recipe.CraftingDelay, currentCrafts[currentCrafts.Count - 1].CraftID);
                return true;
            }
        }

        return false;
    }

    private void InitializeCooldownBar(Transform stackParent, float craftingDelay, int craftID)
    {
        cooldownBar = Instantiate(cooldownBarPrefab, stackParent.position, Quaternion.identity, stackParent);
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
            if (card is CardRessourceGenerator)
                continue;

            Destroy(card.gameObject);
        }

       
        // Instantiate output card at the stack's position
        craftedCard = Instantiate(craftInfo.CraftingRecipe.OutputCardPrefab, craftInfo.StackCards[0].transform.position, Quaternion.identity, craftInfo.StackParent.parent);
        currentCrafts.Remove(craftInfo);
        OnCraftComplete?.Invoke(craftInfo.CraftID, craftInfo.CraftingRecipe.OutputCardID);
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

    public void TryCancelCraft(Card card)
    {
        int craftID = GetCraftIDByCard(card);

        if (craftID == -1)
            return;

        OnCraftCancel?.Invoke(craftID);
        currentCrafts.Remove(GetCraftInfoByID(craftID));
    }

    public void StartCraftingModeTimer()
    {
        CraftingModeDurationTimer = Timer.Register(timeCraftMode, 
            onComplete: GameManager.Instance.SwitchGameMode, 
            onUpdate: timeElapsed => OnTickTimeCraftingMode?.Invoke(timeElapsed));
    }

    public int GetCraftIDByCard(Card card)
    {
        foreach (CraftInfo craftInfo in currentCrafts) 
        {
            foreach (Card cardCraft in craftInfo.StackCards)
            {
                if (cardCraft.GetInstanceID() == card.GetInstanceID())
                    return craftInfo.CraftID;
            }
        }
        return -1;
    }

    public CraftInfo GetCraftInfoByID(int cardID) => currentCrafts.First(craftInfo => craftInfo.CraftID == cardID);

    private bool IsCardsInOnGoingCraft(List<Card> stackCards)
    {
        foreach(Card card in stackCards)
        {
            foreach(CraftInfo craftInfo in currentCrafts)
            {
                if (craftInfo.StackCards.Any(stackCard => stackCard.GetInstanceID() == card.GetInstanceID()))
                {
                    Debug.Log("card is already on on going craft");
                    return true;
                }
            }
        }
        return false;
    }

    public CraftingRecipe GetRecipeByOuputCardID(CardID cardID)
    {
        CraftingRecipe recipe = recipes.FirstOrDefault(recipe => recipe.OutputCardID == cardID);
        return recipe;
    }
}