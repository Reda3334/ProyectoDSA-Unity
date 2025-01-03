using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraDrag : MonoBehaviour
{
    private Vector2 currentPosition = Vector2.zero;
    private Vector3 initialPosition = Vector2.zero;
    private bool isDragging = false;
    public float scrollSpeed = 0.01f;

    private void Awake()
    {
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
            if (IsPointerOverUI()) return;
            StartCoroutine(Drag());
        };
        click.canceled += _ => 
        {
            isDragging = false;
        };
        
    }

    private IEnumerator Drag()
    {
        isDragging = true;
        initialPosition = Camera.main.ScreenToWorldPoint(currentPosition);
        while (isDragging)
        {
            transform.Translate(initialPosition - Camera.main.ScreenToWorldPoint(currentPosition), Space.World); // inverted because we move in opposite direction from scroll
            yield return null;
        }
    }

    private bool IsPointerOverUI()
    {
        List<RaycastResult> eventSystemRaycastResults = GetEventSystemRaycastResults();
        for (int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            RaycastResult curRaycastResult = eventSystemRaycastResults[index];
            if (curRaycastResult.gameObject.CompareTag("BlockingUI")) // detect if the component is the right panel
                return true;
        }
        return false;
    }

    List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = currentPosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        return raycastResults;
    }
}
