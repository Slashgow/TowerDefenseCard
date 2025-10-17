using DG.Tweening;
using UnityEngine;
using UnityTimer;

public class RandomMovement : BaseCardMovement
{
    [Header("Movement Settings")]
    [SerializeField] private bool startMovementOnStart = true;
    [SerializeField, Range(0f,20f)] private float moveSpeed = 2f;
    [SerializeField, Range(0f,10f)] private float movementRadius = 3f;
    [SerializeField, Range(0f,20f)] private float minPauseDuration = 0.5f;
    [SerializeField, Range(0f, 20f)] private float maxPauseDuration = 2f;
    [SerializeField, Range(0f, 20f)] private float minMoveDuration = 1f;
    [SerializeField, Range(0f, 20f)] private float maxMoveDuration = 3f;

    [Header("Tilt Settings")]
    [SerializeField, Range(0f, 45f)] private float tiltAmount = 15f;
    [SerializeField, Range(0f, 5f)] private float tiltSpeed = 1f;

    [Header("Stop Rotation Settings")]
    [SerializeField, Range(-360f, 360f)] private float stopRotationAngle = 360f;
    [SerializeField, Range(0f, 2f)] private float stopRotationDuration = 0.5f;
    [SerializeField] private Ease stopRotationEase = Ease.OutQuad;
    [SerializeField] private RotateMode stopRotateMode = RotateMode.LocalAxisAdd;
    [SerializeField] private RotateMode stopRotateModeTilt = RotateMode.Fast;

    private Vector3 originPosition;
    private Vector3 targetPosition;
    private Vector3 currentVelocity;
    private Vector3 originalRotation;

    private bool isMoving = false;
    private bool isPaused = false;
    private bool canTilt = true;

    private float tiltTimer = 0f;
    private float moveTimer = 0f;
    private float currentMoveDuration = 0f;

    private Timer pauseTimer;
    private Sequence stopSequence;

    protected override void Awake()
    {
        base.Awake();
        originalRotation = Vector3.zero;
        originPosition = transform.position;
    }

    private void Start()
    {
        if (startMovementOnStart)
        {
            StartMoving();
        }
    }

    private void Update()
    {
        if (!isMoving || isPaused)
            return;

        MoveToTarget();
        UpdateTilt();

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    public void StartMoving()
    {
        if (isMoving)
            return;

        isMoving = true;
        isPaused = false;
        originPosition = transform.position;
        originalRotation = Vector3.zero;

        PickNewTarget();
    }

    public void StopMoving()
    {
        isMoving = false;
        isPaused = false;

        CancelTimers();
    }

    private void MoveToTarget()
    {
        moveTimer += Time.deltaTime;

        float t = moveTimer / currentMoveDuration;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, currentMoveDuration * (1f - t));

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f || moveTimer >= currentMoveDuration)
        {
            DoStopRotation();
        }
    }

    private void DoStopRotation()
    {
        isPaused = true;

        if (stopSequence != null)
        {
            stopSequence.Kill();
            stopSequence = null;
        }
        
        Debug.Log("Do Stop Rotation");
        stopSequence = DOTween.Sequence();
        stopSequence.OnComplete(() => StartPause());

        stopSequence.Append(transform.DORotate(new Vector3(stopRotationAngle, 0f, 0f), stopRotationDuration, stopRotateMode)
            .SetEase(stopRotationEase).SetLoops(1, LoopType.Yoyo));

        stopSequence.Append(transform.DORotate(Vector3.zero, 0.2f, stopRotateModeTilt)
            .SetEase(Ease.OutQuad));
        
    }

    private void PickNewTarget()
    {
        Debug.Log("Pick New Target");
        Vector2 randomCircle = Random.insideUnitCircle * movementRadius;
        targetPosition = originPosition + new Vector3(randomCircle.x, randomCircle.y, 0f);

        moveTimer = 0f;
        currentMoveDuration = Random.Range(minMoveDuration, maxMoveDuration);
        currentVelocity = Vector3.zero;

        isPaused = false;
    }

    private void StartPause()
    {
        Debug.Log("Start Pause");
        isPaused = true;
        float pauseDuration = Random.Range(minPauseDuration, maxPauseDuration);

        pauseTimer = Timer.Register(pauseDuration, () =>
        {
            if (isMoving)
                PickNewTarget();
        });
    }

    private void UpdateTilt()
    {
        if (!canTilt || tiltSpeed <= 0f || tiltAmount <= 0f)
            return;

        tiltTimer += Time.deltaTime * tiltSpeed;

        float tiltValue = Mathf.Sin(tiltTimer * 2f * Mathf.PI) * tiltAmount;

        Vector3 currentRotation = originalRotation;
        currentRotation.z = originalRotation.z + tiltValue;
        currentRotation.x = 0f;
        currentRotation.y = 0f;
        transform.eulerAngles = currentRotation;
    }

    private void CancelTimers()
    {
        if (pauseTimer != null)
        {
            pauseTimer.Cancel();
            pauseTimer = null;
        }
    }

    private void OnDestroy()
    {
        CancelTimers();

        if (stopSequence != null)
        {
            stopSequence.Kill();
            stopSequence = null;
        }
    }

    private void OnDisable()
    {
        if (isMoving)
        {
            isMoving = false;
            isPaused = false;
            CancelTimers();

            if (stopSequence != null)
            {
                stopSequence.Kill();
                stopSequence = null;
            }

            if (transform != null)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
    }
}
