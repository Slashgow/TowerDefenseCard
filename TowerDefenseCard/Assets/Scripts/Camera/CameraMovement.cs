using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoSingleton<CameraMovement>
{
    [Header("Start Combat Settings")]
    [SerializeField, Range(0f, 3f)] private float timeToDezoom = 1f;
    [SerializeField, Range(0f, 20f)] private float zoomOutSize = 10f;
    [SerializeField, Range(0f, 3f)] private float timeToMoveToFirstPath = 1f;

    [Header("Camera Movement Settings")]
    [SerializeField, Range(0f,10f)] private float dragMoveSpeed = 5f;
    [SerializeField, Range(0f, 50f)] private float keyMoveSpeed = 5f;
    [SerializeField, Range(0f,10f)] private float zoomSpeed = 2f; 
    [SerializeField,Range(0f,10f)] private float minZoom = 2f; 
    [SerializeField, Range(0f,20f)] private float maxZoom = 10f; 
    [SerializeField,Range(0f,0.5f)] private float smoothTime = 0.1f; 
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f); 
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f);
    [SerializeField, Range(0f, 2f)] private float recenterDuration = 0.5f;
    [SerializeField] private Ease recenterEasing;

    private Camera cam;
    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;
    private CameraInputHandler inputHandler;
    private float zoomVelocity;
    private Vector3 originPosition;
    private bool isRecentering;

    public bool IsDraggindEnable { get; set; }
    public bool IsZoomingEnable { get; set; }
    public bool IsMovingWithWASD { get; set; }

    protected override void Awake()
    {
        base.Awake();
        cam = GetComponent<Camera>();
        inputHandler = GetComponent<CameraInputHandler>();
        originPosition = transform.position;
        IsDraggindEnable = true;
        IsZoomingEnable = true;
        IsMovingWithWASD = true;
    }

    void Start()
    {
        inputHandler.OnRecenterCamera += InputHandler_OnRecenterCamera;
        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;

        GameManager.Instance.OnStartCombatMode += OnStartCombat;
    }

    private void OnDestroy()
    {
        inputHandler.OnRecenterCamera -= InputHandler_OnRecenterCamera;

        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCombatMode -= OnStartCombat;    
    }

    private void OnStartCombat()
    {
        isRecentering = true;
        Vector3 startWavePosition = WaveManager.Instance.CurrentWaveFirstPathStartPosition;
        startWavePosition.z = transform.position.z;
      
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(this.transform.DOMove(startWavePosition, timeToMoveToFirstPath)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() => {
                targetPosition = transform.position;
            }));

        sequence.Append(this.cam.DOOrthoSize(zoomOutSize, timeToDezoom)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true)
            .OnComplete(() => {
                targetZoom = zoomOutSize;
                isRecentering = false;
            }));
    }


    public void StopAllMovement()
    {
        IsDraggindEnable = false;
        IsZoomingEnable = false;
        IsMovingWithWASD = false;
    }

    public void ResumeAllMovement()
    {
        IsDraggindEnable = true;
        IsZoomingEnable = true;
        IsMovingWithWASD = true;
    }

    private void InputHandler_OnRecenterCamera()
    {
        isRecentering = true;
        this.transform.DOMove(originPosition, recenterDuration).SetEase(recenterEasing).SetUpdate(true).OnComplete(() => {
            isRecentering = false;
            targetPosition = transform.position;
        });
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.PLAY)
            return;

        if (isRecentering)
            return;

        if(IsDraggindEnable)
            HandleDragging();
        
        if(IsZoomingEnable)
            HandleZooming();

        if(IsMovingWithWASD)
            HandleWASDMovement();

        SmoothMovement();
        SmoothZooming();
    }
    private void HandleWASDMovement()
    {
        Vector2 moveInput = inputHandler.MoveInput;
        if (moveInput.magnitude > 0)
        {
            Vector3 moveDirection = new Vector3(moveInput.x, moveInput.y, 0) * keyMoveSpeed * Time.unscaledDeltaTime;
            targetPosition += moveDirection;
            targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
        }
    }

    private void HandleDragging()
    {
        if (!inputHandler.IsDragging)
            return;

        Vector3 difference = inputHandler.DragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        targetPosition += difference * dragMoveSpeed * Time.unscaledDeltaTime;
        targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
    }

    private void HandleZooming()
    {
        if (inputHandler.ZoomInput == 0f)
            return;

        targetZoom -= inputHandler.ZoomInput * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    private void SmoothZooming()
    {
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);

    }

    private void SmoothMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
    }
}