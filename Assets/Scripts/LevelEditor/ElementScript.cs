using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ElementScript : MonoBehaviour, IBeginDragHandler
{
    public GameObject elementDraggable;
    public Canvas canvas;
    public EditorScript editorScript;
    public string elementId;
    public GameObject placeable;

    public void OnBeginDrag(PointerEventData data)
    { 

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
        elementDrag.elementId = elementId;
        elementDrag.placeable = placeable;
        o.GetComponent<Image>().sprite = GetComponent<Image>().sprite;
        data.pointerDrag = o; // https://discussions.unity.com/t/how-to-drag-ui-element-immediately-after-instantiating-spawning-it-on-tap-click/223761/4
    }
}
