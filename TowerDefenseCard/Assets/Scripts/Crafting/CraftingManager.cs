using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityTimer;
using System;

public class CraftingManager : MonoSingleton<CraftingManager>, ILoadable, ISavable
{
    [SerializeField, Range(0f, 500f)] private List<float> originalTotalTimesCraftMode;
    [SerializeField] private List<CraftingRecipe> recipes;
    public List<CraftingRecipe> Recipes => recipes;
    [SerializeField] private GameObject cooldownBarPrefab;
    [SerializeField, Range(0f,2f)] private float cooldownBarOffset = 0.3f;
    [SerializeField] private Vector3 craftSpawnOffset;

    public event Action<int> OnCraftCancel = delegate { };
    public event Action<int, CardID> OnCraftComplete = delegate { };
    public event Action<CraftInfo, CardID> OnCraftCompleteWithInfo = delegate { };
    public event Action<CardID> OnDestroyCard = delegate { };

    private float startingTimeElapsed = 0f;
    public float RemainingTime => TimeCraftMode - startingTimeElapsed;
    public float TimeElapsed => timeElapsed + startingTimeElapsed;
    [SerializeField] private float timeElapsed = 0f;
    public Timer CraftingModeDurationTimer { get; private set; }
    public event Action OnStartCraftTimer;
    public event Action<float> OnTickTimeCraftingMode;
    public event Action OnHalfTimeCraftingMode;
    private bool hasTriggeredHalfTimeEvent = false;

    public List<float> OriginalTotalTimesCraftMode => originalTotalTimesCraftMode; 
    public float OriginaCurrentTotalTimeCraftMode
    {
        get
        {
            float originalTotalTimeCraftMode = WaveManager.Instance.CurrentWaveIndex >= originalTotalTimesCraftMode.Count ?
                originalTotalTimesCraftMode[originalTotalTimesCraftMode.Count - 1] : originalTotalTimesCraftMode[WaveManager.Instance.CurrentWaveIndex];

            return originalTotalTimeCraftMode * (1 + (DifficultyManager.HasInstance ? DifficultyManager.Instance.CurrentDifficultyData.TimeBetweenWavesPercentModifier / 100f: 0f));
        }
    }
        
    public float TimeCraftMode
    {
        get
        {
            float originalTotalTimeCraftMode = WaveManager.Instance.CurrentWaveIndex >= originalTotalTimesCraftMode.Count ?
                originalTotalTimesCraftMode[originalTotalTimesCraftMode.Count -1 ] : originalTotalTimesCraftMode[WaveManager.Instance.CurrentWaveIndex];

            return originalTotalTimeCraftMode * (1 + (DifficultyManager.HasInstance ? DifficultyManager.Instance.CurrentDifficultyData.TimeBetweenWavesPercentModifier / 100 : 0f)) - timeElapsed;
        }
    }

    private List<CraftInfo> currentCrafts = new List<CraftInfo>();

    public static event Action OnResetCraftingManagerEasyModeNoDefense;
    public static event Action<Vector3> OnTryToCraftButHasUpgrade;

    private void Start()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCraftMode += GameManager_OnStartCraftMode;

        if (CardManager.HasInstance)
            CardManager.Instance.OnDiscoverArcher += GameManager_OnStartCraftMode;

        if(WaveManager.HasInstance)
            Debug.Log($"time craft : {OriginaCurrentTotalTimeCraftMode}");
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

        StartCraftingModeTimer(TimeCraftMode);
    }

    public bool TryCraft(Transform stackParent, Card movedCard)
    {
        RemoveInvalidCrafts();

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
            
        Dictionary<CardID, int> cardCounts = new Dictionary<CardID, int>();
        foreach (var card in stackCards)
        {
            cardCounts[card.CardData.CardID] = cardCounts.GetValueOrDefault(card.CardData.CardID, 0) + 1;
        }

        foreach (var recipe in recipes)
        {
            if (IsRecipeMatch(recipe, cardCounts))
            {
                // remove cards from stack if too much card per ingredients
                foreach (var ingredient in recipe.Ingredients)
                {
                    if (ingredient.cardID == CardID.FOREST || ingredient.cardID == CardID.MONTAIN || ingredient.cardID == CardID.RICE_PADDY || ingredient.cardID == CardID.SEA || ingredient.cardID == CardID.FARM)
                        continue;

                    if (cardCounts.ContainsKey(ingredient.cardID) && cardCounts[ingredient.cardID] > ingredient.quantity)
                    {
                        int surplueCount = cardCounts[ingredient.cardID] - ingredient.quantity;
                        for (int i = 0; i < surplueCount; i++)
                        {
                            Card cardToRemove = stackCards.Where(card => card.CardData.CardID == ingredient.cardID).First();
                            //cardToRemove.OnUnstack();
                            stackCards.Remove(cardToRemove);
                        }
                    }
                }

                float adjustedDelay = CalculateCraftingDelay(recipe, stackCards);

                currentCrafts.Add(new CraftInfo(stackParent, recipe, stackCards, CardUtility.GenerateUniqueID()));
                InitializeCooldownBar(stackParent, adjustedDelay, currentCrafts[currentCrafts.Count - 1].CraftID);
                return true;
            }
        }

        // No recipe matched — check if one would match if we ignored upgrade cards.
        // If so, the stack is blocked by upgrades: notify the player.
        List<CardID> upgradeCardIDs = CardUtility.GetUpgradeCardIDs(stackCards);
        if (upgradeCardIDs.Count > 0)
        {
            List<Card> nonUpgradeCards = stackCards.Where(c => !(c is CardUpgrade)).ToList();
            Dictionary<CardID, int> nonUpgradeCounts = new Dictionary<CardID, int>();
            foreach (var card in nonUpgradeCards)
                nonUpgradeCounts[card.CardData.CardID] = nonUpgradeCounts.GetValueOrDefault(card.CardData.CardID, 0) + 1;

            foreach (var recipe in recipes)
            {
                if (IsRecipeMatch(recipe, nonUpgradeCounts))
                {
                    OnTryToCraftButHasUpgrade?.Invoke(stackParent.position);
                    Debug.Log("on try to craft but has upgrade");
                    break;
                }
            }
        }


        return false;
    }

    private void InitializeCooldownBar(Transform stackParent, float craftingDelay, int craftID)
    {
        GameObject cooldownBar = Instantiate(cooldownBarPrefab, stackParent.position, Quaternion.identity, stackParent);
        cooldownBar.GetComponent<Canvas>().sortingOrder = 130;
        CooldownBarUI cooldownBarUI = cooldownBar.GetComponentInChildren<CooldownBarUI>();
        //cooldownBarUI.OnCraftDelayEnd -= CooldownBarUI_OnCraftDelayEnd;
        cooldownBarUI.OnCraftDelayEnd += CooldownBarUI_OnCraftDelayEnd;
        cooldownBarUI.Init(stackParent, craftingDelay, cooldownBarOffset, craftID);
        cooldownBar.transform.position = stackParent.position + new Vector3(0, cooldownBarOffset, 0);
    }

    private void CooldownBarUI_OnCraftDelayEnd(int craftID)
    {
        CraftInfo craftInfo = currentCrafts.FirstOrDefault(currentCraft => currentCraft.CraftID == craftID);

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
            CardID cardID = craftInfo.StackCards[i].CardData.CardID;

            CardID ingredientCardID = cardID;
            if ((cardID == CardID.FARMER || cardID == CardID.CARPENTER) &&
                !craftInfo.CraftingRecipe.Ingredients.Any(ing => ing.cardID == cardID))
            {
                // This card is a FARMER or CARPENTER being used as WORKER
                ingredientCardID = CardID.WORKER;
            }

            var matchingIngredient = craftInfo.CraftingRecipe.Ingredients.FirstOrDefault(ingredient =>
                                        ingredient.cardID == ingredientCardID);

            bool shouldBeKept = matchingIngredient.isNotDestroyedOnCraft;

            if (!shouldBeKept)
            {
                craftInfo.StackCards[i].OnUnstack();

                // made for special case like barn stack like sakura sakura (sakura) (sakura !! here) bamboo bamboo (bamboo) (bamoo)
                if (craftInfo.StackCards[i].StackedCards != null && craftInfo.StackCards[i].StackedCards.Count > 0)
                {
                    craftInfo.StackCards[i].StackedCards[0].OnUnstack();
                }

                OnDestroyCard?.Invoke(craftInfo.StackCards[i].CardData.CardID);
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
        craftedCard = Instantiate(outputCard.cardPrefab, craftInfo.StackCards[0].transform.position + craftSpawnOffset, Quaternion.identity); //, craftInfo.StackParent.parent);
        currentCrafts.Remove(craftInfo);
        OnCraftComplete?.Invoke(craftInfo.CraftID, outputCard.cardID);
        OnCraftCompleteWithInfo?.Invoke(craftInfo, outputCard.cardID);
    }

    private bool IsRecipeMatch(CraftingRecipe recipe, Dictionary<CardID, int> cardCounts)
    {
        Dictionary<CardID, int> adjustedCardCounts = new Dictionary<CardID, int>(cardCounts);

        // If the recipe requires WORKER, combine FARMER and CARPENTER counts into WORKER
        bool recipeNeedsWorker = recipe.Ingredients.Any(ingredient => ingredient.cardID == CardID.WORKER);

        if (recipeNeedsWorker)
        {
            int workerCount = adjustedCardCounts.GetValueOrDefault(CardID.WORKER, 0);
            int farmerCount = adjustedCardCounts.GetValueOrDefault(CardID.FARMER, 0);
            int carpenterCount = adjustedCardCounts.GetValueOrDefault(CardID.CARPENTER, 0);

            // Combine all worker-type cards
            adjustedCardCounts[CardID.WORKER] = workerCount + farmerCount + carpenterCount;

            // Remove FARMER and CARPENTER from the adjusted counts for comparison
            if (adjustedCardCounts.ContainsKey(CardID.FARMER))
                adjustedCardCounts.Remove(CardID.FARMER);
            if (adjustedCardCounts.ContainsKey(CardID.CARPENTER))
                adjustedCardCounts.Remove(CardID.CARPENTER);
        }


        if (recipe.Ingredients.Count != adjustedCardCounts.Count)
            return false;

        foreach (var ingredient in recipe.Ingredients)
        {
            if (!adjustedCardCounts.ContainsKey(ingredient.cardID) || adjustedCardCounts[ingredient.cardID] < ingredient.quantity)
            {
                return false;
            }
        }

        if(recipe.OutputCards.Any(outputCard => CardManager.Instance.GetCardDiscoveryStateByCardID(outputCard.cardID).isLocked))
        {
            return false;
        }

        return true;
    }

    private float CalculateCraftingDelay(CraftingRecipe recipe, List<Card> stackCards)
    {
        float baseDelay = recipe.CraftingDelay;
        float totalSpeedModifier = 0f;
        int modifiersApplied = 0;

        // Pour chaque ingrédient WORKER dans la recette
        foreach (var ingredient in recipe.Ingredients)
        {
            if (ingredient.cardID == CardID.WORKER)
            {
                // Compter les cartes qui correspondent à cet ingrédient
                int carpenterCount = 0;
                int farmerCount = 0;
                int workerCount = 0;
                int cardsNeeded = ingredient.quantity;

                // Compter les cartes dans la stack
                foreach (var card in stackCards)
                {
                    CardID cardID = card.CardData.CardID;
                    if (cardID == CardID.CARPENTER)
                        carpenterCount++;
                    else if (cardID == CardID.FARMER)
                        farmerCount++;
                    else if (cardID == CardID.WORKER)
                        workerCount++;
                }

                int totalWorkerCards = carpenterCount + farmerCount + workerCount;

                if (totalWorkerCards > 0)
                {
                    // Calculer le modificateur moyen pondéré pour cet ingrédient
                    float carpenterRatio = (float)carpenterCount / totalWorkerCards;
                    float farmerRatio = (float)farmerCount / totalWorkerCards;
                    float workerRatio = (float)workerCount / totalWorkerCards;

                    float ingredientModifier =
                        (carpenterRatio * ingredient.carpenterSpeedModifier) +
                        (farmerRatio * ingredient.farmerSpeedModifier) +
                        (workerRatio * 1.0f); // WORKER normal = 1.0

                    totalSpeedModifier += ingredientModifier;
                    modifiersApplied++;
                }
            }
        }

        // Calculer la moyenne si plusieurs modificateurs
        if (modifiersApplied > 0)
        {
            totalSpeedModifier /= modifiersApplied; // +1 pour inclure le 1f initial
        }
        else
        {
            totalSpeedModifier = 1f; // Pas de modificateurs
        }

        float finalDelay = baseDelay * totalSpeedModifier;

        Debug.Log($"[CRAFT SPEED] Recipe: {recipe.name}, Base: {baseDelay}s, Modifier: {totalSpeedModifier:F2}x, Final: {finalDelay:F2}s");

        return finalDelay;
    }


    public void TryCancelCraft(Card card)
    {
        int craftID = GetCraftIDByCard(card);

        if (craftID == -1)
            return;

        CraftInfo craftInfo = GetCraftInfoByID(craftID);
        if (craftInfo == null)
            return; // already removed, nothing to cancel

        OnCraftCancel?.Invoke(craftID);
        currentCrafts.Remove(craftInfo);
    }

    public void TryCancelCraft(int craftID)
    {
        if (craftID == -1) 
            return;

        CraftInfo craftInfo = GetCraftInfoByID(craftID);
        if (craftInfo == null) 
            return; // already removed, nothing to cancel

        OnCraftCancel?.Invoke(craftID);
        currentCrafts.Remove(craftInfo);
    }

    public void StartCraftingModeTimer(float duration)
    {
        Debug.Log($"Starting crafting mode timer for duration: {duration} seconds");
        startingTimeElapsed = timeElapsed;

        OnStartCraftTimer?.Invoke();

        CraftingModeDurationTimer = Timer.Register(duration, 
            onComplete: OnCraftingModeTimerComplete, 
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

    private void OnCraftingModeTimerComplete()
    {
        if (DifficultyManager.HasInstance)
        {
            if((DifficultyManager.Instance.CurrentDifficultyData.Difficulty == GameDifficulty.EASY || 
                DifficultyManager.Instance.CurrentDifficultyData.Difficulty == GameDifficulty.ZEN )&&
               WaveManager.Instance.CurrentWaveIndex == 0 &&
               CardManager.Instance.CurrentNumberOfDefenseCards < 2)
            {
                timeElapsed = 0f;

                if (originalTotalTimesCraftMode[0] > 200f)
                    originalTotalTimesCraftMode[0] *= 0.5f;
               
                OnResetCraftingManagerEasyModeNoDefense?.Invoke();
                StartCraftingModeTimer(TimeCraftMode);
            }
            else
            {
                GameManager.Instance.SwitchGameMode();
                timeElapsed = 0f;
            }
        }
        else
        {
            GameManager.Instance.SwitchGameMode();
            timeElapsed = 0f;
        } 
    }

    public void ResetTimeElapsed() => timeElapsed = 0f;

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

    public CraftInfo GetCraftInfoByID(int cardID) => currentCrafts.FirstOrDefault(craftInfo => craftInfo.CraftID == cardID);


    public void RemoveInvalidCrafts()
    {
        for (int i = currentCrafts.Count - 1; i >= 0; i--)
        {
            if (currentCrafts[i].StackCards.Any(card => card == null))
            {
                Debug.LogWarning("Removing invalid craft due to null card in stack");
                OnCraftCancel?.Invoke(currentCrafts[i].CraftID);
                currentCrafts.RemoveAt(i);
            }
        }
    }

    private bool IsCardsInOnGoingCraft(List<Card> stackCards)
    {
        string incomingCardsSummary = string.Join(", ", stackCards.Select(c => $"{c.CardData.CardID}(id:{c.GetInstanceID()})"));
        Debug.Log($"[IsCardsInOnGoingCraft] Checking {stackCards.Count} card(s): [{incomingCardsSummary}] | Active crafts count: {currentCrafts.Count}");

        if (currentCrafts.Count == 0)
        {
            Debug.Log("[IsCardsInOnGoingCraft] No active crafts — returning false");
            return false;
        }

        // Dump all active crafts for reference
        for (int i = 0; i < currentCrafts.Count; i++)
        {
            CraftInfo ci = currentCrafts[i];
            string craftCardsSummary = string.Join(", ", ci.StackCards.Select(c => c != null ? $"{c.CardData.CardID}(id:{c.GetInstanceID()})" : "NULL"));
            Debug.Log($"[IsCardsInOnGoingCraft] Active craft [{i}] — CraftID:{ci.CraftID} | Recipe:{ci.CraftingRecipe.name} | Cards:[{craftCardsSummary}]");
        }



        foreach (Card card in stackCards)
        {
            //Debug.Log($"Checking card {card.CardData.CardID}{card.GetInstanceID()} for ongoing crafts");
            foreach (CraftInfo craftInfo in currentCrafts)
            {
                Card matchedCard = craftInfo.StackCards.FirstOrDefault(stackCard => stackCard != null && stackCard.GetInstanceID() == card.GetInstanceID());
                if (matchedCard != null)
                {
                    Debug.LogWarning($"[IsCardsInOnGoingCraft] BLOCKED — Card {card.CardData.CardID}(id:{card.GetInstanceID()}) " +
                        $"is already in craft CraftID:{craftInfo.CraftID} Recipe:{craftInfo.CraftingRecipe.name} | " +
                        $"Matched stack card: {matchedCard.CardData.CardID}(id:{matchedCard.GetInstanceID()}) | Returning TRUE");
                    return true;
                }
            }
            Debug.Log($"[IsCardsInOnGoingCraft] Card {card.CardData.CardID}(id:{card.GetInstanceID()}) — not found in any active craft");
        }
        Debug.Log("[IsCardsInOnGoingCraft] No conflicts found — returning false");
        return false;
    }

    public CraftingRecipe GetRecipeByOutputCardID(CardID cardID)
    {
        CraftingRecipe recipe = recipes.FirstOrDefault(recipe =>
             recipe.OutputCards.Any(output => output.cardID == cardID));
        return recipe;
    }

    public List<CraftingRecipe> GetRecipesByOutputCardID(CardID cardID)
    {
        List<CraftingRecipe> matchedRecipes = recipes.Where(recipe =>
             recipe.OutputCards.Any(output => output.cardID == cardID)).ToList();
        return matchedRecipes;
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

            // Mirror the same FARMER/CARPENTER ? WORKER substitution used in Craft()
            CardID ingredientCardID = card.CardData.CardID;
            if ((ingredientCardID == CardID.FARMER || ingredientCardID == CardID.CARPENTER) &&
                !craftInfo.CraftingRecipe.Ingredients.Any(ing => ing.cardID == ingredientCardID))
            {
                ingredientCardID = CardID.WORKER;
            }

            // Use FirstOrDefault to avoid the exception if no match is found
            var matchingIngredient = craftInfo.CraftingRecipe.Ingredients
                .FirstOrDefault(ingredient => ingredient.cardID == ingredientCardID);

            // If no matching ingredient found, skip this card
            if (matchingIngredient.cardID == default)
                continue;

            bool willBeDestroyed = !matchingIngredient.isNotDestroyedOnCraft;

            if (isDefenseCard && willBeDestroyed)
                defenseCardsToDestroy++;
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
        gameSaveData.craftTimeElapsed = this.timeElapsed + startingTimeElapsed;
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