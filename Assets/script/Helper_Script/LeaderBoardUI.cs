using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Transform entryContainer;

    [Header("UI Feedback")]
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private GameObject retryButton;

    private void OnEnable()
    {
        FetchLeaderboardWithFeedback();
    }

    public void FetchLeaderboardWithFeedback()
    {
        ShowLoading(true);
        ShowStatus("Fetching leaderboard...");
        retryButton.SetActive(false);

        BackendManager.Instance.FetchLeaderboard((leaderboard) =>
        {
            ShowLoading(false);

            if (leaderboard == null || leaderboard.Count == 0)
            {
                ShowStatus("Failed to load leaderboard.");
                retryButton.SetActive(true);
                return;
            }

            ShowStatus("Leaderboard loaded!");
            retryButton.SetActive(false);
            PopulateLeaderboard(leaderboard);

            SaveOfflineLeaderboard(leaderboard);
        });
    }

    private void PopulateLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        foreach (Transform child in entryContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (LeaderboardEntry entry in leaderboard)
        {
            GameObject go = Instantiate(entryPrefab, entryContainer);
            TMP_Text[] texts = go.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 2)
            {
                texts[0].text = entry.name;
                texts[1].text = entry.score.ToString();
            }
        }
    }

    private void ShowLoading(bool show)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(show);
    }

    private void ShowStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.gameObject.SetActive(true);
        }
    }

    public void OnRetryButtonPressed()
    {
        FetchLeaderboardWithFeedback();
    }

    private void SaveOfflineLeaderboard(List<LeaderboardEntry> leaderboard)
    {
        string json = JsonUtility.ToJson(new LeaderboardWrapper { entries = leaderboard });
        PlayerPrefs.SetString("OfflineLeaderboard", json);
    }

    public void LoadOfflineLeaderboard()
    {
        string json = PlayerPrefs.GetString("OfflineLeaderboard", "");
        if (!string.IsNullOrEmpty(json))
        {
            var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(json);
            PopulateLeaderboard(wrapper.entries);
            ShowStatus("Showing cached leaderboard (offline).");
        }
        else
        {
            ShowStatus("No offline data available.");
        }
    }

    [System.Serializable]
    public class LeaderboardWrapper
    {
        public List<LeaderboardEntry> entries;
    }
}
