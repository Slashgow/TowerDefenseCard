using UnityEngine;

public class FOVMeshRenderer : MonoBehaviour
{
    [Header("FOV Reference")]
    [SerializeField] private bool isFearor = true;
    [SerializeField] private bool isSlower = false;

    [Header("FOV Mesh Visualization")]
    [SerializeField] private Material fovMaterial;
    [SerializeField] private int fovResolution = 30;
    [SerializeField] private Color fovColor = new Color(1f, 0f, 0f, 0.3f);

    private ISlower slower;
    private IFearor fearor;
    private Mesh fovMesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    private void Start()
    {
        if(isSlower)
            slower = GetComponent<ISlower>();

        else if(isFearor)
            fearor = GetComponent<IFearor>();

        SetupMesh();
        DrawFOVMesh();

        if (isSlower)
        {
            if (slower != null && slower.CanCauseSlow)
                meshRenderer.enabled = true;
            else
                meshRenderer.enabled = false;
        }
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
        //meshRenderer.sortingOrder = -1; // Behind other sprites
    }

    private void Update()
    {
        if (isFearor)
        {
            if (fearor != null && fearor.CanCauseFear)
                meshRenderer.enabled = true;
            else
                meshRenderer.enabled = false;
        }
    }

    private void DrawFOVMesh()
    {
        float fovRange = 0f;
        float fieldOfView = 0f;
        if(isSlower && slower != null)
        {
            fieldOfView = slower.SlowFieldOfView;
            fovRange = slower.SlowRange;
        } 
        else if(isFearor && fearor != null)
        {
            fovRange = fearor.FearRange;
            fieldOfView = fearor.FearFieldOfView;
        }

        float halfFOV = fieldOfView * 0.5f;
        Vector3 forward = -transform.up;

        int vertexCount = fovResolution + 2; // +1 for center, +1 for closing the arc
        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uvs = new Vector2[vertexCount]; // Added for UV coordinates
        int[] triangles = new int[(vertexCount - 2) * 3];

        // Center vertex
        vertices[0] = Vector3.zero; // Local space
        uvs[0] = new Vector2(0.5f, 0.5f); // Center of texture

        // Arc vertices and UVs
        for (int i = 0; i <= fovResolution; i++)
        {
            float angle = Mathf.Lerp(halfFOV, -halfFOV, (float)i / fovResolution);
            Vector3 direction = Quaternion.Euler(0, 0, angle) * forward;
            vertices[i + 1] = direction * fovRange;

            // Calculate UVs for arc vertices
            float uvAngle = Mathf.Deg2Rad * angle; // Convert angle to radians for UV mapping
            float uvX = 0.5f + 0.5f * Mathf.Cos(uvAngle); // Map to [0,1] range
            float uvY = 0.5f + 0.5f * Mathf.Sin(uvAngle); // Map to [0,1] range
            uvs[i + 1] = new Vector2(uvX, uvY);
        }

        // Create triangles with reversed winding order
        int triangleIndex = 0;
        for (int i = 0; i < fovResolution; i++)
        {
            triangles[triangleIndex] = 0; // Center
            triangles[triangleIndex + 1] = i + 1; // Reversed order
            triangles[triangleIndex + 2] = i + 2; // Reversed order
            triangleIndex += 3;
        }

        fovMesh.Clear();
        fovMesh.vertices = vertices;
        fovMesh.uv = uvs; // Assign UVs to mesh
        fovMesh.triangles = triangles;
        fovMesh.RecalculateNormals();
    }
}