using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISaveResetter : MonoBehaviour
{
    [SerializeField] private Button resetSaveButton;
    [SerializeField] private TextMeshProUGUI saveResetSuccessText;

    private void Awake()
    {
        resetSaveButton.onClick.AddListener(OnResetSaveButtonClicked);
        saveResetSuccessText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        resetSaveButton.onClick.RemoveListener(OnResetSaveButtonClicked);
    }

    private void OnResetSaveButtonClicked()
    {
        Debug.Log("Resetting all saves...");
        SavePath.ResetAllSaves();
        this.resetSaveButton.gameObject.SetActive(false);
        saveResetSuccessText.gameObject.SetActive(true);
    }
}
