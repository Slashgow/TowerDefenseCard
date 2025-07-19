using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Currency : Card, IEndDragHandler
{
    [SerializeField, Range(0f, 10f)] private float shopDetectionRadius = 1f;
    [SerializeField] private LayerMask shopLayerMask;

    private PoolingSystem pool;

    public void Setup(PoolingSystem pool) => this.pool = pool;

    public void OnEndDrag(PointerEventData eventData)
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, shopDetectionRadius, shopLayerMask);

        if (hit != null)
        {
            if(hit.TryGetComponent(out CardShop cardShop))
            {
                if (StackCount < cardShop.Shop.ShopCost)
                    return;

                cardShop.TryPurchaseBooster();

                var currencyChildren = GetComponentsInChildren<Currency>();

                currencyChildren[cardShop.Shop.ShopCost].transform.SetParent(null);

                for (int i = 1; i < cardShop.Shop.ShopCost; i++)
                {
                    currencyChildren[i].transform.SetParent(null);
                    pool.AddToPool(currencyChildren[i].gameObject);
                }
                pool.AddToPool(this.gameObject);
            }
        }
    }
}
