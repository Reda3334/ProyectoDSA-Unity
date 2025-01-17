using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementScript : MonoBehaviour, IBeginDragHandler
{
    public GameObject elementDraggable;
    public Canvas canvas;
    public EditorScript editorScript;
    public GameObject placeable;

    private TMP_Text textMeshPro;
    private int quantity;
    public int Quantity
    {
        set
        {
            quantity = value;
            if(textMeshPro != null) textMeshPro.text = quantity.ToString();
        }
        get
        {
            return quantity;
        }
    }

    void Start()
    {
        textMeshPro = GetComponentInChildren<TMP_Text>();
        textMeshPro.text = quantity.ToString();
    }

    public void OnBeginDrag(PointerEventData data)
    { 
        if(Quantity <= 0)
        {
            Quantity = 0; // just in case
            return;
        }
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)canvas.transform,
            data.position,
            canvas.worldCamera,
            out position);

        GameObject o = Instantiate(elementDraggable, canvas.transform.TransformPoint(position), Quaternion.identity, canvas.transform);
        ElementDrag elementDrag = o.GetComponent<ElementDrag>();
        elementDrag.canvas = canvas;
        elementDrag.editorScript = editorScript;
        elementDrag.placeable = placeable;
        elementDrag.elementScript = this;
        o.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        data.pointerDrag = o; // https://discussions.unity.com/t/how-to-drag-ui-element-immediately-after-instantiating-spawning-it-on-tap-click/223761/4
    }
}
