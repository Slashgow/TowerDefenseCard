using System;

[Serializable]
public class GameSaveData 
{
    public float craftTimeElapsed;
    public int currentPlayerCoin;
    public GameMode gameMode;
    public int currentWaveIndex;
    public int currentWaveEnnemyIndex;
    public int currentEnnemyCount;
    public int amountOfSpawnedEnemies;
}
