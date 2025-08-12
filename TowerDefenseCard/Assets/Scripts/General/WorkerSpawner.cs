using UnityEngine;

public class WorkerSpawner : MonoSingleton<WorkerSpawner>
{
    [SerializeField, Range(0, 10)] private int startAmountOfWorker;
    [SerializeField] private Transform spawnPoint;

    public int StartAmountOfWorker => startAmountOfWorker;

    private void Start()
    {
        if(SavePath.SaveExists)
            return;

        for (int i = 0; i < startAmountOfWorker; i++)
        {
            Card workerCardPrefab = CardManager.Instance.GetCardPrefabByCardID(CardID.WORKER);
            GameObject workerInstance = Instantiate(workerCardPrefab.gameObject, spawnPoint.position, Quaternion.identity);
        }
    }


}
