using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityTimer;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private Button startEarlyButton;
    [SerializeField] private TextMeshProUGUI additionnalInkText;
    [SerializeField, Range(0f,5f)] private float refreshRateAdditionalInkText = 1f;

    private Timer refreshAdditionalInkTextTimer;

    private void Start()
    {
        GameManager.Instance.OnStartCombatMode += GameManager_OnStartCombatMode;
        GameManager.Instance.OnEndCombatMode += GameManager_OnEndCombatMode;

        WaveManager.Instance.OnWaveStart += WaveManager_OnWaveStart;

        waveText.text = $"{WaveManager.Instance.CurrentWaveIndex}/{WaveManager.Instance.NumberOfWaves}";

        startEarlyButton.onClick.AddListener(ClickOnStartEarly);

        refreshAdditionalInkTextTimer = Timer.Register(refreshRateAdditionalInkText, onComplete : UpdateAdditionalInkText, isLooped: true);
    }

    private void OnDestroy()
    {
        if(WaveManager.HasInstance)
            WaveManager.Instance.OnWaveStart -= WaveManager_OnWaveStart;

        if(GameManager.HasInstance)
        {
            GameManager.Instance.OnStartCombatMode -= GameManager_OnStartCombatMode;
            GameManager.Instance.OnEndCombatMode -= GameManager_OnEndCombatMode;
        }

        startEarlyButton.onClick.RemoveListener(ClickOnStartEarly);
        refreshAdditionalInkTextTimer?.Cancel();
    }
    private void WaveManager_OnWaveStart(int waveNumber)
    {
        waveText.text = $"{waveNumber}/{WaveManager.Instance.NumberOfWaves}";
    }


    private void ClickOnStartEarly()
    {
        ShopManager.Instance.SpawnAdditionalInkEarlyStart();
        GameManager.Instance.SwitchGameMode();
        CraftingManager.Instance.CraftingModeDurationTimer?.Cancel();
        CraftingManager.Instance.ResetTimeElapsed();
    }
    private void GameManager_OnEndCombatMode() => startEarlyButton.interactable = true;
    private void GameManager_OnStartCombatMode() => startEarlyButton.interactable = false;

    private void UpdateAdditionalInkText()
    {
        additionnalInkText.text = $"+{ShopManager.Instance.AdditionalInkFromEarlyStart}";
    }
}
