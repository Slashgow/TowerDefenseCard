using UnityEngine;

public abstract class MeshRangeEffect : MonoBehaviour
{
    [SerializeField] private CardMover cardMover;
    [SerializeField] private bool showDuringMovement = true;
    [SerializeField] private Material rangeMaterial;
    [SerializeField] private int segments = 64;

    protected float range;
    public float Range => range;

    private GameObject rangeIndicatorObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Start()
    {
        CreateRangeIndicator();
        ShowRange(!showDuringMovement);
    }

    private void OnEnable()
    {
        if (cardMover != null && showDuringMovement)
        {
            cardMover.OnPointerDownEvent += CardMover_OnPointerDownEvent;
            cardMover.OnPointerUpEvent += CardMover_OnPointerUpEvent;
        }
    }

    private void OnDisable()
    {
        if (cardMover != null)
        {
            cardMover.OnPointerDownEvent -= CardMover_OnPointerDownEvent;
            cardMover.OnPointerUpEvent -= CardMover_OnPointerUpEvent;
        }
    }

    private void CardMover_OnPointerUpEvent() => ShowRange(false);
    private void CardMover_OnPointerDownEvent() => ShowRange(true);

    [ContextMenu("Create Range Indicator")]
    private void CreateRangeIndicator()
    {
        // Create child GameObject for the range indicator
        rangeIndicatorObject = new GameObject("RangeIndicator");
        rangeIndicatorObject.transform.SetParent(transform);
        rangeIndicatorObject.transform.localPosition = Vector3.zero;
        rangeIndicatorObject.transform.localRotation = Quaternion.identity;
        rangeIndicatorObject.transform.localScale = Vector3.one;

        // Add MeshFilter and MeshRenderer components
        meshFilter = rangeIndicatorObject.AddComponent<MeshFilter>();
        meshRenderer = rangeIndicatorObject.AddComponent<MeshRenderer>();

        // Set the material
        if (rangeMaterial != null)
            meshRenderer.material = rangeMaterial;

        // Create the circle mesh
        CreateCircleMesh();
    }

    public void ShowRange(bool show)
    {
        if (rangeIndicatorObject != null)
            rangeIndicatorObject.SetActive(show);
    }

    private void CreateCircleMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "RangeCircle";

        // Create vertices
        Vector3[] vertices = new Vector3[segments + 1];
        Vector2[] uv = new Vector2[segments + 1];
        int[] triangles = new int[segments * 3];

        // Center vertex
        vertices[0] = Vector3.zero;
        uv[0] = new Vector2(0.5f, 0.5f);

        // Circle vertices
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * range;
            float y = Mathf.Sin(angle) * range;
            vertices[i + 1] = new Vector3(x, y, 0);

            // UV coordinates for proper texture mapping
            uv[i + 1] = new Vector2(
                0.5f + Mathf.Cos(angle) * 0.5f,
                0.5f + Mathf.Sin(angle) * 0.5f
            );
        }

        // Create triangles (connecting center to edge vertices)
        for (int i = 0; i < segments; i++)
        {
            int triangleIndex = i * 3;
            triangles[triangleIndex] = 0; // Center vertex
            triangles[triangleIndex + 1] = ((i + 1) % segments) + 1;
            triangles[triangleIndex + 2] = i + 1;
          
        }

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    public void UpdateCircleMesh(float range)
    {
        this.range = range;

        if (meshFilter == null || meshFilter.mesh == null)
        {
            CreateCircleMesh();
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;

        // Update circle vertices with current range
        float angleStep = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * range;
            float y = Mathf.Sin(angle) * range;
            vertices[i + 1] = new Vector3(x, y, 0);
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
    }

    private void OnDestroy()
    {
        if (rangeIndicatorObject != null)
        {
            if (Application.isPlaying)
                Destroy(rangeIndicatorObject);
            else
                DestroyImmediate(rangeIndicatorObject);
        }
    }
}