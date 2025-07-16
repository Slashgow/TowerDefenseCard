using UnityEngine;

public class CardStorage : Card
{
    [SerializeField, Range(0, 30)] private int numberOfAdditionalCardsAllowed = 10;
    public int NumberOfAdditionalCardsAllowed => numberOfAdditionalCardsAllowed;

    protected override void Start()
    {
        base.Start();

        CardManager.Instance.IncreaseMaxCardsAllowed(numberOfAdditionalCardsAllowed);
    }
}
