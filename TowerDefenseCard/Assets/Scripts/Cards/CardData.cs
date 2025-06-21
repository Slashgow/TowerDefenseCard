using UnityEngine;

[CreateAssetMenu(fileName = "NewCardData", menuName = "William/CardData")]
public class CardData : ScriptableObject
{
    [SerializeField] private CardID cardID;
    public CardID CardID => cardID;

    [SerializeField] private string cardName; 
    public string CardName => cardName;

    [SerializeField] private Sprite cardSprite;
    public Sprite CardSprite => cardSprite;

    [SerializeField] private bool isStackable; 
    public bool IsStackable => isStackable;

}
