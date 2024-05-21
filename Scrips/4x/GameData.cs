using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    [SerializeField]
    public string data;

    public string GetData()
    {
        return data;
    }
    public void setData(string x) { data = x; }
}
