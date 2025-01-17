using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditorScript : MonoBehaviour
{

    [SerializeField] GameObject[] availableElements;
    private Dictionary<string, GameObject> availableElementsInstances = new();
    [SerializeField] GameObject elementIcon;
    [SerializeField] GameObject draggable;
    [SerializeField] Canvas canvas;
    private int editorWidth = BoardManager.columns;
    private int editorHeight = BoardManager.rows;
    [SerializeField] GameObject[] floorTiles;
    [SerializeField] GameObject scrollContent;
    [SerializeField] GameObject confirmationPanel;
    [SerializeField] TMPro.TMP_InputField levelName;
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
            if (Vector2.Distance(currentPosition, startPosition) < 5) // tune this number
            {
                RemoveElement(Camera.main.ScreenToWorldPoint(currentPosition));
            }
            
        };
    }

    public bool AddElementAndDraw(GameObject o, Vector3 location)
    {
        if (location.x > editorWidth || location.y > editorHeight) return false;

        if(elements[(int)location.x, (int)location.y] != null)
        {
            RemoveElement(location);
        }
        GameObject instance = Instantiate(o, location, Quaternion.identity, elementHolder);
        instance.GetComponent<SpriteRenderer>().sortingLayerName = "Items"; // we make sure everything is visible above the board
        elements[(int)location.x, (int)location.y] = instance;
        return true;
    }

    public void RemoveElement(Vector3 location)
    {
        int x = Mathf.RoundToInt(location.x);
        int y = Mathf.RoundToInt(location.y);
        if (location.x > editorWidth || location.y > editorHeight) return;
        if (elements[x, y] != null)
        {
            GameObject element = elements[x, y];
            Destroy(elements[x, y]);
            elements[x, y] = null;

            if (element == null) return;
            ElementId script = element.GetComponent<ElementId>();
            if (script == null) return;
            string elementId = script.elementId;
            if (elementId == null) return;
            
            if(availableElementsInstances.TryGetValue(elementId, out element))
            {
                element.GetComponent<ElementScript>().Quantity++;
            }
        }
    }

    [System.Serializable]
    class InventoryObject
    {
        public string userID;
        public string objectID;
        public double price;
        public int quantity;
        public string url;
        public string description;
        public string name;
    }

    [System.Serializable]
    class RootJSONObject
    {
        public InventoryObject[] objects;
    }

    private void DrawObjects()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var intent = activity.Call<AndroidJavaObject>("getIntent");
        string json = intent.Call<string>("getStringExtra", "userItems");
#else
        string json = "[{\"description\":\"descripció del plàtan\",\"name\":\"Pared Interior\",\"objectID\":\"innerWall\",\"price\":3.0,\"quantity\":1,\"url\":\"images/platano.jpg\",\"userID\":\"6a8abac5-d446-11ef-9e0a-8038fb17f5f1\"}]";
#endif
        RootJSONObject jsonObject = JsonUtility.FromJson<RootJSONObject>("{\"objects\": " + json + "}");
        List<InventoryObject> objects = jsonObject.objects.ToList();

        foreach (GameObject o in availableElements)
        {
            GameObject instance = Instantiate(elementIcon, scrollContent.transform);
            //instance.transform.localPosition = location;
            instance.GetComponent<Image>().sprite = o.GetComponent<SpriteRenderer>().sprite;
            ElementScript elementScript = instance.GetComponent<ElementScript>();
            elementScript.canvas = canvas;
            elementScript.elementDraggable = draggable;
            elementScript.editorScript = this;
            elementScript.placeable = o;
            elementScript.Quantity = 3;

            if (!o.TryGetComponent<ElementId>(out var elementId)) return;
            string objectId = elementId.elementId;
            if(objectId == null) return;

            availableElementsInstances.Add(objectId, instance);

#if UNITY_ANDROID && !UNITY_EDITOR
            InventoryObject io = objects.Find(x => x.objectID == objectId);
            if(io == null)
            {
                elementScript.Quantity = 0;
            }
            else
            {
                elementScript.Quantity = io.quantity;
            }
#endif

        }
    }

    private void DrawBoard(int width, int height)
    {

        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    public void AskSaveLevel()
    {
        confirmationPanel.SetActive(true);
    }

    public void ConfirmSave()
    {
        if(levelName.text != "")
        {
            backendManager.SaveLevel(elements, levelName.text);
        }
        else
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJavaClass unityWrapper = new AndroidJavaClass("com.example.proyectodsa_android.activity.UnityWrapperActivity");
            unityWrapper.CallStatic("sendToast", "Debe introducir un nombre!");
#endif
        }
    }

    public void CancelSave()
    {
        confirmationPanel.SetActive(false);
    }

    public void CloseEditor()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityWrapper = new AndroidJavaClass("com.example.proyectodsa_android.activity.UnityWrapperActivity");
        unityWrapper.CallStatic("closeActivity");
#endif
    }
}
