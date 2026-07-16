using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameQuitManager : MonoBehaviour
{
    public Button QuitButton;
    public Button MainBackButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QuitButton.onClick.AddListener(QuitGame);
        MainBackButton.onClick.AddListener(MainBack);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public static void QuitGame()
    {
        Application.Quit();
    }

    public void MainBack()
    {
        SceneManager.LoadScene("Main");

    }
}
