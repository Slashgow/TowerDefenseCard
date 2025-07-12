using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    [SerializeField] private GameObject objectToToggle; 
    [SerializeField] private InputActionReference toggleInputActionReference;

    private void Awake()
    {
        toggleInputActionReference.action.performed += ctx => ToggleObject();

        objectToToggle.SetActive(false);
    }

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
