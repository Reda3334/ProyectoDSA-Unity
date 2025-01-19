using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MovingObject
{
    //UI RELATED
    public Text FoodText;

    public string playerId;
    public int pointsPerFood = 1;
    public float waitForMessage = 2f;
    public float restartLevelDelay = 1f;
    public GameObject circle;

    public GameManager gameManager;

    private GameObject circleInstance = null;
    private Animator animator;
    private int food = 0;
    private int hp = 1;
    private Vector3 touchOrigin = - Vector3.one;
    private Vector3 currentPosition = Vector3.zero;
    float moveSpeedPerUnit = 5f;


    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        FoodText.text = "Bananas: " + food;
        base.Start();

        InputAction point = InputSystem.actions.FindActionMap("UI").FindAction("Point");
        InputAction click = InputSystem.actions.FindActionMap("UI").FindAction("Click");

        point.performed += obj =>
        {
            currentPosition = Camera.main.ScreenToWorldPoint(obj.ReadValue<Vector2>());
        };

        click.performed += obj =>
        {
            touchOrigin = currentPosition;
            if(circleInstance == null)
            {
                Vector3 circlePosition = currentPosition;
                circlePosition.z = 10;
                circleInstance = Instantiate(circle, circlePosition, Quaternion.identity);
            }
        };

        click.canceled += obj =>
        {
            touchOrigin = -Vector3.one;
            Destroy(circleInstance);
            circleInstance = null;
        };
    }
    private void OnDisable() 
    {
        GameManager.instance.playerFoodPoints = food;
    }
    void Update()
    {
        if (!touchOrigin.Equals(-Vector3.one))
        {
            Vector3 moveSpeed = (currentPosition - touchOrigin) * moveSpeedPerUnit;
            if (moveSpeed.magnitude > 5)
            {
                moveSpeed.Normalize();
                moveSpeed *= 5;
            }
            transform.Translate(moveSpeed * Time.deltaTime);
        }
    }

    private void CheckIfGameOver() {
        if (hp <= 0)
            GameManager.instance.GameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        FoodText.text = "Bananas: " + food;
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
            //Send information to server
            //gameManager.EndGame(playerId,food,gameManager.getLevel());
            if (gameManager.isCustomLevel)
            {
                Loader.CloseGame();
            }
            else
            {
                Invoke("Restart", restartLevelDelay);
                enabled = false;
            }
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            FoodText.text = "+ " + pointsPerFood + " bananas !";
            other.gameObject.SetActive(false);
            Invoke("writeBananas", waitForMessage);
        }
    }
    
    private void writeBananas()
    {
        FoodText.text = "Bananas: " + food;
    }
    
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