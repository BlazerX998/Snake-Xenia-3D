using UnityEngine;
using System.Collections.Generic;


public class OfflineManager : MonoBehaviour
{
    public static OfflineManager Instance;
    private const string OfflineScoresKey = "OfflineScores";

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void SaveScoreOffline(string playerName, int score)
    {
        string offline = PlayerPrefs.GetString(OfflineScoresKey, "");
        offline += $"{playerName}:{score};";
        PlayerPrefs.SetString(OfflineScoresKey, offline);
    }

    public void RetryOfflineSubmissions()
    {
        string offline = PlayerPrefs.GetString(OfflineScoresKey, "");
        if (string.IsNullOrEmpty(offline)) return;

        string[] entries = offline.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (var entry in entries)
        {
            string[] parts = entry.Split(':');
            if (parts.Length == 2)
            {
                BackendManager.Instance.SubmitScore(parts[0], int.Parse(parts[1]));
            }
        }

        PlayerPrefs.DeleteKey(OfflineScoresKey);
    }
}
