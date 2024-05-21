using UnityEngine;

public class ResourceIncrease : MonoBehaviour
{
    private float multiplier = 1.0f;
    public float multiplierIncrease;
    void Start () {
        multiplier += multiplierIncrease;
    }
    public float CalculateTotalIncrease(float baseProduction)
    {
        return baseProduction / 10 * multiplier;
    }
}
