using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "NewCardData", menuName = "William/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private CardID cardID;
    public CardID CardID => cardID;

    [SerializeField] private LocalizedString cardName; 
    public LocalizedString CardName => cardName;

    [SerializeField] private LocalizedString cardDescription;
    public LocalizedString CardDescription => cardDescription;

    [SerializeField] private Sprite cardSprite;
    public Sprite CardSprite => cardSprite;

    [SerializeField] private Sprite cardBackgroundSprite;
    public Sprite CardBackgroundSprite => cardBackgroundSprite;

    [SerializeField] private bool isStackable; 
    public bool IsStackable => isStackable;

    [SerializeField] private int cost;
    public int Cost => cost;

}
