using System;
using System.IO;
using UnityEngine;

public static class SavePath
{
    private static string manualSaveDirectory;
    private static string autoSaveDirectory;

    private static string saveFilePath;
    private static string autoSaveFilePath;
    private static string savePathCardDiscovered;
    private static string savePathTutorial;
    private static string savePathSuccess;
    private static string questSaveFilePath;

    public static string ManualSaveDirectory
    {
        get
        {
            if (string.IsNullOrEmpty(manualSaveDirectory))
            {
                manualSaveDirectory = Path.Combine(Application.persistentDataPath, "ManualSaves");
                if (!Directory.Exists(manualSaveDirectory))
                    Directory.CreateDirectory(manualSaveDirectory);
            }
            return manualSaveDirectory;
        }
    }

    public static string AutoSaveDirectory
    {
        get
        {
            if (string.IsNullOrEmpty(autoSaveDirectory))
            {
                autoSaveDirectory = Path.Combine(Application.persistentDataPath, "AutoSaves");
                if (!Directory.Exists(autoSaveDirectory))
                    Directory.CreateDirectory(autoSaveDirectory);
            }
            return autoSaveDirectory;
        }
    }

    public static string SaveFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(saveFilePath))
                saveFilePath = Path.Combine(ManualSaveDirectory, "gameSave.json");

            return saveFilePath;
        }
    }

    public static string AutoSaveFilePath
    {
        get
        {
            if (string.IsNullOrEmpty(autoSaveFilePath))
                autoSaveFilePath = Path.Combine(AutoSaveDirectory, "autoSave.json");

            return autoSaveFilePath;
        }
    }

    public static string SavePathSuccess
    {
        get
        {
            if (string.IsNullOrEmpty(savePathSuccess))
                savePathSuccess = Path.Combine(Application.persistentDataPath, "successSave.json");

            return savePathSuccess;
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
    public static bool AutoSaveExists => File.Exists(AutoSaveFilePath);
    public static bool SaveSuccessExists => File.Exists(SavePathSuccess);
    public static bool QuestSaveExists => File.Exists(QuestSaveFilePath);
    public static bool TutorialSaveExists => File.Exists(SavePathTutorial);

    public static string GetMostRecentSavePath()
    {
        DateTime manualSaveTime = DateTime.MinValue;
        DateTime autoSaveTime = DateTime.MinValue;

        if (SaveExists)
            manualSaveTime = File.GetLastWriteTime(SaveFilePath);

        if (AutoSaveExists)
            autoSaveTime = File.GetLastWriteTime(AutoSaveFilePath);

        if (autoSaveTime > manualSaveTime && AutoSaveExists)
            return AutoSaveFilePath;
        else if (SaveExists)
            return SaveFilePath;

        return null;
    }

    public static void ResetAllSaves()
    {
        if (SaveExists)
            File.Delete(SaveFilePath);
        if (AutoSaveExists)
            File.Delete(AutoSaveFilePath);
        if (SaveSuccessExists)
            File.Delete(SavePathSuccess);
        if (QuestSaveExists)
            File.Delete(QuestSaveFilePath);
        if (TutorialSaveExists)
            File.Delete(SavePathTutorial);
        if (File.Exists(SavePathCardDiscovered))
            File.Delete(SavePathCardDiscovered);
    }
}