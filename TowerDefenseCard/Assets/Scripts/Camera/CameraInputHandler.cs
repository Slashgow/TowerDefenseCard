using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInputActionAsset;
    [SerializeField] private InputActionReference dragInputActionReference;
    [SerializeField] private InputActionReference dragStartInputActionReference;
    [SerializeField] private InputActionReference zoomInputActionReference;
    [SerializeField] private InputActionReference recenterInputActionReference;
    [SerializeField] private InputActionReference moveInputActionReference;

    private bool isDragging = false;
    private Vector2 dragInput;
    private float zoomInput;
    private Vector3 dragOrigin;
    private Vector2 moveInput;

    public bool IsDragging => isDragging;
    public Vector2 DragInput => dragInput;
    public Vector3 DragOrigin => dragOrigin;
    public float ZoomInput => zoomInput;
    public Vector2 MoveInput => moveInput;

    public event Action OnRecenterCamera;

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
        zoomInputActionReference.action.performed += ctx => zoomInput = ctx.ReadValue<Vector2>().y;
        zoomInputActionReference.action.canceled += ctx => zoomInput = ctx.ReadValue<Vector2>().y;
        recenterInputActionReference.action.performed += ctx => OnRecenterCamera?.Invoke();
        moveInputActionReference.action.performed += ctx => moveInput = ctx.ReadValue<Vector2>(); 
        moveInputActionReference.action.canceled += ctx => moveInput = Vector2.zero; 
    }

    void OnEnable()
    {
        playerInputActionAsset.Enable();
    }

    void OnDisable()
    {
        playerInputActionAsset.Disable();
    }

}