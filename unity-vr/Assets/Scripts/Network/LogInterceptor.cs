using UnityEngine;

public class LogInterceptor : MonoBehaviour
{
    private bool isProcessingLog = false;

    void OnEnable()
    {
        // Subscribe to Unity's global log event when this script is activated
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Unsubscribe when deactivated to prevent memory leaks
        Application.logMessageReceived -= HandleLog;
    }

    // This function automatically runs every time ANY script calls Debug.Log
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Prevent infinite recursion loops if SendEvent triggers a log
        if (isProcessingLog) return;

        // Block any logs originating from the unity-vr Library folder
        if (!string.IsNullOrEmpty(stackTrace) && (stackTrace.Contains("unity-vr\\Library\\") || stackTrace.Contains("unity-vr/Library/")))
        {
            return;
        }

        // We only want to forward standard logs (LogType.Log)
        if (type == LogType.Log)
        {
            // Immediately ignore any log tagged with [Dev]
            if (logString.StartsWith("[Dev]")) return;

            // Only forward logs that match our specific clinical tags
            bool isClinicalLog = logString.StartsWith("[User Action]") ||
                                 logString.StartsWith("[Flight Event]") ||
                                 logString.StartsWith("[System Event]") ||
                                 logString.StartsWith("[Flight Phase]");

            if (isClinicalLog)
            {
                // Check if the SocketManager is ready
                if (SocketManager.instance != null && SocketManager.instance.socket != null)
                {
                    try
                    {
                        isProcessingLog = true;

                        // Forward the clean, tagged log directly to the Node.js server
                        SocketManager.instance.SendEvent("vr_system_log", logString);
                    }
                    finally
                    {
                        isProcessingLog = false;
                    }
                }
            }
        }
    }
}