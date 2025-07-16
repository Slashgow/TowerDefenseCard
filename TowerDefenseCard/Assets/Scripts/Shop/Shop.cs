using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Shop
{
    [SerializeField, Range(0, 50)] private int shopCost;
    public int ShopCost => shopCost;

    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();
    public List<ShopItem> ShopItems => shopItems;

    [SerializeField] private Transform spawnPoint;
    public Transform SpawnPoint => spawnPoint;

    [SerializeField] private Booster booster;
    public Booster Booster => booster;

    [SerializeField] private List<ShopCardIdea> shopCardIdeas = new List<ShopCardIdea>();
    public List<ShopCardIdea> ShopCardIdeas => shopCardIdeas;

    [SerializeField] private CardIdea cardIdeaVisualPrefab;
    public CardIdea CardIdeaVisualPrefab => cardIdeaVisualPrefab;
}
