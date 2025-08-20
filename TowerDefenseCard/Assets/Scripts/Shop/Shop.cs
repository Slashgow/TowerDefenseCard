using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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

    [SerializeField, Range(5f, 50f)] private float dropChanceBonus = 10f;
    public float DropChanceBonus => dropChanceBonus;

    [SerializeField] private List<ShopCardIdea> shopCardIdeas = new List<ShopCardIdea>();
    public List<ShopCardIdea> ShopCardIdeas => shopCardIdeas;

    [SerializeField] private CardIdea cardIdeaVisualPrefab;
    public CardIdea CardIdeaVisualPrefab => cardIdeaVisualPrefab;

    public ShopCardIdea LastAttemptedCardIdea { get; private set; } = null;
    public bool LastAttemptCardIdeaFailed { get; private set; } = false;

    private int currentShopCost;
    public int CurrentShopCost
    {
        get
        {
            if (currentShopCost <= 0)
                currentShopCost = shopCost;

            return currentShopCost;
        }
        set
        {
            currentShopCost = Mathf.Max(0, value); 
        }
    }


    public void ResetShopCost()
    {
        currentShopCost = shopCost;
    }

    public void SetLastAttemptedCardIdea(ShopCardIdea cardIdea)
    {
        LastAttemptedCardIdea = cardIdea;
        LastAttemptCardIdeaFailed = true;
    }

    public void ResetLastAttemptedCardIdea()
    {
        LastAttemptedCardIdea = null;
        LastAttemptCardIdeaFailed = false;
    }
}
