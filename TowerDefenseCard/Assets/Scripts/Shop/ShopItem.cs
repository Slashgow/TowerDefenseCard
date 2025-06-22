using UnityEngine;

[System.Serializable]
public class ShopItem
{
    [SerializeField, Range(0f, 100f)] private float dropPercentage;
    public float DropPercentage => dropPercentage;

    [SerializeField] private GameObject cardPrefab;
    public GameObject CardPrefab => cardPrefab;
}
