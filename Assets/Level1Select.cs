using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Select : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Level1");
    }
}
