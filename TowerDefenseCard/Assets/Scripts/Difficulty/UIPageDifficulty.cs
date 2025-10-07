using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPageDifficulty : UIPage
{
    [SerializeField] private List<UIDifficulty> difficultyOptions = new List<UIDifficulty>();

    [SerializeField] private GameObject warningDeleteSavePopUp;
    [SerializeField] private Button startButton;

    private void Start()
    {
        startButton.onClick.AddListener(OnClickStartButton);

        foreach (var option in difficultyOptions)
        {
            if(option.DifficultyData.Difficulty == DifficultyManager.Instance.CurrentDifficultyData.Difficulty)
            {
                option.OutlineSelector.Select();
            }
            else
            {
                option.OutlineSelector.Deselect();
            }
        }
    }

    private void OnDestroy() => startButton.onClick.RemoveListener(OnClickStartButton);

    private void OnClickStartButton()
    {
        if (SavePath.SaveExists || SavePath.AutoSaveExists)
            warningDeleteSavePopUp.SetActive(true);
        else
            SceneLoader.Instance.LoadNextSceneAsync();
    }

    public override void Show()
    {
        base.Show();
        foreach (var option in difficultyOptions)
        {
            option.OnSelectEvent -= OnDifficultySelected;
            option.OnSelectEvent += OnDifficultySelected;
        }
    }

    private void OnDifficultySelected(DifficultyData difficultyData)
    {
        DifficultyManager.Instance.SetDifficulty(difficultyData);
    }
}