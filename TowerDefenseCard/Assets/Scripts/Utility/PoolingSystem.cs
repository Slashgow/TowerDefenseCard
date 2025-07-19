using System.Collections.Generic;
using UnityEngine;

public class PoolingSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    [SerializeField, Range(0, 20)]
    private int growPoolSize = 10;

    private Queue<GameObject> availablePrefab = new Queue<GameObject>();

    public int AvailablePrefabCount() => availablePrefab.Count;

    private void Awake()
    {
        GrowPool();
    }

    public GameObject GetPrefabFromPool()
    {
        if (availablePrefab.Count == 0)
            GrowPool();

        var instance = availablePrefab.Dequeue();
        instance.SetActive(true);
        return instance;
    }

    public GameObject GetPrefabFromPool(Vector3 position)
    {
        if (availablePrefab.Count == 0)
            GrowPool();

        var instance = availablePrefab.Dequeue();
        instance.transform.position = position;
        instance.SetActive(true);
        return instance;
    }

    private void GrowPool()
    {
        for (int i = 0; i < growPoolSize; i++)
        {
            var instanceToAdd = Instantiate(prefab);
            instanceToAdd.transform.SetParent(transform);
            AddToPool(instanceToAdd);
        }
    }
    public void AddToPool(GameObject instance)
    {
        instance.SetActive(false);
        instance.transform.SetParent(transform);
        availablePrefab.Enqueue(instance);
    }

}