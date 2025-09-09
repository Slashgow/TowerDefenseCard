using System;
using Unity.VisualScripting;
using UnityEngine;

public class Stealable : MonoBehaviour, IStealable
{
    private Card card;
    private BaseCardMovement baseCardMovement;

    private bool isStolen = false;
    public bool IsStolen => isStolen;

    public event Action OnStealApplied;
    public event Action OnStealRemoved;
    public static event Action OnDestroy;
    private void Awake()
    {
        card = GetComponent<Card>();
        baseCardMovement = GetComponent<BaseCardMovement>();
    }

    public void ApplySteal(Transform stealerTransform, Vector3 offset)
    {
        isStolen = true;

        if(baseCardMovement != null)
            baseCardMovement.enabled = false;

        if(card != null)
        {
            card.OnUnstack();

            if(card.StackedCards != null && card.StackedCards.Count > 0)
            {
                card.StackedCards[0].OnUnstack();
            }
        }
            

        Follower follower = this.AddComponent<Follower>();
        follower.SetupFollower(stealerTransform, offset);
    }

    public void RemoveSteal()
    {
        isStolen = false;

        if(baseCardMovement != null)
            baseCardMovement.enabled = true;

        if(TryGetComponent(out Follower follower))
        {
            follower.enabled = false;
            Destroy(follower);
        }
    }

    public void Destroy()
    {
       OnDestroy?.Invoke();
       Destroy(this.gameObject);
    }
}
