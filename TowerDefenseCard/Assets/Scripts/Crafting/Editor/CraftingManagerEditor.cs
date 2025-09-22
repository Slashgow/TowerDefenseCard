using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(CraftingManager))]
public class CraftingManagerEditor : Editor
{
    private CraftingRecipe selectedRecipe;
    private CardID selectedOutputCardID = CardID.BAMBOO;
    private int selectedRecipeIndex = 0;
    private Vector2 scrollPosition;
    private bool showCraftingEmulator = true;
    private bool showCurrentCrafts = true;
    private bool showRecipeDetails = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CraftingManager craftingManager = (CraftingManager)target;

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Crafting emulation is only available in Play Mode", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Crafting Manager Editor Tools", EditorStyles.boldLabel);

        // Crafting Emulator Section
        showCraftingEmulator = EditorGUILayout.Foldout(showCraftingEmulator, "Crafting Emulator", true);
        if (showCraftingEmulator)
        {
            DrawCraftingEmulator(craftingManager);
        }

        EditorGUILayout.Space(5);

        // Current Crafts Section
        showCurrentCrafts = EditorGUILayout.Foldout(showCurrentCrafts, "Current Crafts", true);
        if (showCurrentCrafts)
        {
            DrawCurrentCrafts(craftingManager);
        }

        EditorGUILayout.Space(5);

        // Recipe Details Section
        showRecipeDetails = EditorGUILayout.Foldout(showRecipeDetails, "Recipe Details", true);
        if (showRecipeDetails)
        {
            DrawRecipeDetails(craftingManager);
        }
    }

    private void DrawCraftingEmulator(CraftingManager craftingManager)
    {
        EditorGUILayout.BeginVertical("box");

        // Get recipes through reflection since the field is private
        var recipesField = typeof(CraftingManager).GetField("recipes",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var recipes = recipesField?.GetValue(craftingManager) as List<CraftingRecipe>;

        if (recipes == null || recipes.Count == 0)
        {
            EditorGUILayout.HelpBox("No recipes found in CraftingManager", MessageType.Warning);
            EditorGUILayout.EndVertical();
            return;
        }

        // Recipe Selection
        EditorGUILayout.LabelField("Select Recipe to Emulate:", EditorStyles.miniBoldLabel);
        string[] recipeNames = recipes.Select(r => r.name).ToArray();
        selectedRecipeIndex = EditorGUILayout.Popup("Recipe", selectedRecipeIndex, recipeNames);

        if (selectedRecipeIndex >= 0 && selectedRecipeIndex < recipes.Count)
        {
            selectedRecipe = recipes[selectedRecipeIndex];
        }

        if (selectedRecipe != null)
        {
            EditorGUILayout.Space(5);

            // Show recipe ingredients
            EditorGUILayout.LabelField("Required Ingredients:", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;
            foreach (var ingredient in selectedRecipe.Ingredients)
            {
                EditorGUILayout.LabelField($"• {ingredient.quantity}x {ingredient.cardID}");
            }
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(5);

            // Output Card Selection
            EditorGUILayout.LabelField("Choose Output Card:", EditorStyles.miniBoldLabel);
            var possibleOutputs = craftingManager.GetPossibleOutputs(selectedRecipe);

            if (possibleOutputs.Count > 0)
            {
                string[] outputOptions = possibleOutputs.Select(o => $"{o.cardID} ({o.chance:P1})").ToArray();
                int currentOutputIndex = possibleOutputs.FindIndex(o => o.cardID == selectedOutputCardID);
                if (currentOutputIndex == -1) currentOutputIndex = 0;

                int newOutputIndex = EditorGUILayout.Popup("Output Card", currentOutputIndex, outputOptions);
                selectedOutputCardID = possibleOutputs[newOutputIndex].cardID;
            }

            EditorGUILayout.Space(10);

            // Emulate Craft Buttons
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Emulate Full Craft", GUILayout.Height(30)))
            {
                EmulateFullCraft(craftingManager);
            }

            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("Direct Instantiate", GUILayout.Height(30)))
            {
                EmulateCraft(craftingManager);
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(2);
            EditorGUILayout.LabelField("• Full Craft: Creates ingredients and uses TryCraft", EditorStyles.miniLabel);
            EditorGUILayout.LabelField("• Direct Instantiate: Creates output card directly", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawCurrentCrafts(CraftingManager craftingManager)
    {
        EditorGUILayout.BeginVertical("box");

        // Get current crafts through reflection
        var currentCraftsField = typeof(CraftingManager).GetField("currentCrafts",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var currentCrafts = currentCraftsField?.GetValue(craftingManager) as System.Collections.IList;

        if (currentCrafts == null || currentCrafts.Count == 0)
        {
            EditorGUILayout.LabelField("No active crafts", EditorStyles.centeredGreyMiniLabel);
        }
        else
        {
            EditorGUILayout.LabelField($"Active Crafts: {currentCrafts.Count}", EditorStyles.miniBoldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.MaxHeight(150));

            for (int i = 0; i < currentCrafts.Count; i++)
            {
                var craftInfo = currentCrafts[i];
                var craftIDProperty = craftInfo.GetType().GetProperty("CraftID");
                var recipeProperty = craftInfo.GetType().GetProperty("CraftingRecipe");

                if (craftIDProperty != null && recipeProperty != null)
                {
                    int craftID = (int)craftIDProperty.GetValue(craftInfo);
                    var recipe = recipeProperty.GetValue(craftInfo) as CraftingRecipe;

                    EditorGUILayout.BeginHorizontal("box");
                    EditorGUILayout.LabelField($"ID: {craftID} | Recipe: {recipe?.name}");

                    GUI.backgroundColor = Color.red;
                    if (GUILayout.Button("Cancel", GUILayout.Width(60)))
                    {
                        craftingManager.TryCancelCraft(craftID);
                    }
                    GUI.backgroundColor = Color.white;

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawRecipeDetails(CraftingManager craftingManager)
    {
        EditorGUILayout.BeginVertical("box");

        var recipesField = typeof(CraftingManager).GetField("recipes",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var recipes = recipesField?.GetValue(craftingManager) as List<CraftingRecipe>;

        if (recipes != null && recipes.Count > 0)
        {
            EditorGUILayout.LabelField($"Total Recipes: {recipes.Count}", EditorStyles.miniBoldLabel);

            foreach (var recipe in recipes)
            {
                EditorGUILayout.BeginVertical("helpbox");
                EditorGUILayout.LabelField(recipe.name, EditorStyles.boldLabel);

                EditorGUILayout.LabelField("Ingredients:");
                EditorGUI.indentLevel++;
                foreach (var ingredient in recipe.Ingredients)
                {
                    string destroyText = ingredient.isNotDestroyedOnCraft ? " (Not Destroyed)" : " (Destroyed)";
                    EditorGUILayout.LabelField($"• {ingredient.quantity}x {ingredient.cardID}{destroyText}");
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Possible Outputs:");
                EditorGUI.indentLevel++;
                var outputs = craftingManager.GetPossibleOutputs(recipe);
                foreach (var output in outputs)
                {
                    EditorGUILayout.LabelField($"• {output.cardID} ({output.chance:P1} chance)");
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }
        }
        else
        {
            EditorGUILayout.LabelField("No recipes available", EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.EndVertical();
    }

    private void EmulateFullCraft(CraftingManager craftingManager)
    {
        if (selectedRecipe == null)
        {
            Debug.LogWarning("No recipe selected for emulation");
            return;
        }

        Debug.Log($"[CRAFTING EMULATOR] Emulating full craft process for recipe '{selectedRecipe.name}'");

        // Check if the selected output exists in the recipe
        var selectedOutput = selectedRecipe.OutputCards.FirstOrDefault(o => o.cardID == selectedOutputCardID);

    

        // Call the full emulation method
        var emulateMethod = typeof(CraftingManager).GetMethod("EmulateFullCraftForEditor",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (emulateMethod != null)
        {
            emulateMethod.Invoke(craftingManager, new object[] { selectedRecipe, selectedOutputCardID });
        }
        else
        {
            Debug.LogError("EmulateFullCraftForEditor method not found in CraftingManager. Please add the method shown in the instructions.");
        }
    }

    private void EmulateCraft(CraftingManager craftingManager)
    {
        if (selectedRecipe == null)
        {
            Debug.LogWarning("No recipe selected for emulation");
            return;
        }

        Debug.Log($"[CRAFTING EMULATOR] Emulating craft of recipe '{selectedRecipe.name}' with output '{selectedOutputCardID}'");

        // Check if the selected output exists in the recipe
        var selectedOutput = selectedRecipe.OutputCards.FirstOrDefault(o => o.cardID == selectedOutputCardID);



        // Call the emulation method that you'll need to add to CraftingManager
        var emulateMethod = typeof(CraftingManager).GetMethod("EmulateCraftForEditor",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        if (emulateMethod != null)
        {
            emulateMethod.Invoke(craftingManager, new object[] { selectedRecipe, selectedOutputCardID });
        }
        else
        {
            Debug.LogError("EmulateCraftForEditor method not found in CraftingManager. Please add the method shown in the instructions.");
        }
    }
}