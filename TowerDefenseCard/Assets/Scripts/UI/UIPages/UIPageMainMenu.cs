using UnityEngine;
using UnityEngine.UI;

public class UIPageMainMenu : UIPage
{
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    [SerializeField] private GameObject warningDeleteSavePopUp;
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueGameButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        continueGameButton.onClick.AddListener(OnClickContinueGameButton);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnClickStartButton);
        continueGameButton.onClick.RemoveListener(OnClickContinueGameButton);
    }

    private void OnClickStartButton()
    {
        if(GameSaveSystem.saveExists)
            warningDeleteSavePopUp.SetActive(true);
        else
            SceneLoader.Instance.LoadNextScene();
    }

    private void OnClickContinueGameButton() => SceneLoader.Instance.LoadNextScene();

    public override void Show()
    {
        base.Show();

        if(GameSaveSystem.saveExists)
            continueGameButton.gameObject.SetActive(true);
        else
            continueGameButton.gameObject.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        layoutGroup.enabled = false;
    }

    public override void Hide()
    {
        base.Hide();
        layoutGroup.enabled = true;
    }
}
