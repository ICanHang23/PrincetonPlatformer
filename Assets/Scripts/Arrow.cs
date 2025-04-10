using UnityEngine;

public class Arrow : MonoBehaviour
{
    public double angle = 45;
    public int direction = 1;

    int visible_time = 0;

    private void Update()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (visible_time >= 1)
        {
            visible_time -= 1;
        }

        if (visible_time > 0)
        {
            renderer.enabled = true;
        }
        else
        {
            renderer.enabled = false;
        }
    }

    public void resetVisibily()
    {
        visible_time = 40;
        rotate(0);
    }

    public void rotate(double alpha)
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
