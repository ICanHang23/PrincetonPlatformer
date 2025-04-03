using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LevelCompleteUIScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button toMain = root.Q<Button>("NextButton");
        toMain.clicked += () => GotoMain();

        Button restart = root.Q<Button>("RestartButton");
        restart.clicked += () => RestartLevel();
    }

    void GotoMain()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    void RestartLevel()
    {
        Scene level = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(level.name);
    }
}
