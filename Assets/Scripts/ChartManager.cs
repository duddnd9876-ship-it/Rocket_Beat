using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class ChartManager : MonoBehaviour
{
    public GoldManager goldManager;

    public TMP_Text ChartNameText;
    public TMP_Text ChartValueText;
    public TMP_Text ChartPriceText;
    public TMP_Text ChartPiecesText;
    public Button ChartBuyButton;
    public Button ChartSellButton;
    public TMP_Text currentGold;
    public TMP_Text MaxBuyText;
    public Button MaxBuyUpButton;
    public Button MaxBuyDownButton;

    private double ChartPrice;
    private string[] chartName = new string[] { "Floor tile construction", "King of Janggi", "Cure Corporation", "CAOCAO constraints", "will do it yesterday", "white streaming", "Markdown SP", "Rocket Coin" };
    private int[] chartPrice = new int[] { 100, 15000, 5000, 20000, 30000, 50000, 100000, 500000 };
    private string[] chartValue = new string[] { "제조업", "식료품", "IT", "비트코인" };
    private int ChartPieces = 0;
    private int maxBuy = 1;


    private List<double> chartHistory = new List<double>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 0번째 회사 가격으로 시작
        ChartPrice = chartPrice[0];
        ChartPriceText.text = ChartPrice.ToString();
        ChartPiecesText.text = ChartPieces.ToString();
        MaxBuyText.text = maxBuy.ToString();

        // 처음 가격 저장
        chartHistory.Add(ChartPrice);

        // 10초마다 가격 변경 시작
        StartCoroutine(ChangePrice());

        ChartBuyButton.onClick.AddListener(BuyChart);
        ChartSellButton.onClick.AddListener(SellChart);
        MaxBuyUpButton.onClick.AddListener(MaxBuyUp);
        MaxBuyDownButton.onClick.AddListener(MaxBuyDown);

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {

    }
    IEnumerator ChangePrice()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            // -200 ~ 200
            int randomValue = Random.Range(-200, 201);

            // 가격 증감
            ChartPrice += randomValue;
            ChartPriceText.text = ChartPrice.ToString();

            // 가격이 0보다 작아지지 않도록
            if (ChartPrice < 1)
                ChartPrice = 1;

            // 기록 저장
            chartHistory.Add(ChartPrice);

            // 화면 출력
            ChartPriceText.text = ChartPrice.ToString("F0");

            Debug.Log($"변동 : {randomValue}, 현재가격 : {ChartPrice}");
        }

    }
    public void BuyChart()
    {
        if (maxBuy * ChartPrice <= goldManager.userGold)
        {
            for (int i = 0; i < maxBuy; i++)
            {
                if (goldManager.userGold >= ChartPrice)
                {
                    goldManager.userGold -= (int)ChartPrice;
                    UpdateText();

                    ChartPieces++;
                    ChartPiecesText.text = ChartPieces.ToString();
                }
                else
                {
                    Debug.Log("너 돈 없어");
                }
            }
        }
        else
        {
            Debug.Log("너 돈없어");
        }
    }
    public void SellChart()
    {
        if (ChartPieces >= maxBuy)
        {
            for (int i = 0; i < maxBuy; i++)
            {
                if (ChartPieces >= 1)
                {
                    goldManager.userGold += (int)ChartPrice;
                    UpdateText();

                    ChartPieces--;
                    ChartPiecesText.text = ChartPieces.ToString();
                }
                else
                {
                    Debug.Log("너 주식 없어");
                }
            }
        }
        else
        {
            Debug.Log("가진거 보다 많습니다");
        }
    }
    public void MaxBuyUp()
    {
        maxBuy = int.Parse(MaxBuyText.text);
        maxBuy++;
        UpdateBuyText();

    }
    public void MaxBuyDown()
    {
        if (maxBuy > 0)
        {
            maxBuy = int.Parse(MaxBuyText.text);
            maxBuy--;
            UpdateBuyText();
        }

    }
    private void UpdateText()
    {
        double userGold = goldManager.userGold;
        currentGold.text = userGold.ToString();
    }
    private void UpdateBuyText()
    {
        MaxBuyText.text = maxBuy.ToString();
    }
}
