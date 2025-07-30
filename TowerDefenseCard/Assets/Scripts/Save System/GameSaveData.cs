using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData 
{
    public int currentNumberOfCards;
    public int maxCardsAllowed;
    public float craftTimeElapsed;
    public int currentPlayerCoin;
    public GameMode gameMode;
    public int currentWaveIndex;
    public int currentWaveEnnemyIndex;
    public int currentEnnemyCount;
    public int amountOfSpawnedEnemies;
    public List<StackSaveData> cardStacks;

    public GameSaveData()
    {
        cardStacks = new List<StackSaveData>();
    }
}
