using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    public CardData CardData => cardData;

    [SerializeField] private SpriteRenderer backgroundSprite;
    public SpriteRenderer BackgroundSprite => backgroundSprite;

    [SerializeField] private SpriteRenderer cardSprite;
    public SpriteRenderer CardSprite => cardSprite;

    public int StackCount { get; set; }

    private void Awake()
    {
        cardSprite.sprite = cardData.CardSprite;
    }

    void Start()
    {
        StackCount = 1; 
    }
}
