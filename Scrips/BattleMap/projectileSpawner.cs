using UnityEngine;

public class ProjectileSpwner : MonoBehaviour
{
    public GameObject bullet;
    public GameObject allyProjectiles, enemyProjectiles;
  
    //TODO: force og retning
    public void NewInstantiate(
        Vector3 targetPos, Vector3 shooterPos, float dmg, float proSpeed, float range, float colliderRadius, string[] spirtes, string[] effektType, string side, DamageType dmgType, Vector2 scale, bool fromHero
        )
    {
        GameObject newBullet = Instantiate(bullet);
        bulletScript bs = newBullet.GetComponent<bulletScript>();
     

        bs.targetPos = targetPos;
        bs.shooterPos = shooterPos;
        bs.dmg = dmg;
        bs.proSpeed = proSpeed;
        bs.range = range;
        bs.sprites = spirtes;
        bs.effektType = effektType;
        bs.side = side;
        bs.dmgType = dmgType;
        bs.scale = scale;
        bs.colliderRadius = colliderRadius;
        bs.fromHero = fromHero;
       

        newBullet.layer = LayerMask.NameToLayer("Projectiles");
        Destroy( newBullet, 1f );
        if(side == "ally")
            newBullet.transform.parent = allyProjectiles.transform;
        else if(side == "enemy")
            newBullet.transform.parent = enemyProjectiles.transform;
    }
}
public enum DamageType
{
    Normal,
    Fire,
    Ice,
    // Add more damage types as needed
}