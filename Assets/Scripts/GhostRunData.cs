using UnityEngine;

[CreateAssetMenu]
public class GhostRunData : ScriptableObject
{
    public float timeTaken = 0;
    public int level = 1;
    public int deaths = 0;
    public string netid = "";
    public string error = "";

    public string header = "";
    public string statOverview = "";
    public string coolMessage = "";
    
    public void build()
    {
        if (error == "")
        {
            // build header
            header = "You're about to play Level " + level + " alongside " + netid + ".";

            // make rounded time
            float timeRound = Mathf.Round(timeTaken * 10) / 10;

            // build overview
            statOverview = netid + " completed the level in " + timeRound + " seconds";
            if (deaths == 0)
            {
                statOverview += " without dying.";
            }
            else if (deaths == 1)
            {
                statOverview += " and died once.";
            }
            else if (deaths == 2)
            {
                statOverview += " and died twice.";
            }
            else
            {
                statOverview += " and died " + deaths + " times.";
            }

            // make message
            int n = Random.Range(0, 3);

            switch (n) {
                case 0:
                    coolMessage = "Ready to play?";
                    break;
                case 1:
                    coolMessage = "Can you do better?";
                    break;
                case 2:
                    coolMessage = "Ready to hop in?";
                    break;
            }
        }

        else
        {
            header = "An error occured.";
        }
    }
}
