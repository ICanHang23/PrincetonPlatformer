using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine("Goto");
        }
    }

    void Goto()
    {
        SceneManager.LoadSceneAsync("Level1");
    }
}
