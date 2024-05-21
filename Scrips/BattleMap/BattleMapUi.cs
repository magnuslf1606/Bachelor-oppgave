using UnityEngine;
using TMPro;

public class BattleMapUi : MonoBehaviour
{
    public TextMeshProUGUI time;
    private float timer = 0;

    void Update()
    {
        //Timer
        timer += Time.deltaTime;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
