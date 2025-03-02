using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField] GameObject levelComplete;

    private void Awake()
    {
        Debug.Log("I exist");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        Debug.Log("Detected");

        if (obj.tag.Equals("Player"))
        {
            GameScript.reachedGoal = true;
            Instantiate(levelComplete);
        }
    }
}
