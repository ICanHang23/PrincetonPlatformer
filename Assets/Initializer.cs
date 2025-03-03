using UnityEngine;

public class Initializer : MonoBehaviour
{
    [SerializeField] GameData data;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data.reachedGoal = false;
        data.deathCount = 0;
        data.startTime = Time.fixedTime;
    }

}
