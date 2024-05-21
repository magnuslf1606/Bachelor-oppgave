using UnityEngine;

public class DataAccessor : MonoBehaviour
{
    public GameData gameData;
    private string[] _info;
    public string[] Info
    {
        get { return _info; }
        set { _info = value; }
    }
    void Awake()
    {
        if (gameData != null)
        {
            string data = gameData.GetData();
            Info = ParseData(data);
        }
    }

    void Update()
    {
        string data = gameData.GetData();
        Info = ParseData(data);
    }
    public string[] ParseData(string data)
    {
        // Split the string on commas, colons, and spaces
        string[] separators = new string[] { ",", ":", " " };
        string[] dataArr = data.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
        string[] ut = new string[dataArr.Length];
        for (int i = 0; i < dataArr.Length; i++)
        {
            ut[i] = dataArr[i];
        }
        return ut;
    }
}