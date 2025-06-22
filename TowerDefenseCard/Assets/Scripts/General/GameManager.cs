using System;
using UnityEngine;
using UnityTimer;

public class GameManager : MonoSingleton<GameManager>
{
    public GameMode CurrentGameMode {  get; private set; }

    public event Action OnStartCraftMode;
    public event Action OnEndCraftMode;
    public event Action OnStartCombatMode;
    public event Action OnEndCombatMode;

    protected override void Awake()
    {
        base.Awake();
        SceneLoader.Instance.OnSceneLoaded += SceneLoader_OnSceneLoaded;
    }

    private void SceneLoader_OnSceneLoaded(int buildIndex)
    {
        Debug.Log("On Scene loaded");
        CurrentGameMode = GameMode.CRAFTING;
        CraftingManager.Instance.StartCraftingModeTimer();
    }

    public void SwitchGameMode()
    {
        Debug.Log("timer completed");

        if (CurrentGameMode == GameMode.CRAFTING)
        {
            OnEndCraftMode?.Invoke();
            CurrentGameMode = GameMode.COMBAT;
            OnStartCombatMode?.Invoke();
        }
        else if (CurrentGameMode == GameMode.COMBAT)
        { 
            OnEndCombatMode?.Invoke();
            CurrentGameMode = GameMode.CRAFTING; 
            OnStartCraftMode?.Invoke();
        }
    }
}
