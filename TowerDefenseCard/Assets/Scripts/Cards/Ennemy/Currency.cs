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
                var currencyChildren = GetComponentsInChildren<Currency>();
                int availableCurrency = StackCount;
                int requiredCost = cardShop.Shop.CurrentShopCost;

                if (availableCurrency < requiredCost)
                {
                    cardShop.Shop.CurrentShopCost = requiredCost - availableCurrency;

                    for (int i = 0; i < availableCurrency; i++)
                    {
                        currencyChildren[i].OnUnstack();
                        pool.AddToPool(currencyChildren[i].gameObject);
                    }

                    ShopManager.Instance.RemovePlayerCoin(availableCurrency);
                    cardShop.UpdateCardShopData();
                }
                else
                {
                    cardShop.TryPurchaseBooster();

                    if (availableCurrency > requiredCost)
                    {
                        currencyChildren[requiredCost].OnUnstack();
                    }

                    for (int i = 0; i < requiredCost; i++)
                    {
                        currencyChildren[i].OnUnstack();
                        pool.AddToPool(currencyChildren[i].gameObject);
                    }
                }

              
                //cardShop.TryPurchaseBooster();
            }
        }
    }
}
