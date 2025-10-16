using UnityEngine;
using System.Collections.Generic;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "William/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public CardID cardID;
        public int quantity;
        public bool isNotDestroyedOnCraft;

        [ShowIf("cardID", CardID.WORKER), AllowNesting]
        [Range(0.1f, 2f), Tooltip("Speed multiplier when using CARPENTER (1.0 = normal, <1.0 = faster, >1.0 = slower)")]
        public float carpenterSpeedModifier;

        [ShowIf("cardID", CardID.WORKER), AllowNesting]
        [Range(0.1f, 2f), Tooltip("Speed multiplier when using FARMER (1.0 = normal, <1.0 = faster, >1.0 = slower)")]
        public float farmerSpeedModifier;
    }

    [System.Serializable]
    public struct OutputCard
    {
        public CardID cardID;
        public GameObject cardPrefab;
        [Range(0f, 100f)] public float dropChance; 
    }

    [SerializeField] private List<Ingredient> ingredients; 
    public List<Ingredient> Ingredients => ingredients;

    [SerializeField, Range(0f, 120f)] private float craftingDelay;
    public float CraftingDelay => craftingDelay;

    [SerializeField] private List<OutputCard> outputCards;
    public List<OutputCard> OutputCards => outputCards;


    //[SerializeField] private CardID outputCardID; 
    //public CardID OutputCardID => outputCardID;
    //
    //[SerializeField] private GameObject outputCardPrefab; 
    //public GameObject OutputCardPrefab => outputCardPrefab;

    public OutputCard? GetRandomOutputCard()
    {
        if (outputCards == null || outputCards.Count == 0)
            return null;

        // Calculate total weight for normalization
        float totalWeight = 0f;
        foreach (var output in outputCards)
        {
            totalWeight += output.dropChance;
        }

        if (totalWeight <= 0f)
            return null;

        // Generate random value
        float randomValue = Random.Range(0f, totalWeight);

        // Select card based on weighted random
        float currentWeight = 0f;
        foreach (var output in outputCards)
        {
            currentWeight += output.dropChance;
            if (randomValue <= currentWeight)
            {
                return output;
            }
        }

        // Fallback to first card if something goes wrong
        return outputCards[0];
    }

    public bool IsValidOutputConfiguration()
    {
        if (outputCards == null || outputCards.Count == 0)
            return false;

        foreach (var output in outputCards)
        {
            if (output.cardPrefab == null || output.dropChance <= 0f)
                return false;
        }

        return true;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (outputCards != null)
        {
            for (int i = 0; i < outputCards.Count; i++)
            {
                var output = outputCards[i];
                output.dropChance = Mathf.Clamp(output.dropChance, 0f, 100f);
                outputCards[i] = output;
            }
        }
    }
#endif

}