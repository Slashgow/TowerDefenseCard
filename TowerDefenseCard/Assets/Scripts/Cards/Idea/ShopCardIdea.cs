using System;
using UnityEngine;

[Serializable]
public class ShopCardIdea
{
    [SerializeField, Range(0f, 100f)] private float dropPercentage;
    public float DropPercentage => dropPercentage;

    [SerializeField] private Card cardIdeaPrefab;
    public Card CardIdeaPrefab => cardIdeaPrefab;
}
