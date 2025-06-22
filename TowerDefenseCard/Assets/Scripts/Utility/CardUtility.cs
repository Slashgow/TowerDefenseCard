using System.Collections.Generic;
using UnityEngine;

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
}
