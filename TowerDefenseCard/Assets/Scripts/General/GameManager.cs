using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Logger logger;

    [SerializeField, Range(0f,10f)] private float delayBeforeCraftTimerStart;

    [SerializeField, Range(1f, 5f)] private float speedUpGameSpeed;
    public GameMode CurrentGameMode {  get; private set; }
    public GameState CurrentGameState { get; set; }
    public bool IsPaused { get; private set; }
    public float CurrentGameSpeed { get; private set; }

    public UnityEvent OnStartCraftModeUnity;
    public event Action OnStartCraftMode;

    public UnityEvent OnEndCraftModeUnity;
    public event Action OnEndCraftMode;

    public UnityEvent OnStartCombatModeUnity;
    public event Action OnStartCombatMode;

    public UnityEvent OnEndCombatModeUnity;
    public event Action OnEndCombatMode;

    public event Action OnPause; 
    public event Action OnResume;
    public UnityEvent OnPauseUnity;
    public UnityEvent OnResumeUnity;

    private Coroutine pauseSimulationCoroutine;

    protected override void Awake()
    {
        base.Awake();
        SceneLoader.Instance.OnSceneLoaded += SceneLoader_OnSceneLoaded;
        CurrentGameState = GameState.PLAY;
        StartCraftMode();
    }

    private void Start()
    {
        WaveManager.Instance.OnWaveEnd -= WaveManager_OnWaveEnd;
        WaveManager.Instance.OnWaveEnd += WaveManager_OnWaveEnd;

        PlayerHealth.OnPlayerDie += CardPlayerHealth_OnPlayerDie;
    }

    private void OnDestroy()
    {
        SceneLoader.Instance.OnSceneLoaded -= SceneLoader_OnSceneLoaded;
        PlayerHealth.OnPlayerDie -= CardPlayerHealth_OnPlayerDie;
    }

    private void WaveManager_OnWaveEnd()
    {
        SwitchGameMode();
    }

    private void SceneLoader_OnSceneLoaded(int buildIndex)
    {
        logger.Log("On Scene loaded", this);
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
        if (CurrentGameMode == GameMode.CRAFTING)
        {
            EndCraftMode();

            if (!WaveManager.Instance.IsAllWavesCompleted)
            {
                StartCombatMode();
            }
        }
        else if (CurrentGameMode == GameMode.COMBAT)
        {
            EndCombatMode();
            StartCraftMode();
        }
    }

    private void EndCraftMode()
    {
        OnEndCraftModeUnity?.Invoke();
        OnEndCraftMode?.Invoke();
    }

    private void EndCombatMode()
    {
        OnEndCombatModeUnity?.Invoke();
        OnEndCombatMode?.Invoke();
    }

    private void StartCombatMode()
    {
        CurrentGameMode = GameMode.COMBAT;
        logger.Log("Start Combat mode", this);
        OnStartCombatModeUnity?.Invoke();
        OnStartCombatMode?.Invoke();
    }

    private void StartCraftMode()
    {
        CurrentGameMode = GameMode.CRAFTING;
        logger.Log("Start craft mode", this);
        OnStartCraftModeUnity?.Invoke();
        OnStartCraftMode?.Invoke();
    }

    public void Pause()
    {
        if (IsPaused)
            return;

        IsPaused = true;
        Time.timeScale = 0f;
        CurrentGameSpeed = Time.timeScale;
        pauseSimulationCoroutine = StartCoroutine(TemporarlyAdjustTimeScale(2));
        OnPause?.Invoke();
        OnPauseUnity?.Invoke();
    }
    private IEnumerator TemporarlyAdjustTimeScale(int frameToSimulate)
    {
        while (IsPaused)
        {
            Time.timeScale = 0.01f;
            SimulationMode2D originalSimulationMode = Physics2D.simulationMode;
            Physics2D.simulationMode = SimulationMode2D.Script;


            for (int i = 0; i < frameToSimulate; i++)
            {
                //Debug.Log("Simulate Physics");
                Physics2D.Simulate(Time.fixedDeltaTime);
            }

            Physics2D.simulationMode = originalSimulationMode;
            Time.timeScale = 0f;
            yield return null;
        }
    }
    public void Resume()
    {
        if (!IsPaused)
            return;

        if (pauseSimulationCoroutine != null)
        {
            StopCoroutine(pauseSimulationCoroutine);
            pauseSimulationCoroutine = null;
        }

        IsPaused = false;
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;
        Time.timeScale = 1f;
        CurrentGameSpeed = Time.timeScale;
        OnResume?.Invoke();
        OnResumeUnity?.Invoke();
    }

    public void SpeedUpGame()
    {
        CurrentGameSpeed = speedUpGameSpeed;
        Time.timeScale = CurrentGameSpeed;
    }

    public void ResetGameSpeed()
    {
        CurrentGameSpeed = 1f;
        Time.timeScale = 1f;
    }
    private void CardPlayerHealth_OnPlayerDie()
    {
        SceneLoader.Instance.LoadNextScene();
    }
}
