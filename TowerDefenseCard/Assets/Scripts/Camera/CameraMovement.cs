using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoSingleton<CameraMovement>
{
    [SerializeField, Range(0f,10f)] private float moveSpeed = 5f; 
    [SerializeField, Range(0f,10f)] private float zoomSpeed = 2f; 
    [SerializeField,Range(0f,10f)] private float minZoom = 2f; 
    [SerializeField, Range(0f,10f)] private float maxZoom = 10f; 
    [SerializeField,Range(0f,0.5f)] private float smoothTime = 0.1f; 
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -5f); 
    [SerializeField] private Vector2 maxBounds = new Vector2(10f, 5f); 

    private Camera cam;
    private Vector3 targetPosition;
    private float targetZoom;
    private Vector3 velocity = Vector3.zero;
    private CameraInputHandler inputHandler;
    private float zoomVelocity;

    void Start()
    {
        cam = GetComponent<Camera>();
        inputHandler = GetComponent<CameraInputHandler>();
        targetPosition = transform.position;
        targetZoom = cam.orthographicSize;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameState.PLAY)
            return;

        HandleDragging();
        HandleZooming();
        SmoothMovement();
        SmoothZooming();
    }

    private void HandleDragging()
    {
        if (!inputHandler.IsDragging)
            return;

        Vector3 difference = inputHandler.DragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        targetPosition += difference * moveSpeed * Time.deltaTime;
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
        cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, targetZoom, ref zoomVelocity, smoothTime);

    }

    private void SmoothMovement()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}