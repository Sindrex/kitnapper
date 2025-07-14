using UnityEngine;
using System;

public static class CLogger
{
    public static DateTime Date => DateTime.Now;

    /// <summary>
    /// Logs string to console. Includes timestamp.
    /// </summary>
    public static void Log(string s)
    {
        Debug.Log($"{Date}: {s}");
    }

    /// <summary>
    /// Logs int to console. Includes timestamp.
    /// </summary>
    public static void Log(int i)
    {
        Log(i.ToString());
    }

    /// <summary>
    /// Logs exception to console. Includes timestamp.
    /// </summary>
    public static void LogError(Exception e)
    {
        LogError($"{e.Message} \r\n {e.StackTrace}");
    }

    /// <summary>
    /// Logs string error to console. Includes timestamp.
    /// </summary>
    public static void LogError(string s, GameObject gameObject = null)
    {
        var gameObjectName = gameObject?.name ?? null;
        Debug.LogError($"{Date}: {gameObjectName} {s}");
    }

    /// <summary>
    /// Logs string warnings to console. Includes timestamp.
    /// </summary>
    public static void LogWarning(string s, GameObject gameObject = null)
    {
        var gameObjectName = gameObject?.name ?? null;
        Debug.LogWarning($"{Date}: {gameObjectName} {s}");
    }
}