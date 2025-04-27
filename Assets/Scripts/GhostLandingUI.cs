using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class GhostLandingUI : MonoBehaviour
{
    [SerializeField] GhostRunData data;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button back = root.Q<Button>("BackButton");
        back.clicked += () => Back();

        Button play = root.Q<Button>("PlayButton");

        if (data.error == "")
        {
            play.clicked += () => Play();
        }
        else
        {
            play.style.backgroundColor = Color.gray;
        }
    }

    void Play()
    {
        SceneManager.LoadSceneAsync("Level" + data.level);
    }

    void Back()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

}
