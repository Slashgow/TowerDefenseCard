using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardName; // e.g., "Villager", "Berry Bush"
    public bool isStackable; // Can this card stack with others?
    public int stackCount; // Number of cards in this stack
    private Vector2 startPosition;
    private Transform startParent;
    private bool isDragging = false;

    void Start()
    {
        stackCount = 1; // Initialize as a single card
    }
}
