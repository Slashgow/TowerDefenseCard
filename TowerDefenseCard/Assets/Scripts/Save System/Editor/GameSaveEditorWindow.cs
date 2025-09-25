using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;

[Serializable]
public class GameSaveEditorWindow : EditorWindow
{
    private GameSaveData saveData = new GameSaveData();
    private Vector2 scrollPosition;
    private Vector2 quickGenScrollPosition;

    // UI State
    private bool showBasicSettings = true;
    private bool showCardStacks = false;
    private bool showQuickGeneration = true;
    private bool showAdvanced = false;

    // Quick Generation Settings
    private int quickWave = 1;
    private Dictionary<CardID, int> selectedCardsWithQuantities = new Dictionary<CardID, int>();
    private int quickPlayerHealth = 100;
    private int quickPlayerCoins = 500;
    private GameMode quickGameMode = GameMode.CRAFTING;
    private bool randomizePositions = true;
    private float spawnAreaSize = 10f;
    private Vector3 spawnCenter = Vector3.zero;

    // Card selection UI
    private CardID selectedCardToAdd = CardID.BAMBOO;
    private int quantityToAdd = 1;
    private Vector2 cardSelectionScroll;
    private bool showCardSelection = false;

    // File settings
    private string saveFolderPath = "Assets/GameSaves";
    private string fileName = "customSave.json";

    // Save templates for quick generation
    private readonly Dictionary<string, GameSaveTemplate> saveTemplates = new Dictionary<string, GameSaveTemplate>
    {
        ["Early Game"] = new GameSaveTemplate
        {
            wave = 1,
            health = 100,
            coins = 100,
            cardQuantities = new Dictionary<CardID, int> {
                {CardID.BAMBOO, 3}, {CardID.JADE, 2}, {CardID.SAKURA, 2}
            }
        },
        ["Mid Game"] = new GameSaveTemplate
        {
            wave = 5,
            health = 80,
            coins = 500,
            cardQuantities = new Dictionary<CardID, int> {
                {CardID.BAMBOO, 5}, {CardID.JADE, 3}, {CardID.ARCHER, 2}, {CardID.TORII_GATE, 1}, {CardID.MATCHA, 3}
            }
        },
        ["Late Game"] = new GameSaveTemplate
        {
            wave = 10,
            health = 60,
            coins = 1000,
            cardQuantities = new Dictionary<CardID, int> {
                {CardID.DRAGON, 2}, {CardID.PHOENIX, 2}, {CardID.HACHIMAN, 1}, {CardID.AKITA_INU, 3}, {CardID.AMETHYSTE, 5}
            }
        },
        ["Boss Fight"] = new GameSaveTemplate
        {
            wave = 15,
            health = 40,
            coins = 2000,
            cardQuantities = new Dictionary<CardID, int> {
                {CardID.AMATERASU, 1}, {CardID.SUSANOO, 1}, {CardID.YAMATA_NO_OROCHI, 1}, {CardID.DRAGON, 3}
            }
        }
    };

    // Card categories for easier selection
    private readonly Dictionary<string, List<CardID>> cardCategories = new Dictionary<string, List<CardID>>
    {
        ["🌿 Basic Resources"] = new List<CardID> { CardID.BAMBOO, CardID.JADE, CardID.SAKURA, CardID.SPIRIT_ESSENCE, CardID.AMETHYSTE, CardID.KAMI_ESSENCE, CardID.BAMBOO_PLANK, CardID.SAKURA_BRICK, CardID.STRAW },
        ["🏭 Factories"] = new List<CardID> { CardID.BAMBOO_FACTORY, CardID.SAKURA_FACTORY, CardID.JADE_FACTORY, CardID.SPIRIT_FACTORY, CardID.KAMI_FACTORY, CardID.AMETHYSTE_FACTORY, CardID.BAMBOO_PLANK_FACTORY, CardID.SAKURA_BRICK_FACTORY },
        ["🏗️ Buildings"] = new List<CardID> { CardID.TEMPLE, CardID.HOUSE, CardID.BARN, CardID.WAREHOUSE, CardID.CHEST },
        ["⚔️ Combat Units"] = new List<CardID> { CardID.ARCHER, CardID.WHITE_SNAKE, CardID.PHOENIX, CardID.RED_CROWN_CRATE, CardID.DRAGON, CardID.HACHIMAN, CardID.AKITA_INU, CardID.KOMAINU, CardID.FUJIN,
        CardID.RAIJIN, CardID.OKUNINUSHI, CardID.AMATERASU, CardID.SUSANOO},
        ["🍜 Food"] = new List<CardID> { CardID.RICE, CardID.NOODLES, CardID.LEAVES, CardID.EGG, CardID.WATER, CardID.WASABI, CardID.RAMEN, CardID.SPCIY_RAMEN, CardID.SAKE, CardID.TAMAGO_GOHAN, CardID.MATCHA },
        ["🌾 Farming"] = new List<CardID> { CardID.FOREST, CardID.RICE_PADDY, CardID.FARM, CardID.MONTAIN },
        ["🎁 Special"] = new List<CardID> { CardID.CURRENCY, CardID.WORKER },
    };

    [MenuItem("William/Game Save Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<GameSaveEditorWindow>("Game Save Editor");
        window.minSize = new Vector2(450, 700);
    }

    private void OnEnable()
    {
        // Ensure the dedicated folder exists
        if (!AssetDatabase.IsValidFolder(saveFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "GameSaves");
        }

        // Initialize with default values
        ResetToDefault();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        DrawQuickGeneration();
        DrawBasicSettings();
        DrawCardStacks();
        DrawAdvancedSettings();
        DrawFileOperations();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical("box");
        var headerStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
        GUILayout.Label("🎮 Game Save Editor", headerStyle);
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();
    }

    private void DrawQuickGeneration()
    {
        EditorGUILayout.BeginVertical("box");
        showQuickGeneration = EditorGUILayout.Foldout(showQuickGeneration, "⚡ Quick Generation", true, EditorStyles.foldoutHeader);

        if (showQuickGeneration)
        {
            EditorGUI.indentLevel++;

            // Templates section
            GUILayout.Label("📋 Templates", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            foreach (var template in saveTemplates)
            {
                if (GUILayout.Button(template.Key, GUILayout.Height(25)))
                {
                    ApplyTemplate(template.Value);
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Quick settings
            GUILayout.Label("⚙️ Quick Settings", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Wave:", GUILayout.Width(60));
            quickWave = EditorGUILayout.IntSlider(quickWave, 1, 20);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Health:", GUILayout.Width(60));
            quickPlayerHealth = EditorGUILayout.IntSlider(quickPlayerHealth, 1, 200);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Coins:", GUILayout.Width(60));
            quickPlayerCoins = EditorGUILayout.IntSlider(quickPlayerCoins, 0, 5000);
            EditorGUILayout.EndHorizontal();

            quickGameMode = (GameMode)EditorGUILayout.EnumPopup("Game Mode:", quickGameMode);

            GUILayout.Space(5);
            GUILayout.Label("🃏 Card Selection with Quantities", EditorStyles.boldLabel);

            // Show selected cards with quantities
            EditorGUILayout.BeginVertical("helpBox");
            int totalCards = selectedCardsWithQuantities.Values.Sum();
            GUILayout.Label($"Selected Cards ({selectedCardsWithQuantities.Count} types, {totalCards} total):", EditorStyles.miniLabel);

            if (selectedCardsWithQuantities.Count == 0)
            {
                EditorGUILayout.HelpBox("No cards selected. Add cards using the controls below.", MessageType.Info);
            }
            else
            {
                // Display selected cards with editable quantities
                var cardsToRemove = new List<CardID>();
                var cardsToUpdate = new Dictionary<CardID, int>();

                foreach (var kvp in selectedCardsWithQuantities)
                {
                    EditorGUILayout.BeginHorizontal();

                    GUILayout.Label($"• {kvp.Key}", GUILayout.Width(180));

                    // Editable quantity field
                    int newQuantity = EditorGUILayout.IntField(kvp.Value, GUILayout.Width(50));
                    if (newQuantity != kvp.Value)
                    {
                        if (newQuantity <= 0)
                        {
                            cardsToRemove.Add(kvp.Key);
                        }
                        else
                        {
                            cardsToUpdate[kvp.Key] = newQuantity;
                        }
                    }

                    GUILayout.Label("qty", GUILayout.Width(25));

                    if (GUILayout.Button("❌", GUILayout.Width(25)))
                    {
                        cardsToRemove.Add(kvp.Key);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                // Apply updates
                foreach (var card in cardsToRemove)
                {
                    selectedCardsWithQuantities.Remove(card);
                }
                foreach (var kvp in cardsToUpdate)
                {
                    selectedCardsWithQuantities[kvp.Key] = kvp.Value;
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.Space(5);

            // Card addition controls
            EditorGUILayout.BeginHorizontal();
            selectedCardToAdd = (CardID)EditorGUILayout.EnumPopup("Add Card:", selectedCardToAdd, GUILayout.ExpandWidth(true));

            GUILayout.Label("Qty:", GUILayout.Width(30));
            quantityToAdd = EditorGUILayout.IntField(quantityToAdd, GUILayout.Width(50));
            quantityToAdd = Mathf.Max(1, quantityToAdd); // Ensure minimum of 1

            if (GUILayout.Button("➕", GUILayout.Width(30)))
            {
                if (selectedCardsWithQuantities.ContainsKey(selectedCardToAdd))
                {
                    selectedCardsWithQuantities[selectedCardToAdd] += quantityToAdd;
                }
                else
                {
                    selectedCardsWithQuantities[selectedCardToAdd] = quantityToAdd;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Category-based selection
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("📂 Browse by Category"))
            {
                showCardSelection = !showCardSelection;
            }
            if (GUILayout.Button("🗑️ Clear All"))
            {
                if (EditorUtility.DisplayDialog("Clear Cards", "Remove all selected cards?", "Yes", "Cancel"))
                {
                    selectedCardsWithQuantities.Clear();
                }
            }
            EditorGUILayout.EndHorizontal();

            // Category browser
            if (showCardSelection)
            {
                EditorGUILayout.BeginVertical("box");
                GUILayout.Label("📂 Browse Cards by Category", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Default quantity to add:", GUILayout.Width(140));
                quantityToAdd = EditorGUILayout.IntField(quantityToAdd, GUILayout.Width(50));
                quantityToAdd = Mathf.Max(1, quantityToAdd);
                EditorGUILayout.EndHorizontal();

                cardSelectionScroll = EditorGUILayout.BeginScrollView(cardSelectionScroll, GUILayout.Height(200));

                foreach (var category in cardCategories)
                {
                    GUILayout.Label(category.Key, EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    int cardsPerRow = 2;
                    int currentRow = 0;

                    foreach (var card in category.Value)
                    {
                        bool isSelected = selectedCardsWithQuantities.ContainsKey(card);
                        string buttonText = isSelected ? $"{card} ({selectedCardsWithQuantities[card]})" : card.ToString();

                        GUI.backgroundColor = isSelected ? Color.green : Color.white;

                        if (GUILayout.Button(buttonText, GUILayout.Width(180), GUILayout.Height(20)))
                        {
                            if (isSelected)
                            {
                                selectedCardsWithQuantities[card] += quantityToAdd;
                            }
                            else
                            {
                                selectedCardsWithQuantities[card] = quantityToAdd;
                            }
                        }

                        GUI.backgroundColor = Color.white;

                        currentRow++;
                        if (currentRow >= cardsPerRow)
                        {
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            currentRow = 0;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(5);
                }

                EditorGUILayout.EndScrollView();
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);
            GUILayout.Label("📍 Positioning", EditorStyles.boldLabel);
            randomizePositions = EditorGUILayout.Toggle("Randomize Positions", randomizePositions);

            if (randomizePositions)
            {
                spawnCenter = EditorGUILayout.Vector3Field("Spawn Center", spawnCenter);
                spawnAreaSize = EditorGUILayout.Slider("Spawn Area Size", spawnAreaSize, 1f, 50f);
            }

            GUILayout.Space(10);

            // Generation buttons
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("🚀 Generate Quick Save", GUILayout.Height(30)))
            {
                GenerateQuickSave();
            }
            if (GUILayout.Button("🎲 Random Cards", GUILayout.Width(100), GUILayout.Height(30)))
            {
                GenerateRandomCardSelection();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void DrawBasicSettings()
    {
        EditorGUILayout.BeginVertical("box");
        showBasicSettings = EditorGUILayout.Foldout(showBasicSettings, "⚙️ Basic Settings", true, EditorStyles.foldoutHeader);

        if (showBasicSettings)
        {
            EditorGUI.indentLevel++;

            // Player stats
            GUILayout.Label("👤 Player Stats", EditorStyles.boldLabel);
            saveData.currentPlayerHealth = EditorGUILayout.FloatField("Health", saveData.currentPlayerHealth);
            saveData.currentPlayerCoin = EditorGUILayout.IntField("Coins", saveData.currentPlayerCoin);

            GUILayout.Space(5);

            // Game state
            GUILayout.Label("🎯 Game State", EditorStyles.boldLabel);
            saveData.gameMode = (GameMode)EditorGUILayout.EnumPopup("Game Mode", saveData.gameMode);
            saveData.currentWaveIndex = EditorGUILayout.IntField("Wave Index", saveData.currentWaveIndex);
            saveData.craftTimeElapsed = EditorGUILayout.FloatField("Craft Time", saveData.craftTimeElapsed);

            GUILayout.Space(5);

            // Card limits
            GUILayout.Label("🃏 Card Limits", EditorStyles.boldLabel);
            saveData.currentNumberOfCards = EditorGUILayout.IntField("Current Cards", saveData.currentNumberOfCards);

            GUILayout.Space(5);

            // Pack states
            GUILayout.Label("📦 Pack States", EditorStyles.boldLabel);
            saveData.isBasePackFirstTimeOpened = EditorGUILayout.Toggle("Base Pack Opened", saveData.isBasePackFirstTimeOpened);
            saveData.isDefensePackFirstTimeOpened = EditorGUILayout.Toggle("Defense Pack Opened", saveData.isDefensePackFirstTimeOpened);
            saveData.isEngineeringPackFirstTimeOpened = EditorGUILayout.Toggle("Engineering Pack Opened", saveData.isEngineeringPackFirstTimeOpened);
            saveData.isFoodPackFirstTimeOpened = EditorGUILayout.Toggle("Food Pack Opened", saveData.isFoodPackFirstTimeOpened);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void DrawCardStacks()
    {
        EditorGUILayout.BeginVertical("box");
        showCardStacks = EditorGUILayout.Foldout(showCardStacks, $"🃏 Card Stacks ({saveData.cardStacks?.Count ?? 0})", true, EditorStyles.foldoutHeader);

        if (showCardStacks)
        {
            EditorGUI.indentLevel++;

            if (saveData.cardStacks == null)
                saveData.cardStacks = new List<StackSaveData>();

            for (int i = 0; i < saveData.cardStacks.Count; i++)
            {
                EditorGUILayout.BeginVertical("helpBox");

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"🔸 Stack {i + 1} ({saveData.cardStacks[i].cards?.Count ?? 0} cards)", EditorStyles.boldLabel);

                if (GUILayout.Button("❌", GUILayout.Width(25), GUILayout.Height(20)))
                {
                    saveData.cardStacks.RemoveAt(i);
                    i--;
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    continue;
                }
                EditorGUILayout.EndHorizontal();

                StackSaveData stack = saveData.cardStacks[i];
                if (stack.cards == null)
                    stack.cards = new List<CardSaveData>();

                // Stack cards
                for (int j = 0; j < stack.cards.Count; j++)
                {
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label($"Card {j + 1}", EditorStyles.miniLabel);
                    if (GUILayout.Button("❌", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        stack.cards.RemoveAt(j);
                        j--;
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.EndVertical();
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();

                    CardSaveData card = stack.cards[j];
                    card.cardID = (CardID)EditorGUILayout.EnumPopup("Card ID", card.cardID);
                    card.position = EditorGUILayout.Vector3Field("Position", card.position);
                    card.rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", card.rotation.eulerAngles));
                    card.scale = EditorGUILayout.Vector3Field("Scale", card.scale);
                    card.stackCount = EditorGUILayout.IntField("Stack Count", card.stackCount);
                    card.currentAmountOfCurrency = EditorGUILayout.IntField("Currency", card.currentAmountOfCurrency);

                    EditorGUILayout.EndVertical();
                }

                if (GUILayout.Button("➕ Add Card to Stack"))
                {
                    var newCard = new CardSaveData();
                    if (randomizePositions)
                    {
                        newCard.position = GetRandomPosition();
                        newCard.scale = Vector3.one;
                    }
                    stack.cards.Add(newCard);
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(5);
            }

            if (GUILayout.Button("➕ Add New Stack", GUILayout.Height(25)))
            {
                saveData.cardStacks.Add(new StackSaveData());
            }

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void DrawAdvancedSettings()
    {
        EditorGUILayout.BeginVertical("box");
        showAdvanced = EditorGUILayout.Foldout(showAdvanced, "🔧 Advanced Settings", true, EditorStyles.foldoutHeader);

        if (showAdvanced)
        {
            EditorGUI.indentLevel++;

            // Wave details
            GUILayout.Label("🌊 Wave Details", EditorStyles.boldLabel);
            saveData.currentWaveEnnemyIndex = EditorGUILayout.IntField("Enemy Index", saveData.currentWaveEnnemyIndex);
            saveData.currentEnnemyCount = EditorGUILayout.IntField("Enemy Count", saveData.currentEnnemyCount);
            saveData.amountOfSpawnedEnemies = EditorGUILayout.IntField("Spawned Enemies", saveData.amountOfSpawnedEnemies);

            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }

    private void DrawFileOperations()
    {
        EditorGUILayout.BeginVertical("box");
        GUILayout.Label("💾 File Operations", EditorStyles.boldLabel);

        fileName = EditorGUILayout.TextField("File Name", fileName);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔄 Reset to Default", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Reset Save Data", "Are you sure you want to reset all data to default values?", "Yes", "Cancel"))
            {
                ResetToDefault();
            }
        }

        if (GUILayout.Button("📁 Open Save Folder", GUILayout.Height(25)))
        {
            EditorUtility.RevealInFinder(saveFolderPath);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("💾 Save to File", GUILayout.Height(30)))
        {
            SaveToFile();
        }

        if (GUILayout.Button("📂 Load from File", GUILayout.Height(30)))
        {
            LoadFromFile();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void GenerateQuickSave()
    {
        // Reset save data
        saveData = new GameSaveData();

        // Apply basic settings
        saveData.currentPlayerHealth = quickPlayerHealth;
        saveData.currentPlayerCoin = quickPlayerCoins;
        saveData.gameMode = quickGameMode;
        saveData.currentWaveIndex = quickWave;

        // Generate cards from selected dictionary
        GenerateCardsFromDictionary(selectedCardsWithQuantities);

        // Update current card count
        saveData.currentNumberOfCards = saveData.cardStacks?.Sum(s => s.cards?.Count ?? 0) ?? 0;

        int totalCards = selectedCardsWithQuantities.Values.Sum();
        Debug.Log($"Generated quick save: Wave {quickWave}, {saveData.currentNumberOfCards} cards ({totalCards} total), {quickPlayerHealth} health, {quickPlayerCoins} coins");
    }

    private void GenerateCardsFromDictionary(Dictionary<CardID, int> cardQuantities)
    {
        if (cardQuantities == null || cardQuantities.Count == 0)
        {
            Debug.LogWarning("No cards selected for generation!");
            return;
        }

        saveData.cardStacks = new List<StackSaveData>();

        foreach (var kvp in cardQuantities)
        {
            CardID cardID = kvp.Key;
            int quantity = kvp.Value;

            for (int i = 0; i < quantity; i++)
            {
                var stack = new StackSaveData();
                stack.cards = new List<CardSaveData>();

                var card = new CardSaveData
                {
                    cardID = cardID,
                    position = randomizePositions ? GetRandomPosition() : Vector3.zero,
                    rotation = Quaternion.identity,
                    scale = Vector3.one,
                    stackCount = 1,
                    currentAmountOfCurrency = 0
                };

                stack.cards.Add(card);
                saveData.cardStacks.Add(stack);
            }
        }

        var cardList = cardQuantities.Select(kvp => $"{kvp.Key}x{kvp.Value}").ToList();
        Debug.Log($"Generated cards: {string.Join(", ", cardList)}");
    }

    private void GenerateRandomCardSelection()
    {
        selectedCardsWithQuantities.Clear();

        var allCards = Enum.GetValues(typeof(CardID)).Cast<CardID>().ToList();
        // Remove system cards from random selection
        allCards.Remove(CardID.SHOP);
        allCards.Remove(CardID.PLAYER_HEALTH);

        int cardTypes = UnityEngine.Random.Range(3, 8);
        for (int i = 0; i < cardTypes && allCards.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, allCards.Count);
            CardID selectedCard = allCards[randomIndex];
            int randomQuantity = UnityEngine.Random.Range(1, 6); // 1-5 of each card

            selectedCardsWithQuantities[selectedCard] = randomQuantity;
            allCards.RemoveAt(randomIndex); // Prevent duplicates
        }

        var cardList = selectedCardsWithQuantities.Select(kvp => $"{kvp.Key}x{kvp.Value}").ToList();
        Debug.Log($"Generated random card selection: {string.Join(", ", cardList)}");
    }

    private Vector3 GetRandomPosition()
    {
        return spawnCenter + new Vector3(
            UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize),
            UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize),
            UnityEngine.Random.Range(-spawnAreaSize, spawnAreaSize)
        );
    }

    private void ApplyTemplate(GameSaveTemplate template)
    {
        quickWave = template.wave;
        quickPlayerHealth = template.health;
        quickPlayerCoins = template.coins;
        selectedCardsWithQuantities = new Dictionary<CardID, int>(template.cardQuantities);
    }

    private void ResetToDefault()
    {
        saveData = new GameSaveData
        {
            currentPlayerHealth = 100,
            currentPlayerCoin = 100,
            gameMode = GameMode.CRAFTING,
            currentWaveIndex = 0,
            cardStacks = new List<StackSaveData>(),
            questSaveDatas = new List<QuestSaveData>(),
            autoCardMovementDatas = new List<AutoCardMovementData>()
        };

        quickWave = 1;
        quickPlayerHealth = 100;
        quickPlayerCoins = 100;
        selectedCardsWithQuantities = new Dictionary<CardID, int>
        {
            {CardID.BAMBOO, 2},
            {CardID.JADE, 1},
            {CardID.SAKURA, 1}
        };
        quantityToAdd = 1;
    }

    private void SaveToFile()
    {
        try
        {
            string fullPath = Path.Combine(saveFolderPath, fileName);
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Save Successful", $"Save file created at:\n{fullPath}", "OK");
            Debug.Log($"✅ Save file generated at: {fullPath}");
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Save Failed", $"Failed to save file:\n{e.Message}", "OK");
            Debug.LogError($"❌ Failed to save: {e.Message}");
        }
    }

    private void LoadFromFile()
    {
        try
        {
            string fullPath = EditorUtility.OpenFilePanel("Load Save File", saveFolderPath, "json");
            if (!string.IsNullOrEmpty(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                saveData = JsonUtility.FromJson<GameSaveData>(json);

                EditorUtility.DisplayDialog("Load Successful", $"Loaded save from:\n{fullPath}", "OK");
                Debug.Log($"✅ Loaded save from: {fullPath}");
            }
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Load Failed", $"Failed to load file:\n{e.Message}", "OK");
            Debug.LogError($"❌ Failed to load: {e.Message}");
        }
    }
}

[Serializable]
public class GameSaveTemplate
{
    public int wave;
    public int health;
    public int coins;
    public List<CardID> cards;
    public Dictionary<CardID, int> cardQuantities;
}