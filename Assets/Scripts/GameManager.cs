using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
    //public float levelstartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;

    //private Text levelText;
    //private GameObject levelImage;
    private bool doingSetup;
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
        InitGame();
    }
    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }
    void InitGame()
    {
        doingSetup = true;
        //levelImage = GameObject.Find("LevelImage");
        //levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.text = "Level " + level;
        //levelImage.SetActive(true);
        //Invoke("HideLevelImage", levelstartDelay);
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    //private void HideLevelImage()
    //{
    //levelImage.SetActive(false);
    //doingSetup = false;
    //}
    public void GameOver()
    {
        //levelText.text = "Has llegado hasta el nivel " + level;
        //levelImage.SetActive(true);
        enabled = false;
    }
    void Update()
    {
        if (playersTurn || enemiesMoving )//|| doingSetup 
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



}


