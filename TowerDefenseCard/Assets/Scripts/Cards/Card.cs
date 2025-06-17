using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private CardData cardData;
    public CardData CardData => cardData;
    public int StackCount { get; set; }
    void Start()
    {
        StackCount = 1; 
    }
}
