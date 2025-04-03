using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.SceneManagement;

public class UploadScore : MonoBehaviour
{

    private static string url = "http://localhost:5000/receivescore";

    [SerializeField] GameData data;

    public void SendScore(int deaths, float time)
    {
        string level = SceneManager.GetActiveScene().name;
        StartCoroutine(UploadScoreCoroutine(level, deaths, time));
    }

    private static IEnumerator UploadScoreCoroutine(string level, int deaths, float time){

        ScoreData scoreData = new ScoreData
        {
            level = level,
            deaths = deaths,
            time = time
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
        public string level;
        public int deaths;
        public float time;
    }
}
