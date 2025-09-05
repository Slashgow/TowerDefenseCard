using System;
using System.Collections.Generic;

[Serializable]
public class SuccessStatData
{
    public int boosterOpenedCounterAllTime;
    public int boosterOpenedCounterInGame;

    public int craftedCardCounterAllTime;
    public int soldCardCounterAllTime;

    public List<CardID> factoriesIDThisGame;
    public List<CardID> defenseIDThisGame;

    public SuccessStatData()
    {
        boosterOpenedCounterAllTime = 0;
        boosterOpenedCounterInGame = 0;
        craftedCardCounterAllTime = 0;
        soldCardCounterAllTime = 0;
        factoriesIDThisGame = new List<CardID>();
        defenseIDThisGame = new List<CardID>();
    }

    public SuccessStatData(int boosterOpenedCounterAllTime, int boosterOpenedCounterInGame, int craftedCardCounterAllTime, int soldCardCounterAllTime)
    {
        this.boosterOpenedCounterAllTime = boosterOpenedCounterAllTime;
        this.boosterOpenedCounterInGame = boosterOpenedCounterInGame;
        this.craftedCardCounterAllTime = craftedCardCounterAllTime;
        this.soldCardCounterAllTime= soldCardCounterAllTime;
        factoriesIDThisGame = new List<CardID>();
        defenseIDThisGame = new List<CardID>();
    }

    public SuccessStatData(int boosterOpenedCounterAllTime, int boosterOpenedCounterInGame, int craftedCardCounterAllTime, int soldCardCounterAllTime, 
        List<CardID> factoriesIDThisGame, List<CardID> defenseIDThisGame)
    {
        this.boosterOpenedCounterAllTime = boosterOpenedCounterAllTime;
        this.boosterOpenedCounterInGame = boosterOpenedCounterInGame;
        this.craftedCardCounterAllTime = craftedCardCounterAllTime;
        this.soldCardCounterAllTime = soldCardCounterAllTime;
        this.factoriesIDThisGame = new List<CardID>(factoriesIDThisGame);
        this.defenseIDThisGame = new List<CardID>(defenseIDThisGame);
    }

    public void ResetGameSpecificData()
    {
        boosterOpenedCounterInGame = 0;
        factoriesIDThisGame.Clear();
        defenseIDThisGame.Clear();
    }
}
