using System;
using TMPro;
using UnityEngine;

public class UIDifficulty : MonoBehaviour, IUISelectable<DifficultyData>
{
    [SerializeField] private UIOutlineSelector<DifficultyData> outlineSelector;
    public UIOutlineSelector<DifficultyData> OutlineSelector => outlineSelector;

    [SerializeField] private DifficultyData difficultyData;
    public DifficultyData DifficultyData => difficultyData;

    [SerializeField] private TextMeshProUGUI timeBetweenWaveModifierValueText;
    [SerializeField] private TextMeshProUGUI ennemyHealthModifierValueText;
    [SerializeField] private TextMeshProUGUI ennemyDamageModifierValueText;
    [SerializeField] private TextMeshProUGUI ennemyAttackSpeedModifierValueText;
    [SerializeField] private TextMeshProUGUI startCardsAllowedValueText;
    [SerializeField] private TextMeshProUGUI startDefenseCardsAllowedValueText;

    public event Action<DifficultyData> OnSelectEvent;

    public DifficultyData GetSelectableData() => difficultyData;
    public void OnSelect(DifficultyData data) => OnSelectEvent?.Invoke(difficultyData);

    private void Awake()
    {

        float modifier = difficultyData.TimeBetweenWavesPercentModifier;
        string sign = modifier >= 0 ? "+" : "-";
        timeBetweenWaveModifierValueText.text = $"{sign}{Mathf.Abs(modifier)}%";

        modifier = difficultyData.EnnemyHealthPercentModifier;
        sign = modifier >= 0 ? "+" : "-";
        ennemyHealthModifierValueText.text = $"{sign}{Mathf.Abs(modifier)}%";

        modifier = difficultyData.EnnemyDamagePercentModifier;
        sign = modifier >= 0 ? "+" : "-";
        ennemyDamageModifierValueText.text = $"{sign}{Mathf.Abs(modifier)}%";

        modifier = difficultyData.EnnemyAttackSpeedPercentModifier;
        sign = modifier >= 0 ? "+" : "-";
        ennemyAttackSpeedModifierValueText.text = $"{sign}{Mathf.Abs(modifier)}%";

        startCardsAllowedValueText.text = difficultyData.StartMaxCardsAllowed.ToString();
        startDefenseCardsAllowedValueText.text = difficultyData.StartMaxDefenseCardsAllowed.ToString();
    }
}
