using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStartManager : MonoBehaviour
{
    public Button StartButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        StartButton.onClick.AddListener(GameStart);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GameStart()
    {
        SceneManager.LoadScene("Start");
    }
}
