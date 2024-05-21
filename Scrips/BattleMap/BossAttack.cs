using UnityEngine;
using System.Collections;
public class BossAttack : MonoBehaviour
{
    [SerializeField] public float hp, attackDmg, moveSpeed, range, attackSpeed, xp;
    private Rigidbody2D rb;
    private Vector3 localScale;
    public float knockbackForce;
    public float KnockBackCounter;
    public float knockbackTotalTime;
    public bool knock;
    private Movement player;
    private Player playerInfo;
    private GameObject closestEnemy;
    private GameObject[] enemies;
    private CollisionScipt collisionScipt;
    private bool isEnemyInRange = false, canAttack = true;
    private Animator animator;
    private ProjectileSpwner projectileSpawner;
    public string[] projectileArray;

    // Start is called before the first frame update
    void Start()
    {
        collisionScipt = GameObject.FindGameObjectWithTag("CollisionScript").GetComponent<CollisionScipt>();
        player = GameObject.FindWithTag("UniqeTage").GetComponent<Movement>();
        playerInfo = GameObject.FindWithTag("UniqeTage").GetComponent<Player>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        localScale = transform.localScale;
        enemies = collisionScipt.FindAllUnits("Entities", "Allies");
        projectileSpawner = GameObject.FindGameObjectWithTag("ProjectileScript").GetComponent<ProjectileSpwner>();
    }

    // Update is called once per frame
    void Update()
    {

        closestEnemy = collisionScipt.FindClosestUnit(enemies);
        var closestEnemyCopy = collisionScipt.FindClosestUnit(new GameObject[] { player.gameObject, closestEnemy });
        closestEnemy = closestEnemyCopy;
        if (hp <= 0)
        {
            Destroy(gameObject);
            playerInfo.SetXp(playerInfo.GetXp() + xp);
        }


        CheckForEnemyInRange();
        Attack();
        
    }
    void Move()
    {
        // Check if an enemy is in range and stop moving
        if (isEnemyInRange)
        {
            // Enemy is in range, stop moving
            rb.velocity = Vector2.zero;
        }
        else if (closestEnemy && player.gameObject)
        {
            // Beregn avstanden til spilleren og den n�rmeste allierte
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            float distanceToAlly = Mathf.Infinity; // Sett initialt til uendelig
            if (closestEnemy)
            {
                distanceToAlly = Vector3.Distance(transform.position, closestEnemy.transform.position);
            }

            // Velg m�let basert p� den n�rmeste av spilleren og den n�rmeste allierte
            GameObject target = player.gameObject; // Start med � bevege seg mot spilleren
            if (distanceToAlly < distanceToPlayer)
            {
                target = closestEnemy; // Hvis den n�rmeste allierte er n�rmere enn spilleren, beveg deg mot den allierte
            }

            // Beregn retningen mot m�let
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Vector3 moveDirection = new Vector3(directionToTarget.x, directionToTarget.y, 0f);

            // Beveg fienden mot m�let
            rb.velocity = moveDirection * moveSpeed;
        }
        // Set the "run" parameter in the Animator based on the velocity magnitude
        animator.SetBool("run", rb.velocity.magnitude > 0);
    }

    private void CheckForEnemyInRange()
    {
        // Use Physics2D.OverlapCircleAll to check for enemies within the detection range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);

        // Check if any of the colliders are enemies
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ally"))
            {
                // Enemy is in range, stop moving
                isEnemyInRange = true;
                return;
            }
        }

        // No enemies in range, continue moving
        isEnemyInRange = false;
    }

    void Attack()
    {
        if (canAttack && isEnemyInRange)
        {
            StartCoroutine(AttackCooldown(1f));
                                                            // Kall NewInstantiate-metoden for � skyte p� fienden
          projectileSpawner.NewInstantiate(closestEnemy.transform.position, transform.position, attackDmg, 8f, range, 0.1408689f, projectileArray, null, "enemy", DamageType.Normal, new Vector3(1, 1, 5), false);

           
        }
        
    }

    IEnumerator DestroyProjectile(GameObject projectile)
    {
        yield return new WaitForSeconds(2f);

        if(projectile != null)
        {
            Destroy(projectile);
        }

    }

    IEnumerator AttackCooldown(float time)  
    {
        canAttack = false; // Sett canAttack til false for � hindre flere angrep mens cooldown-telleren kj�rer
        float cooldownTimer = 0f;

        // Vent til cooldown-tiden er over
        while (cooldownTimer < time)
        {
            cooldownTimer += Time.deltaTime; // �k cooldown-telleren basert p� tiden som har g�tt siden forrige oppdatering
            float progress = cooldownTimer / time; // Beregn fremdriften som en andel av den totale cooldown-tiden

            // Vent p� neste rammeforberedelse f�r du g�r til neste iterasjon av l�kken
            yield return null;
        }

        canAttack = true; // Tillat fienden � angripe igjen etter cooldown-tiden er over
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

            // Legg til en y-komponent for � f� en liten vertikal bevegelse
            rb.velocity = new Vector2(rb.velocity.x, knockbackForce * 0.5f);

            KnockBackCounter -= Time.deltaTime;
        }
        //Rettning
        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
        else if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ally"))
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
    public void SetHp(float hp) { this.hp = hp; }
    public float GetHp() { return hp; }
}
