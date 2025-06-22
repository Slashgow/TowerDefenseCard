using System;
using UnityEngine;
using UnityTimer;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField, Range(0f, 500f)] private float timeCraftMode;

    private Timer craftingTimer;
    public GameMode CurrentGameMode {  get; private set; }

    public event Action OnStartCraftMode;
    public event Action OnEndCraftMode;
    public event Action OnStartCombatMode;
    public event Action OnEndCombatMode;

    private void Start()
    {
        SceneLoader.Instance.OnSceneLoaded += SceneLoader_OnSceneLoaded;
    }

    private void SceneLoader_OnSceneLoaded(int buildIndex)
    {
        CurrentGameMode = GameMode.CRAFTING;
        craftingTimer = Timer.Register(timeCraftMode, onComplete: SwitchGameMode);
    }

    private void SwitchGameMode()
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
