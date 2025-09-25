public class WarningDeleteSavePopUp : PopUp
{
    protected override void OnClickDoActionButton()
    {
        GameSaveSystem.ResetGameSave();
        SceneLoader.Instance.LoadNextSceneAsync();
    }
}
