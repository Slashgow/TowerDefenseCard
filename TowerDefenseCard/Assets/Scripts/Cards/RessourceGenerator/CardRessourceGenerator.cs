using UnityEngine;

public class CardRessourceGenerator : Card
{
    [SerializeField] private CardID craftableCardID; // The card to craft (must match a recipe)
    [SerializeField] private int craftCount = 1; // Number of cards to craft per cycle

    private bool isCrafting = false;
    private int currentCraftID = -1;

   protected override void Start()
   {
        base.Start();
        //StartCrafting();
        CraftingManager.Instance.OnCraftComplete += OnCraftComplete;
    }
   
   private void OnDisable()
   {
        //StopCrafting();

        if(CraftingManager.HasInstance)
            CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
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
            CraftingManager.Instance.OnCraftComplete -= OnCraftComplete;
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

    private void OnCraftComplete(int craftID, CardID outputCardID)
    {
        if (craftableCardID == outputCardID)
        {
            Debug.Log("on craft complete reinitate craft");
            currentCraftID = -1;
            InitiateCraft();
        }
    }
}