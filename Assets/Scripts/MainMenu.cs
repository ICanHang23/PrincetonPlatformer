using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuActions : MonoBehaviour
{
    public void OnLevel1Clicked()
    {
        Debug.Log("Level Select Pressed!");
        SceneManager.LoadScene("Level1");
    }

    public void OnTutorialClicked()
    {
        Debug.Log("Tutorial Pressed!");
        SceneManager.LoadScene("Tutorial");
    }
    public void OnLevel2Clicked()
    {
        Debug.Log("Level Select Pressed!");
        SceneManager.LoadScene("Level2");
    }

    public void OnLevel3Clicked()
    {
        Debug.Log("Level Select Pressed!");
        SceneManager.LoadScene("Level3");
    }
    public void OnLevel4Clicked()
    {
        Debug.Log("Level Select Pressed!");
        SceneManager.LoadScene("Level4");
    }
    
}
