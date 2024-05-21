using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxHealth, attack;
    private float currHealth;

    public GameOver gameOver;
    private HealthBar healthBar;
    private float xp;
    private GameObject tags;
    public InventoryManager inventoryManager;

    // Start is called before the first frame update
    void Start()
    {
        /*   tags = GameObject.FindWithTag("Healthbar");
           if (tags) print("FOUND TAG: " + tags.name);
           gameOver = GameObject.FindWithTag("GameOver").GetComponent<GameOver>();

           healthBar = tags.GetComponent<HealthBar>();

           if (healthBar) print("FOUND HEALTH");
           // Get all components attached to the GameObject
           Component[] components = GetComponents<Component>();
           print("LENGTH: " + components.Length);
           // Loop through each component and print its type
           foreach (var component in components)
           {
               Debug.Log(component.GetType().Name);
           }

           if (healthBar) print("FOUND");
           if (healthBar)
               print("Found Health!");
        */
        inventoryManager = GameObject.FindGameObjectWithTag("InventoryUI").GetComponent<InventoryManager>();
        healthBar = GameObject.FindGameObjectWithTag("Healthbar").GetComponent<HealthBar>();
        maxHealth += inventoryManager.GetHealth();
        attack += inventoryManager.GetAttack();
        healthBar.SetMaxHealth(maxHealth);
        currHealth = maxHealth; 

        if (gameOver) print("FOUND");
        // currHealth = maxHealth;
        //  healthBar.SetMaxHealth(currHealth);
    }

    public void TakeDamage(float damage) {
      
        currHealth -= damage;
        //healthBar.GetComponent<HealthBar>().SetHealth(currHealth);
        if(currHealth <= 0) {
            gameOver.GameOverState("YOU LOSE!");
        }
    }
    public void SetXp(float x) { xp = x;}
    public float GetXp() { return xp;}
    public float GetHealth()
    {
        return currHealth;
    }
    public void SetHealth(float health)
    {
        currHealth = Mathf.Clamp(health, 0f, maxHealth); 
        healthBar.SetHealth(currHealth);
      //  currHealth -= health;
        //healthBar.GetComponent<HealthBar>().SetHealth(currHealth);
        if (currHealth <= 0)
        {
          
            gameOver.GameOverState("YOU LOSE!");
            
        }
    }
    public void SetAttack(float x) { attack = x;}
    public float GetAttack() { return attack;}
}
