using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class PawseScreen : MonoBehaviour
{
    Player player;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        player = GameObject.Find("Player").GetComponent<Player>();

        Button restart = root.Q<Button>("RestartButton");
        restart.clicked += () => RestartLevel();

        Button quit = root.Q<Button>("QuitButton");
        quit.clicked += () => Quit();

        Button cont = root.Q<Button>("ContinueButton");
        cont.clicked += () => Unpause();
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        Scene level = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(level.name);
    }

    void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync("MainMenu");
    }

    void Unpause()
    {
        player.pause();
    }
}
