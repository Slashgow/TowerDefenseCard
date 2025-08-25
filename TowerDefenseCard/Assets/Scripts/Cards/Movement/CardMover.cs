using System;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMover : BaseCardMovement , IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool autoStackOnEnable = true;
    [SerializeField, Range(0f,1f)] private float smoothTime = 0.02f;
    [SerializeField, Range(0f, 180f)] private float maxTiltAngle = 20f;
    [SerializeField, Range(0f, 1f)] private float overlapRadius = 0.5f;
  
    [SerializeField] private LayerMask detectionLayerMaskReseller;
    [SerializeField][Range(0f, 1f)] private float stackingHeight = 0.1f;

    private Vector2 startPosition;
    private Transform startParent;
    private bool isDragging = false;
    public bool IsDragging => isDragging;
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPos;

    public event Action OnPointerDownEvent;
    public event Action OnPointerUpEvent;

    protected override void OnEnable()
    {
        if (autoStackOnEnable)
            TryStackCards();

        base.OnEnable();
    }

    protected void Start()
    {
        mainCamera = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log($"on pointer down {this.name}");
        OnPointerDownEvent?.Invoke();

        CardManager.Instance.ToggleCardsOutline(card);
        CameraMovement.Instance.IsDraggindEnable = false;

        if (!card.CardData.IsStackable && card.StackCount > 1) 
            return; // Prevent dragging stacks unless allowed

        if (card.transform.parent != null && card.transform.parent.GetComponent<Card>())
        {
            CraftingManager.Instance.TryCancelCraft(this.card);
            card.OnUnstack();
        }     

        startPosition = transform.position;
        isDragging = true;

        CardUtility.AssignSortingOrderRecursively(card.transform, 20);
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log($"on drag {this.name}");
        if (!isDragging) 
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -0f; // Ensure 2D
        targetPos = mousePos;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

        // Optional: Tilt card like Hearthstone 
        Vector3 delta = mousePos - (Vector3)startPosition;
        float tiltAngle = Mathf.Clamp(delta.x * 10f, -maxTiltAngle, maxTiltAngle); // Tilt based on movement
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);

        if(CardMovementInput.Instance.IsMagnetCardActive)
            MagnetSameTypeOfCard();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log($"on pointer up {this.name}");
        OnPointerUpEvent?.Invoke();
        isDragging = false;
        transform.rotation = Quaternion.identity; 
        HandleDrop();
        CardManager.Instance.HideAllCardsOutline();
        CameraMovement.Instance.IsDraggindEnable = true;
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

            if (otherCard.StackedCards.Count > 0)
                continue;

            if (otherCard != null && otherCard.CardData.IsStackable)
            {
                card.OnStack(otherCard);

                Vector3 newPos = Vector3.zero;
                newPos.y = -stackingHeight * (otherCard.StackedCards.Count);//.transform.childCount);
                transform.localPosition = newPos;
                CardUtility.AssignSortingOrderRecursively(card.transform, otherCard.CardSprite.sortingOrder + otherCard.transform.childCount);

                if (CraftingManager.Instance.TryCraft(otherCard.transform.root, this.card))
                    return;

                return;
            }
        }

        Vector3 pos2D = transform.position;
        pos2D.z = 0.0f;
        transform.position = pos2D;
        CardUtility.AssignSortingOrderRecursively(card.transform, 0);
        this.card.OnUnstack();
        //transform.SetParent(startParent, false);
    }


    private void MagnetSameTypeOfCard()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapRadius, detectionLayerMaskCards);

        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject)
                continue;


            if (hit.transform.IsChildOf(this.transform))
                continue;

            Card otherCard = hit.GetComponent<Card>();
            if (otherCard == null)
                continue;

            //if (otherCard.StackedCards.Count > 0)
            //    continue;

            if (otherCard.StackParent != null)
                continue;

            if (!otherCard.CardData.IsStackable)
                continue;

            if (otherCard.CardData == this.card.CardData ||
                otherCard.CardData.CardID == this.card.CardData.CardID)
            {
                Card targetCard = this.card.GetLastCardInStack();

                otherCard.OnStack(targetCard);

                Vector3 newPos = Vector3.zero;
                newPos.y = -stackingHeight * (targetCard.StackedCards.Count);
                otherCard.transform.localPosition = newPos;

                CardUtility.AssignSortingOrderRecursively(otherCard.transform,
                    targetCard.CardSprite.sortingOrder + targetCard.transform.childCount);

                if (CraftingManager.Instance.TryCraft(targetCard.transform.root, otherCard))
                    continue;
            }
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, overlapRadius); 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"on pointer enter {this.name}");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"on pointer exit {this.name}");
    }
}
