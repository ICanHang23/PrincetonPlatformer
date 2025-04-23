using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class WaveMover : MonoBehaviour
{
    [SerializeField] float delay = 0;
    SplineAnimate spline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spline = GetComponent<SplineAnimate>();
        StartCoroutine(ExampleCoroutine());
    }


    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(delay);
        spline.Play();
    }
}
