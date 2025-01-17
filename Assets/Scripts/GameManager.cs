using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour 
{
    //UI RELATED
    public float levelstartDelay = 2f;
    private GameObject levelImage;
    private bool doingSetup;
    private Text levelText;

    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;

    public BackendManager backendManager;
    private int level = 0;
    public int playerFoodPoints = 0;
    private List<Enemy> enemies;
    private bool enemiesMoving;
    [HideInInspector] public bool playersTurn = true;
     
 

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy> ();
        boardScript = GetComponent<BoardManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnLevelWasLoaded(int level)
    {
        level++;

        InitGame();
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }
    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelstartDelay);

        //levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.text = "Level " + level;
        enemies.Clear();
        boardScript.SetupScene(level);
    }
    
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    
    public void GameOver()
    {
        levelText.text = "You got till level " + level;
        levelImage.SetActive(true);
        enabled = false;
        Debug.Log("Game Over!"); // Mensaje para depurar
        
    }
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            //yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        //enemiesMoving = false;
    }

    public void EndGame(string userID,int score, int level)
    {
        backendManager.SendScore(userID, score, level);
    } 

    public int getLevel()
    {
        return level;
    }


}


