using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Card : MonoBehaviour
{
    [SerializeField] protected CardData cardData;
    public CardData CardData => cardData;

    [SerializeField] protected SpriteRenderer backgroundSprite;
    public SpriteRenderer BackgroundSprite => backgroundSprite;

    [SerializeField] protected SpriteRenderer cardSprite;
    public SpriteRenderer CardSprite => cardSprite;

    [SerializeField] protected CardUI cardUI;

    public int StackCount { get; set; } = 1;
    // Reference to the stack this card belongs to(null if it's a root card)
    public Card StackParent { get; set; } = null;

    // List of cards stacked on top of this card
    public List<Card> StackedCards { get; private set; } = new List<Card>();

    private List<Card> entireStackParent = new List<Card>();


    protected virtual void Awake()
    {
        cardSprite.sprite = cardData.CardSprite;
        backgroundSprite.sprite = cardData.CardBackgroundSprite;
        cardUI.SetupCard(cardData.CardName.GetLocalizedString(), cardData.Cost.ToString());
        //StackCount = 1;
        //StackedCards = new List<Card>();
        //entireStackParent = new List<Card>();
        //StackParent = null;
    }

    private void OnDisable()
    {
        this.OnUnstack(false);
        StackCount = 1;
        StackedCards = new List<Card>();
        entireStackParent = new List<Card>();
        StackParent = null;
    }

    private void LocalizationSettings_SelectedLocaleChanged(UnityEngine.Localization.Locale Locale)
    {
        cardUI.SetupCard(cardData.CardName.GetLocalizedString(), cardData.Cost.ToString());
    }

    protected virtual void Start()
    {
        LocalizationSettings.SelectedLocaleChanged -= LocalizationSettings_SelectedLocaleChanged;
        LocalizationSettings.SelectedLocaleChanged += LocalizationSettings_SelectedLocaleChanged;
    }

    public virtual void OnStack(Card targetCard)
    {
        //Debug.Log($"Stack {this.cardData.CardID} {this.GetInstanceID()}  on {targetCard.cardData.CardID}{targetCard.GetInstanceID()}");

        transform.SetParent(targetCard.transform, false);

        StackParent = targetCard;

        if(entireStackParent != null)
            entireStackParent.Clear();

        entireStackParent = StackParent.GetEntireStack();
        foreach (Card card in entireStackParent)
        {
            if (card == this)
                continue;

            card.StackCount += this.StackCount;
        }


        targetCard.StackedCards.Add(this);
    }
    public virtual void OnStackInitiate(Card targetCard) { /* Default implementation */ }
    public virtual void OnUnstack() 
    {
        transform.SetParent(null, true);

        //Debug.Log($" Try Unstack {this.cardData.CardID} {this.GetInstanceID()}");

        if (StackParent != null)
        {
            //Debug.Log($"Unstack {this.cardData.CardID} {this.GetInstanceID()}  from {StackParent.cardData.CardID} {StackParent.GetInstanceID()}");

            StackParent.StackedCards.Remove(this);

            if (entireStackParent != null)
                entireStackParent.Clear();

            entireStackParent = StackParent.GetEntireStack();
            foreach (Card card in entireStackParent)
            {
                if (card == this)
                    continue;

                card.StackCount -=  this.StackCount;
                Mathf.Clamp(card.StackCount, 1, int.MaxValue); 
            }

            StackParent = null;

        }

        if (!GetComponentInChildren<Card>())
            StackedCards.Clear();
    }

    public virtual void OnUnstack(bool setParent)
    {
        if(setParent)
            transform.SetParent(null, true);

        //Debug.Log($" Try Unstack {this.cardData.CardID} {this.GetInstanceID()}");

        if (StackParent != null)
        {
            //Debug.Log($"Unstack {this.cardData.CardID} {this.GetInstanceID()}  from {StackParent.cardData.CardID} {StackParent.GetInstanceID()}");

            StackParent.StackedCards.Remove(this);

            if (entireStackParent != null)
                entireStackParent.Clear();

            entireStackParent = StackParent.GetEntireStack();
            foreach (Card card in entireStackParent)
            {
                if (card == this)
                    continue;

                card.StackCount -= this.StackCount;
                Mathf.Clamp(card.StackCount, 1, int.MaxValue);
            }

            StackParent = null;

        }

        if (!GetComponentInChildren<Card>())
            StackedCards.Clear();
    }
    public bool IsStackRoot() => StackParent == null;

    public List<Card> GetEntireStack()
    {
        List<Card> stack = new List<Card>();
        HashSet<Card> visited = new HashSet<Card>();

        Card root = this;
        while (root.StackParent != null)
        {
            if (visited.Contains(root))
            {
                Debug.LogError("Circular reference detected in stack hierarchy!");
                break;
            }
            visited.Add(root);
            root = root.StackParent;
        }

        CardUtility.AddCardAndChildrenToList(root, stack);
        return stack;
    }
}
