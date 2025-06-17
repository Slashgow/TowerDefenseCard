using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "William/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public CardID cardID;
        public int quantity;
    }

    [SerializeField] private List<Ingredient> ingredients; 
    public List<Ingredient> Ingredients => ingredients;

    [SerializeField, Range(0f, 120f)] private float craftingDelay;
    public float CraftingDelay => craftingDelay;

    [SerializeField] private CardID outputCardID; 
    public CardID OutputCardID => outputCardID;

    [SerializeField] private GameObject outputCardPrefab; 
    public GameObject OutputCardPrefab => outputCardPrefab;
    
}