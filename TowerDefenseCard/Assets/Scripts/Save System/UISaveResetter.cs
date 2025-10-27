using UnityEngine;
using UnityEngine.UI;

public class UISaveResetter : MonoBehaviour
{
    [SerializeField] private Button resetSaveButton;
    [SerializeField] private PopUp warningResetAllSavePopUp;


    private void Awake()
    {
        resetSaveButton.onClick.AddListener(OnResetSaveButtonClicked);
    }

    private void OnDestroy()
    {
        resetSaveButton.onClick.RemoveListener(OnResetSaveButtonClicked);
    }

    private void OnResetSaveButtonClicked()
    {
        //Debug.Log("Resetting all saves...");
        //SavePath.ResetAllSaves();

        warningResetAllSavePopUp.gameObject.SetActive(true);

        this.resetSaveButton.gameObject.SetActive(false);
    }
}
