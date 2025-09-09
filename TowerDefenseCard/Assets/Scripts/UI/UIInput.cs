using System;
using Newtonsoft.Json.Converters;
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

    public event Action OnShowPageCollection;

    public event Action OnStartReceivingInput;
    public event Action OnStopReceivingInput;
    public event Action OnPauseBlocked;
    public event Action OnPauseUnBlocked;
    public bool IsReceivingInput { get; private set; }
    public bool IsPauseBlocked { get; private set; }
    private void Awake()
    {
        IsReceivingInput = true;
        IsPauseBlocked = false;
        toggleCollectionMenuInputActionReference.action.performed += ToggleCollectionMenuPerformed;
        toggleSettingMenuInputActionReference.action.performed += ToggleSettingsMenuPerformed;
        playPauseInputActionReference.action.performed += PlayPause;
        speedUpDownInputActionReference.action.performed += SpeedUpDown;
    }

    private void Start()
    {
        if (!GameManager.Instance.AllowPauseDuringCombat)
        {
            GameManager.Instance.OnStartCombatMode += OnPauseBlocked;
            GameManager.Instance.OnEndCombatMode += OnPauseUnBlocked;
        }
    }

    public void StartReceivingInput()
    {
        IsReceivingInput = true;
        OnStartReceivingInput?.Invoke();
    }

    public void StopRecevingInput()
    {
        IsReceivingInput = false;
        OnStopReceivingInput?.Invoke();
    }

    public void BlockPause()
    {
        IsPauseBlocked = true;
        OnPauseBlocked?.Invoke();
    }

    public void UnBlockPause()
    {
        IsPauseBlocked = false;
        OnPauseUnBlocked?.Invoke();
    }

    private void OnDestroy()
    {
        toggleCollectionMenuInputActionReference.action.performed -= ToggleCollectionMenuPerformed;
        toggleSettingMenuInputActionReference.action.performed -= ToggleSettingsMenuPerformed;
        playPauseInputActionReference.action.performed -= PlayPause;
        speedUpDownInputActionReference.action.performed -= SpeedUpDown;

        if (GameManager.HasInstance && !GameManager.Instance.AllowPauseDuringCombat)
        {
            GameManager.Instance.OnStartCombatMode -= OnPauseBlocked;
            GameManager.Instance.OnEndCombatMode -= OnPauseUnBlocked;
        }
    }

    private void ToggleCollectionMenuPerformed(InputAction.CallbackContext context)
    {
        if(!IsReceivingInput)
            return;

        ShowPage(collectionMenu);
    }

    private void ToggleSettingsMenuPerformed(InputAction.CallbackContext context)
    {
        if (!IsReceivingInput)
            return;

        ShowPage(settingsMenu);
    }

    private void SpeedUpDown(InputAction.CallbackContext obj)
    {
        if (!IsReceivingInput)
            return;

        uiTime.ToggleSpeed();
    }

    private void PlayPause(InputAction.CallbackContext obj)
    {
        if (!IsReceivingInput)
            return;

        if (IsPauseBlocked)
            return;

        uiTime.TogglePlayResume();
    }

    public void ShowPage(UIPage page)
    {
        if (page == collectionMenu)
            OnShowPageCollection?.Invoke();

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
    }
}
