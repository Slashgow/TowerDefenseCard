using System;
using System.Collections.Generic;

[Serializable]
public class StackSaveData
{
    public List<CardSaveData> cards;

    public StackSaveData()
    {
        cards = new List<CardSaveData>();
    }

    public StackSaveData(List<CardSaveData> cards)
    {
        this.cards = new List<CardSaveData>(cards);
    }

    public void AddCard(CardSaveData cardData)
    {
        cards.Add(cardData);
    }

    public CardSaveData GetRootCard()
    {
        return cards.Count > 0 ? cards[0] : default(CardSaveData);
    }

    public CardSaveData GetTopCard()
    {
        return cards.Count > 0 ? cards[cards.Count - 1] : default(CardSaveData);
    }
}
