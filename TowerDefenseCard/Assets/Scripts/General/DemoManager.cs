using UnityEngine;
using UnityEngine.Localization;


public class DemoManager : MonoSingleton<DemoManager>
{
    [SerializeField] private bool useDemoMode = false;
    public bool UseDemoMode => useDemoMode;

    [SerializeField, Range(0, 10)] private int endWaveIndex = 5;
    public int EndWaveIndex => endWaveIndex;

    [SerializeField] private LocalizedString endTextDemo;
    public LocalizedString EndTextDemo => endTextDemo;

    public bool IsDemoEnd()
    {
        if(!useDemoMode)
            return false;

        return WaveManager.Instance.CurrentWaveIndex >= endWaveIndex;
    }
}
