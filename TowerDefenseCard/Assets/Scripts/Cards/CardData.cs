using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "William/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private CardID cardID;
    public CardID CardID => cardID;

    [SerializeField] private string cardName; 
    public string CardName => cardName;

    [SerializeField, TextArea] private string cardDescription;
    public string CardDescription => cardDescription;

    [SerializeField] private Sprite cardSprite;
    public Sprite CardSprite => cardSprite;

    [SerializeField] private Sprite cardBackgroundSprite;
    public Sprite CardBackgroundSprite => cardBackgroundSprite;

    [SerializeField] private bool isStackable; 
    public bool IsStackable => isStackable;

    [SerializeField] private int cost;
    public int Cost => cost;

}
