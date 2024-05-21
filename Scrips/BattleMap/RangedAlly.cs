using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
public class RangedAlly : MonoBehaviour
{
    [SerializeField] public float hp, attackDmg, moveSpeed, range, attackSpeed;
    private Rigidbody2D rb;
    private Vector3 localScale;
    public float knockbackForce;
    public float KnockBackCounter;
    public float knockbackTotalTime;
    public bool knock;
    private GameObject closestEnemy;
    private GameObject[] enemies;
    private CollisionScipt collisionScipt;
    private bool isEnemyInRange = false, canAttack = true;
    private Animator animator;
    private ProjectileSpwner projectileSpawner;
    public string[] projectileArray;
    private EnemyDamage enm;
    private Transform techTree;
    // Start is called before the first frame update
    void Start()
    {
        techTree = GameObject.FindGameObjectWithTag("TechTreeUI").transform;
        collisionScipt = GameObject.FindGameObjectWithTag("CollisionScript").GetComponent<CollisionScipt>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        localScale = transform.localScale;
        enemies = collisionScipt.FindAllUnits("Entities", "Enemies");
        //enm = GameObject.FindWithTag("Enemy").GetComponent<EnemyDamage>();

        projectileSpawner = GameObject.FindGameObjectWithTag("ProjectileScript").GetComponent<ProjectileSpwner>();
        GetStatsFromTech();
    }

    // Update is called once per frame
    void Update()
    {
        enemies = collisionScipt.FindAllUnits("Entities", "Enemies");
        closestEnemy = collisionScipt.FindClosestUnit(enemies);

        if (hp <= 0)
        {
            Destroy(gameObject);
        }

        CheckForEnemyInRange();
        Attack();

    }
    void GetStatsFromTech() {
        var x = techTree.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");
        switch(transform.name) {
            case "AllyBow(Clone)" : {
                if(x.transform.Find("Bow").transform.Find("Time").GetComponent<Text>().text == "") {
                    range += 1;
                }
                break;
            }
            case "Wizard(Clone)" : {
                if(x.transform.Find("Wizard").transform.Find("Time").GetComponent<Text>().text == "") {
                    range *= .33f;
                    hp += 5;
                }
                break;
            }
            case "Witch(Clone)" : {
                if(x.transform.Find("Witch").transform.Find("Time").GetComponent<Text>().text == "") {
                    attackDmg += 5;
                }
                break;
            }
        }
    }
    void Move()
    {
        // Check if an enemy is in range and stop moving
        if (isEnemyInRange)
        {
            // Enemy is in range, stop moving
            rb.velocity = Vector2.zero;
        }
        else  
        {
            if(closestEnemy) {
                // Beregn avstanden til spilleren og den nærmeste allierte
                Vector2 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;

                // Sett bevegelseshastigheten mot fienden
                rb.velocity = directionToEnemy * moveSpeed;
            }
            
        }
        // Set the "run" parameter in the Animator based on the velocity magnitude
        animator.SetBool("run", rb.velocity.magnitude > 0);
    }

    private void CheckForEnemyInRange()
    {
        // Bruk Physics2D.OverlapCircleAll for å sjekke for fiender innenfor detekteringsrekkevidden
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);

        // Sjekk om noen av kolliderene er fiender
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                // Fienden er innenfor rekkevidde, sett isEnemyInRange til true og avslutt løkken
                isEnemyInRange = true;
                return;
            }
        }
        // Hvis vi kommer hit, betyr det at det ikke er noen fiender innenfor rekkevidden
        isEnemyInRange = false;
    }


    void Attack() {
        if (canAttack && isEnemyInRange)
        {
            StartCoroutine(AttackCooldown(attackSpeed));
            
            projectileSpawner.NewInstantiate(closestEnemy.transform.position, transform.position, attackDmg, 8f, range, 0.1408689f, projectileArray, null, "ally",DamageType.Normal, new Vector3(1, 1, 5), false);
        }
    }
    IEnumerator AttackCooldown(float time)
    {
        canAttack = false;
        float cooldownTimer = 0f;

        while (cooldownTimer < time)
        {
            cooldownTimer += Time.deltaTime;
            float progress = cooldownTimer / time;

            yield return null;
        }
        canAttack = true;
    }

    public void FixedUpdate()
    {
        if (KnockBackCounter <= 0)
        {
            Move();
        }
        else
        {
            if (knock)
                rb.velocity = new Vector2(-knockbackForce, knockbackForce);
            else
                rb.velocity = new Vector2(knockbackForce, knockbackForce);

            // Legg til en y-komponent for å få en liten vertikal bevegelse
            rb.velocity = new Vector2(rb.velocity.x, knockbackForce * 0.5f);

            KnockBackCounter -= Time.deltaTime;
        }
        //Rettning
        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
        else if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            KnockBackCounter = knockbackTotalTime;

            // Get the knockback direction from the enemy to the enemy
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

            // Check the direction for knockback
            knock = (knockbackDirection.x > 0);

            // Apply knockback to the enemy
            Rigidbody2D allyRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (allyRb != null)
            {
                allyRb.velocity = new Vector2(knockbackDirection.x * knockbackForce, knockbackDirection.y * knockbackForce);
                // Add a small vertical movement
                allyRb.velocity = new Vector2(allyRb.velocity.x, knockbackDirection.y * knockbackForce * 0.5f);
            }  
        }
    } 
    public void SetHp(float hp) { this.hp = hp;}
    public float GetHp() { return hp; }
}
