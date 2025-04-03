using UnityEngine;

public class GoalScript : MonoBehaviour
{
    [SerializeField] GameObject levelComplete;
    [SerializeField] GameData game;
    float startTime;
    private void Awake()
    {
        startTime = Time.fixedTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (obj.tag.Equals("Player") && other is BoxCollider2D)
        {
            game.reachedGoal = true;
            game.deathString = "Deaths: " + game.deathCount;
            float elapsedPlayTime = Time.fixedTime - game.startTime;
            game.timeString = "Time taken: " + elapsedPlayTime + " seconds";


            Debug.Log("Congratulations! You completed the level with " + game.deathCount + " deaths!");
            Debug.Log("Completion time: " + (Time.fixedTime - startTime));

            // Putting in a score submission
            us.SendScore(game.deathCount, elapsedPlayTime);

            Instantiate(levelComplete);

        }
    }
}
