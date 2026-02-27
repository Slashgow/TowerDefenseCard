public class UICardOutlineSelector : UIOutlineSelector<CardID>
{
}

public class CardWithRecipe
{
    private CardID cardID;
    private CraftingRecipe craftingRecipe;

    public CardWithRecipe(CardID cardID, CraftingRecipe craftingRecipe)
    {
        this.cardID = cardID;
        this.craftingRecipe = craftingRecipe;
    }

    public CardID CardID => cardID;
    public CraftingRecipe CraftingRecipe => craftingRecipe;

}