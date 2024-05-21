using UnityEngine;

public class Health : MonoBehaviour
{

    public float maxHealth = 3;
    public float currentHealth;
    // Start is called before the first frame update

    public Animator a;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(GameObject target, float damageAmount)
{
    Health targetHealth = target.GetComponent<Health>();

    if (targetHealth != null)
    {
        targetHealth.currentHealth -= damageAmount;

        if (targetHealth.currentHealth <= 0)
        {
            //targetHealth.Die();
        }
    }
}
}
