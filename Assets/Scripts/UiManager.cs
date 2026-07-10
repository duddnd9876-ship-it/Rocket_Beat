using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public GameObject ChartPanel;
    public GameObject ShopPanel;
    public Button ChartUiButton;
    public Button ShopUiButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChartPanel.SetActive(false);

        ChartUiButton.onClick.AddListener(OpenChart);
        ShopUiButton.onClick.AddListener(OpenShop);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OpenChart()
    {
        ChartPanel.SetActive(true);
        ShopPanel.SetActive(false);
    }
    private void OpenShop()
    {
        ChartPanel.SetActive(false);
        ShopPanel.SetActive(true);
    }
}
