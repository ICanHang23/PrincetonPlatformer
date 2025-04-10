using UnityEngine;

public class Arrow : MonoBehaviour
{
    public void toggleVisibily(bool visible)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = visible;
    }
}
