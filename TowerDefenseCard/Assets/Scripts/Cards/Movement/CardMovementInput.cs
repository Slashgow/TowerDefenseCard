using UnityEngine;
using UnityEngine.InputSystem;


public class CardMovementInput : MonoSingleton<CardMovementInput>
{
    [SerializeField] private InputActionReference magnetCardInputActionReference;
    

    private bool isMagnetCardActive = false;
    public bool IsMagnetCardActive => isMagnetCardActive;


    protected override void Awake()
    {
        base.Awake();
        magnetCardInputActionReference.action.performed += MagnetCardPerformed;
        magnetCardInputActionReference.action.canceled += MagnetCardCanceled;
    }

    private void OnDisable()
    {
        magnetCardInputActionReference.action.performed -= MagnetCardPerformed;
        magnetCardInputActionReference.action.canceled -= MagnetCardCanceled;
    }
    private void MagnetCardCanceled(InputAction.CallbackContext context) => isMagnetCardActive = false;

    private void MagnetCardPerformed(InputAction.CallbackContext context) => isMagnetCardActive = true;
}
