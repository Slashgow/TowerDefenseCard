using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonLinkOpener : MonoBehaviour
{
    [SerializeField] private string url;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OpenURL);
    }

    private void OnDestroy() => button.onClick.RemoveListener(OpenURL);
    private void OpenURL() => Application.OpenURL(url);
}
