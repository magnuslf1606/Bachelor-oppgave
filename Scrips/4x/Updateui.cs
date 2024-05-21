using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Updateui : MonoBehaviour
{
    public GameInfo gameInfo;
    public Text goldText;
    public Text ironText;
    public Text stoneText;
    public Text woodText;
    public Text foodText;
    public Text oilText;

    void Update()
    {
        // Oppdater tekstene med verdiene fra GameInfo
        goldText.text = ((int)gameInfo.GetTotalGold()).ToString();
        ironText.text = ((int)gameInfo.GetTotalIron()).ToString();
        stoneText.text = ((int)gameInfo.GetTotalStone()).ToString();
        woodText.text = ((int)gameInfo.GetTotalWood()).ToString();
        foodText.text = ((int)gameInfo.GetTotalFood()).ToString();
        oilText.text = ((int)gameInfo.GetTotalOil()).ToString();
    }
}
