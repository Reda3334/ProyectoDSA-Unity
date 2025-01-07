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
    /* UI RELATED
    public float levelstartDelay = 2f;
    private TextMeshProUGUI levelText;
    private GameObject levelImage;
    private bool doingSetup;*/

    private Text levelText;

    public float turnDelay = 0.1f;
    public static GameManager instance = null;
    public BoardManager boardScript;

    public BackendManager backendManager;
    private int level = 1;
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
    private void OnSceneLoaded(Scene s, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }
    void InitGame()
    {
        /*doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<TextMeshProUGUI>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelstartDelay);*/
        //levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.text = "Level " + level;
        enemies.Clear();
        boardScript.SetupScene(level);
    }
    /*
    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    */
    public void GameOver()
    {
        /*levelText.text = "You got till level " + level;
        levelImage.SetActive(true);*/
        enabled = false;
        Debug.Log("Game Over!"); // Mensaje para depurar
        
    }
    void Update()
    {
        if (playersTurn || enemiesMoving /*|| doingSetup*/)
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
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playersTurn = true;
        enemiesMoving = false;
    }

    void EndGame()
    {
        string userID = "4af326f4-c532-11ef-8eb7-0a0027000007"; //id del user = y, email = y, pass = y
        int score = playerFoodPoints; 
        int level = this.level; 

        backendManager.SendScore(userID, score, level);
    } 


}


