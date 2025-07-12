using UnityEngine;
using UnityEngine.UI;

public class UIPageMainMenu : UIPage
{
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    [SerializeField] private Button startButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnClickStartButton);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnClickStartButton);
    }

    private void OnClickStartButton() => SceneLoader.Instance.LoadNextScene();

    public override void Show()
    {
        base.Show();
        layoutGroup.enabled = false;
    }

    public override void Hide()
    {
        base.Hide();
        layoutGroup.enabled = true;
    }
}
