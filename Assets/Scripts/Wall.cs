using UnityEngine;

public class Wall : MonoBehaviour
{
    
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
