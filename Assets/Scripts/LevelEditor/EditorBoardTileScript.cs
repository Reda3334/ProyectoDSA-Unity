using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EditorBoardTileScript : MonoBehaviour
{
    [SerializeField] Color mouseOverMask;
    public EditorScript editorScript;

    /*
    private void OnMouseOver()
    {
        gameObject.GetComponent<SpriteRenderer>().color = mouseOverMask;
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    */

    public void OnMouseUp()
    {
        editorScript.RemoveElement(this.transform.position);
    }
}
