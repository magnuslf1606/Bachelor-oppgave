using UnityEngine;
using System.Collections.Generic;
using System;

public class bulletScript : MonoBehaviour
{
    public Vector3 targetPos, shooterPos;
    public float dmg, proSpeed, range, colliderRadius;
    public string[] sprites, effektType; 
    public string side;
    public bool fromHero;
    public DamageType dmgType;
    public Vector2 scale;
    private Sprite currentSpirte;
    private CircleCollider2D myCollider;
    private HashSet<GameObject> damagedEnteties = new HashSet<GameObject>(); // Keep track of damaged enemies
    //private Layer
    public GameObject FloatingTextPref;
   

    // Start is called before the first frame update
    void Start()
    {
        currentSpirte = GetComponent<Sprite>();
        SpirteHandler();
        transform.position = new Vector3(shooterPos.x, shooterPos.y, 2);
        RotateProjectile();
        transform.localScale = scale;
        myCollider = GetComponent<CircleCollider2D>();
        myCollider.radius = colliderRadius;
    }

   
    // Update is called once per frame
    void Update() {

        if (side == "ally")
    {
        HandleCollisionsWithSide("Enemy", (enemy) =>
        {
            
            MeleeEnemy meleeEnemy = enemy.GetComponent<MeleeEnemy>();
            RangedEnemy rangedEnemy = enemy.GetComponent<RangedEnemy>();
            BossAttack bossEnemy = enemy.GetComponent<BossAttack>();
            if (meleeEnemy)
            {
                meleeEnemy.SetHp(meleeEnemy.GetHp() - dmg);

                if (FloatingTextPref)
                {
                    ShowDamageNumber(enemy);
                }
            }
            else if (rangedEnemy)
            {
                rangedEnemy.SetHp(rangedEnemy.GetHp() - dmg);
                if (FloatingTextPref)
                {
                    ShowDamageNumber(enemy);
                }
            }
            else if (bossEnemy)
            {
                bossEnemy.SetHp(bossEnemy.GetHp() - dmg);
                if (FloatingTextPref)
                {
                    ShowDamageNumber(enemy);
                }
            }
               
        });
    }
    else if (side == "enemy")
    {
        //Funjer kasnkje ikke på prosjektiler mot hero -> evt sjekke mot tag("UniqeTage")
        HandleCollisionsWithSide("Ally", (enemy) =>
        {
            MeleeAlly meleeAlly = enemy.GetComponent<MeleeAlly>();
            RangedAlly rangedAlly = enemy.GetComponent<RangedAlly>();
            Player player = enemy.GetComponent<Player>();
            if(meleeAlly)
                meleeAlly.SetHp(meleeAlly.GetHp() - dmg);
            else if(rangedAlly)
                rangedAlly.SetHp(rangedAlly.GetHp() - dmg);
            else if (player)
            {
                player.SetHealth(player.GetHealth() - dmg);
            }

               
        });
    }
    if(targetPos != null) { //Hvis target dør før man treffer
        //Beveges mot målet
        transform.position = Vector3.MoveTowards(transform.position, targetPos, proSpeed * Time.deltaTime);
    }
        

        CheckDistance();
        
    }
    void CheckDistance() {
        float distance = Vector3.Distance(shooterPos, transform.position);
        if (distance > range + 1) //Liten buffer
        {
            Destroy(gameObject); // Destroy the bullet when it reaches its range
            //Funker ikke hvis man bommer på notHero
        }
    }
    void HandleCollisionsWithSide(string enemyLayer, Action<GameObject> handleCollision)
    {
        
        if (damagedEnteties.Count == 0) // Sjekker om prosjektilet allerede har truffet noe
        {
            int layerMask = 1 << LayerMask.NameToLayer(enemyLayer);
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, colliderRadius, layerMask);

            foreach (Collider2D collider in colliders)
            {
                // Behandle kollisjonen med entiteten
                handleCollision(collider.gameObject);

                // Legg til entiteten i settet av skadede entiteter
                damagedEnteties.Add(collider.gameObject);

                // Slett prosjektilet ved treff, med mindre det kommer fra helten
                if (!fromHero)
                    Destroy(gameObject);
            }
        }
    }


    void ShowDamageNumber(GameObject enemy)
    {
        // Opprett skadenummerobjektet på fiendens posisjon med en liten vertikal offset
        Vector3 position = enemy.transform.position + Vector3.up * 0.7f;
        var damageNumber = Instantiate(FloatingTextPref, position, Quaternion.identity, transform);
      
        // Sett fienden som parent for skadenummeret for å følge fienden
        damageNumber.GetComponent<TextMesh>().text = dmg+""; 
        damageNumber.transform.parent = enemy.transform;

        // Ødelegg skadenummerobjektet etter en viss tid
        Destroy(damageNumber, 1f);
    }

    void RotateProjectile() {
        Vector3 direction = (targetPos - shooterPos).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void SpirteHandler() {
        if(sprites.Length > 1) {
            switch(sprites[0]) {
                case "alt" : {
                    var rnd = UnityEngine.Random.Range(1, sprites.Length);
                    
                    var indexOfPath = sprites[rnd].Substring(sprites[rnd].Length-1, 1);
                    
                    switch(sprites[rnd].Substring(0, sprites[rnd].Length-1)) {
                        case "SwordSlash" : {
                                    currentSpirte = Resources.Load<Sprite>("ProjectileSprites_SwordSlash" + indexOfPath);
                                  //  currentSpirte = Resources.Load<Sprite>("Archer_10");
                                    break;
                        }
                    }
                    
                    if(currentSpirte == null)
                        Debug.Log("Blankt");
                    break;
                }
                case "anim" : {
                    
                    break;
                }
            }
        } else if(sprites.Length == 1) {
            //setter spirte
            currentSpirte = Resources.Load<Sprite>(sprites[0]);
        } 
        GetComponent<SpriteRenderer>().sprite = currentSpirte;
    }
}
