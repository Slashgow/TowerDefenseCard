using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [SerializeField] private Button doActionButton, cancelButton;

    protected virtual void Awake() => gameObject.SetActive(false);

    private void OnEnable()
    {
        doActionButton.onClick.AddListener(OnClickDoActionButton);
        cancelButton.onClick.AddListener(OnClickCancelButton);
    }

    private void OnDisable()
    {
        doActionButton.onClick.RemoveListener(OnClickDoActionButton);
        cancelButton.onClick.RemoveListener(OnClickCancelButton);
    }

    protected virtual void OnClickCancelButton()
    {
        this.gameObject.SetActive(false);
    }

    protected virtual void OnClickDoActionButton()
    {
    }
}
