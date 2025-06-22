using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMover : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Range(0f,1f)] private float smoothTime = 0.02f;
    [SerializeField, Range(0f, 180f)] private float maxTiltAngle = 20f;
    [SerializeField, Range(0f, 1f)] private float overlapRadius = 0.5f;
    [SerializeField] private LayerMask detectionLayerMaskCards;
    [SerializeField] private LayerMask detectionLayerMaskReseller;
    [SerializeField][Range(0f, 1f)] private float stackingHeight = 0.1f;

    private Card card;
    private Vector2 startPosition;
    private Transform startParent;
    private bool isDragging = false;
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPos;

    void Start()
    {
        card = GetComponent<Card>();
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!card.CardData.IsStackable && card.StackCount > 1) 
            return; // Prevent dragging stacks unless allowed

        if(card.transform.parent != null && card.transform.parent.GetComponent<Card>())
            CraftingManager.Instance.CancelCraft();

        startPosition = transform.position;
        transform.SetParent(null, true);
        isDragging = true;

        CardUtility.AssignSortingOrderRecursively(card.transform, 20);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) 
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -0f; // Ensure 2D
        targetPos = mousePos;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);

        // Optional: Tilt card like Hearthstone 
        Vector3 delta = mousePos - (Vector3)startPosition;
        float tiltAngle = Mathf.Clamp(delta.x * 10f, -maxTiltAngle, maxTiltAngle); // Tilt based on movement
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        transform.rotation = Quaternion.identity; 
        HandleDrop();
    }

    private void HandleDrop()
    {
        TryResell();
        TryStackCards();
    }

    private void TryResell()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, overlapRadius, detectionLayerMaskReseller);

        if (hit == null)
            return;

        if(hit.TryGetComponent(out Reseller reseller))
            reseller.Resell(CardUtility.GetAllCards(card.gameObject));
    }

    private void TryStackCards()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapRadius, detectionLayerMaskCards);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject)
                continue;

            if (hit.transform.IsChildOf(this.transform))
                continue;

            Card otherCard = hit.GetComponent<Card>();
            Debug.Log(otherCard);
            if (otherCard.transform.childCount > 2)
                continue;

            if (otherCard != null && otherCard.CardData.IsStackable)
            {
                otherCard.StackCount += card.StackCount;
                // Make the dragged card a child of the target card
                transform.SetParent(otherCard.transform, false);

                Vector3 newPos = Vector3.zero;
                newPos.y = -stackingHeight * (otherCard.transform.childCount);
                transform.localPosition = newPos;
                CardUtility.AssignSortingOrderRecursively(card.transform, otherCard.CardSprite.sortingOrder + 1);

                if (CraftingManager.Instance.TryCraft(otherCard.transform.root, out GameObject craftedCard))
                    return;

                return;
            }
        }

        Vector3 pos2D = transform.position;
        pos2D.z = 0.0f;
        transform.position = pos2D;
        CardUtility.AssignSortingOrderRecursively(card.transform, 0);
        transform.SetParent(startParent, false);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, overlapRadius); 
    }
}
