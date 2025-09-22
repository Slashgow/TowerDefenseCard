using UnityEngine;
using UnityEngine.InputSystem;

public class SaveInput : MonoSingleton<SaveInput>
{
    [SerializeField] private GameSaveSystem gameSaveSystem;
    [SerializeField] private InputActionReference saveInputActionReference;


    protected override void Awake()
    {
        base.Awake();
        saveInputActionReference.action.performed += SaveInputPerformed;
    }

    private void OnDisable()
    {
        saveInputActionReference.action.performed -= SaveInputPerformed;
    }

    private void SaveInputPerformed(InputAction.CallbackContext context)
    {
        gameSaveSystem.SaveGame();
    }
}
