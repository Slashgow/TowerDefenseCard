using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

public class CraftingManager : MonoSingleton<CraftingManager>, ILoadable, ISavable
{
    [SerializeField, Range(0f, 500f)] private float originalTimeCraftMode;
    [SerializeField] private List<CraftingRecipe> recipes;
    [SerializeField] private GameObject cooldownBarPrefab;
    [SerializeField, Range(0f,2f)] private float cooldownBarOffset = 0.3f;

    public event Action<int> OnCraftCancel = delegate { };
    public event Action<int, CardID> OnCraftComplete = delegate { };
    public event Action OnDestroyCard = delegate { };

    [SerializeField, HideInInspector] private float timeElapsed = 0f;
    private Timer CraftingModeDurationTimer;
    public event Action<float> OnTickTimeCraftingMode;

    public float OrginalTimeCraftMode => originalTimeCraftMode; 
    public float TimeCraftMode => originalTimeCraftMode - timeElapsed;
  
    private GameObject cooldownBar;
    private List<CraftInfo> currentCrafts = new List<CraftInfo>();

    private void Start()
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
            movedCard.OnUnstack();
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
                        int surplueCount = cardCounts[ingredient.cardID] - ingredient.quantity;
                        for (int i = 0; i < surplueCount; i++)
                        {
                            Card cardToRemove = stackCards.Where(card => card.CardData.CardID == ingredient.cardID).First();
                            cardToRemove.OnUnstack();
                            stackCards.Remove(cardToRemove);
                        }
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
        craftedCard = null;

        if (CardManager.Instance.IsMaxCardsReached && IsAdditionalCardAfterCraft(craftInfo))
        {
            TryCancelCraft(craftInfo.CraftID);
            craftedCard = null;
            return;
        }

        CraftingRecipe.OutputCard? selectedOutput = craftInfo.CraftingRecipe.GetRandomOutputCard();

        if (!selectedOutput.HasValue)
        {
            Debug.LogError($"Failed to select output card for recipe {craftInfo.CraftingRecipe.name}");
            TryCancelCraft(craftInfo.CraftID);
            return;
        }

        var outputCard = selectedOutput.Value;


        for (int i = craftInfo.StackCards.Count - 1; i >= 0; i--)
        {
            bool shouldBeKept = craftInfo.CraftingRecipe.Ingredients.First(ingredient => 
                                        ingredient.cardID == craftInfo.StackCards[i].CardData.CardID).isNotDestroyedOnCraft;
            if (!shouldBeKept)
            {
                craftInfo.StackCards[i].OnUnstack();
                OnDestroyCard?.Invoke();
                Destroy(craftInfo.StackCards[i].gameObject);
            }
            else if(!craftInfo.StackCards.Any(card => card is CardExploitation || card is CardRessourceGenerator))  //!outputCard.cardPrefab.GetComponent<CardRessource>())
            {
                craftInfo.StackCards[i].OnUnstack();
            }
        }
        // Instantiate output card at the stack's position
        craftedCard = Instantiate(outputCard.cardPrefab, craftInfo.StackCards[0].transform.position, Quaternion.identity, craftInfo.StackParent.parent);
        currentCrafts.Remove(craftInfo);
        OnCraftComplete?.Invoke(craftInfo.CraftID, outputCard.cardID);
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

    public void TryCancelCraft(int craftID)
    {
        if (craftID == -1)
            return;

        OnCraftCancel?.Invoke(craftID);
        currentCrafts.Remove(GetCraftInfoByID(craftID));
    }

    public void StartCraftingModeTimer()
    {
        float startingTimeElapsed = timeElapsed;
        CraftingModeDurationTimer = Timer.Register(TimeCraftMode, 
            onComplete: () => {
                GameManager.Instance.SwitchGameMode();
                timeElapsed = 0;
                }, 
            onUpdate: timeElapsed => {
                OnTickTimeCraftingMode?.Invoke(timeElapsed + startingTimeElapsed);
                this.timeElapsed = timeElapsed;
                });
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
        CraftingRecipe recipe = recipes.FirstOrDefault(recipe =>
             recipe.OutputCards.Any(output => output.cardID == cardID));
        return recipe;
    }

    public List<(CardID cardID, float chance)> GetPossibleOutputs(CraftingRecipe recipe)
    {
        var outputs = new List<(CardID, float)>();
        if (recipe?.OutputCards != null)
        {
            foreach (var output in recipe.OutputCards)
            {
                outputs.Add((output.cardID, output.dropChance));
            }
        }
        return outputs;
    }


    private bool IsAdditionalCardAfterCraft(CraftInfo craftInfo)
    {
        int numberOfDestroyedCard = craftInfo.CraftingRecipe.Ingredients.Sum(ingredient => {
                        return ingredient.isNotDestroyedOnCraft ? 0 : ingredient.quantity;
                        });
        int numberOfRemainingCards = craftInfo.CraftingRecipe.Ingredients.Sum(ingredient => {
            return ingredient.isNotDestroyedOnCraft ? ingredient.quantity : 0;
        }) + 1; 

        Debug.Log($"Crafting Recipe: {craftInfo.CraftingRecipe.name}, Number of Remaining Cards: {numberOfRemainingCards}, Number of Destroyed Cards: {numberOfDestroyedCard}");
        return numberOfRemainingCards > numberOfDestroyedCard;
    }
    public void Load(GameSaveData gameSaveData)
    {
        this.timeElapsed = gameSaveData.craftTimeElapsed;
    }

    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.craftTimeElapsed = this.timeElapsed;
    }
}