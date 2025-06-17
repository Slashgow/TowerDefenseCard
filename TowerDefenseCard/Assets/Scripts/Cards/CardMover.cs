using UnityEngine;
using UnityEngine.EventSystems;

public class CardMover : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Range(0f,1f)] private float smoothTime = 0.02f;
    [SerializeField, Range(0f, 180f)] private float maxTiltAngle = 20f;
    [SerializeField, Range(0f, 1f)] private float overlapRadius = 0.5f;
    [SerializeField] private LayerMask detectionLayerMask;
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) 
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure 2D
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, overlapRadius, detectionLayerMask); 
        foreach (var hit in hits)
        {
            if (hit.gameObject == this.gameObject) 
                continue;

            if (hit.transform.IsChildOf(this.transform))
                continue;

            Card otherCard = hit.GetComponent<Card>();
           
            if (otherCard != null && otherCard.CardData.IsStackable)
            {
                otherCard.StackCount += card.StackCount;
                // Make the dragged card a child of the target card
                transform.SetParent(otherCard.transform, false);
                
                Vector3 newPos = Vector3.zero;
                newPos.y = -stackingHeight * (otherCard.transform.childCount); // Stack upwards in 2D (negative z for visibility)
                newPos.z = -0.1f * (otherCard.transform.childCount);
                transform.localPosition = newPos;

                if (CraftingManager.Instance.TryCraft(otherCard.transform.root, out GameObject craftedCard))
                {
                    return;
                }

                return;
            }
        }


        transform.SetParent(startParent, false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; 
        Gizmos.DrawWireSphere(transform.position, overlapRadius); 
    }
}
