public class WarningDeleteSavePopUp : PopUp
{
    protected override void OnClickDoActionButton()
    {
        GameSaveSystem.ResetSave();
        SceneLoader.Instance.LoadNextScene();
    }
}
