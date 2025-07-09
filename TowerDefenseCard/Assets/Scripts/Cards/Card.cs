using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] protected CardData cardData;
    public CardData CardData => cardData;

    [SerializeField] private SpriteRenderer backgroundSprite;
    public SpriteRenderer BackgroundSprite => backgroundSprite;

    [SerializeField] protected SpriteRenderer cardSprite;
    public SpriteRenderer CardSprite => cardSprite;

    [SerializeField] private CardUI cardUI;

    public int StackCount { get; set; }

    protected virtual void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName, cardData.Cost.ToString());
    }

    protected virtual void Start()
    {
        StackCount = 1; 
    }

    public virtual void OnStack(Card targetCard) { /* Default implementation */ }
    public virtual void OnStackInitiate(Card targetCard) { /* Default implementation */ }
    public virtual void OnUnstack(Card targetCard) { /* Default implementation */ }

}
