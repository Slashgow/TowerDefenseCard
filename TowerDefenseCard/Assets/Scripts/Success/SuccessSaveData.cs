using System;
using System.Collections.Generic;

[Serializable]
public class SuccessSaveData
{
    public List<bool> successCompletionStates;
    public SuccessStatData successStatData;

    public SuccessSaveData()
    {
        successCompletionStates = new List<bool>();
        successStatData = new SuccessStatData();
    }

    public SuccessSaveData(int boosterOpenedCounterAllTime, int boosterOpenedCounterInGame, int craftedCardCounterAllTime, int soldCardCounterAllTime)
    {
        successCompletionStates = new List<bool>();
        successStatData = new SuccessStatData(boosterOpenedCounterAllTime, boosterOpenedCounterInGame, craftedCardCounterAllTime, soldCardCounterAllTime);
    }

    public SuccessSaveData(int boosterOpenedCounterAllTime, int boosterOpenedCounterInGame, int craftedCardCounterAllTime, int soldCardCounterAllTime,
         List<CardID> factoriesIDThisGame, List<CardID> defenseIDThisGame, List<SuccessData> allSuccessData)
    {
        successStatData = new SuccessStatData(boosterOpenedCounterAllTime, boosterOpenedCounterInGame, 
            craftedCardCounterAllTime, soldCardCounterAllTime, factoriesIDThisGame, defenseIDThisGame);

        successCompletionStates = new List<bool>();
        foreach (SuccessData success in allSuccessData)
        {
            successCompletionStates.Add(success.isDone);
        }
    }
}
