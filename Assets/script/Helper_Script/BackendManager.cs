using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class BackendManager : MonoBehaviour
{
    [Header("API Endpoints (Replace with actual)")]
    [SerializeField] private string submitUrl = "https://sravwofs77.execute-api.us-east-1.amazonaws.com/prod/submit-score";
    [SerializeField] private string leaderboardUrl = "https://sravwofs77.execute-api.us-east-1.amazonaws.com/prod/leaderboard";

    [Header("Debug")]
    [SerializeField] private bool printDebugLogs = true;

    public void SubmitScore(string playerName, int score)
    {
        StartCoroutine(SubmitScoreRoutine(playerName, score));
    }

    public void GetLeaderboard()
    {
        StartCoroutine(GetLeaderboardRoutine());
    }

    private IEnumerator SubmitScoreRoutine(string playerName, int score)
    {
        if (printDebugLogs) Debug.Log($"Submitting Score: {playerName} - {score}");

        ScoreData data = new ScoreData(playerName, score);
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(submitUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[SubmitScore] Error: {request.error}");
        }
        else
        {
            Debug.Log("[SubmitScore] Success: " + request.downloadHandler.text);
        }
    }

    private IEnumerator GetLeaderboardRoutine()
    {
        if (printDebugLogs) Debug.Log("Fetching leaderboard...");

        UnityWebRequest request = UnityWebRequest.Get(leaderboardUrl);
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"[Leaderboard] Error: {request.error}");
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("[Leaderboard] Raw Response: " + response);
         
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public string playerName;
        public int score;

        public ScoreData(string name, int s)
        {
            playerName = name;
            score = s;
        }
    }
}
