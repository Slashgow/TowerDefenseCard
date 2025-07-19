using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] protected CardData cardData;
    public CardData CardData => cardData;

    [SerializeField] protected SpriteRenderer backgroundSprite;
    public SpriteRenderer BackgroundSprite => backgroundSprite;

    [SerializeField] protected SpriteRenderer cardSprite;
    public SpriteRenderer CardSprite => cardSprite;

    [SerializeField] protected CardUI cardUI;

    public int StackCount { get; set; }

    protected virtual void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName, cardData.Cost.ToString());
        StackCount = 1;
    }

    protected virtual void Start()
    {
      
    }

    public virtual void OnStack(Card targetCard) { /* Default implementation */ }
    public virtual void OnStackInitiate(Card targetCard) { /* Default implementation */ }
    public virtual void OnUnstack(Card targetCard) { /* Default implementation */ }

}
