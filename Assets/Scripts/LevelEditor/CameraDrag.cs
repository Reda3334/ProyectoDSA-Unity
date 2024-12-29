using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    private Vector3 previousPosition = Vector2.zero;

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                previousPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (previousPosition.Equals(Vector2.zero)) return;
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.Translate(previousPosition - newPosition, Space.World); // inverted because we move in opposite direction from scroll
                                                                                  // we don't need to update the previousPosition as it's always the same! the camera has moved, so we are pointing at the same place
            }

            if (Input.GetMouseButtonUp(0))
            {
                previousPosition = Vector2.zero;
            }
        }

    }
}
