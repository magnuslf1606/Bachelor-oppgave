using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
public class MeleeAlly : MonoBehaviour
{
    [SerializeField] public float health, attackDmg, moveSpeed, attackSpeed, range;
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
    private bool isEnemyInRange, canAttack = true;
    public bool fromHero;
    private HashSet<GameObject> damagedEnteties = new HashSet<GameObject>();
    private Transform techTree;

    void Start()
    {
        techTree = GameObject.FindGameObjectWithTag("TechTreeUI").transform;
        collisionScipt = GameObject.FindGameObjectWithTag("CollisionScript").GetComponent<CollisionScipt>();
        rb = GetComponent<Rigidbody2D>();

         
        animator = GetComponent<Animator>();
       
        localScale = transform.localScale;
        enemies = collisionScipt.FindAllUnits("Entities", "Enemies");
        GetStatsFromTech();
    }

    void GetStatsFromTech() {
        var x = techTree.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");
        switch(transform.name) {
            case "DaggermanAlly(Clone)" : {
                if(x.transform.Find("Dagger").transform.Find("Time").GetComponent<Text>().text == "") {
                    attackDmg += 5;
                }
                break;
            }
            case "SwordsmanAlly(Clone)" : {
                if(x.transform.Find("Sword").transform.Find("Time").GetComponent<Text>().text == "") {
                    attackDmg += 2;
                    health += 2;
                }
                break;
            }
            case "Knight(Clone)" : {
                if(x.transform.Find("Knight").transform.Find("Time").GetComponent<Text>().text == "") {
                    health += 5;
                    attackDmg += 5;
                }
                break;
            }
            case "Catwoman(Clone)" : {
                if(x.transform.Find("CatWoman").transform.Find("Time").GetComponent<Text>().text == "") {
                    knockbackForce /= 2;
                }
                break;
            }
            case "OrcMale(Clone)" : {
                if(x.transform.Find("Orc").transform.Find("Time").GetComponent<Text>().text == "") {
                    health += 60;
                }
                break;
            }
            case "Skeleton(Clone)" : {
                if(x.transform.Find("Skeleton").transform.Find("Time").GetComponent<Text>().text == "") {
                    health += 20;
                }
                break;
            }
        }

    }
    void Update() {

        enemies = collisionScipt.FindAllUnits("Entities", "Enemies");
        // Hvis den nåværende nærmeste fienden ikke eksisterer (f.eks. den ble ødelagt), oppdater closestEnemy
        closestEnemy = collisionScipt.FindClosestUnit(enemies);
        
        CheckForEnemyInRange();
    

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    private void CheckForEnemyInRange()
    {
        // Use Physics2D.OverlapCircleAll to check for enemies within the detection range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, range);

        // Check if any of the colliders are enemies
        foreach (Collider2D collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                
                // Enemy is in range, stop moving
                isEnemyInRange = true;
                return;
            }
        }
        // No enemies in range, continue moving
        isEnemyInRange = false;
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
            MeleeEnemy meleeEnemy = collision.gameObject.GetComponent<MeleeEnemy>();
            RangedEnemy rangedEnemy = collision.gameObject.GetComponent<RangedEnemy>();

            // Sjekk om vi har fått en gyldig referanse til fienden
            if (meleeEnemy)
            {
                // Reduser fiendens helse
                meleeEnemy.SetHp(meleeEnemy.GetHp() - attackDmg);
            }
            else if (rangedEnemy)
            {
                // Reduser fiendens helse
                rangedEnemy.SetHp(rangedEnemy.GetHp() - attackDmg);
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
            animator.SetBool("run", false);
            print("OVerlap: " + isEnemyInRange);
        }
        else
        {
            if(closestEnemy) {
                // Beregn avstanden til spilleren og den nærmeste allierte
                Vector2 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
                // Sett bevegelseshastigheten mot fienden
                rb.velocity = directionToEnemy * moveSpeed;
            }
            animator.SetBool("run", rb.velocity.magnitude > 0);
        }
        // Set the "run" parameter in the Animator based on the velocity magnitude
        
    }

    public void FixedUpdate()
    {
        Move();
        
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
        
        //Rettning
        if(rb.velocity.x > 0)
            transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z);
        else if(rb.velocity.x < 0)
            transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
    }  
    public void SetHp(float hp) { 
        health = hp;
       
    }
    public float GetHp() { return health; }
}