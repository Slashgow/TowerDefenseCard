using System.IO;
using UnityEngine;

public static class SavePath
{
    private static string saveFilePath;
    private static string savePathCardDiscovered;

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
    public static bool SaveExists => File.Exists(SaveFilePath);
}
