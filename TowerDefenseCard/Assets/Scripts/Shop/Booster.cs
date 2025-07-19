using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using UnityEngine.Events;

public class Booster : Card, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField, Range(1, 10)] private int maxCardCount = 3; 
    public int MaxCardCount => maxCardCount;

    private int remainingCards;
    public int RemainingCards => remainingCards;
    private Shop shop;
    public static event Action<CardID> OnOpenBooster;
    public UnityEvent OnOpenBoosterUnity;
    public static event Action OnOpenCardIdea;

    private void Awake() => remainingCards = maxCardCount;

    public void Initialize(Shop shop) => this.shop = shop;

    public void OnPointerUp(PointerEventData eventData)
    {
        TrySpawnCardFromShopItemList();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //TrySpawnCardFromShopItemList();
    }

    private void TrySpawnCardFromShopItemList()
    {
        if (CardManager.Instance.IsMaxCardsReached)
            return;

        if (remainingCards > 0)
        {
            if(remainingCards == maxCardCount)
            {
                ShopCardIdea selectedShopCardIdea = SelectShopCardIdea();
                Debug.Log($"Shop card idea : {selectedShopCardIdea}");
                if (selectedShopCardIdea != null)
                    SpawnCardIdea(selectedShopCardIdea);
                else
                    SpawnCard();
            }
            else
                SpawnCard();

            OnOpenBoosterUnity?.Invoke();

            if (remainingCards <= 0)
            {
                Destroy(gameObject);
                Debug.Log("Booster exhausted and destroyed!");
            }
        }
    }

    private void SpawnCard()
    {
        ShopItem selectedItem = SelectShopItem();

        Instantiate(selectedItem.CardPrefab, this.transform.position, Quaternion.identity);
        remainingCards--;
        Debug.Log($"{selectedItem.CardPrefab.GetComponent<Card>().CardData.CardName} spawned from booster! {remainingCards} cards left.");
        OnOpenBooster?.Invoke(selectedItem.CardPrefab.GetComponent<Card>().CardData.CardID);
    }

    private void SpawnCardIdea(ShopCardIdea selectedShopCardIdea)
    {
        GameObject cardIdeaVisualInstance = Instantiate(shop.CardIdeaVisualPrefab.gameObject, this.transform.position, Quaternion.identity);
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
}