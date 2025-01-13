using UnityEngine;

public static class Debugger
{
    public static bool IsDebugEnabled = true;

    public static void Log(object log)
    {
        if (IsDebugEnabled)
        {
            Debug.Log(log);
        }
    }
    public static void Warning(object warning)
    {
        if (IsDebugEnabled)
        {
            Debug.LogWarning(warning);
        }
    }
    public static void Error(object error)
    {
        if (IsDebugEnabled)
        {
            Debug.LogError(error);
        }
    }
}
