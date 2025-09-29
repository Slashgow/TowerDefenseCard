using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Booster : Card, IPointerUpHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField, Range(1, 10)] private int maxCardCount = 3;
    [SerializeField] private Vector3 spawnOffset = Vector3.down * 3;
    public int MaxCardCount => maxCardCount;

    private int remainingCards;
    public int RemainingCards => remainingCards;
    private Shop shop;
    private ShopID shopID;
    public static event Action OnDestroyBooster;
    public static event Action<CardID> OnOpenBooster;
    public UnityEvent OnOpenBoosterUnity;
    public static event Action OnOpenCardIdea;

    private bool isDragging = false;

    protected override void Awake()
    {
        base.Awake();
        if (remainingCards <= 0)
            remainingCards = maxCardCount;
    }

    protected override void Start()
    {
        base.Start();
        if(shopID != ShopID.NONE)
            shop = ShopManager.Instance.GetShopByID(shopID);
    }
    public void Initialize(Shop shop) => this.shop = shop;

    public void OnPointerUp(PointerEventData eventData)
    {
        if(isDragging)
            return;

        TrySpawnCardFromShopItemList();
    }

    public void OnEndDrag(PointerEventData eventData) => isDragging = false;
    public void OnBeginDrag(PointerEventData eventData) => isDragging = true;


    private void TrySpawnCardFromShopItemList()
    {
        if (CardManager.Instance.IsMaxCardsReached)
            return;

        if (remainingCards > 0)
        {
            if(remainingCards == maxCardCount)
            {
                if (shop.IsFirstBoosterRigged && !shop.isFirstBoosterOpened)
                    SpawnCard();

                else
                {
                    ShopCardIdea selectedShopCardIdea = SelectShopCardIdeaOrdered(shop.DropChanceBonus);
                    if (selectedShopCardIdea != null)
                        SpawnCardIdea(selectedShopCardIdea);
                    else
                        SpawnCard();
                }
                
            }
            else
                SpawnCard();

            OnOpenBoosterUnity?.Invoke();

            if (remainingCards <= 0)
            {
                OnDestroyBooster?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void SpawnCard()
    {
        ShopItem selectedItem = null;

        if (shop.IsFirstBoosterRigged && !shop.isFirstBoosterOpened)
        {
            selectedItem = shop.GetNextRiggedCard();

            if (!shop.HasMoreRiggedCards())
                shop.isFirstBoosterOpened = true;
        }
        if (selectedItem == null)
            selectedItem = SelectShopItem();
         
        
        Instantiate(selectedItem.CardPrefab, this.transform.position + spawnOffset, Quaternion.identity);

        if (selectedItem.CardPrefab.GetComponent<Currency>())
        {
            ShopManager.Instance.AddPlayerCoin(1);
        }

        remainingCards--;
        OnOpenBooster?.Invoke(selectedItem.CardPrefab.GetComponent<Card>().CardData.CardID);
    }

    private void SpawnCardIdea(ShopCardIdea selectedShopCardIdea)
    {
        GameObject cardIdeaVisualInstance = Instantiate(shop.CardIdeaVisualPrefab.gameObject, this.transform.position + spawnOffset, Quaternion.identity);
        cardIdeaVisualInstance.GetComponent<CardIdea>().Initialize(selectedShopCardIdea.CardIdeaPrefab);
        remainingCards--;
        OnOpenCardIdea?.Invoke();
    }

    private ShopItem SelectShopItem()
    {
        // Weighted random selection from the pool
        float totalWeight = shop.ShopItems.Sum(item => item.DropPercentage);

        if (totalWeight <= 0)
            totalWeight = 1f;

        float roll = UnityEngine.Random.value;
        ShopItem selectedItem = null;
        float cumulativeWeight = 0f;

        foreach (var item in shop.ShopItems)
        {
            cumulativeWeight += item.DropPercentage / totalWeight;
            if (roll <= cumulativeWeight)
            {
                selectedItem = item;
                break;
            }
        }

        if (selectedItem == null && shop.ShopItems.Count > 0)
            selectedItem = shop.ShopItems[0];

        return selectedItem;
    }

    private ShopCardIdea SelectShopCardIdea()
    {
        ShopCardIdea selectedShopIdea = null;

        var undiscoveredIdeas = shop.ShopCardIdeas
                    .Where(idea => CardManager.Instance.AllCards.Any(state => state.Card.CardData.CardID == idea.CardIdeaPrefab.CardData.CardID && !state.isDiscovered))
                    .ToList();

        if (undiscoveredIdeas.Count <= 0)
            return selectedShopIdea;

        float totalWeight = undiscoveredIdeas.Sum(item => item.DropPercentage);

        if (totalWeight <= 0)
            totalWeight = 1f;

        float roll = UnityEngine.Random.value;
        
        float cumulativeWeight = 0f;

        foreach (var item in undiscoveredIdeas)
        {
            cumulativeWeight += item.DropPercentage / totalWeight;
            if (roll <= cumulativeWeight)
            {
                selectedShopIdea = item;
                break;
            }
        }

        if (selectedShopIdea == null && undiscoveredIdeas.Count > 0)
            selectedShopIdea = shop.ShopCardIdeas[0];

        return selectedShopIdea;
    }

    private ShopCardIdea SelectShopCardIdeaOrdered(float dropChanceBonus)
    {
        foreach (var idea in shop.ShopCardIdeas)
        {
            bool isUndiscovered = CardManager.Instance.AllCards.Any(state =>
                state.Card.CardData.CardID == idea.CardIdeaPrefab.CardData.CardID && !state.isDiscovered);

            if (isUndiscovered)
            {
                float effectiveDropChance = idea.DropPercentage;
                //Debug.Log($"Effective drop chance for {idea.CardIdeaPrefab.CardData.CardID}: {effectiveDropChance}");
                if (shop.LastAttemptedCardIdea == idea && shop.LastAttemptCardIdeaFailed)
                {
                    effectiveDropChance += dropChanceBonus;
                    effectiveDropChance = Mathf.Min(effectiveDropChance, 100f); 
                }

                float roll = UnityEngine.Random.value * 100f; 
                //Debug.Log($"Rolled: {roll} for {idea.CardIdeaPrefab.CardData.CardID} with effective drop chance: {effectiveDropChance}");
                if (roll <= effectiveDropChance)
                {
                    shop.ResetLastAttemptedCardIdea();
                    return idea;
                }
                else
                {
                    shop.SetLastAttemptedCardIdea(idea);
                    return null;
                }
            }
        }

        shop.ResetLastAttemptedCardIdea();
        return null;
    }


    public void Load(BoosterSaveData boosterSaveData)
    {
        remainingCards = boosterSaveData.remainingCards;
        shopID = boosterSaveData.shopID;

    }

    public BoosterSaveData Save()
    {
        return new BoosterSaveData
        {
            remainingCards = this.remainingCards,
            shopID = this.shop.ShopID
        };
    }

   
}