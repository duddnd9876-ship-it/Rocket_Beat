using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ShopUIManager : MonoBehaviour
{
    public GoldManager goldManager;
    public ShopDataManager dataManager;

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

    private int pickaxeNumber;
    private int jHammerNumber;
    private int drillNumber;
    private int dynamiteNumber;
    public Image PickaxeImage;
    public Image JackHammerImage;
    public Image DrillImage;
    public Image DynamiteImage;
    bool initialized;

    public GameObject UnlockChartPanel01;
    public GameObject UnlockChartPanel02;
    public GameObject UnlockChartPanel03;
    public GameObject UnlockChartPanel04;

    private void Start()
    {
        PickaxePanel.SetActive(false);

        JackHammerLockPanel.SetActive(true);
        DrillLockPanel.SetActive(true);
        DynamiteLockPanel.SetActive(true);

        ItemBuyButton.onClick.AddListener(PickAxeGoldValueUp);
        JHbuyButton.onClick.AddListener(JackHammerBuy);
        DrillBuyButton.onClick.AddListener(DrillBuy);
        DynamiteBuyButton.onClick.AddListener(DynamiteBuy);
    }

    private void Update()
    {
        if (initialized)
            return;

        if (dataManager != null && dataManager.IsLoaded)
        {
            initialized = true;

            UpdatePickaxeUI();
            UpdateJackHammerUI();
            UpdateDrillUI();
            UpdateDynamiteUI();
        }
    }

    #region Pickaxe

    void UpdatePickaxeUI()
    {
        if (pickaxeNumber >= dataManager.pickaxeItems.Count)
        {
            ItemName.text = "Sold Out";
            ItemPrice.text = "-";
            ItemValue.text = "-";
            return;
        }

        var item = dataManager.pickaxeItems[pickaxeNumber];

        ItemName.text = item.name;
        ItemPrice.text = item.price.ToString();
        ItemValue.text = item.up.ToString(); 

        Sprite sprite = Resources.Load<Sprite>("ShopImages/" + item.img);

        if (sprite != null)
            PickaxeImage.sprite = sprite;
    }

    public void PickAxeGoldValueUp()
    {
        if (pickaxeNumber >= dataManager.pickaxeItems.Count)
            return;

        var item = dataManager.pickaxeItems[pickaxeNumber];

        if (goldManager.userGold < item.price)
            return;

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;
        goldManager.UserUpGold.text = goldManager.userUpGold.ToString();

        pickaxeNumber++;

        UpdatePickaxeUI();

        if (pickaxeNumber >= dataManager.pickaxeItems.Count)
        {
            JackHammerLockPanel.SetActive(false);
            PickaxePanel.SetActive(true);
            PickaxePanelText.text = "Sold Out";
            UnlockChartPanel01.SetActive(false);
        }
    }

    #endregion

    #region JackHammer

    void UpdateJackHammerUI()
    {
        if (jHammerNumber >= dataManager.jackHammerItems.Count)
        {
            JHItemName.text = "Sold Out";
            JHItemPrice.text = "-";
            JHItemValue.text = "-";
            return;
        }

        var item = dataManager.jackHammerItems[jHammerNumber];

        JHItemName.text = item.name;
        JHItemPrice.text = item.price.ToString();
        JHItemValue.text = item.up.ToString();
        Sprite sprite = Resources.Load<Sprite>("ShopImages/" + item.img);

        if (sprite != null)
            JackHammerImage.sprite = sprite;
    }

    public void JackHammerBuy()
    {
        if (jHammerNumber >= dataManager.jackHammerItems.Count)
            return;

        var item = dataManager.jackHammerItems[jHammerNumber];

        if (goldManager.userGold < item.price)
            return;

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;
        goldManager.UserUpGold.text = goldManager.userUpGold.ToString();

        jHammerNumber++;

        UpdateJackHammerUI();

        if (jHammerNumber >= dataManager.jackHammerItems.Count)
        {
            DrillLockPanel.SetActive(false);
            JackHammerLockPanel.SetActive(true);
            JHPanelText.text = "Sold Out";
            UnlockChartPanel02.SetActive(false);
        }
    }

    #endregion

    #region Drill

    void UpdateDrillUI()
    {
        if (drillNumber >= dataManager.drillItems.Count)
        {
            DrillItemName.text = "Sold Out";
            DrillItemPrice.text = "-";
            DrillItemValue.text = "-";
            return;
        }

        var item = dataManager.drillItems[drillNumber];

        DrillItemName.text = item.name;
        DrillItemPrice.text = item.price.ToString();
        DrillItemValue.text = item.up.ToString();
        Sprite sprite = Resources.Load<Sprite>("ShopImages/" + item.img);

        if (sprite != null)
            DrillImage.sprite = sprite;
    }

    public void DrillBuy()
    {
        if (drillNumber >= dataManager.drillItems.Count)
            return;

        var item = dataManager.drillItems[drillNumber];

        if (goldManager.userGold < item.price)
            return;

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;
        goldManager.UserUpGold.text = goldManager.userUpGold.ToString();

        drillNumber++;

        UpdateDrillUI();

        if (drillNumber >= dataManager.drillItems.Count)
        {
            DynamiteLockPanel.SetActive(false);
            DrillLockPanel.SetActive(true);
            DrillpanelText.text = "Sold Out";
            UnlockChartPanel03.SetActive(false);
        }
    }

    #endregion

    #region Dynamite

    void UpdateDynamiteUI()
    {
        if (dynamiteNumber >= dataManager.dynamiteItems.Count)
        {
            DynamiteItemName.text = "Sold Out";
            DynamiteItemPrice.text = "-";
            DynamiteItemValue.text = "-";
            return;
        }

        var item = dataManager.dynamiteItems[dynamiteNumber];

        DynamiteItemName.text = item.name;
        DynamiteItemPrice.text = item.price.ToString();
        DynamiteItemValue.text = item.up.ToString(); 
        
        Sprite sprite = Resources.Load<Sprite>("ShopImages/" + item.img);

        if (sprite != null)
            DynamiteImage.sprite = sprite;
    }

    public void DynamiteBuy()
    {
        if (dynamiteNumber >= dataManager.dynamiteItems.Count)
            return;

        var item = dataManager.dynamiteItems[dynamiteNumber];

        if (goldManager.userGold < item.price)
            return;

        goldManager.userGold -= item.price;
        goldManager.userUpGold += item.up;
        goldManager.UserUpGold.text = goldManager.userUpGold.ToString();

        dynamiteNumber++;

        UpdateDynamiteUI();

        if (dynamiteNumber >= dataManager.dynamiteItems.Count)
        {
            DynamiteLockPanel.SetActive(true);
            DynamitepanelText.text = "Sold Out";
            UnlockChartPanel04.SetActive(false);
        }
    }

    #endregion
}
