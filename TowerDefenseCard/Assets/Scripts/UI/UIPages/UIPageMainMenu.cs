using System;
using UnityEngine;
using UnityEngine.UI;

public class UIPageMainMenu : UIPage
{
    [SerializeField] private VerticalLayoutGroup layoutGroup;

    [SerializeField] private GameObject warningDeleteSavePopUp;
    [SerializeField] private Button startButton;
    [SerializeField] private Button continueGameButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnClickStartButton);
        continueGameButton.onClick.AddListener(OnClickContinueGameButton);
        quitButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnClickStartButton);
        continueGameButton.onClick.RemoveListener(OnClickContinueGameButton);
        quitButton.onClick.AddListener(OnClickQuitButton);
    }

    private void OnClickStartButton()
    {
        if(SavePath.SaveExists || SavePath.AutoSaveExists)
            warningDeleteSavePopUp.SetActive(true);
        else
            SceneLoader.Instance.LoadNextSceneAsync();
    }

    private void OnClickContinueGameButton() => SceneLoader.Instance.LoadNextSceneAsync();

    private void OnClickQuitButton() => Application.Quit();

    public override void Show()
    {
        base.Show();

        if(SavePath.SaveExists || SavePath.AutoSaveExists)
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
