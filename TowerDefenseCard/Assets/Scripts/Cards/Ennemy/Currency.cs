using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Currency : Card, IEndDragHandler
{
    [SerializeField, Range(0f, 10f)] private float shopDetectionRadius = 1f;
    [SerializeField] private LayerMask shopLayerMask;

    private PoolingSystem pool;
    public PoolingSystem Pool => pool;

    protected override void Start()
    {
        base.Start();
        if(pool == null)
            pool = Reseller.Instance.CurrencyPool;
    }

    public void Setup(PoolingSystem pool) => this.pool = pool;

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, shopDetectionRadius, shopLayerMask);

        if (hit != null)
        {
            if(hit.TryGetComponent(out CardShop cardShop))
            {
                if(!cardShop.Shop.IsUnlocked)
                    return;

                // BUG : currencycount different stackcount
                var currencyChildren = GetComponentsInChildren<Currency>();
                int availableCurrency = currencyChildren.Length; //StackCount;
                int requiredCost = cardShop.Shop.CurrentShopCost;

                if(StackCount != currencyChildren.Length)
                {
                    Debug.LogWarning($"Currency StackCount {StackCount} different from children count {currencyChildren.Length}");
                }
                Debug.Log($"Try Purchase Booster {availableCurrency} / {requiredCost} || currency children {currencyChildren.Length}");

                if (availableCurrency < requiredCost)
                {
                    if(CardManager.Instance.TotalCostCardsOnBoard < ShopManager.Instance.MinimumShopCost) // -availableCurrency
                        return;

                    cardShop.Shop.CurrentShopCost = requiredCost - availableCurrency;

                    for (int i = 0; i < availableCurrency; i++)
                    {
                        //if(currencyChildren.Length <= i)
                        //    break;

                        currencyChildren[i].OnUnstack();
                        pool.AddToPool(currencyChildren[i].gameObject);
                    }

                    ShopManager.Instance.RemovePlayerCoin(availableCurrency);
                    ShopManager.OnPurchasePartiallyBooster();
                    cardShop.UpdateCardShopData();
                }
                else
                {
                    //cardShop.TryPurchaseBooster(false);

                    if (availableCurrency > requiredCost)
                    {
                        currencyChildren[requiredCost].OnUnstack();
                    }

                    for (int i = 0; i < requiredCost; i++)
                    {
                        //if(currencyChildren.Length <= i)
                        //    break;

                        currencyChildren[i].OnUnstack();
                        pool.AddToPool(currencyChildren[i].gameObject);
                    }

                    cardShop.TryPurchaseBooster(false);

                }


                //cardShop.TryPurchaseBooster();
            }
        }
    }
}
