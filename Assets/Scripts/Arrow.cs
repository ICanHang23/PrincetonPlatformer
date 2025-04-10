using UnityEngine;

public class Arrow : MonoBehaviour
{
    public double angle = 45;
    public float scale_x = 5;
    public int direction = 1;


    public void toggleVisibily(bool visible)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.enabled = visible;

        if (visible)
        {
            transform.localScale = new Vector3(scale_x, 5, 5);
        }
    }

    public void rotate(int alpha)
    {
        double delta = alpha * 1;

        if (angle + delta < 85 && angle + delta > 10)
        {
            angle += delta;
        }

        double toRotate = angle;

        if (direction == -1)
        {
            double gamma = 2 * (90 - angle);
            toRotate += gamma;
        }

        Quaternion newRotation = Quaternion.AngleAxis((float)toRotate, Vector3.forward);
        transform.rotation = newRotation;
    }

    public void dir(int scale)
    {
        direction = scale;
    }
}
