using UnityEngine;

public static class DebugManager
{
    public static void Log(string message)
    {
        Debug.Log("[DEBUG] " + message);
    }

    public static void LogWarning(string message)
    {
        Debug.LogWarning("[WARNING] " + message);
    }

    public static void LogError(string message)
    {
        Debug.LogError("[ERROR] " + message);
    }
}