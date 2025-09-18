using System.IO;
using UnityEngine;

public static class SavePath
{
    private static string saveFilePath;
    private static string savePathCardDiscovered;
    private static string savePathTutorial;
    private static string savePathSuccess;
    private static string questSaveFilePath;

    public static string SavePathSuccess
    {
        get
        {
            if (string.IsNullOrEmpty(savePathSuccess))
                savePathSuccess = Path.Combine(Application.persistentDataPath, "successSave.json");

            return savePathSuccess;
        }
    }

    public static string SaveFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(saveFilePath))
                saveFilePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
            
            return saveFilePath;
        }
    }
    public static string SavePathCardDiscovered
    {
        get
        {
            if (string.IsNullOrEmpty(savePathCardDiscovered))
                savePathCardDiscovered = Path.Combine(Application.persistentDataPath, "cardDiscoveredSave.json");

            return savePathCardDiscovered;
        }
    }

    public static string SavePathTutorial
    {
        get
        {
            if (string.IsNullOrEmpty(savePathTutorial))
                savePathTutorial = Path.Combine(Application.persistentDataPath, "tutorialSave.json");
            
            return savePathTutorial;
        }
    }

    public static string QuestSaveFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(questSaveFilePath))
                questSaveFilePath = Path.Combine(Application.persistentDataPath, "questSave.json");

            return questSaveFilePath;
        }
    }
    public static bool SaveExists => File.Exists(SaveFilePath);
    public static bool SaveSuccessExists => File.Exists(SavePathSuccess);
    public static bool QuestSaveExists => File.Exists(QuestSaveFilePath);
}
