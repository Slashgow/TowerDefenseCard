using System;
using UnityEngine;


public class CardSpawner : MonoBehaviour
{
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private CardID cardIDToSpawn;
    [SerializeField, Range(0, 10)] private int startAmount;
    [SerializeField] protected Transform spawnPoint;

    public int StartAmount => startAmount;

    public event Action<CardID> OnSpawnCard;

    protected virtual void Start()
    {
        if(SavePath.SaveExists)
            return;

        if(!spawnOnStart)
            return;

        for (int i = 0; i < startAmount; i++)
        {
            SpawnCard();
        }
    }

    public void SpawnCard()
    {
        Card cardPrefab = CardManager.Instance.GetCardPrefabByCardID(cardIDToSpawn);
        GameObject cardInstance = Instantiate(cardPrefab.gameObject, spawnPoint.position, Quaternion.identity);
        OnSpawnCard?.Invoke(this.cardIDToSpawn);
    }

}
