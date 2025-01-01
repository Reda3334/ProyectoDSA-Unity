using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class BackendManager : MonoBehaviour
{
#if UNITY_EDITOR
    private string baseUrl = "http://localhost:8080/dsaApp"; // Replace with backend URL, no se cual poner
    private string cookie = "abc";
    private string playerId = "123";
    private string playerName = "jan";
#else
    private string baseUrl = PlayerPrefs.GetString("baseUrl");
    private string cookie = PlayerPrefs.GetString("cookie");
    private string playerId = PlayerPrefs.GetString("playerId");
    private string playerName = PlayerPrefs.GetString("playerName");
#endif

    public void SendScore(string userID, int score, int level)
    {
        StartCoroutine(SendScoreCoroutine(userID, score, level));
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
                Debug.Log(element);
                if (element == null) continue;
                ElementId script = element.GetComponent<ElementId>();
                if (script == null) continue;
                string elementId = script.elementId;
                if (elementId == null) continue;

                customLevel.elements.Add(new MapElement(elementId, x, y));
            }
        }

        customLevel.levelName = levelName;
        customLevel.userId = playerId;

        string url = $"{baseUrl}/uploadLevel";
        string jsonData = JsonUtility.ToJson(customLevel);
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
    public string id;
    public int x;
    public int y;

    public MapElement(string id, int x, int y)
    {
        this.id = id;
        this.x = x; 
        this.y = y;
    }
}

[System.Serializable]
public class CustomLevel
{
    public List<MapElement> elements = new();
    public string levelName;
    public string userId;
}