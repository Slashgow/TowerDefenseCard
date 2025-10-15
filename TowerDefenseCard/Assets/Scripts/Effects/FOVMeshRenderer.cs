using UnityEngine;

public class FOVMeshRenderer : MonoBehaviour
{
    [Header("FOV Reference")]
    [SerializeField] private FOVType fovType = FOVType.Fearor;

    [Header("FOV Mesh Visualization")]
    [SerializeField] private Material fovMaterial;
    [SerializeField] private int fovResolution = 30;
    [SerializeField] private Color fovColor = new Color(1f, 0f, 0f, 0.3f);

    private enum FOVType
    {
        Fearor,
        Slower,
        Occluder
    }

    private ISlower slower;
    private IFearor fearor;
    private IOccluder occluder;
    private Mesh fovMesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Start()
    {
        switch (fovType)
        {
            case FOVType.Slower:
                slower = GetComponent<ISlower>();
                break;
            case FOVType.Fearor:
                fearor = GetComponent<IFearor>();
                break;
            case FOVType.Occluder:
                occluder = GetComponent<IOccluder>();
                break;
        }

        SetupMesh();
        DrawFOVMesh();
        UpdateMeshVisibility();
    }

    private void SetupMesh()
    {
        // Create child object for the mesh
        GameObject fovObject = new GameObject("FOV_Mesh");
        transform.rotation = Quaternion.identity;
        fovObject.transform.SetParent(transform);
        fovObject.transform.localPosition = Vector3.zero;

        meshFilter = fovObject.AddComponent<MeshFilter>();
        meshRenderer = fovObject.AddComponent<MeshRenderer>();

        fovMesh = new Mesh();
        meshFilter.mesh = fovMesh;

        // Setup material
        if (fovMaterial == null)
        {
            fovMaterial = new Material(Shader.Find("Sprites/Default"));
        }
        fovMaterial.color = fovColor;
        meshRenderer.material = fovMaterial;
    }

    private void Update()
    {
        UpdateMeshVisibility();
    }

    private void UpdateMeshVisibility()
    {
        bool shouldBeVisible = false;

        switch (fovType)
        {
            case FOVType.Fearor:
                shouldBeVisible = fearor != null && fearor.CanCauseFear;
                break;
            case FOVType.Slower:
                shouldBeVisible = slower != null && slower.CanCauseSlow;
                break;
            case FOVType.Occluder:
                shouldBeVisible = occluder != null && occluder.CanOcclude;
                break;
        }

        meshRenderer.enabled = shouldBeVisible;
    }

    public void DrawFOVMesh(float range = 0f)
    {
        float fovRange = 0f;
        float fieldOfView = 0f;
        Vector3 forward = Vector3.zero;

        InitParameterWithType(ref fovRange, ref fieldOfView, ref forward);

        if (range > 0f)
            fovRange = range;

        if (fovRange <= 0f || fieldOfView <= 0f)
            return;

        float halfFOV = fieldOfView * 0.5f;

        int vertexCount = fovResolution + 2; // +1 for center, +1 for closing the arc
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        // Center vertex
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        // Arc vertices and UVs
        for (int i = 0; i <= fovResolution; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, (float)i / fovResolution);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * forward;
            vertices[i + 1] = direction * fovRange;

            float uvAngle = Mathf.Deg2Rad * angle;
            float uvX = 0.5f + 0.5f * Mathf.Cos(uvAngle);
            float uvY = 0.5f + 0.5f * Mathf.Sin(uvAngle);
            uvs[i + 1] = new Vector2(uvX, uvY);
        }

        // Create triangles with reversed winding order
        int triangleIndex = 0;
        for (int i = 0; i < fovResolution; i++)
        {
            triangles[triangleIndex] = 0;
            triangles[triangleIndex + 1] = i + 1;
            triangles[triangleIndex + 2] = i + 2;
            triangleIndex += 3;
        }

        fovMesh.Clear();
        fovMesh.vertices = vertices;
        fovMesh.uv = uvs;
        fovMesh.triangles = triangles;
        fovMesh.RecalculateNormals();
    }

    private void InitParameterWithType(ref float fovRange, ref float fieldOfView, ref Vector3 forward)
    {
        switch (fovType)
        {
            case FOVType.Slower:
                if (slower != null)
                {
                    fieldOfView = slower.SlowFieldOfView;
                    fovRange = slower.SlowRange;
                    forward = -transform.up;
                }
                break;
            case FOVType.Fearor:
                if (fearor != null)
                {
                    fovRange = fearor.FearRange;
                    fieldOfView = fearor.FearFieldOfView;
                    forward = -transform.up;
                }
                break;
            case FOVType.Occluder:
                if (occluder != null)
                {
                    fovRange = occluder.OcclusionRange;
                    fieldOfView = occluder.OcclusionFieldOfView;
                    forward = transform.up;
                }
                break;
        }
    }

}