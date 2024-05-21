using UnityEngine;

public class CollisionScipt : MonoBehaviour
{   
    //FUNKER IKKE FOR EN FUKKA GRUNN!??!?!!?!?!??!?!?!?!?!?!?!?!?!??!?!! CANCER FAEN
    public void Move(GameObject closest, Rigidbody2D rb, float moveSpeed) {
        if (closest) {
            var directionToPlayer = (closest.transform.position - gameObject.transform.position).normalized;

            // Assuming the entity moves on the X and Y axes, adjust as needed
            rb.velocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * moveSpeed;
        }
    }
    public GameObject FindClosestUnit(GameObject[] arr) {
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject a in arr)
        {
            if (a != null)
            {
                float dist = Vector3.Distance(a.transform.position, currentPos);

                if (dist < minDist)
                {
                    closest = a;
                    minDist = dist;
                }
            }
        }
        return closest;
    }
    public GameObject[] FindAllUnits(string path, string type) {
        Transform entitiesTransform = GameObject.Find(path).transform;
        Transform enemiesTransform = entitiesTransform.Find(type);

        if (enemiesTransform != null)
        {
            GameObject[] units = new GameObject[enemiesTransform.childCount];
            for (int i = 0; i < enemiesTransform.childCount; i++)
            {
                units[i] = enemiesTransform.GetChild(i).gameObject;
            }
            return units;
        }

        return null; // Return null if no units are found
    }
}
