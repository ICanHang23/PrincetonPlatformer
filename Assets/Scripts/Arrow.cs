using UnityEngine;

public class Arrow : MonoBehaviour
{
    public double angle = 45;

    public void toggleVisibily(bool visible)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = visible;
    }

    public void rotate(int alpha)
    {
        double delta = alpha * 1;

        if (angle + delta < 85 && angle + delta > 10)
        {
            angle += delta;
            Quaternion newRotation = Quaternion.AngleAxis((float) angle, Vector3.forward);
            transform.rotation = newRotation;
        }
    }

    public void xScale(int scale)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        if (scale == -1)
        {
            renderer.flipX = true;
        }
        else
        {
            renderer.flipX = false;
        }
    }
}
