using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using Newtonsoft.Json;

public class GhostRedirect : MonoBehaviour
{
    public static GhostRedirect Instance { get; private set; }
    [SerializeField] GameData game;
    [SerializeField] HTTPData http;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;  // Set the singleton instance
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }

        redirect();
    }

    static void redirect()
    {
        Instance.StartCoroutine(FetchGhostCoroutine(Instance.http, Instance.game));
    }

    private static IEnumerator FetchGhostCoroutine(HTTPData http, GameData game)
    {
        // Sending over
        using (UnityWebRequest www = new UnityWebRequest(http.prefix + "/getghost", "GET"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();

            // Wait for the request to complete
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                GhostHTTPResponse response = JsonConvert.DeserializeObject<GhostHTTPResponse>(responseText);
                game.ghostDiary = response.inputs;
                SceneManager.LoadSceneAsync("Level" + response.lvl);

            }
            // CHECK FOR ERRORS HERE
            else
            {
                SceneManager.LoadSceneAsync("MainMenu");
            }
        }
    }

    class GhostHTTPResponse
    {
        public string inputs;
        public int lvl;
        public int deaths;
        public float time;
    }
}
