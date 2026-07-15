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

    public Text currentGold;
    public UIStockChart stockChart; // 선 그래프 컴포넌트 참조

    [System.Serializable]
    public class ChartButtonUI
    {
        [Header("종목 선택")]
        public Button button;
        public Text nameText;  // 차트명
        public Text priceText; // 차트가격
        public Text typeText;  // 차트 종류

        [Header("이 종목 전용 거래 UI")]
        public Text piecesText;        // 이 종목 보유 수량
        public Text maxBuyText;        // 이 종목 1회 구매/판매 수량
        public Button buyButton;
        public Button sellButton;
        public Button maxBuyUpButton;
        public Button maxBuyDownButton;
    }

    [Header("차트 선택 버튼 (4개) - 각자 이름/가격/종류 + 거래UI를 가짐")]
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

    // 종목별 상태 (index = 차트 인덱스)
    private int[] chartPieces;   // 종목별 보유 수량
    private int[] maxBuyArr;     // 종목별 1회 구매/판매 수량

    private int currentChart = 0; // 그래프에 표시 중인(선택된) 종목
    private bool initialized = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chartPieces = new int[chartButtons.Length];
        maxBuyArr = new int[chartButtons.Length];

        for (int i = 0; i < chartButtons.Length; i++)
        {
            maxBuyArr[i] = 1;

            int index = i; // 클로저 캡처 문제 방지

            ChartButtonUI ui = chartButtons[i];

            if (ui.buyButton != null)
                ui.buyButton.onClick.AddListener(() => BuyChart(index));

            if (ui.sellButton != null)
                ui.sellButton.onClick.AddListener(() => SellChart(index));

            if (ui.maxBuyUpButton != null)
                ui.maxBuyUpButton.onClick.AddListener(() => MaxBuyUp(index));

            if (ui.maxBuyDownButton != null)
                ui.maxBuyDownButton.onClick.AddListener(() => MaxBuyDown(index));

            UpdatePiecesText(i);
            UpdateMaxBuyText(i);
        }

        UpdateGoldText();
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

    // index: 어떤 종목을 사고파는지
    public void BuyChart(int index)
    {
        if (index < 0 || index >= dataManager.chartList.Count)
            return;

        ChartData data = dataManager.chartList[index];
        int buyCount = maxBuyArr[index];

        if (buyCount * data.currentPrice <= goldManager.userGold)
        {
            for (int i = 0; i < buyCount; i++)
            {
                if (goldManager.userGold >= data.currentPrice)
                {
                    goldManager.userGold -= (int)data.currentPrice;
                    UpdateGoldText();

                    chartPieces[index]++;
                    UpdatePiecesText(index);
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

    public void SellChart(int index)
    {
        if (index < 0 || index >= dataManager.chartList.Count)
            return;

        int sellCount = maxBuyArr[index];

        if (chartPieces[index] >= sellCount)
        {
            ChartData data = dataManager.chartList[index];

            for (int i = 0; i < sellCount; i++)
            {
                if (chartPieces[index] >= 1)
                {
                    goldManager.userGold += (int)data.currentPrice;
                    UpdateGoldText();

                    chartPieces[index]--;
                    UpdatePiecesText(index);
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

    public void MaxBuyUp(int index)
    {
        if (index < 0 || index >= maxBuyArr.Length)
            return;

        maxBuyArr[index]++;
        UpdateMaxBuyText(index);
    }

    public void MaxBuyDown(int index)
    {
        if (index < 0 || index >= maxBuyArr.Length)
            return;

        if (maxBuyArr[index] > 1)
        {
            maxBuyArr[index]--;
            UpdateMaxBuyText(index);
        }
    }

    private void UpdateGoldText()
    {
        double userGold = goldManager.userGold;
        currentGold.text = userGold.ToString();
    }

    private void UpdatePiecesText(int index)
    {
        if (chartButtons[index].piecesText != null)
            chartButtons[index].piecesText.text = chartPieces[index].ToString();
    }

    private void UpdateMaxBuyText(int index)
    {
        if (chartButtons[index].maxBuyText != null)
            chartButtons[index].maxBuyText.text = maxBuyArr[index].ToString();
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