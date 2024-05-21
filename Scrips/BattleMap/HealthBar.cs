using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Transform playerTransform;
    public Vector3 offset;
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            // Set health bar position based on player position and offset
            transform.position = playerTransform.position + offset;
        }
    }
    public void SetHealth(float health)
    {
        slider.value = health;
    }
     public void UpdateSlider(float health)
    {
        slider.value = health;
    }
    
}
