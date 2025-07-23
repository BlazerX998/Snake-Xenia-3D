using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BackendManager : MonoBehaviour
{
    public static BackendManager instance;

    public string submitUrl ="https://sravwofs77.execute-api.us-east-1.amazonaws.com/prod/submit-score";
    public string leaderboardUrl = "https://sravwofs77.execute-api.us-east-1.amazonaws.com/prod/leaderboard";

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    [System.Serializable]
    public class ScoreData
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
    }

    [System.Serializable]
    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> leaderboard;
    }

    public void SubmitScore(string playerName, int score)
    {
        StartCoroutine(SubmitScoreCoroutine(playerName, score));
    }

    private IEnumerator SubmitScoreCoroutine(string playerName, int score)
    {
        ScoreData data = new ScoreData { playerName = playerName, score = score };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest www = new UnityWebRequest(submitUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score submitted successfully!");
            UIManager.instance?.ShowMessage("Score Submitted!");
        }
        else
        {
            Debug.LogError("Score submission failed: " + www.error);
            UIManager.instance?.ShowMessage("Submission failed. Saving offline...");
            SaveScoreOffline(playerName, score);
        }
    }

    public void GetLeaderboard()
    {
        StartCoroutine(GetLeaderboardCoroutine());
    }

    private IEnumerator GetLeaderboardCoroutine()
    {
        UnityWebRequest www = UnityWebRequest.Get(leaderboardUrl);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            LeaderboardResponse data = JsonUtility.FromJson<LeaderboardResponse>("{\"leaderboard\":" + json + "}");
            LeaderboardUI.instance?.DisplayLeaderboard(data.leaderboard);
        }
        else
        {
            Debug.LogWarning(" Failed to fetch leaderboard: " + www.error);
        }
    }

    [System.Serializable]
    public class OfflineScores
    {
        public List<ScoreData> scores = new List<ScoreData>();
    }

    private void SaveScoreOffline(string playerName, int score)
    {
        OfflineScores existing = LoadOfflineScores();
        existing.scores.Add(new ScoreData { playerName = playerName, score = score });
        string json = JsonUtility.ToJson(existing);
        PlayerPrefs.SetString("OfflineScores", json);
        PlayerPrefs.Save();
    }

    private OfflineScores LoadOfflineScores()
    {
        string json = PlayerPrefs.GetString("OfflineScores", "");
        if (string.IsNullOrEmpty(json)) return new OfflineScores();
        return JsonUtility.FromJson<OfflineScores>(json);
    }

    public void RetryOfflineSubmissions()
    {
        OfflineScores offline = LoadOfflineScores();
        foreach (var score in offline.scores)
        {
            StartCoroutine(SubmitScoreCoroutine(score.playerName, score.score));
        }
        PlayerPrefs.DeleteKey("OfflineScores");
    }
}

