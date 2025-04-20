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

    public void Update()
    {
        if (!game.reachedGoal)
        {
            game.updateTime(Time.fixedTime, startTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (obj.tag.Equals("Player") && other is BoxCollider2D)
        {
            game.reachedGoal = true;

            // Log the inputs
            Player player = obj.GetComponent<Player>();
            player.logInputs();

            // Putting in a score submission
            UploadScore.SendScore(game.deathCount, game.elapsed);

            Instantiate(levelComplete);

        }
    }
}
