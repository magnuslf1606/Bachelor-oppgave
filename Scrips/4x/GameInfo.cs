using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public float startGold, startGoldE;
    public float totalGold, totalIron, totalStone, totalWood, totalFood, totalOil;
    //ENEMY
    public float totalGoldE, totalIronE, totalStoneE, totalWoodE, totalFoodE, totalOilE;
    
    // Start is called before the first frame update
    void Start()
    {
        totalGold += startGold;
        totalGoldE += startGoldE;
    }
    // MY
    public void IncreseTotalGold(float x) { totalGold += x; }
    public void IncreseTotalWood(float x) { totalWood += x; }
    public void IncreseTotalIron(float x) { totalIron += x; }
    public void IncreseTotalFood(float x) { totalFood += x; }
    public void IncreseTotalStone(float x) { totalStone += x; }
    public void IncreseTotalOil(float x) { totalOil += x; }
    //Enemy
    public void IncreseTotalGoldE(float x) { totalGoldE += x; }
    public void IncreseTotalWoodE(float x) { totalWoodE += x; }
    public void IncreseTotalIronE(float x) { totalIronE += x; }
    public void IncreseTotalFoodE(float x) { totalFoodE += x; }
    public void IncreseTotalStoneE(float x) { totalStoneE += x; }
    public void IncreseTotalOilE(float x) { totalOilE += x; }

    // MY
    public float GetTotalGold() { return totalGold; }
    public float GetTotalIron() { return totalIron; }
    public float GetTotalStone() { return totalStone; }
    public float GetTotalWood() { return totalWood; }
    public float GetTotalFood() { return totalFood; }
    public float GetTotalOil() { return totalOil; }

    //ENEMY
    public float GetTotalGoldE() { return totalGoldE; }
    public float GetTotalIronE() { return totalIronE; }
    public float GetTotalStoneE() { return totalStoneE; }
    public float GetTotalWoodE() { return totalWoodE; }
    public float GetTotalFoodE() { return totalFoodE; }
    public float GetTotalOilE() { return totalOilE; }
}
