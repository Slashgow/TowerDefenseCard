using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardManagerUtility))]
public class CardManagerUtilityEditor : Editor
{
    private CardManagerUtility cardManagerUtility;

    private void OnEnable()
    {
        cardManagerUtility = (CardManagerUtility)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Card Scanner", EditorStyles.boldLabel);

        // Quick scan buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Scan Prefabs"))
        {
            cardManagerUtility.ScanPrefabsForCards();
        }
        if (GUILayout.Button("Scan & Discover All"))
        {
            cardManagerUtility.ScanAndSetAllDiscovered();
        }
        EditorGUILayout.EndHorizontal();

        // Maintenance buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Invalid"))
        {
            cardManagerUtility.RemoveAllInvalidCards();
        }
        if (GUILayout.Button("Remove Duplicates"))
        {
            cardManagerUtility.RemoveDuplicateCards();
        }
        EditorGUILayout.EndHorizontal();

        // Information buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Validate Cards"))
        {
            cardManagerUtility.ValidateAllCards();
        }
        if (GUILayout.Button("Show Statistics"))
        {
            cardManagerUtility.ShowCardStatistics();
        }
        EditorGUILayout.EndHorizontal();

        // Danger zone
        EditorGUILayout.Space();
        GUI.color = Color.red;
        if (GUILayout.Button("Clear All Cards"))
        {
            if (EditorUtility.DisplayDialog("Clear All Cards",
                "Are you sure you want to clear all cards from the list? This cannot be undone.",
                "Yes", "Cancel"))
            {
                cardManagerUtility.ClearAllCardsList();
            }
        }
        GUI.color = Color.white;

        // Display current stats
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Current Status", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Total Cards: {cardManagerUtility.CardManager.AllCards.Count}");

        int discoveredCount = cardManagerUtility.CardManager.AllCards.Count(c => c.isDiscovered);
        EditorGUILayout.LabelField($"Discovered: {discoveredCount}");
        EditorGUILayout.LabelField($"Undiscovered: {cardManagerUtility.CardManager.AllCards.Count - discoveredCount}");
    }
}