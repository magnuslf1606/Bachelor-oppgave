using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] public float health, attackDmg, moveSpeed, xp, range;
    private Rigidbody2D rb;
    private Vector3 localScale;
    public float knockbackForce;
    public float KnockBackCounter;
    public float knockbackTotalTime;
    public bool knock;
    private Animator animator;
    private GameObject closestEnemy;
    private GameObject[] enemies;
    private CollisionScipt collisionScipt;
    private Movement player;
    private Player playerInfo;
    private bool isEnemyInRange = false, canAttack = true;
    public bool fromHero;
    private HashSet<GameObject> damagedEnteties = new HashSet<GameObject>();

    void Start()
    {
        collisionScipt = GameObject.FindGameObjectWithTag("CollisionScript").GetComponent<CollisionScipt>();
        player = GameObject.FindWithTag("UniqeTage").GetComponent<Movement>();
        playerInfo = GameObject.FindWithTag("UniqeTage").GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
        localScale = transform.localScale;
        

    }
    void Update()
    {
        enemies = collisionScipt.FindAllUnits("Entities", "Allies");
        closestEnemy = collisionScipt.FindClosestUnit(enemies);
        var closestEnemyCopy = collisionScipt.FindClosestUnit(new GameObject[] { player.gameObject, closestEnemy });
        closestEnemy = closestEnemyCopy;
        CheckForEnemyInRange();
        Move();

        if (health <= 0)
        {
            Destroy(gameObject);
        }


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

            // Apply damage to the enemy
            MeleeAlly meleeEnemy = collision.gameObject.GetComponent<MeleeAlly>();
            RangedAlly rangedEnemy = collision.gameObject.GetComponent<RangedAlly>();
            Player player = collision.gameObject.GetComponent<Player>();
            if (meleeEnemy)
            {
                meleeEnemy.SetHp(meleeEnemy.GetHp() - attackDmg);
               
            }
            else if (rangedEnemy)
            {
                rangedEnemy.SetHp(rangedEnemy.GetHp() - attackDmg);
            } else if(player)
            {
                player.SetHealth(player.GetHealth() - attackDmg);
            }
        }

    }
   

    void Move()
    {
        // Sjekk om både spilleren og de allierte er tilgjengelige
        if (closestEnemy && player.gameObject)
        {
            // Beregn avstanden til spilleren og den nærmeste allierte
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            float distanceToAlly = Mathf.Infinity; // Sett initialt til uendelig
            if (closestEnemy)
            {
                distanceToAlly = Vector3.Distance(transform.position, closestEnemy.transform.position);
            }

            // Velg målet basert på den nærmeste av spilleren og den nærmeste allierte
            GameObject target = player.gameObject; // Start med å bevege seg mot spilleren
            if (distanceToAlly < distanceToPlayer)
            {
                target = closestEnemy; // Hvis den nærmeste allierte er nærmere enn spilleren, beveg deg mot den allierte
            }

            // Beregn retningen mot målet
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            Vector3 moveDirection = new Vector3(directionToTarget.x, directionToTarget.y, 0f);

            // Beveg fienden mot målet
            rb.velocity = moveDirection * moveSpeed;
        }
        // Sett "run"-parameteren i animatoren basert på hastigheten
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


    public void FixedUpdate()
    {/*
        if (KnockBackCounter <= 0)
        {
            //collisionScipt.Move(closestEnemy, rb, moveSpeed);
            
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
        */
        //Rettning
        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
        else if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
    }
    public void SetHp(float hp)
    {
        health = hp;
      
    }
    public float GetHp() { return health; }
}