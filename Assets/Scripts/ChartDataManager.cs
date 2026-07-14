using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class ChartData
{
    public int id;
    public string name;
    public string value;
    public int startPrice;
    public int maxPrice;
    public int minChange;
    public int maxChange;
    public int odds;

    public double currentPrice;
    public List<double> history ;

}
public class ChartDataManager : MonoBehaviour
{
    public string csvURL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vRT-2B0_NgkhdNT0cMvizl1AqeCPUak2K9CaYztuv6VPbcOUO6DqaYK55kGloWLtdLjOaB45yqJEI3R/pub?gid=1130107618&single=true&output=csv";
    public List<ChartData> chartList = new();

    public bool IsLoaded { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadCSV());

    }

    // Update is called once per frame


    void Update()
    {
    }
    IEnumerator LoadCSV()
    {
        UnityWebRequest www = UnityWebRequest.Get(csvURL);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
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

            if (col.Length < 8)
                continue;

            if (!int.TryParse(col[0].Trim(), out _))
                continue;

            if (col.Length < 8)
                continue;

            ChartData data = new ChartData();

            data.id = int.Parse(col[0].Trim());

            data.name = col[1].Trim();

            data.value = col[2].Trim();

            data.startPrice = int.Parse(col[3].Trim());

            data.maxPrice = int.Parse(col[4].Trim());

            data.minChange = int.Parse(col[5].Trim());

            data.maxChange = int.Parse(col[6].Trim());

            data.odds = int.Parse(col[7].Trim());

            data.currentPrice = data.startPrice;
            data.history = new List<double> { data.startPrice };   // ← Add 대신 이렇게
            chartList.Add(data);
        }
    }
}
