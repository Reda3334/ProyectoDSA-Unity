using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ElementDrag : MonoBehaviour
{
    public Canvas canvas;
    public EditorScript editorScript;
    public string elementId;
    public GameObject placeable;

    public void DragHandler(BaseEventData eventData)
    {
        PointerEventData data = (PointerEventData) eventData;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform) canvas.transform,
            data.position,
            canvas.worldCamera,
            out position);

        transform.position = canvas.transform.TransformPoint(position);
    }

    public void DropHandler(BaseEventData eventData)
    {
        PointerEventData data = ( PointerEventData) eventData;
        Destroy(gameObject);
        Vector3 location = Camera.main.ScreenToWorldPoint(data.position);
        location.x = Mathf.RoundToInt(location.x);
        location.y = Mathf.RoundToInt(location.y);
        location.z = 10; // higher than ground

        editorScript.AddElementAndDraw(placeable, location);
    }
}
