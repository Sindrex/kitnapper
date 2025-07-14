using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettingsLoader
{
    public static readonly string Path = Application.persistentDataPath;
    public const string FileName = "GameSettings";

    public static void Save(GameSettings gameSettings)
    {
        //CLogger.Log($"Saving GameSettings at {Path}");
        JsonSaveLoader.SaveAsJson(gameSettings, Path, FileName);
    }

    public static GameSettings Load()
    {
        return JsonSaveLoader.LoadJson<GameSettings>(Path, FileName);
    }

    public static bool GameSettingsExist()
    {
        return JsonSaveLoader.JsonExists(Path, FileName);
    }
}