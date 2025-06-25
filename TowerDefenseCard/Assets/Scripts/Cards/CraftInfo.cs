using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CraftInfo
{
    private Transform stackParent;
    private CraftingRecipe craftingRecipe;
    private List<Card> stackCards;
    private int craftID;
    public List<Card> StackCards => stackCards;
    public Transform StackParent => stackParent;
    public CraftingRecipe CraftingRecipe => craftingRecipe;
    public int CraftID => craftID;
    public CraftInfo(Transform stackParent, CraftingRecipe craftingRecipe, List<Card> stackCards, int craftID)
    {
        this.stackParent = stackParent;
        this.craftingRecipe = craftingRecipe;
        this.stackCards = stackCards;
        this.craftID = craftID;
    }
}
