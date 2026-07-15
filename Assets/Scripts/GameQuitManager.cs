using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameQuitManager : MonoBehaviour
{
    public Button QuitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
