using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle;
    [SerializeField] UITime uiTime;

    [SerializeField] private InputActionReference toggleInputActionReference;
    [SerializeField] private InputActionReference playPauseInputActionReference;
    [SerializeField] private InputActionReference speedUpDownInputActionReference;

    private void Awake()
    {
        toggleInputActionReference.action.performed += ctx => ToggleObject();
        playPauseInputActionReference.action.performed += PlayPause;
        speedUpDownInputActionReference.action.performed += SpeedUpDown;

        objectToToggle.SetActive(false);
    }

    private void SpeedUpDown(InputAction.CallbackContext obj) => uiTime.ToggleSpeed();
    private void PlayPause(InputAction.CallbackContext obj) => uiTime.TogglePlayResume();

    private void ToggleObject()
    {
        if (objectToToggle != null)
        {
            SetGameState(!objectToToggle.activeSelf);
            objectToToggle.SetActive(!objectToToggle.activeSelf);
            //Debug.Log($"Toggled {objectToToggle.name} to {objectToToggle.activeSelf}");
        }
    }

    private void SetGameState(bool enable)
    {
        if(enable)
            GameManager.Instance.CurrentGameState = GameState.IN_MENU;
        else
            GameManager.Instance.CurrentGameState = GameState.PLAY;
    }
}
