public class WarningBackToMainMenu : PopUp
{
    protected override void OnClickDoActionButton()
    {
        GameSaveSystem.Instance.SaveGame();
        SceneLoader.Instance.LoadMainMenu();
    }
}
