using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : PersistentMonoSingleton<DifficultyManager>, ILoadable, ISavable
{
    [SerializeField] private DifficultyData defaultDifficulty;

    [SerializeField] private List<DifficultyData> allDifficultyDatas = new List<DifficultyData>();
    public DifficultyData CurrentDifficultyData { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        CurrentDifficultyData = defaultDifficulty;
    }

    public void SetDifficulty(DifficultyData difficultyData)
    {
        CurrentDifficultyData = difficultyData;
        Debug.Log($"Difficulty set to: {CurrentDifficultyData.Difficulty}");
    }


    public DifficultyData GetDifficultyDataByDifficulty(GameDifficulty gameDifficulty)
    {
        return allDifficultyDatas.Find(difficultyData => difficultyData.Difficulty == gameDifficulty);
    }
    public void Save(GameSaveData gameSaveData)
    {
        gameSaveData.gameDifficulty = CurrentDifficultyData.Difficulty;
    }

    public void Load(GameSaveData gameSaveData)
    {
        CurrentDifficultyData = GetDifficultyDataByDifficulty(gameSaveData.gameDifficulty);
    }
}
