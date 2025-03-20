using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Select : MonoBehaviour
{
    public void SelectLevel()
    {
        SceneManager.LoadSceneAsync("Level1");
    }
}
