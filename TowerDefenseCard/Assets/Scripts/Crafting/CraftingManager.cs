using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

public class CraftingManager : MonoSingleton<CraftingManager>, ILoadable, ISavable
{
    [SerializeField, Range(0f, 500f)] private List<float> originalTotalTimesCraftMode;
    [SerializeField] private List<CraftingRecipe> recipes;
    [SerializeField] private GameObject cooldownBarPrefab;
    [SerializeField, Range(0f,2f)] private float cooldownBarOffset = 0.3f;

    public event Action<int> OnCraftCancel = delegate { };
    public event Action<int, CardID> OnCraftComplete = delegate { };
    public event Action OnDestroyCard = delegate { };

    [SerializeField, HideInInspector] private float timeElapsed = 0f;
    private Timer CraftingModeDurationTimer;
    public event Action OnStartCraftTimer;
    public event Action<float> OnTickTimeCraftingMode;
    public event Action OnHalfTimeCraftingMode;
    private bool hasTriggeredHalfTimeEvent = false;

    public List<float> OriginalTotalTimesCraftMode => originalTotalTimesCraftMode; 
    public float OriginaCurrentTotalTimeCraftMode => originalTotalTimesCraftMode[WaveManager.Instance.CurrentWaveIndex];
    public float TimeCraftMode => originalTotalTimesCraftMode[WaveManager.Instance.CurrentWaveIndex] - timeElapsed;
  
    private GameObject cooldownBar;
    private List<CraftInfo> currentCrafts = new List<CraftInfo>();

    private void Start()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCraftMode += GameManager_OnStartCraftMode;

        if (CardManager.HasInstance)
            CardManager.Instance.OnDiscoverArcher += GameManager_OnStartCraftMode;
    }

    private void OnDisable()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCraftMode -= GameManager_OnStartCraftMode;

        if (CardManager.HasInstance)
            CardManager.Instance.OnDiscoverArcher -= GameManager_OnStartCraftMode;
    }

    private void GameManager_OnStartCraftMode()
    {
        CardDiscoveryState cardDiscoveryStateArcher = CardManager.Instance.GetCardDiscoveryStateByCardID(CardID.ARCHER);
        if (cardDiscoveryStateArcher == null || !cardDiscoveryStateArcher.isDiscovered)
            return;

        StartCraftingModeTimer();
    }

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

        if (craftInfo == null)
        {
            Debug.LogWarning($"No CraftInfo found for craftID {craftID}.");
            return;
        }

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

        // prevent crafting attack cards if max defense cards reached
        //if (CardManager.Instance.IsMaxDefenseCardsReached && selectedOutput.Value.cardPrefab.GetComponent<CardDefense>())
        //{
        //    TryCancelCraft(craftInfo.CraftID);
        //    craftedCard = null;
        //    return;
        //}

        if (!selectedOutput.HasValue)
        {
            Debug.LogError($"Failed to select output card for recipe {craftInfo.CraftingRecipe.name}");
            TryCancelCraft(craftInfo.CraftID);
            return;
        }

        var outputCard = selectedOutput.Value;

        if (WillExceedMaxDefenseCards(craftInfo, outputCard))
        {
            TryCancelCraft(craftInfo.CraftID);
            craftedCard = null;
            return;
        }


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
            else if (craftInfo.StackCards.Any(card => card is CardRessourceGenerator) && craftInfo.CraftingRecipe.OutputCards[0].cardPrefab.GetComponent<CardRessourceGenerator>())
            {
                craftInfo.StackCards[i].OnUnstack();
            }
            else if (!craftInfo.StackCards.Any(card => card is CardExploitation || card is CardRessourceGenerator))  //!outputCard.cardPrefab.GetComponent<CardRessource>())
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

        OnStartCraftTimer?.Invoke();

        CraftingModeDurationTimer = Timer.Register(TimeCraftMode, 
            onComplete: () => {
                GameManager.Instance.SwitchGameMode();
                timeElapsed = 0;
                }, 
            onUpdate: timeElapsed =>
            {
                OnTickTimeCraftingMode?.Invoke(timeElapsed + startingTimeElapsed);
                this.timeElapsed = timeElapsed;

                if (this.timeElapsed + startingTimeElapsed >= OriginaCurrentTotalTimeCraftMode / 2f && !hasTriggeredHalfTimeEvent)
                {
                    OnHalfTimeCraftingMode?.Invoke();
                    hasTriggeredHalfTimeEvent = true;
                }
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

    private bool WillExceedMaxDefenseCards(CraftInfo craftInfo, CraftingRecipe.OutputCard outputCard)
    {
        bool isOutputDefenseCard = outputCard.cardPrefab.GetComponent<CardDefense>() != null;

        if (!isOutputDefenseCard)
            return false; 

        int defenseCardsToDestroy = 0;
        foreach (var card in craftInfo.StackCards)
        {
            bool isDefenseCard = card.GetComponent<CardDefense>() != null;
            bool willBeDestroyed = !craftInfo.CraftingRecipe.Ingredients
                .First(ingredient => ingredient.cardID == card.CardData.CardID)
                .isNotDestroyedOnCraft;

            if (isDefenseCard && willBeDestroyed)
            {
                defenseCardsToDestroy++;
            }
        }

        int netDefenseCardChange = 1 - defenseCardsToDestroy;
        int currentDefenseCards = CardManager.Instance.CurrentNumberOfDefenseCards; 

        return (currentDefenseCards + netDefenseCardChange) > CardManager.Instance.MaxCardsDefenseAllowed;
    }
    public void Load(GameSaveData gameSaveData)
    {
        this.timeElapsed = gameSaveData.craftTimeElapsed;
    }

    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.craftTimeElapsed = this.timeElapsed;
    }


#if UNITY_EDITOR
    public void EmulateCraftForEditor(CraftingRecipe recipe, CardID forcedOutputCardID)
    {
        if (recipe == null)
        {
            Debug.LogError("Recipe is null for editor emulation");
            return;
        }

        // Find the output card data
        var selectedOutput = recipe.OutputCards.FirstOrDefault(o => o.cardID == forcedOutputCardID);
  

        // Generate a unique craft ID for the emulation
        int craftID = CardUtility.GenerateUniqueID();

        // Check if we can spawn the card (same checks as in normal crafting)
        if (CardManager.Instance.IsMaxCardsReached)
        {
            Debug.LogWarning("[EMULATOR] Cannot emulate craft - max cards reached");
            return;
        }

        // Check defense card limits if applicable
        bool isOutputDefenseCard = selectedOutput.cardPrefab.GetComponent<CardDefense>() != null;
        if (isOutputDefenseCard && CardManager.Instance.CurrentNumberOfDefenseCards >= CardManager.Instance.MaxCardsDefenseAllowed)
        {
            Debug.LogWarning("[EMULATOR] Cannot emulate craft - max defense cards reached");
            return;
        }

        // Find a suitable spawn position (you might want to adjust this)
        Vector3 spawnPosition = Vector3.zero;
        if (Camera.main != null)
        {
            spawnPosition = Camera.main.transform.position + Vector3.forward * 5f;
        }

        // Instantiate the output card
        GameObject craftedCard = Instantiate(selectedOutput.cardPrefab, spawnPosition, Quaternion.identity);

        // Trigger the craft complete event
        OnCraftComplete?.Invoke(craftID, forcedOutputCardID);

        Debug.Log($"[CRAFTING EMULATOR] Successfully created {forcedOutputCardID} at position {spawnPosition}");
    }
#endif
#if UNITY_EDITOR
    public void EmulateFullCraftForEditor(CraftingRecipe recipe, CardID forcedOutputCardID)
    {
        if (recipe == null)
        {
            Debug.LogError("Recipe is null for editor emulation");
            return;
        }

        // Find the output card data
        var selectedOutput = recipe.OutputCards.FirstOrDefault(o => o.cardID == forcedOutputCardID);
     

        Debug.Log($"[FULL CRAFT EMULATOR] Starting full craft emulation for {recipe.name}");

        // Check if we have enough space for all the cards we're about to create
        int totalCardsToCreate = recipe.Ingredients.Sum(i => i.quantity);
        if (CardManager.Instance.CurrentNumberOfCards + totalCardsToCreate > CardManager.Instance.MaxCardsAllowed)
        {
            Debug.LogWarning("[FULL CRAFT EMULATOR] Cannot emulate - would exceed max card limit");
            return;
        }

        // Find suitable spawn position
        Vector3 basePosition = Vector3.zero;
        if (Camera.main != null)
        {
            basePosition = Camera.main.transform.position + Vector3.forward * 5f;
        }

        // Create all ingredient cards and collect them
        List<GameObject> createdIngredients = new List<GameObject>();
        Transform stackParent = null;

        try
        {
            for (int i = 0; i < recipe.Ingredients.Count; i++)
            {
                var ingredient = recipe.Ingredients[i];

                for (int quantity = 0; quantity < ingredient.quantity; quantity++)
                {
                    // Get the card prefab for this ingredient
                    GameObject ingredientPrefab = GetCardPrefabByCardID(ingredient.cardID);

                    if (ingredientPrefab == null)
                    {
                        Debug.LogError($"[FULL CRAFT EMULATOR] Could not find prefab for card ID: {ingredient.cardID}");
                        CleanupCreatedCards(createdIngredients);
                        return;
                    }

                    // Calculate position for stacking
                    Vector3 spawnPosition = basePosition + Vector3.up * (createdIngredients.Count * 0.1f);

                    // Instantiate the ingredient card
                    GameObject ingredientCard = Instantiate(ingredientPrefab, spawnPosition, Quaternion.identity);
                    createdIngredients.Add(ingredientCard);

                    // Set up the stack - first card is the parent
                    if (stackParent == null)
                    {
                        stackParent = ingredientCard.transform;
                    }
                    else if (quantity > 0 || i > 0) // Stack additional cards
                    {
                        ingredientCard.transform.SetParent(stackParent);
                        ingredientCard.transform.localPosition = Vector3.down * (createdIngredients.Count - 1) * 0.35f;
                    }

                    Debug.Log($"[FULL CRAFT EMULATOR] Created ingredient: {ingredient.cardID} ({quantity + 1}/{ingredient.quantity})");
                }
            }

            Debug.Log($"[FULL CRAFT EMULATOR] Created {createdIngredients.Count} ingredient cards");

            // Wait a frame to ensure all cards are properly initialized
            UnityEditor.EditorApplication.delayCall += () => {
                // Get the moved card (we'll use the last created card as the "moved" card)
                Card movedCard = createdIngredients.LastOrDefault()?.GetComponent<Card>();

                if (movedCard == null)
                {
                    Debug.LogError("[FULL CRAFT EMULATOR] Could not get Card component from created ingredients");
                    CleanupCreatedCards(createdIngredients);
                    return;
                }

                // Temporarily override the recipe's random output to force our selected output
                var originalOutputCards = recipe.OutputCards.ToList();

                // Create a temporary output list with only our selected card at 100% chance
                var tempOutput = new CraftingRecipe.OutputCard
                {
                    cardID = forcedOutputCardID,
                    cardPrefab = selectedOutput.cardPrefab,
                    dropChance = 1.0f
                };

                // Use reflection to temporarily modify the output cards
                var outputCardsField = typeof(CraftingRecipe).GetField("OutputCards",
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (outputCardsField != null)
                {
                    outputCardsField.SetValue(recipe, new CraftingRecipe.OutputCard[] { tempOutput });
                }

                // Try to craft using the normal crafting system
                //bool craftSuccess = TryCraft(stackParent, movedCard);

                // Restore original output cards
                if (outputCardsField != null)
                {
                    outputCardsField.SetValue(recipe, originalOutputCards.ToArray());
                }

                //if (craftSuccess)
                //{
                //    Debug.Log($"[FULL CRAFT EMULATOR] Successfully initiated craft! Output will be: {forcedOutputCardID}");
                //}
                //else
                //{
                //    Debug.LogError("[FULL CRAFT EMULATOR] TryCraft failed - cleaning up created cards");
                //    CleanupCreatedCards(createdIngredients);
                //}
            };
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[FULL CRAFT EMULATOR] Exception during emulation: {e.Message}");
            CleanupCreatedCards(createdIngredients);
        }
    }

    private GameObject GetCardPrefabByCardID(CardID cardID)
    {
        return CardManager.Instance.GetCardPrefabByCardID(cardID).gameObject;
    }

    private void CleanupCreatedCards(List<GameObject> cardsToCleanup)
    {
        foreach (var card in cardsToCleanup)
        {
            if (card != null)
            {
                DestroyImmediate(card);
            }
        }
        cardsToCleanup.Clear();
        Debug.Log("[FULL CRAFT EMULATOR] Cleaned up created cards");
    }
#endif

}