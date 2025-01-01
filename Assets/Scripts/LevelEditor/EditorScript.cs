using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditorScript : MonoBehaviour
{

    [SerializeField] GameObject[] availableElements;
    [SerializeField] GameObject elementIcon;
    [SerializeField] GameObject draggable;
    [SerializeField] Canvas canvas;
    [SerializeField] int editorWidth;
    [SerializeField] int editorHeight;
    [SerializeField] GameObject[] floorTiles;
    [SerializeField] GameObject scrollContent;
    [SerializeField] Transform boardHolder;
    [SerializeField] Transform elementHolder;
    [SerializeField] BackendManager backendManager;

    private GameObject[,] elements;
    private Vector2 currentPosition, startPosition;

    void Start()
    {
        elements = new GameObject[editorWidth, editorHeight];
        DrawBoard(editorWidth, editorHeight);
        DrawObjects();

        InputAction point = InputSystem.actions.FindActionMap("UI").FindAction("Point");
        InputAction click = InputSystem.actions.FindActionMap("UI").FindAction("Click");
        point.Enable();
        click.Enable();

        point.performed += obj =>
        {
            currentPosition = obj.ReadValue<Vector2>();
        };
        click.performed += obj =>
        {
            startPosition = currentPosition;
        };
        click.canceled += _ =>
        {
            if (Vector2.Distance(currentPosition, startPosition) < 5) return; // tune this number
            RemoveElement(Camera.main.ScreenToWorldPoint(currentPosition));
        };
    }

    public void AddElementAndDraw(GameObject o, Vector3 location)
    {
        if (location.x > editorWidth || location.y > editorHeight) return;

        if(elements[(int)location.x, (int)location.y] != null)
        {
            Destroy(elements[(int)location.x, (int)location.y]);
        }
        GameObject instance = Instantiate(o, location, Quaternion.identity, elementHolder);
        instance.GetComponent<SpriteRenderer>().sortingLayerName = "Items"; // we make sure everything is visible above the board
        elements[(int)location.x, (int)location.y] = instance;
    }

    public void RemoveElement(Vector3 location)
    {
        int x = Mathf.RoundToInt(location.x);
        int y = Mathf.RoundToInt(location.y);
        if (location.x > editorWidth || location.y > editorHeight) return;
        Debug.Log(x + " " + y);
        if (elements[x, y] != null)
        {
            Destroy(elements[x, y]);
            elements[x, y] = null;
        }
    }

    private void DrawObjects()
    {
        foreach(GameObject o in availableElements)
        {
            GameObject instance = Instantiate(elementIcon, scrollContent.transform);
            //instance.transform.localPosition = location;
            instance.GetComponent<Image>().sprite = o.GetComponent<SpriteRenderer>().sprite;
            ElementScript elementScript = instance.GetComponent<ElementScript>();
            elementScript.canvas = canvas;
            elementScript.elementDraggable = draggable;
            elementScript.editorScript = this;
            elementScript.placeable = o;
        }
    }

    private void DrawBoard(int width, int height)
    {

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    public void AskSaveLevel()
    {
        SaveLevel("aa");
        //TODO confirmation popup
    }

    public void SaveLevel(string levelName)
    {
        backendManager.SaveLevel(elements, levelName);
#if UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("finish");
#endif
    }
}
