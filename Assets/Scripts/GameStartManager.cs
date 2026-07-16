using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    public Button StartButton;
    public Button QuitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        StartButton.onClick.AddListener(GameStart);
        QuitButton.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GameStart()
    {
        SceneManager.LoadScene("Start");
    }
    public static void QuitGame()
    {
        Application.Quit();
    }
}
