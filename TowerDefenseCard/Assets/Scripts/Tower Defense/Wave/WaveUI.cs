using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waveText;

    private void Start()
    {
        WaveManager.Instance.OnWaveStart += WaveManager_OnWaveStart;

        waveText.text = $"{WaveManager.Instance.CurrentWaveIndex}/{WaveManager.Instance.NumberOfWaves}";
    }

    private void OnDestroy()
    {
        if(WaveManager.HasInstance)
            WaveManager.Instance.OnWaveStart -= WaveManager_OnWaveStart;
    }
    private void WaveManager_OnWaveStart(int waveNumber)
    {
        waveText.text = $"{waveNumber}/{WaveManager.Instance.NumberOfWaves}";
    }
}
