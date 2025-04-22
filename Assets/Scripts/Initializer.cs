using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Initializer : MonoBehaviour
{
    [SerializeField] GameData data;
    [SerializeField] GameObject ghost;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        data.Reset();

        GameObject pauseScreen = GameObject.Find("PauseScreen");
        if (pauseScreen != null)
        {
            pauseScreen.SetActive(false);
        }

        // StartCoroutine(GetRequest("http://localhost:5000"));


        if (!data.ghostDiary.Equals(""))
        {
            Vector3 ghostPosition = new Vector3(-5, 0, 0);
            Instantiate(ghost, ghostPosition, Quaternion.identity);
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}
