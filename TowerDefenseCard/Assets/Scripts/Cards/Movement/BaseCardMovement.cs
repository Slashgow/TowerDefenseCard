using System;
using System.Collections;
using UnityEngine;

public abstract class BaseCardMovement : MonoBehaviour
{
    [SerializeField] protected LayerMask detectionLayerMaskCards;

    [Header("Start Movement")]
    [SerializeField, Range(0f, 3f)] private float moveStep = 0.1f;
    [SerializeField, Range(0f, 1f)] private float checkRadius = 0.5f;
    [SerializeField, Range(0, 10)] private int maxIterations = 10;
    [SerializeField, Range(0f, 2f)] private float startMoveDuration = 0.5f;

    protected Card card;

    private void Awake()
    {
        card = GetComponent<Card>();
    }

    protected virtual void Start()
    {
        if (card.transform.root.GetComponent<Card>() && card.transform.root.GetComponent<Card>() != card)
            return;

        StartCoroutine(SmoothMoveToClearSpot());
    }

    private IEnumerator SmoothMoveToClearSpot()
    {
        int iterations = 0;
        Vector3 startPosition = transform.position;
        while (iterations < maxIterations)
        {
            Vector2 moveDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
            Vector3 targetPosition = transform.position + new Vector3(moveDirection.x * moveStep, moveDirection.y * moveStep, 0f);

            float elapsedTime = 0f;
            Vector3 initialPosition = transform.position;

            while (elapsedTime < startMoveDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / startMoveDuration;
                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                yield return null; // Wait for next frame
            }

            transform.position = targetPosition; // Ensure exact target position
            iterations++;

            if(!IsOverlapping())
                yield break;
        }

        InitializeSortOrder();
    }

    private void InitializeSortOrder()
    {
        if (IsOverlapping())
        {
            CardUtility.AssignSortingOrderRecursively(card.transform, GetOverlapSortOrder());
        }
    }

    private bool IsOverlapping()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius, detectionLayerMaskCards);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != gameObject)// && !IsStacked(hit.transform))
                return true;
        }
        return false;
    }

    public int GetOverlapSortOrder()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius, detectionLayerMaskCards);
        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject == gameObject)
                 continue;

            Card otherCard = hit.GetComponent<Card>();
            return otherCard.CardSprite.sortingOrder + otherCard.transform.childCount;
        }
        return 0;
    }
}
