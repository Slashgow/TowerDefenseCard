using UnityEngine;

public class CardRessourceGenerator : Card
{
    [SerializeField] private Card craftableCardPrefab; // The card to craft (must match a recipe)
    [SerializeField] private int craftCount = 1; // Number of cards to craft per cycle

    private bool isCrafting = false;
    private int currentCraftID = -1;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCrafting();
    }

    private void OnDisable()
    {
        StopCrafting();
    }

    public void StartCrafting()
    {
        if (isCrafting)
            return;

        isCrafting = true;
        InitiateCraft();
        Debug.Log($"Started continuous crafting of {craftableCardPrefab.name}");
    }

    public void StopCrafting()
    {
        if(!isCrafting)
            return;

        isCrafting = false;
        if (currentCraftID != -1 && CraftingManager.Instance != null)
        {
            CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
            CraftingManager.Instance.TryCancelCraft(this); 
        }
        Debug.Log("Crafting stopped");
    }

    private void InitiateCraft()
    {
        if (isCrafting && CraftingManager.Instance != null)
        {
            // Ensure the resource card itself can be used as the movedCard for a recipe
            if (CraftingManager.Instance.TryCraft(transform, this))
            {
                currentCraftID = CraftingManager.Instance.GetCraftIDByCard(this);

                if (currentCraftID != -1)
                {
                    CraftingManager.Instance.OnCraftComplete += OnCraftComplete;
                    Debug.Log($"Initiated craft with ID {currentCraftID}");
                }
                else
                {
                    Debug.LogWarning("Craft initiation failed, no valid craft ID assigned");
                    isCrafting = false; // Stop if craft fails
                }
            }
            else
            {
                Debug.LogWarning("Craft initiation failed, no matching recipe found");
                isCrafting = false; // Stop if no recipe matches
            }
        }
    }

    private void OnCraftComplete(int craftID, CardID outputCardID)
    {
        if (isCrafting && craftID == currentCraftID)
        {
            CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
            currentCraftID = -1;
            InitiateCraft();
        }
    }
}