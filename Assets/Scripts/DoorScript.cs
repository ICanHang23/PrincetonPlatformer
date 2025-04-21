using UnityEngine;
using UnityEngine.Splines;
using Newtonsoft.Json;

public class DoorScript : MonoBehaviour
{
    SplineAnimate spline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spline = GetComponent<SplineAnimate>();
    }

    public void Open()
    {
        spline.Play();
    }
}
