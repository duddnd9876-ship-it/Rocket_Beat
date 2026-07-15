using Unity.AppUI.UI;
using UnityEngine;

public class GameQuitManager : MonoBehaviour
{
    public Button QuitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void QuitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }
}
