using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BackendManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:8080/dsaApp"; // Replace with backend URL, no se cual poner

    public void SendScore(string userID, int score, int level)
    {
        StartCoroutine(SendScoreCoroutine(userID, score, level));
    }

    private IEnumerator SendScoreCoroutine(string userID, int score, int level)
    {
        string url = $"http://localhost:8080/dsaApp//scores/{userID}";
        ScoreData scoreData = new ScoreData { Score = score, Level = level };
        string jsonData = JsonUtility.ToJson(scoreData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Score sent successfully!");
        }
        else
        {
            Debug.LogError($"Error sending score: {request.error}");
        }
    }
}

[System.Serializable]
public class ScoreData
{
    public int Score;
    public int Level;
}