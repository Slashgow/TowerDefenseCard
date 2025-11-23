using UnityEngine;

public class CardRessourceGenerator : Card
{
    [SerializeField] private CardID craftableCardID; 
    [SerializeField] private int craftCount = 1; 

    private bool isCrafting = false;
    private int currentCraftID = -1;

   protected override void Start()
    {
        base.Start();
        ListenToCraft();
    }

    private void ListenToCraft()
    {
        if(CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnCraftCompleteWithInfo -= OnCraftCompleteWithInfo;
            CraftingManager.Instance.OnCraftCompleteWithInfo += OnCraftCompleteWithInfo;
        } 
    }

    private void OnDisable()
    {
        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftCompleteWithInfo -= OnCraftCompleteWithInfo;
    }

    public void StartCrafting()
    {
        if (isCrafting)
            return;

        isCrafting = true;
        InitiateCraft();
    }

    public void StopCrafting()
    {
        if(!isCrafting)
            return;

        isCrafting = false;
        if (currentCraftID != -1 && CraftingManager.HasInstance)
        {
            CraftingManager.Instance.OnCraftCompleteWithInfo -= OnCraftCompleteWithInfo;
            CraftingManager.Instance.TryCancelCraft(this); 
        }
        Debug.Log("Crafting stopped");
    }

    private void InitiateCraft()
    {
        if (CraftingManager.HasInstance)
        {
            // Ensure the resource card itself can be used as the movedCard for a recipe
            if (CraftingManager.Instance.TryCraft(transform.root, this))
            {
                currentCraftID = CraftingManager.Instance.GetCraftIDByCard(this);
                Debug.Log($"Ressource Generator : Initiated craft with ID {currentCraftID}");

            }
            else
            {
                Debug.LogWarning("Craft initiation failed, no matching recipe found");
                isCrafting = false; // Stop if no recipe matches
            }
        }
    }

    private void OnCraftCompleteWithInfo(CraftInfo craftInfo, CardID outputCardID)
    {
        if(!craftInfo.StackCards.Contains(this))
            return;

        if (craftableCardID == outputCardID)
        {
            Debug.Log("on craft complete reinitate craft");
            currentCraftID = -1;
            InitiateCraft();
        }
    }
}