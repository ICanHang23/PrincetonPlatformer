using UnityEngine;

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public bool reachedGoal = false;
    public int deathCount = 0;
    public float startTime = 0;

    public string deathString = "Deaths: 0";
    public string timeString = "Time taken: 0 seconds";
}
