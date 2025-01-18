using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class BackendManager : MonoBehaviour
{
    private string baseUrl;
    private string cookie;
    private string playerId;
    private string playerName;

    public void SendScore(string userID, int score, int level)
    {
        StartCoroutine(SendScoreCoroutine(userID, score, level));
    }

    private void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        baseUrl = PlayerPrefs.GetString("baseUrl");
        cookie = PlayerPrefs.GetString("cookie");
        playerId = PlayerPrefs.GetString("userId");
        playerName = PlayerPrefs.GetString("playerName");
#else
        baseUrl = "http://localhost:8080/dsaApp"; // Replace with backend URL, no se cual poner
        cookie = "abc";
        playerId = "123";
        playerName = "jan";
#endif
    }

    private IEnumerator SendScoreCoroutine(string userID, int score, int level)
    {
        string url = $"{baseUrl}/scores/{userID}";
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

    public void SaveLevel(GameObject[,] elements, string levelName)
    {
        StartCoroutine(SaveLevelCoroutine(elements, levelName));
    }

    public IEnumerator SaveLevelCoroutine(GameObject[,] elements, string levelName)
    { 
        CustomLevel customLevel = new();
        for (int x = 0; x < elements.GetLength(0); x++)
        {
            for(int y = 0;  y < elements.GetLength(1); y++)
            {
                GameObject element = elements[x, y];
                if (element == null) continue;
                ElementId script = element.GetComponent<ElementId>();
                if (script == null) continue;
                string elementId = script.elementId;
                if (elementId == null) continue;

                customLevel.elements.Add(new MapElement(elementId, x, y));
            }
        }

        customLevel.levelName = levelName;
        customLevel.userName = playerName;

        string url = $"{baseUrl}/uploadLevel";
        string jsonData = JsonUtility.ToJson(customLevel);

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityWrapper = new AndroidJavaClass("com.example.proyectodsa_android.activity.UnityWrapperActivity");
        unityWrapper.CallStatic("sendNewLevel", jsonData);
        yield return null;
#else
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Level sent successfully!");
        }
        else
        {
            Debug.LogError($"Error sending level: {request.error}");
        }
#endif
    }
}

[System.Serializable]
public class ScoreData
{
    public int Score;
    public int Level;
}

[System.Serializable]
public class MapElement
{
    public string elementId;
    public int x;
    public int y;

    public MapElement(string id, int x, int y)
    {
        this.elementId = id;
        this.x = x; 
        this.y = y;
    }
}

[System.Serializable]
public class CustomLevel
{
    public List<MapElement> elements = new();
    public string levelName;
    public string userName;
}