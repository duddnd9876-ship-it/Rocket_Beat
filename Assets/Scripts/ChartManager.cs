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
    public ChartDataManager dataManager;

    public Text ChartPiecesText;
    public Button ChartBuyButton;
    public Button ChartSellButton;
    public Text currentGold;
    public Text MaxBuyText;
    public Button MaxBuyUpButton;
    public Button MaxBuyDownButton;
    public UIStockChart stockChart; // 선 그래프 컴포넌트 참조

    [System.Serializable]
    public class ChartButtonUI
    {
        public Button button;
        public Text nameText;  // 차트명
        public Text priceText; // 차트가격
        public Text typeText;  // 차트 종류
    }

    [Header("차트 선택 버튼 (4개) - 각자 이름/가격/종류 텍스트를 가짐")]
    public ChartButtonUI[] chartButtons; // 인스펙터에서 4개 항목, index 0~3이 데이터 테이블 0~3번째 줄에 대응

    [Header("차트 해금 패널 (ShopUIManager의 UnlockChartPanel01~04와 동일한 오브젝트)")]
    // 패널이 활성화(true) 상태면 아직 잠긴 것으로 보고 가격을 고정한다.
    // ShopUIManager가 아이템을 다 사면 해당 패널을 SetActive(false) 하는데, 그 순간부터 가격이 움직인다.
    public GameObject[] unlockChartPanels; // index 0~3, chartButtons/데이터 순서와 동일하게

    private bool IsChartUnlocked(int index)
    {
        if (index < 0 || index >= unlockChartPanels.Length || unlockChartPanels[index] == null)
            return true; // 패널을 연결하지 않았으면 기본적으로 해금 상태로 취급

        return !unlockChartPanels[index].activeSelf;
    }

    private int ChartPieces = 0;
    private int maxBuy = 1;
    private int currentChart = 0;
    private bool initialized = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChartPiecesText.text = ChartPieces.ToString();
        MaxBuyText.text = maxBuy.ToString();

        ChartBuyButton.onClick.AddListener(BuyChart);
        ChartSellButton.onClick.AddListener(SellChart);
        MaxBuyUpButton.onClick.AddListener(MaxBuyUp);
        MaxBuyDownButton.onClick.AddListener(MaxBuyDown);
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
            return;

        if (dataManager != null &&
            dataManager.IsLoaded &&
            dataManager.chartList.Count > 0)
        {
            initialized = true;

            SetupChartButtons();

            SelectChart(0); // 처음엔 데이터 테이블 첫 줄을 기본 선택

            ChartPiecesText.text = ChartPieces.ToString();
            MaxBuyText.text = maxBuy.ToString();

            StartCoroutine(ChangePrice());
        }
    }

    // 버튼 각각에 리스너를 걸고, 이름/종류/초기 가격 텍스트를 채운다.
    private void SetupChartButtons()
    {
        for (int i = 0; i < chartButtons.Length; i++)
        {
            if (i >= dataManager.chartList.Count)
                break;

            int index = i; // 클로저 캡처 문제 방지용 지역 변수
            ChartData data = dataManager.chartList[i];
            ChartButtonUI ui = chartButtons[i];

            if (ui.button != null)
                ui.button.onClick.AddListener(() => SelectChart(index));

            if (ui.nameText != null)
                ui.nameText.text = data.name;

            if (ui.typeText != null)
                ui.typeText.text = data.value;

            if (ui.priceText != null)
                ui.priceText.text = data.currentPrice.ToString("F0");
        }
    }

    IEnumerator ChangePrice()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);

            // 모든 종목의 가격을 매 틱마다 갱신한다 (실제 주식시장처럼 동시에 변동).
            // 단, 아직 해금되지 않은(잠긴) 차트는 건너뛰어서 가격이 고정되게 한다.
            for (int i = 0; i < dataManager.chartList.Count; i++)
            {
                if (!IsChartUnlocked(i))
                    continue;

                ChartData data = dataManager.chartList[i];

                int chance = Random.Range(0, 101);
                int value = (chance <= data.odds)
                    ? Random.Range(0, data.maxChange + 1)
                    : Random.Range(data.minChange, 1);

                data.currentPrice += value;

                if (data.currentPrice > data.maxPrice)
                    data.currentPrice = data.maxPrice;

                // 가격이 0보다 작아지지 않도록
                if (data.currentPrice < 1)
                    data.currentPrice = 1;

                // 기록 저장 (종목별로 각자 히스토리를 가짐)
                data.history.Add(data.currentPrice);

                // 너무 많은 데이터가 쌓이면 그래프가 복잡해지므로 최근 30개로 유지
                if (data.history.Count > 30)
                {
                    data.history.RemoveAt(0);
                }

                // 해당 종목 버튼의 가격 텍스트도 실시간으로 갱신
                if (i < chartButtons.Length && chartButtons[i].priceText != null)
                {
                    chartButtons[i].priceText.text = data.currentPrice.ToString("F0");
                }
            }

            // 현재 선택된 종목의 그래프만 갱신
            RefreshChart();
        }
    }

    public void BuyChart()
    {
        ChartData data = dataManager.chartList[currentChart];

        if (maxBuy * data.currentPrice <= goldManager.userGold)
        {
            for (int i = 0; i < maxBuy; i++)
            {
                if (goldManager.userGold >= data.currentPrice)
                {
                    goldManager.userGold -= (int)data.currentPrice;
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
            ChartData data = dataManager.chartList[currentChart];

            for (int i = 0; i < maxBuy; i++)
            {
                if (ChartPieces >= 1)
                {
                    goldManager.userGold += (int)data.currentPrice;
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

    // index: 0 = 데이터 테이블 첫 번째 줄, 1 = 두 번째 줄, ...
    public void SelectChart(int index)
    {
        if (index < 0 || index >= dataManager.chartList.Count)
        {
            Debug.LogWarning($"SelectChart: 잘못된 인덱스 {index} (데이터 {dataManager.chartList.Count}개)");
            return;
        }

        currentChart = index;

        RefreshChart();
    }

    // 상단 텍스트가 아니라, 선택된 종목의 그래프만 갱신한다.
    // 이름/가격/종류는 각 버튼 자체 텍스트(chartButtons)가 이미 표시하고 있다.
    void RefreshChart()
    {
        ChartData data = dataManager.chartList[currentChart];

        if (stockChart != null)
        {
            stockChart.UpdateData(data.history);
        }
    }
}
