using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Player : MovingObject
{
    /*UI RELATED
    public TextMeshProUGUI FoodText;*/
    public int pointsPerFood = 1;
    public float waitForMessage = 2f;
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food = 0;
    private int hp = 1;
    private Vector2 touchOrigin = -Vector2.one;



    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        //FoodText.text = "Bananas: " + food;
        base.Start();
    }
    private void OnDisable() 
    {
        GameManager.instance.playerFoodPoints = food;
    }
    void Update()
    {
        if (!GameManager.instance.playersTurn) return;

        float moveSpeed = 5f;
        int horizontal = 0;
        int vertical = 0;
     #if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER)
        
            horizontal = (int)(Input.GetAxisRaw("Horizontal"));
            vertical = (int)(Input.GetAxisRaw("Vertical"));
            /*if (horizontal != 0)
                vertical = 0;
            if (horizontal != 0 || vertical != 0)
                AttemptMove<Wall>(horizontal, vertical);*/
            Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;
            transform.Translate(movement * moveSpeed * Time.deltaTime);

     # else
        
            if (Input.touchCount > 0)
            {
                Touch myTouch = Input.touches[0];

                if (myTouch.phase == TouchPhase.Began)
                {
                    touchOrigin = myTouch.position;
                }

                else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
                {
                    Vector2 touchEnd = myTouch.position;
                    float x = touchEnd.x - touchOrigin.x;
                    float y = touchEnd.y - touchOrigin.y;
                    touchOrigin.x = -1;
                    if (Mathf.Abs(x) > Mathf.Abs(y))
                    {
                        horizontal = x > 0 ? 1 : -1;
                    }
                    else 
                    {
                        vertical = y > 0 ? 1 : -1;
                    }

                }
            }
#endif
    }

    private void CheckIfGameOver() {
        if (hp <= 0)
            GameManager.instance.GameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //FoodText.text = "Bananas: " + food;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }
    protected void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            //FoodText.text = "+ " + pointsPerFood + " bananas !" + " Bananas: " + food;
            other.gameObject.SetActive(false);
            //Invoke("writeBananas", waitForMessage);
        }
    }
    /*
    private void writeBananas()
    {
        FoodText.text = "Bananas: " + food;
    }
    */
    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
    }

    public void LoseHp(int loss) 
    {
        hp -= loss;
        CheckIfGameOver();
    }



}