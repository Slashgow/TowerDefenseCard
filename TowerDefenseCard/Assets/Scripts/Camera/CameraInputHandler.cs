using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInputActionAsset;
    [SerializeField] private InputActionReference dragInputActionReference;
    [SerializeField] private InputActionReference dragStartInputActionReference;
    [SerializeField] private InputActionReference ZoomInputActionReference;

    private bool isDragging = false;
    private Vector2 dragInput;
    private float zoomInput;
    private Vector3 dragOrigin;

    public bool IsDragging => isDragging;
    public Vector2 DragInput => dragInput;
    public Vector3 DragOrigin => dragOrigin;
    public float ZoomInput => zoomInput;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();

        dragStartInputActionReference.action.performed += ctx => { 
            isDragging = true; 
            dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        };
        dragStartInputActionReference.action.canceled += ctx => isDragging = false;
        dragInputActionReference.action.performed += ctx => dragInput = ctx.ReadValue<Vector2>();
        ZoomInputActionReference.action.performed += ctx => zoomInput = ctx.ReadValue<Vector2>().y;
        ZoomInputActionReference.action.canceled += ctx => zoomInput = ctx.ReadValue<Vector2>().y;
    }

    void OnEnable()
    {
        playerInputActionAsset.Enable();
    }

    void OnDisable()
    {
        playerInputActionAsset.Disable();
    }

    //void LateUpdate()
    //{
    //    zoomInput = 0f; // Reset zoom input each frame to avoid continuous zooming
    //}
}