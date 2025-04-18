using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public bool reachedGoal = false;
    public int deathCount = 0;
    public float elapsed = 0;

    public string deathString = "Deaths: 0";
    public string timeString = "Time taken: 0 seconds";
    public string timeStringGUI = "Time taken: 0";

    public void Reset()
    {
        reachedGoal = false;
        deathCount = 0;
        elapsed = 0;
        deathString = "Deaths: 0";
        timeString = "Time taken: 0 seconds";
        timeStringGUI = "Time taken: 0";
    }
    
    public void addDeath()
    {
        deathCount++;
        deathString = "Deaths: " + deathCount;
    }

    public void updateTime(float current, float start)
    {
        elapsed = current - start;
        timeStringGUI = "Time taken: " + (Mathf.Round(elapsed * 100) / 100);
        timeString = timeStringGUI + " seconds";
    }
}
