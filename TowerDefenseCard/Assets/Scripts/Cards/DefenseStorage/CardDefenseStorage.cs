using System;
using UnityEngine;


public class CardDefenseStorage : Card
{

    [SerializeField, Range(0, 30)] private int numberOfAdditionalCardsDefenseAllowed = 10;
    public int NumberOfAdditionalCardsDefenseAllowed => numberOfAdditionalCardsDefenseAllowed;

    //public static event Action<int> OnUpdateMaxNumberOfCardsDefense;

    protected override void Start()
    {
        base.Start();

        CardManager.Instance.IncreaseMaxCardsDefenseAllowed(numberOfAdditionalCardsDefenseAllowed);
    }
}
