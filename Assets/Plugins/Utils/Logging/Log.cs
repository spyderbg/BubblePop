namespace Utils.Logging {
    
/// <summary>
/// Provides the main logging mechanism in the project; wraps around Unity's logger.
/// </summary>
public static class Log
{
    public static void Error(string format, params object[] args)
    {
        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    public static void Warning(string format, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    public static void Message(string format, params object[] args)
    {
        UnityEngine.Debug.LogFormat(format, args);
    }

    public static void Debug(string format, params object[] args)
    {
        var message = string.Format(format, args);
        var stamped = $"[debug]::{message}";
        UnityEngine.Debug.Log(stamped);
    }

    public static void Info(string format, params object[] args)
    {
        var message = string.Format(format, args);
        var stamped = $"[info]::{message}";
        UnityEngine.Debug.Log(stamped);
    }

    public static void Dev(string format, params object[] args)
    {
        var message = string.Format(format, args);
        var stamped = $"[DEV]::{message}";
        UnityEngine.Debug.Log(stamped);            
    }
}

} // namespace Utils.Logging 

