using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BackendManager : MonoBehaviour
{
    private string submitUrl = "https://your-api-id.execute-api.region.amazonaws.com/prod/submit-score";
    private string leaderboardUrl = "https://your-api-id.execute-api.region.amazonaws.com/prod/leaderboard";

    public void SubmitScore(string playerName, int score)
    {
        StartCoroutine(SubmitScoreCoroutine(playerName, score));
    }

    private IEnumerator SubmitScoreCoroutine(string playerName, int score)
    {
        var jsonData = JsonUtility.ToJson(new ScoreData(playerName, score));
        UnityWebRequest request = new UnityWebRequest(submitUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error submitting score: " + request.error);
        }
        else
        {
            Debug.Log("Score submitted successfully!");
        }
    }

    public void GetLeaderboard(System.Action<List<ScoreData>> onResult)
    {
        StartCoroutine(GetLeaderboardCoroutine(onResult));
    }

    private IEnumerator GetLeaderboardCoroutine(System.Action<List<ScoreData>> onResult)
    {
        UnityWebRequest request = UnityWebRequest.Get(leaderboardUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error fetching leaderboard: " + request.error);
            onResult?.Invoke(null);
        }
        else
        {
            string json = "{\"items\":" + request.downloadHandler.text + "}";
            LeaderboardWrapper wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
            onResult?.Invoke(wrapper.items);
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public string playerName;
        public int score;

        public ScoreData(string name, int score)
        {
            this.playerName = name;
            this.score = score;
        }
    }

    [System.Serializable]
    public class LeaderboardWrapper
    {
        public List<ScoreData> items;
    }
}
