using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour, ILoadable, ISavable
{
    [SerializeField] protected CardData cardData;
    public CardData CardData => cardData;

    [SerializeField] protected SpriteRenderer backgroundSprite;
    public SpriteRenderer BackgroundSprite => backgroundSprite;

    [SerializeField] protected SpriteRenderer cardSprite;
    public SpriteRenderer CardSprite => cardSprite;

    [SerializeField] protected CardUI cardUI;

    public int StackCount { get; set; }
    // Reference to the stack this card belongs to(null if it's a root card)
    public Card StackParent { get; set; }

    // List of cards stacked on top of this card
    public List<Card> StackedCards { get; private set; }


    protected virtual void OnEnable()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName, cardData.Cost.ToString());
        StackCount = 1;
        StackedCards = new List<Card>();
    }

    protected virtual void Start()
    {
      
    }

    public virtual void OnStack(Card targetCard)
    {
        Debug.Log($"Stack {this.cardData.CardID} on {targetCard.cardData.CardID}");
        StackParent = targetCard;
        targetCard.StackedCards.Add(this);
    }
    public virtual void OnStackInitiate(Card targetCard) { /* Default implementation */ }
    public virtual void OnUnstack(Card targetCard) 
    {
        Debug.Log($"Unstack {this.cardData.CardID} from {targetCard.cardData.CardID}");
        if (StackParent != null)
        {
            StackParent.StackedCards.Remove(this);
            StackParent = null;
        }
        StackedCards.Clear();
    }
    public bool IsStackRoot() => StackParent == null;

    public List<Card> GetEntireStack()
    {
        List<Card> stack = new List<Card>();

        Card root = this;
        while (root.StackParent != null)
        {
            root = root.StackParent;
        }

        CardUtility.AddCardAndChildrenToList(root, stack);
        return stack;
    }



    public void Load(GameSaveData gameSaveData)
    {
        throw new System.NotImplementedException();
    }

    public void Save(GameSaveData gameSaveData)
    {
        throw new System.NotImplementedException();
    }
}
