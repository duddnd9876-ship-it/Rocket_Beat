using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ShopItem
{
    public int id;
    public string name;
    public int level;
    public int up;
    public int price;
}

public class ShopitemManager : MonoBehaviour
{
    public GoldManager goldManager;

    public Text ItemValue;
    public Button ItemBuyButton;
    public Text ItemPrice;
    public Text ItemName;
    public GameObject PickaxePanel;
    public Text PickaxePanelText;

    public Text JHItemName;
    public Text JHItemValue;
    public Text JHItemPrice;
    public Button JHbuyButton;
    public Text JHPanelText;

    public GameObject JackHammerLockPanel;
    public GameObject DrillLockPanel;
    public GameObject DynamiteLockPanel;

    public Text DrillItemName;
    public Text DrillItemPrice;
    public Text DrillItemValue;
    public Button DrillBuyButton;
    public Text DrillpanelText;

    public Text DynamiteItemName;
    public Text DynamiteItemPrice;
    public Text DynamiteItemValue;
    public Button DynamiteBuyButton;
    public Text DynamitepanelText;

    public string csvURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRT-2B0_NgkhdNT0cMvizl1AqeCPUak2K9CaYztuv6VPbcOUO6DqaYK55kGloWLtdLjOaB45yqJEI3R/pub?gid=1479347155&single=true&output=csv";

    private List<ShopItem> pickaxeItems = new List<ShopItem>();
    private List<ShopItem> jackHammerItems = new List<ShopItem>();
    private List<ShopItem> drillItems = new List<ShopItem>();
    private List<ShopItem> dynamiteItems = new List<ShopItem>();

    private int pickaxeNumber = 0;
    private int jHammerNumber = 0;
    private int drillNumber = 0;
    private int dynamiteNumber = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PickaxePanel.SetActive(false);

        JackHammerLockPanel.SetActive(true);
        DrillLockPanel.SetActive(true);
        DynamiteLockPanel.SetActive(true);

        ItemBuyButton.onClick.AddListener(PickAxeGoldValueUp);
        JHbuyButton.onClick.AddListener(JackHammerBuy);
        DrillBuyButton.onClick.AddListener(DrillBuy);
        DynamiteBuyButton.onClick.AddListener(DynamiteBuy);

        StartCoroutine(LoadCSV());
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    public void PickAxeGoldValueUp()
    {
        if (pickaxeNumber >= pickaxeItems.Count)
            return;

        ShopItem item = pickaxeItems[pickaxeNumber];

        if (goldManager.userGold < item.price)
        {
            Debug.Log("Not Enough Money");
            return;
        }

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;

        pickaxeNumber++;

        UpdatePickaxeUI();
        if (pickaxeNumber >= pickaxeItems.Count)
        {
            JackHammerLockPanel.SetActive(false);
            PickaxePanel.SetActive(true);
            PickaxePanelText.text = "Sold Out";

        }
    }
    void UpdatePickaxeUI()
    {
        if (pickaxeNumber >= pickaxeItems.Count)
        {
            ItemName.text = "Sold Out";
            ItemPrice.text = "-";
            ItemValue.text = "-";
            return;
        }

        ShopItem item = pickaxeItems[pickaxeNumber];

        ItemName.text = item.name;
        ItemPrice.text = item.price.ToString();
        ItemValue.text = item.up.ToString();
    }
    IEnumerator LoadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(csvURL);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
            yield break;
        }

        ParseCSV(www.downloadHandler.text);
        UpdatePickaxeUI();
        UpdateJackHammerUI();
        UpdateDrillUI();
        UpdateDynamiteUI();
    }
    void ParseCSV(string csv)
    {
        string[] lines = csv.Split('\n');

        for (int i = 3; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] col = lines[i].Split(',');

            if (col.Length < 5)
                continue;

            ShopItem item = new ShopItem();

            item.id = int.Parse(col[0].Trim());
            item.name = col[1].Trim();
            item.level = int.Parse(col[2].Trim());
            item.up = int.Parse(col[3].Trim());
            item.price = int.Parse(col[4].Trim());

            int type = item.id / 100;

            switch (type)
            {
                case 100: pickaxeItems.Add(item); break;
                case 101: jackHammerItems.Add(item); break;
                case 102: drillItems.Add(item); break;
                case 103: dynamiteItems.Add(item); break;
            }
        }
    }
    void UpdateJackHammerUI()
    {
        if (jHammerNumber >= jackHammerItems.Count)
        {
            JHItemName.text = "Sold Out";
            JHItemPrice.text = "-";
            JHItemValue.text = "-";
            return;
        }

        ShopItem item = jackHammerItems[jHammerNumber];

        JHItemName.text = item.name;
        JHItemPrice.text = item.price.ToString();
        JHItemValue.text = item.up.ToString();
    }
    public void JackHammerBuy()
    {
        if (jHammerNumber >= jackHammerItems.Count)
            return;

        ShopItem item = jackHammerItems[jHammerNumber];

        if (goldManager.userGold < item.price)
        {
            Debug.Log("Not Enough Money");
            return;
        }

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;

        jHammerNumber++;

        UpdateJackHammerUI();

        if (jHammerNumber >= jackHammerItems.Count)
        {
            DrillLockPanel.SetActive(false);
            JackHammerLockPanel.SetActive(true);
            JHPanelText.text = "Sold Out";
        }
    }
    void UpdateDrillUI()
    {
        if (drillNumber >= drillItems.Count)
        {
            DrillItemName.text = "Sold Out";
            DrillItemPrice.text = "-";
            DrillItemValue.text = "-";
            return;
        }

        ShopItem item = drillItems[drillNumber];

        DrillItemName.text = item.name;
        DrillItemPrice.text = item.price.ToString();
        DrillItemValue.text = item.up.ToString();
    }
    public void DrillBuy()
    {
        if (drillNumber >= drillItems.Count)
            return;

        ShopItem item = drillItems[drillNumber];

        if (goldManager.userGold < item.price)
        {
            Debug.Log("Not Enough Money");
            return;
        }

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;

        drillNumber++;

        UpdateDrillUI();

        if (drillNumber >= drillItems.Count)
        {
            DynamiteLockPanel.SetActive(false);
            DrillLockPanel.SetActive(true);
            DrillpanelText.text = "Sold Out";
        }
    }
    void UpdateDynamiteUI()
    {
        if (dynamiteNumber >= dynamiteItems.Count)
        {
            DynamiteItemName.text = "Sold Out";
            DynamiteItemPrice.text = "-";
            DynamiteItemValue.text = "-";
            return;
        }

        ShopItem item = dynamiteItems[dynamiteNumber];

        DynamiteItemName.text = item.name;
        DynamiteItemPrice.text = item.price.ToString();
        DynamiteItemValue.text = item.up.ToString();
    }
    public void DynamiteBuy()
    {
        if (dynamiteNumber >= dynamiteItems.Count)
            return;

        ShopItem item = dynamiteItems[dynamiteNumber];

        if (goldManager.userGold < item.price)
        {
            Debug.Log("Not Enough Money");
            return;
        }

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;

        dynamiteNumber++;

        UpdateDynamiteUI(); 
        if (dynamiteNumber >= dynamiteItems.Count)
        {
            DynamiteLockPanel.SetActive(true);
            DynamitepanelText.text = "Sold Out";
        }
    }
}
