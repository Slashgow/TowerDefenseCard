using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardMover : BaseCardMovement , IPointerDownHandler, IDragHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private bool autoStackOnEnable = true;
    [SerializeField, Range(0f,1f)] private float smoothTime = 0.02f;
    [SerializeField, Range(0f, 180f)] private float maxTiltAngle = 20f;
    [SerializeField, Range(0f,2f)] private float returnDuration = 0.2f;
    [SerializeField, Range(0f, 1f)] protected float overlapRadius = 0.5f;
  
    [SerializeField] private LayerMask detectionLayerMaskReseller;
    [SerializeField][Range(0f, 1f)] protected float stackingHeight = 0.1f;

    public Vector3 TargetStackPosition => new Vector3(0f, -stackingHeight, 0f);

    [Header("Lag Effect")]
    [SerializeField] protected ParentFollower parentFollower;

    private Vector2 startPosition;
    private Transform startParent;
    private bool isDragging = false;
    public bool IsDragging => isDragging;
    protected Camera mainCamera;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetPos;
    private Vector3 dragOffset;

    public static event Action OnStartDragCard;
    public static event Action OnEndDragCard;
    public static event Action OnHoverEnterCard;
    public static event Action OnHoverExitCard;
    public static event Action OnGrabCard;
    public static event Action OnReleaseCard;
    public event Action OnPointerDownEvent;
    public event Action OnPointerUpEvent;

    private Tween resetTiltLerp;

    protected override void OnEnable()
    {
        //Debug.Log($"on enable card mover : {this.card.CardData.CardID} {this.GetInstanceID()}");

        if (autoStackOnEnable)
            TryStackCards();

        base.OnEnable();
    }

    protected void Start()
    {
        mainCamera = Camera.main;

        if (parentFollower != null)
            parentFollower.enabled = false;
    }


    private void OnDestroy()
    {
        resetTiltLerp?.Kill();
    }

    public override void OnEndStartMove()
    {
        base.OnEndStartMove();
        TryStackCards();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != 0)
            return;

        //Debug.Log($"on pointer down {this.name}");
        OnPointerDownEvent?.Invoke();
        OnGrabCard?.Invoke();

        CardManager.Instance.ToggleCardsOutline(card);
        CameraMovement.Instance.IsDraggindEnable = false;

        if (!card.CardData.IsStackable && card.StackCount > 1) 
            return; // Prevent dragging stacks unless allowed

        if (card.transform.parent != null && card.transform.parent.GetComponent<Card>())
        {
            CraftingManager.Instance.TryCancelCraft(this.card);
            card.OnUnstack();

            if (parentFollower != null)
                parentFollower.enabled = false;
        }     

        startPosition = transform.position;
        isDragging = true;

        // NEW: Calculate the offset between mouse position and card position
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        dragOffset = transform.position - mousePos;

        resetTiltLerp?.Kill();

        CardUtility.AssignSortingOrderRecursively(card.transform, 20);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != 0)
            return;

        //Debug.Log($"on drag {this.name}");
        if (!isDragging) 
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -0f; // Ensure 2D
        targetPos = mousePos + dragOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

        // Optional: Tilt card like Hearthstone 
        Vector3 delta = mousePos - (Vector3)startPosition;
        float tiltAngle = Mathf.Clamp(delta.x * 10f, -maxTiltAngle, maxTiltAngle); // Tilt based on movement
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);

        if (CardMovementInput.Instance.IsMagnetCardActive)
            MagnetSameTypeOfCard();
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != 0)
            return;

        //Debug.Log($"on pointer up {this.name}");
        OnPointerUpEvent?.Invoke();
        OnReleaseCard?.Invoke();
        isDragging = false;

        resetTiltLerp?.Kill();
        resetTiltLerp = transform.DORotateQuaternion(Quaternion.identity, returnDuration).SetEase(Ease.OutQuad).SetUpdate(true);

        //transform.rotation = Quaternion.identity;

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

    protected virtual void TryStackCards()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapRadius, detectionLayerMaskCards);
        if (!this.card.CardData.IsStackable)
            hits = null;

        if(hits != null)
        {
            foreach (var hit in hits)
            {
                if (hit.gameObject == this.gameObject)
                    continue;

                if (hit.transform.IsChildOf(this.transform))
                    continue;

                //Also skip if we are already a child of this card
                if (this.transform.IsChildOf(hit.transform))
                    continue;

                Card otherCard = hit.GetComponent<Card>();

                if (otherCard.StackedCards.Count > 0)
                {
                    // double check if stack cards contain null references and clean them
                    if (otherCard.TryClearStackCards())
                    {
                        if (otherCard.StackedCards.Count > 0)
                        {
                            //continue;
                            Card targetCard = otherCard.GetLastCardInStack();
                            TryManualStack(targetCard);
                            return;
                        }
                            
                    }
                    else
                    {
                        //continue;
                        Card targetCard = otherCard.GetLastCardInStack();
                        TryManualStack(targetCard);
                        return;
                    }
                        
                }

                if (otherCard != null && otherCard.CardData.IsStackable)
                {
                    card.OnStack(otherCard);

                    Vector3 newPos = Vector3.zero;
                    newPos.y = -stackingHeight * (otherCard.StackedCards.Count);//.transform.childCount);
                    transform.localPosition = newPos;

                    // Ensure the card has a ParentFollower and set its target offset
                    //ParentFollower follower = card.GetComponent<ParentFollower>();
                    //if (follower == null)
                    //{
                    //    follower = card.gameObject.AddComponent<ParentFollower>();
                    //}
                    //follower.enabled = false;
                   


                    CardUtility.AssignSortingOrderRecursively(card.transform, otherCard.CardSprite.sortingOrder + otherCard.transform.childCount);

                    if (CraftingManager.Instance.TryCraft(otherCard.transform.root, this.card))
                        return;

                    return;
                }
            }
        }
        

        Vector3 pos2D = transform.position;
        pos2D.z = 0.0f;
        transform.position = pos2D;
        CardUtility.AssignSortingOrderRecursively(card.transform, 3);
        this.card.OnUnstack();
        // Disable ParentFollower when not stacked
        if (parentFollower != null)
        {
            parentFollower.enabled = false;
        }
        //transform.SetParent(startParent, false);
    }

    public void TryManualStack(Card targetCard)
    {
        if (targetCard != null && targetCard.CardData.IsStackable)
        {
            card.OnStack(targetCard);

            Vector3 newPos = Vector3.zero;
            newPos.y = -stackingHeight * (targetCard.StackedCards.Count);//.transform.childCount);
            transform.localPosition = newPos;

            CardUtility.AssignSortingOrderRecursively(card.transform, targetCard.CardSprite.sortingOrder + targetCard.transform.childCount);

            if (CraftingManager.Instance.TryCraft(targetCard.transform.root, this.card))
                return;

            return;
        }
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

                // Ensure the card has a ParentFollower and set its target offset
                ParentFollower follower = otherCard.GetComponent<ParentFollower>();
                if (follower == null)
                {
                    follower = otherCard.gameObject.AddComponent<ParentFollower>();
                }
                follower.enabled = true; // Enable during magnet stacking

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
        OnHoverEnterCard?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnHoverExitCard?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != 0)
            return;

        OnStartDragCard?.Invoke();

        List<Card> cards = CardUtility.GetAllCards(card.gameObject);

        for (int i = 1; i < cards.Count; i++)
        {
            var stackedCard = cards[i];

            if (stackedCard is CardUpgrade && card is CardDefense)
                continue;

            if (stackedCard != null)
            {
                ParentFollower follower = stackedCard.GetComponent<ParentFollower>();
                if (follower == null)
                {
                    follower = stackedCard.gameObject.AddComponent<ParentFollower>();
                }
                follower.enabled = true;
                follower.CancelDisableSchedule();// Enable during drag
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != 0)
            return;

        OnEndDragCard?.Invoke();

        List<Card> cards = CardUtility.GetAllCards(card.gameObject);

        for (int i = 1; i < cards.Count; i++)
        {
            var stackedCard = cards[i];
            if (stackedCard != null)
            {
                ParentFollower follower = stackedCard.GetComponent<ParentFollower>();
                if (follower != null)
                {
                    //follower.enabled = true; // Keep enabled briefly

                    if(follower.isActiveAndEnabled)
                        follower.ScheduleDisable(); // Schedule disable after duration
                }
            }
        }
    }
}
