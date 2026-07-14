using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
public class ShopDataManager : MonoBehaviour
{
    [System.Serializable]
    public class ShopItem
    {
        public int id;
        public string name;
        public int level;
        public int up;
        public int price;
        public string img;
    }
    public string csvURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRT-2B0_NgkhdNT0cMvizl1AqeCPUak2K9CaYztuv6VPbcOUO6DqaYK55kGloWLtdLjOaB45yqJEI3R/pub?gid=1479347155&single=true&output=csv";

    public List<ShopItem> pickaxeItems = new();
    public List<ShopItem> jackHammerItems = new();
    public List<ShopItem> drillItems = new();
    public List<ShopItem> dynamiteItems = new();

    public bool IsLoaded { get; private set; }

    private void Start()
    {
        StartCoroutine(LoadCSV());
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
        IsLoaded = true;
    }

    void ParseCSV(string csv)
    {
        string[] lines = csv.Split('\n');

        for (int i = 3; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            string[] col = lines[i].Split(',');

            if (col.Length < 6)
                continue;

            ShopItem item = new ShopItem
            {
                id = int.Parse(col[0].Trim()),
                name = col[1].Trim(),
                level = int.Parse(col[2].Trim()),
                up = int.Parse(col[3].Trim()),
                price = int.Parse(col[4].Trim()),
                img = col[5].Trim()   
            };

            int type = item.id / 100;

            switch (type)
            {
                case 100:
                    pickaxeItems.Add(item);
                    break;

                case 101:
                    jackHammerItems.Add(item);
                    break;

                case 102:
                    drillItems.Add(item);
                    break;

                case 103:
                    dynamiteItems.Add(item);
                    break;
            }
        }
    }

// Start is called once before the first execution of Update after the MonoBehaviour is created

}
