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
    [SerializeField] GhostRunData ghostData;
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
        using (UnityWebRequest www = new UnityWebRequest(http.prefix() + "/getghost", "GET"))
        {
            www.downloadHandler = new DownloadHandlerBuffer();
            GhostRunData ghostData = Instance.ghostData;

            // Wait for the request to complete
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                GhostHTTPResponse response = JsonConvert.DeserializeObject<GhostHTTPResponse>(responseText);

                // initialize scriptable objects
                game.ghostDiary = response.inputs;
                ghostData.deaths = response.deaths;
                ghostData.timeTaken = response.time;
                ghostData.level = response.lvl;
                ghostData.netid = response.netid;
                ghostData.build();

                SceneManager.LoadSceneAsync("GhostLanding");

            }
            // CHECK FOR ERRORS HERE
            else if (www.responseCode == 400)
            {
                ghostData.error = "The request to fetch run data was improperly formatted. Please try again or" +
                    " contact the server administrator if the issue persists.";
                ghostData.build();
                SceneManager.LoadSceneAsync("GhostLanding");
            }
            else if (www.responseCode == 404)
            {
                ghostData.error = "We could not find data that matches the run you wanted to spectate. Please try" +
                    " again or contact the server administrator if the issue persists.";
                ghostData.build();
                SceneManager.LoadSceneAsync("GhostLanding");
            }
            else
            {
                SceneManager.LoadSceneAsync("MainMenu");
            }
        }
    }

    class GhostHTTPResponse
    {
        public string netid;
        public string inputs;
        public int lvl;
        public int deaths;
        public float time;
    }
}
