using System.Collections.Generic;
using UnityEngine;
using static SimpleDamageor;

public static class CardUtility 
{
    public static List<Card> GetAllCards(GameObject rootObject)
    {
        List<Card> cards = new List<Card>();

        if (rootObject == null) return cards;

        // Get all Card components in the root and its children
        Card[] cardComponents = rootObject.GetComponentsInChildren<Card>(includeInactive: true);
        cards.AddRange(cardComponents);

        return cards;
    }

    public static void AssignSortingOrderRecursively(Transform transform, int startSortingOrder)
    {
        SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sortingOrder = startSortingOrder;

        if (transform.TryGetComponent(out Canvas canvas))
            canvas.sortingOrder = startSortingOrder;

        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            AssignSortingOrderRecursively(child, startSortingOrder + i + 1);
        }
    }

    public static void DestroyAllCards(List<Card> cards)
    {
        for (int i = cards.Count - 1; i >= 0; i--)
        {
            GameObject.Destroy(cards[i].gameObject);
        }
    }

    public static int GenerateUniqueID()
    {
        System.DateTime now = System.DateTime.Now;
        int id = now.Year + now.Month + now.Day + now.Hour + now.Minute + now.Second + now.Millisecond;
        return id;
        //return Mathf.Abs(seed % int.MaxValue); // Start with a unique seed
    }

    public static void DestroyAllChildren(this Transform transform)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public static void AddCardAndChildrenToList(Card card, List<Card> list)
    {
        list.Add(card);

        if (card.StackedCards == null)
            return;

        foreach (Card stackedCard in card.StackedCards)
        {
            AddCardAndChildrenToList(stackedCard, list);
        }
    }

    public static List<DamageableTarget> GetDamageableTargets(Collider2D[] hits)
    {
        List<DamageableTarget> targets = new List<DamageableTarget>();
        foreach (Collider2D hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                targets.Add(new DamageableTarget(hit, damageable));
            }
        }

        return targets;
    }

}
