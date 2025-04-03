using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class UploadScore : MonoBehaviour
{

    private static string url = "http://localhost:5000/receivescore";

    public static void SendScore(string deathString, string timeString)
    {
        string level = SceneManager.GetActiveScene().name;
        GameManager.Instance.StartCoroutine(UploadScoreCoroutine(level, deathString, timeString));
    }

    private static IEnumerator UploadScoreCoroutine(string level, string deathString, string timeString)
    {
        ScoreData scoreData = new ScoreData
        {
            level = level,
            deaths = int.Parse(deathString.Split(':')[1].Trim()), 
            time = float.Parse(timeString.Split(':')[1].Trim()) 
        };

        //Converting to JSON
        string jsonData = JsonUtility.ToJson(scoreData);

        //SEnding over
        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            www.uploadHandler = new UploadHandlerRaw(jsonToSend);
            www.downloadHandler = new DownloadHandlerBuffer();
            

            // Wait for the request to complete
            yield return www.SendWebRequest();

            // Check for errors
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Score submitted successfully!");
            }
            else
            {
                Debug.LogError("Error submitting score: " + www.error);
            }
        }
    }

    // Helper class to serialize the score data into JSON format
    [System.Serializable]
    public class ScoreData
    {
        public int level;
        public int deaths;
        public float time;
    }
}
