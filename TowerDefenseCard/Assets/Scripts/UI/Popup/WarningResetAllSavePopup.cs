using TMPro;
using UnityEngine;

public class WarningResetAllSavePopup : PopUp
{
    [SerializeField] private TextMeshProUGUI saveResetSuccessText;

    protected override void Awake()
    {
        base.Awake();
        saveResetSuccessText.gameObject.SetActive(false);
    }

    protected override void OnClickDoActionButton()
    {
        Debug.Log("Resetting all saves...");
        SavePath.ResetAllSaves();
        saveResetSuccessText.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
