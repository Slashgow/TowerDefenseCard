using TMPro;
using UnityEngine;

public class UISpawner : MonoBehaviour
{
    [Header("References")]
    public PoolingSystem uiPrefabPool;
    public Canvas worldSpaceCanvas;

    [Header("Settings")]
    public Vector3 worldOffset = Vector3.up * 2f;

    public void SpawnTextAbove(Vector3 targetWorldPosition, string text)
    {
        GameObject instance = uiPrefabPool.GetPrefabFromPool(worldSpaceCanvas.transform);

        instance.transform.position = targetWorldPosition + worldOffset;

        instance.GetComponent<TextMeshProUGUI>().text = text;
    }

    public GameObject GetSpawnTextAbove(Vector3 targetWorldPosition, string text)
    {
        GameObject instance = uiPrefabPool.GetPrefabFromPool(worldSpaceCanvas.transform);

        instance.transform.position = targetWorldPosition + worldOffset;

        instance.GetComponent<TextMeshProUGUI>().text = text;
        return instance;
    }
}
