using UnityEngine;
using UnityEngine.UI;
public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Transform playerTransform;
    public Vector3 offset;

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
    }
    void Update()
    {
        if (playerTransform != null)
        {
            // Set health bar position based on player position and offset
            transform.position = playerTransform.position + offset;
            
        }
    }
    public void SetStamina(int stamina)
    {
        slider.value = stamina;
    }
}
