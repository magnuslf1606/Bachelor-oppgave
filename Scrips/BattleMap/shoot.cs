using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 mousePos;
    private bool canAttack = true;
    public GameObject character;
    private StaminaBar staminaBar; // Legg til referanse til StaminaBar
    private ProjectileSpwner projectileSpawner;
    private int MaxStamina = 100;
    [SerializeField] public float attackDmg;

    //private BattleSound baSound;



    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        staminaBar = GameObject.FindGameObjectWithTag("Staminabar").GetComponent<StaminaBar>();
        projectileSpawner = GameObject.FindGameObjectWithTag("ProjectileScript").GetComponent<ProjectileSpwner>();
        staminaBar.SetStamina(MaxStamina);
        
       // baSound = GetComponent<BattleSound>();
    }

    void FixedUpdate()
    {
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0) && canAttack)
        {

            //   projectileSpawner.NewInstantiate(mousePos, character.transform.position, 1f, 20f, 5f, 0.13f, new string[] { "alt", "SwordSlash1", "SwordSlash2" }, null, "ally",DamageType.Normal, new Vector3(2, 2, 5), true);
            //   StartCoroutine(AttackCooldown(2f));
            StartCoroutine(AttackCooldown(.4f));
          
            projectileSpawner.NewInstantiate(mousePos, transform.position, attackDmg, 20f, 5f, 0.13f, new string[] { "ProjectileSprite1"}, null, "ally", DamageType.Normal, new Vector3(2, 2, 5), true);
            //baSound.playSword();
        }
    }
   
    IEnumerator AttackCooldown(float time) {
        canAttack = false;
        float cooldownTimer = 0f;
        
        while (cooldownTimer < time)
        {
            cooldownTimer += Time.deltaTime;
            float progress = cooldownTimer / time;

            // Calculate the stamina increase based on the progress
            float staminaIncrease = progress;

            // Update the stamina bar using the calculated increase
            if(staminaBar != null)
                staminaBar.SetStamina(Mathf.FloorToInt(MaxStamina * staminaIncrease));

            yield return null;
        }

        // Ensure the stamina bar is fully filled after the cooldown
        staminaBar.SetStamina(MaxStamina);

        canAttack = true;
    }
}

