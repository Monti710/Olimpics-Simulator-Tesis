using UnityEngine;
using TMPro;

public class VRLogger : MonoBehaviour
{
    public TextMeshProUGUI logText;

    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logText != null)
            logText.text += logString + "\n";
    }
}
