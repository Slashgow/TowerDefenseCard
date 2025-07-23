using TMPro;
using UnityEngine;

public class UISpawner : MonoBehaviour
{
    [Header("References")]
    public PoolingSystem uiPrefabPool;
    public Canvas worldSpaceCanvas;

    [Header("Settings")]
    [SerializeField] private Vector3 worldOffset = Vector3.up * 2f;
    [SerializeField] protected Color damageColor;
    [SerializeField] protected Color healColor;

    public void SpawnTextAbove(Vector3 targetWorldPosition, string text, Color color)
    {
        GameObject instance = uiPrefabPool.GetPrefabFromPool(worldSpaceCanvas.transform);

        instance.transform.position = targetWorldPosition + worldOffset;

        var textmesh = instance.GetComponent<TextMeshProUGUI>();
        textmesh.text = text;
        textmesh.color = color;
    }

    public GameObject GetSpawnTextAbove(Vector3 targetWorldPosition, string text, Color color)
    {
        GameObject instance = uiPrefabPool.GetPrefabFromPool(worldSpaceCanvas.transform);

        instance.transform.position = targetWorldPosition + worldOffset;

        var textmesh = instance.GetComponent<TextMeshProUGUI>();
        textmesh.text = text;
        textmesh.color = color;

        return instance;
    }
}
