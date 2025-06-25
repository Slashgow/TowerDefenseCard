using UnityEngine;


public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    public CardData CardData => cardData;

    [SerializeField] private SpriteRenderer backgroundSprite;
    public SpriteRenderer BackgroundSprite => backgroundSprite;

    [SerializeField] private SpriteRenderer cardSprite;
    public SpriteRenderer CardSprite => cardSprite;

    [SerializeField] private CardUI cardUI;

    public int StackCount { get; set; }

    protected virtual void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        cardUI.SetupCard(cardData.CardName, cardData.Cost.ToString());
    }

    protected virtual void Start()
    {
        StackCount = 1; 
    }
}
