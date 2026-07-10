using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GoldManager : MonoBehaviour
{
    public TMP_Text currentGold;   // 숫자를 표시할 텍스트
    public int userGold = 0;        // 현재 숫자
    public int userUpGold = 1; // 버튼을 누를 때 증가량
    public Button goldButton;

    void Start()
    {
        goldButton.onClick.AddListener(IncreaseNumber);
    }
    private void FixedUpdate()
    {
        UpdateText();

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
