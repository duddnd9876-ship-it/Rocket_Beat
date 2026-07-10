using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ShopitemManager : MonoBehaviour
{
    public GoldManager goldManager;
    public TMP_Text ItemValue;
    public Button ItemBuyButton;
    public TMP_Text ItemPrice;
    public TMP_Text ItemName;
    public GameObject PickaxePanel;

    private int pickaxeNumber = 0;
    private int[] pickaxeValue = new int[6] { 1, 2, 3, 4, 5, 0};
    private string[] itemName = new string[6] {"Wood Pickaxe", "Rock Pickaxe", "Iron Pickaxe", "Diamond Pickaxe", "Titanium Pickaxe", "Sold Out"};
    private int[] pickaxePrice = new int[6] { 20, 40, 60, 140, 160, 0 };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (pickaxeNumber < pickaxeValue.Length)
        {
            ItemBuyButton.onClick.AddListener(GoldValueUp);
            ItemName.text = itemName[pickaxeNumber];
            ItemPrice.text = pickaxePrice[pickaxeNumber].ToString();
            ItemValue.text = pickaxeValue[pickaxeNumber].ToString();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
    public void GoldValueUp()
    {
        if (pickaxeNumber < pickaxeValue.Length)
        {
            if (goldManager.userGold >= pickaxePrice[pickaxeNumber])
            {
                goldManager.userGold -= pickaxePrice[pickaxeNumber];
                int itemValue = int.Parse(ItemValue.text);
                goldManager.userUpGold += itemValue;
                pickaxeNumber++;
                ItemName.text = itemName[pickaxeNumber];
                ItemPrice.text = pickaxePrice[pickaxeNumber].ToString();
                ItemValue.text = pickaxeValue[pickaxeNumber].ToString();
                if (pickaxeNumber == 5)
                {
                    PickaxePanel.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Not Enough Money");
            }
        }
    }
}
