using System;
using UnityEngine;

public class UIDifficulty : MonoBehaviour, IUISelectable<DifficultyData>
{
    [SerializeField] private UIOutlineSelector<DifficultyData> outlineSelector;
    public UIOutlineSelector<DifficultyData> OutlineSelector => outlineSelector;

    [SerializeField] private DifficultyData difficultyData;
    public DifficultyData DifficultyData => difficultyData;

    public event Action<DifficultyData> OnSelectEvent;

    public DifficultyData GetSelectableData() => difficultyData;
    public void OnSelect(DifficultyData data) => OnSelectEvent?.Invoke(difficultyData);
}
