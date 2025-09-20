using UnityEngine;

public class CardStorage : Card
{
    [SerializeField, Range(0, 30)] private int numberOfAdditionalCardsAllowed = 10;
    public int NumberOfAdditionalCardsAllowed => numberOfAdditionalCardsAllowed;

    protected override void Start()
    {
        base.Start();

        if (SavePath.SaveExists)
            return;

        CardManager.Instance.IncreaseMaxCardsAllowed(numberOfAdditionalCardsAllowed);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if(CardManager.HasInstance)
        {
            CardManager.Instance.DecreaseMaxCardsAllowed(numberOfAdditionalCardsAllowed);
        }
    }
}
