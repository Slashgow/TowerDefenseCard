using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Shop : IUnlockable
{
    [SerializeField, Range(0, 50)] private int shopCost;
    public int ShopCost => shopCost;

    [SerializeField] private bool isFirstBoosterRigged = false;
    public bool IsFirstBoosterRigged => isFirstBoosterRigged;

    [SerializeField] private List<ShopItem> riggedBoosterCards = new List<ShopItem>();
    public List<ShopItem> RiggedBoosterCards => riggedBoosterCards;

    private int riggedCardIndex = 0;
    public int RiggedCardIndex => riggedCardIndex;
    public bool isFirstBoosterOpened { get; set; } = false;

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

    private bool isUnlocked = false;
    public event Action OnUnlock;
    public static event Action OnAnyShopUnlock;
    public bool IsUnlocked => isUnlocked;
    public string LockUIDescription => "????";

    public void ResetShopCost() => currentShopCost = shopCost;

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
    public ShopItem GetNextRiggedCard()
    {
        if (riggedBoosterCards == null || riggedBoosterCards.Count == 0 || riggedCardIndex >= riggedBoosterCards.Count)
            return null;

        ShopItem card = riggedBoosterCards[riggedCardIndex];
        riggedCardIndex++;
        return card;
    }

    public bool HasMoreRiggedCards() => riggedBoosterCards != null && riggedCardIndex < riggedBoosterCards.Count;
    public void ResetRiggedCardIndex() => riggedCardIndex = 0;
    public void Unlock()
    {
        isUnlocked = true;
        OnAnyShopUnlock?.Invoke();
        OnUnlock?.Invoke();
    }
}
