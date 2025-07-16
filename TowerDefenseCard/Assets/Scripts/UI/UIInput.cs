using UnityEngine;
using UnityEngine.InputSystem;

public class UIInput : MonoBehaviour
{
    [SerializeField] private UIPageController uIPageController;
    [SerializeField] private UIPage collectionMenu, settingsMenu, inGameMenu;
    [SerializeField] UITime uiTime;

    [SerializeField] private InputActionReference toggleCollectionMenuInputActionReference;
    [SerializeField] private InputActionReference toggleSettingMenuInputActionReference;
    [SerializeField] private InputActionReference playPauseInputActionReference;
    [SerializeField] private InputActionReference speedUpDownInputActionReference;

    private void Awake()
    {
        toggleCollectionMenuInputActionReference.action.performed += ctx => ShowPage(collectionMenu);
        toggleSettingMenuInputActionReference.action.performed += ctx => ShowPage(settingsMenu);
        playPauseInputActionReference.action.performed += PlayPause;
        speedUpDownInputActionReference.action.performed += SpeedUpDown;
    }

    private void SpeedUpDown(InputAction.CallbackContext obj) => uiTime.ToggleSpeed();
    private void PlayPause(InputAction.CallbackContext obj) => uiTime.TogglePlayResume();

    public void ShowPage(UIPage page)
    {
        bool isPageEnable = page.GetComponent<CanvasGroup>().alpha == 1f;

        if (isPageEnable || page == inGameMenu)
        {
            uIPageController.ShowPage(inGameMenu);
            GameManager.Instance.CurrentGameState = GameState.PLAY;

        }
        else
        {
            uIPageController.ShowPage(page);
            GameManager.Instance.CurrentGameState = GameState.IN_MENU;
        }

        //SetGameState(page == inGameMenu);
      
    }

    private void SetGameState(bool enable)
    {
        if(enable)
            GameManager.Instance.CurrentGameState = GameState.IN_MENU;
        else
            GameManager.Instance.CurrentGameState = GameState.PLAY;
    }
}
