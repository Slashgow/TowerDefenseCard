using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityTimer;

public class AutoSaveSystem : MonoSingleton<AutoSaveSystem>
{
    [Header("Auto Save Settings")]
    [SerializeField] private float autoSaveInterval = 60f; 
    [SerializeField] private int maxAutoSaveFiles = 5; 
    [SerializeField] private bool enableAutoSave = true;
    [SerializeField] private bool saveOnGameModeChange = true;
    [SerializeField] private bool saveOnWaveComplete = true;

    [Header("Dependencies")]
    [SerializeField] private Logger logger;

    private Timer autoSaveTimer;
    private bool isAutoSaving = false;

    protected override void Awake()
    {
        base.Awake();
        InitializeAutoSave();
    }

    private void Start()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCombatMode += OnStartCombatMode;

        if(WaveManager.HasInstance)
            WaveManager.Instance.OnWaveEnd += OnWaveCompleted;
    }
    private void OnDestroy()
    {
        if(GameManager.HasInstance)
            GameManager.Instance.OnStartCombatMode -= OnStartCombatMode;
        if(WaveManager.HasInstance)
            WaveManager.Instance.OnWaveEnd -= OnWaveCompleted;

        StopAutoSaveTimer();
    }


    private void OnStartCombatMode() => PerformAutoSave();
    private void OnWaveCompleted(int obj)
    {
        if(!WaveManager.Instance.IsAllWavesCompleted)
            PerformAutoSave();
    }

    private void InitializeAutoSave()
    {
        if (enableAutoSave)
        {
            StartAutoSaveTimer();
            logger?.Log($"Auto save system initialized with {autoSaveInterval}s interval", this);
        }
    }

    public void StartAutoSaveTimer()
    {
        StopAutoSaveTimer();

        if (enableAutoSave && autoSaveInterval > 0)
        {
            autoSaveTimer = Timer.Register(autoSaveInterval, PerformAutoSave, isLooped: true);
            logger?.Log("Auto save timer started", this);
        }
    }

    public void StopAutoSaveTimer()
    {
        if (autoSaveTimer != null && !autoSaveTimer.isDone)
        {
            autoSaveTimer.Cancel();
            logger?.Log("Auto save timer stopped", this);
        }
    }

    public void PerformAutoSave()
    {
        if (!enableAutoSave || isAutoSaving)
            return;

        StartCoroutine(AutoSaveCoroutine());
    }

    private System.Collections.IEnumerator AutoSaveCoroutine()
    {
        isAutoSaving = true;

        try
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string autoSaveFileName = $"autoSave_{timestamp}.json";
            string autoSaveFilePath = Path.Combine(SavePath.AutoSaveDirectory, autoSaveFileName);

            // Use GameSaveSystem to save to the timestamped file
            GameSaveSystem.Instance.SaveGame(autoSaveFilePath);

            // Also update the main auto save file
            GameSaveSystem.Instance.SaveGame(SavePath.AutoSaveFilePath);

            // Clean up old auto save files
            CleanupOldAutoSaves();

            logger?.Log($"Auto save completed: {autoSaveFileName}", this);
        }
        catch (Exception e)
        {
            logger?.LogError($"Auto save failed: {e.Message}", this);
        }
        finally
        {
            isAutoSaving = false;
        }

        yield return null;
    }

    private void CleanupOldAutoSaves()
    {
        try
        {
            string[] autoSaveFiles = Directory.GetFiles(SavePath.AutoSaveDirectory, "autoSave_*.json");

            if (autoSaveFiles.Length <= maxAutoSaveFiles)
                return;

            // Sort by creation time (oldest first)
            Array.Sort(autoSaveFiles, (x, y) => File.GetCreationTime(x).CompareTo(File.GetCreationTime(y)));

            // Delete oldest files, keeping only maxAutoSaveFiles
            int filesToDelete = autoSaveFiles.Length - maxAutoSaveFiles;
            for (int i = 0; i < filesToDelete; i++)
            {
                File.Delete(autoSaveFiles[i]);
                logger?.Log($"Deleted old auto save: {Path.GetFileName(autoSaveFiles[i])}", this);
            }
        }
        catch (Exception e)
        {
            logger?.LogError($"Failed to cleanup old auto saves: {e.Message}", this);
        }
    }



    public void SetAutoSaveEnabled(bool enabled)
    {
        enableAutoSave = enabled;

        if (enabled)
            StartAutoSaveTimer();
        else
            StopAutoSaveTimer();
    }

    public void SetAutoSaveInterval(float intervalInSeconds)
    {
        autoSaveInterval = intervalInSeconds;

        if (enableAutoSave)
        {
            StartAutoSaveTimer(); 
        }
    }

    public bool HasRecentAutoSave() => SavePath.AutoSaveExists;

    public DateTime GetLastAutoSaveTime()
    {
        if (SavePath.AutoSaveExists)
            return File.GetLastWriteTime(SavePath.AutoSaveFilePath);

        return DateTime.MinValue;
    }

    public List<string> GetAutoSaveFilesList()
    {
        List<string> autoSaveFiles = new List<string>();

        try
        {
            string[] files = Directory.GetFiles(SavePath.AutoSaveDirectory, "autoSave_*.json");
            Array.Sort(files, (x, y) => File.GetCreationTime(y).CompareTo(File.GetCreationTime(x))); // Newest first
            autoSaveFiles.AddRange(files);
        }
        catch (Exception e)
        {
            logger?.LogError($"Failed to get auto save files list: {e.Message}", this);
        }

        return autoSaveFiles;
    }

   

   
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && enableAutoSave)
        {
            PerformAutoSave(); // Save when app is paused
        }
    }
}