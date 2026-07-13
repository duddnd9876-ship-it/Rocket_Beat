using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public Text currentGold;   // 숫자를 표시할 텍스트
    public int userGold = 0;        // 현재 숫자
    public int userUpGold = 1; // 버튼을 누를 때 증가량
    public Button goldButton;
    public Text TargetGold;

    private int clearMissionGold = 999999999;

    void Start()
    {
        goldButton.onClick.AddListener(IncreaseNumber);
        clearMissionGold = int.Parse(TargetGold.text);
    }
    private void FixedUpdate()
    {
        UpdateText();
        if(userGold >= clearMissionGold)
        {
            SceneManager.LoadScene("Ending");
        }

    }

    public void IncreaseNumber()
    {
        userGold += userUpGold;
        UpdateText();
    }

    private void UpdateText()
    {
        currentGold.text = userGold.ToString();
    }
}
