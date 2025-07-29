using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CardManagerUtility : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Card Scanner Settings")]
    [SerializeField] private string prefabScanPath = "Assets/Prefabs";
    [SerializeField] private bool includeSubfolders = true;
    [SerializeField] private bool clearListBeforeScanning = false;
    [SerializeField] private bool autoSetDiscoveredOnScan = false;
    [SerializeField] private List<string> excludedFolders = new List<string>();
    [SerializeField] private CardManager cardManager;

    public CardManager CardManager => cardManager;


    [ContextMenu("Remove All Invalid Cards")]
    public void RemoveAllInvalidCards()
    {
        int originalCount = cardManager.allCards.Count;
        List<string> removedReasons = new List<string>();

        cardManager.allCards.RemoveAll(cardState =>
        {
            // Check for null CardDiscoveryState
            if (cardState == null)
            {
                removedReasons.Add("Null CardDiscoveryState");
                return true;
            }

            // Check for null Card
            if (cardState.Card == null)
            {
                removedReasons.Add("Null Card component");
                return true;
            }

            // Check for null CardData
            if (cardState.Card.CardData == null)
            {
                removedReasons.Add($"Card '{cardState.Card.name}' has null CardData");
                return true;
            }

            // Check if the prefab still exists in the project
            string prefabPath = AssetDatabase.GetAssetPath(cardState.Card);
            if (string.IsNullOrEmpty(prefabPath))
            {
                removedReasons.Add($"Card '{cardState.Card.name}' prefab no longer exists");
                return true;
            }

            // Card is valid
            return false;
        });

        int removedCount = originalCount - cardManager.allCards.Count;

        if (removedCount > 0)
        {
            EditorUtility.SetDirty(this);
            Debug.Log($"Removed {removedCount} invalid cards from the list");

            // Group and count reasons
            var reasonGroups = removedReasons.GroupBy(r => r).ToList();
            foreach (var group in reasonGroups.Take(10))
            {
                Debug.Log($"- {group.Key}: {group.Count()} cards");
            }

            if (reasonGroups.Count > 10)
            {
                Debug.Log($"... and {reasonGroups.Count - 10} more types of issues");
            }
        }
        else
        {
            Debug.Log("No invalid cards found");
        }
    }

    private bool IsFolderExcluded(string folderPath)
    {
        foreach (string excludedFolder in excludedFolders)
        {
            if (string.IsNullOrEmpty(excludedFolder)) continue;

            string normalizedExcluded = excludedFolder.Replace('\\', '/');
            string normalizedFolder = folderPath.Replace('\\', '/');

            // Check if the folder path starts with the excluded path
            if (normalizedFolder.StartsWith(normalizedExcluded, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    [ContextMenu("Scan Prefabs Folder for Cards")]
    public void ScanPrefabsForCards()
    {
        ScanPrefabsForCards(prefabScanPath, includeSubfolders, clearListBeforeScanning);
    }

    [ContextMenu("Scan and Set All Discovered")]
    public void ScanAndSetAllDiscovered()
    {
        ScanPrefabsForCards(prefabScanPath, includeSubfolders, clearListBeforeScanning);
        cardManager.CheckDiscovered();
    }

    [ContextMenu("Clear All Cards List")]
    public void ClearAllCardsList()
    {
        cardManager.allCards.Clear();
        EditorUtility.SetDirty(this);
        Debug.Log("All cards list cleared");
    }

    [ContextMenu("Remove Null Cards")]
    public void RemoveNullCards()
    {
        int originalCount = cardManager.allCards.Count;
        cardManager.allCards.RemoveAll(cardState => cardState?.Card == null);
        int removedCount = originalCount - cardManager.allCards.Count;

        if (removedCount > 0)
        {
            EditorUtility.SetDirty(this);
            Debug.Log($"Removed {removedCount} null card entries");
        }
        else
        {
            Debug.Log("No null cards found");
        }
    }

    [ContextMenu("Remove Duplicate Cards")]
    public void RemoveDuplicateCards()
    {
        int originalCount = cardManager.allCards.Count;

        // Group by CardID and keep only the first occurrence
        var uniqueCards = cardManager.allCards
            .Where(cardState => cardState?.Card?.CardData != null)
            .GroupBy(cardState => cardState.Card.CardData.CardID)
            .Select(group => group.First())
            .ToList();

        // Add back any cards that don't have CardData (to preserve them)
        var cardsWithoutData = cardManager.allCards
            .Where(cardState => cardState?.Card != null && cardState.Card.CardData == null)
            .ToList();

        uniqueCards.AddRange(cardsWithoutData);

        cardManager.allCards = uniqueCards;
        int removedCount = originalCount - cardManager.allCards.Count;

        if (removedCount > 0)
        {
            EditorUtility.SetDirty(this);
            Debug.Log($"Removed {removedCount} duplicate card entries");
        }
        else
        {
            Debug.Log("No duplicate cards found");
        }
    }

    [ContextMenu("Validate All Cards")]
    public void ValidateAllCards()
    {
        int validCards = 0;
        int invalidCards = 0;
        List<string> issues = new List<string>();

        foreach (var cardState in cardManager.allCards)
        {
            if (cardState == null)
            {
                invalidCards++;
                issues.Add("Null CardDiscoveryState found");
                continue;
            }

            if (cardState.Card == null)
            {
                invalidCards++;
                issues.Add("CardDiscoveryState with null Card found");
                continue;
            }

            if (cardState.Card.CardData == null)
            {
                invalidCards++;
                issues.Add($"Card '{cardState.Card.name}' has null CardData");
                continue;
            }

            validCards++;
        }

        Debug.Log($"Validation complete: {validCards} valid cards, {invalidCards} invalid cards");

        if (issues.Count > 0)
        {
            Debug.LogWarning("Issues found:\n" + string.Join("\n", issues.Take(10)));
            if (issues.Count > 10)
            {
                Debug.LogWarning($"... and {issues.Count - 10} more issues");
            }
        }
    }

    public void ScanPrefabsForCards(string folderPath, bool includeSubfolders = true, bool clearList = false)
    {
        if (clearList)
        {
            cardManager.allCards.Clear();
        }

        // Ensure the path starts with "Assets/"
        if (!folderPath.StartsWith("Assets/"))
        {
            folderPath = "Assets/" + folderPath;
        }

        // Check if folder exists
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError($"Folder does not exist: {folderPath}");
            return;
        }

        // Find all prefab files
        string[] prefabGUIDs;
        if (includeSubfolders)
        {
            prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
        }
        else
        {
            // For top directory only, filter manually
            string[] allGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });
            prefabGUIDs = allGUIDs.Where(guid =>
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string directoryPath = Path.GetDirectoryName(path).Replace('\\', '/');
                return directoryPath == folderPath;
            }).ToArray();
        }

        int addedCount = 0;
        int skippedCount = 0;
        int excludedCount = 0;
        List<string> addedCards = new List<string>();
        List<string> skippedCards = new List<string>();
        List<string> excludedFoldersList = new List<string>();

        foreach (string guid in prefabGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            folderPath = Path.GetDirectoryName(assetPath).Replace('\\', '/');

            // Check if this prefab is in an excluded folder
            if (IsFolderExcluded(folderPath))
            {
                excludedCount++;
                if (!excludedFoldersList.Contains(folderPath))
                {
                    excludedFoldersList.Add(folderPath);
                }
                continue;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

            if (prefab != null)
            {
                Card cardComponent = prefab.GetComponent<Card>();

                if (cardComponent != null)
                {
                    // Check if card already exists in the list
                    bool alreadyExists = cardManager.allCards.Any(cardState =>
                        cardState?.Card != null &&
                        cardState.Card == cardComponent);

                    // Also check by CardID if CardData exists
                    if (!alreadyExists && cardComponent.CardData != null)
                    {
                        alreadyExists = cardManager.allCards.Any(cardState =>
                            cardState?.Card?.CardData != null &&
                            cardState.Card.CardData.CardID == cardComponent.CardData.CardID);
                    }

                    if (!alreadyExists)
                    {
                        CardDiscoveryState newCardState = new CardDiscoveryState();

                        // Use reflection to set the private card field
                        var cardField = typeof(CardDiscoveryState).GetField("card",
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        cardField?.SetValue(newCardState, cardComponent);

                        newCardState.isDiscovered = autoSetDiscoveredOnScan;
                        newCardState.isClickedAfterNotification = false;

                        cardManager.allCards.Add(newCardState);
                        addedCount++;
                        addedCards.Add(cardComponent.name);
                    }
                    else
                    {
                        skippedCount++;
                        skippedCards.Add(cardComponent.name);
                    }
                }
            }
        }

        // Mark the object as dirty so changes are saved
        EditorUtility.SetDirty(this);

        Debug.Log($"Card scan complete!");
        Debug.Log($"Added {addedCount} new cards, skipped {skippedCount} existing cards, excluded {excludedCount} cards from excluded folders");
        Debug.Log($"Total cards in list: {cardManager.allCards.Count}");

        if (excludedCount > 0)
        {
            Debug.Log($"Excluded folders: {string.Join(", ", excludedFoldersList.Take(5))}");
            if (excludedFoldersList.Count > 5)
            {
                Debug.Log($"... and {excludedFoldersList.Count - 5} more excluded folders");
            }
        }

        if (addedCount > 0 && addedCards.Count <= 10)
        {
            Debug.Log("Added cards: " + string.Join(", ", addedCards));
        }
        else if (addedCount > 10)
        {
            Debug.Log("Added cards: " + string.Join(", ", addedCards.Take(10)) + $" ... and {addedCount - 10} more");
        }

        if (skippedCount > 0 && skippedCards.Count <= 5)
        {
            Debug.Log("Skipped existing cards: " + string.Join(", ", skippedCards));
        }
        else if (skippedCount > 5)
        {
            Debug.Log($"Skipped {skippedCount} existing cards");
        }
    }

    // Method to scan specific subfolders
    public void ScanSpecificFolders(string[] folderPaths)
    {
        foreach (string folderPath in folderPaths)
        {
            Debug.Log($"Scanning folder: {folderPath}");
            ScanPrefabsForCards(folderPath, true, false);
        }
    }

    // Method to get statistics about the current card list
    [ContextMenu("Show Card Statistics")]
    public void ShowCardStatistics()
    {
        int totalCards = cardManager.allCards.Count;
        int discoveredCards = cardManager.allCards.Count(c => c.isDiscovered);
        int undiscoveredCards = totalCards - discoveredCards;
        int cardsWithNotifications = cardManager.allCards.Count(c => c.isClickedAfterNotification);
        int nullCards = cardManager.allCards.Count(c => c?.Card == null);
        int cardsWithoutData = cardManager.allCards.Count(c => c?.Card != null && c.Card.CardData == null);

        Debug.Log("=== Card Statistics ===");
        Debug.Log($"Total Cards: {totalCards}");
        Debug.Log($"Discovered Cards: {discoveredCards}");
        Debug.Log($"Undiscovered Cards: {undiscoveredCards}");
        Debug.Log($"Cards with Notifications: {cardsWithNotifications}");
        Debug.Log($"Null Cards: {nullCards}");
        Debug.Log($"Cards without CardData: {cardsWithoutData}");

        if (totalCards > 0)
        {
            float discoveryPercentage = (float)discoveredCards / totalCards * 100f;
            Debug.Log($"Discovery Percentage: {discoveryPercentage:F1}%");
        }
    }
#endif
}
