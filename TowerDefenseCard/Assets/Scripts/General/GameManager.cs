using System;
using System.Collections;
using UnityEngine;
using UnityTimer;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField, Range(0f,10f)] private float delayBeforeCraftTimerStart;
    public GameMode CurrentGameMode {  get; private set; }
    public GameState CurrentGameState { get; set; }

    public event Action OnStartCraftMode;
    public event Action OnEndCraftMode;
    public event Action OnStartCombatMode;
    public event Action OnEndCombatMode;

    protected override void Awake()
    {
        base.Awake();
        SceneLoader.Instance.OnSceneLoaded += SceneLoader_OnSceneLoaded;
    }

    private void Start()
    {
        WaveManager.Instance.OnEndWaves += WaveManager_OnEndWaves;
    }
    private void OnDisable()
    {
        WaveManager.Instance.OnEndWaves -= WaveManager_OnEndWaves;
    }

    private void WaveManager_OnEndWaves()
    {
        SwitchGameMode();
    }

    private void SceneLoader_OnSceneLoaded(int buildIndex)
    {
        Debug.Log("On Scene loaded");
        StartCoroutine(StartCraftingModeAfterDelay());
    }

    public IEnumerator StartCraftingModeAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeCraftTimerStart);
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
