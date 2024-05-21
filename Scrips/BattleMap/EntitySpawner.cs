using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public GameObject allies, enemies, evenHero;
    private DataAccessor dataAccessor;
    public new Camera camera;
    public int entitesPerSide;
    public TextMeshProUGUI enemy, ally;
    //NB :   DISSE MÅ VÆRE SAMME REKKEFØLGE SOM STRINGEN GAME DATA FOR BEGGE SIDENE
    private readonly string[] prefabsAlly = {"DaggermanAlly", "SwordsmanAlly", "AllyBow", "Wizard", "Knight", "Catwoman", "Witch", "OrcMale", "Skeleton" };
    private readonly string[] prefabsEnemy = { "EnemyDagger", "SwordsmanEnemy", "bowEnemy", "WizardEnemy", "EnemyKnight", "CatwomanEnemy", "WitchEnemy", "OrcMaleEnemy", "ESkeleton" };
    
    private readonly String evenPrefab =  "EvenBoss";
    public Dictionary<string, Tuple<int, int>> unitCounts = new Dictionary<string, Tuple<int, int>>();
    public GameOver gameOver;
    private bool evenHeroSpawned = false;

    void Start()
    {   
        dataAccessor = GameObject.FindWithTag("DataAccessor").GetComponent<DataAccessor>();
        SetEntityCount();
        
    }
    //Finner hvor mange tropper det er per unit type
    void SetEntityCount() {
        int help = 0, copy = 0;
        string key = "";
        var arr = dataAccessor.Info;
        for(int i = 4; i < arr.Length; i++) {
            if(help % 3 == 0) key = arr[i];
            else if(help % 3 == 1) copy = int.Parse(arr[i]); 
            else if (help % 3 == 2) unitCounts.Add(key, new Tuple<int, int>(copy, int.Parse(arr[i])));
            help++;
        }
        
    }
    //Sjekker om det er nok enheter på banen
    //Hvis ja -> spawn en random unit og trakk fra en på total unitcount for den spesifike unit
    void FixedUpdate()
    {
        //Ikke optimalt!
        enemy.text = enemies.transform.childCount + " : remaning";
        ally.text =  "remaning : " + allies.transform.childCount;

         
        int maxTries = 50;
        if (enemies.transform.childCount < entitesPerSide)
        {
            int rnd = UnityEngine.Random.Range(0, unitCounts.Count);
            int tries = 0;
            while(true) {
                string unitType = unitCounts.Keys.ElementAt(rnd);
                if(unitCounts[unitType].Item2 > 0) {
                    SpawnEntity(prefabsEnemy[rnd], "enemy");
                   // SpawnEntity(evenPrefab, "enemy");
                    unitCounts[unitType] = Tuple.Create(unitCounts[unitType].Item1, unitCounts[unitType].Item2 - 1); // Decrease enemy count
                    break;
                }
                tries++;
                if(tries > maxTries) break; //forhindrer infinte loop
                rnd = UnityEngine.Random.Range(0, unitCounts.Count);
            } 
        }
     
        if (allies.transform.childCount < entitesPerSide)
        {
            int rnd = UnityEngine.Random.Range(0, unitCounts.Count);
            int tries = 0;
            while(true) {
                string unitType = unitCounts.Keys.ElementAt(rnd);
                if(unitCounts[unitType].Item1 > 0) {
                    SpawnEntity(prefabsAlly[rnd], "ally");
                    unitCounts[unitType] = Tuple.Create(unitCounts[unitType].Item1 - 1, unitCounts[unitType].Item2); // Decrease ally count
                    break;
                }
                tries++;
                if(tries > maxTries) break; //forhindrer infinte loop
                rnd = UnityEngine.Random.Range(0, unitCounts.Count);
            }
        }
        if(enemies.transform.childCount <= 0 && !evenHeroSpawned) {
            // gameOver.GameOverState("YOU WON!");
            SpawnEntity(evenPrefab, "enemy");
            evenHeroSpawned = true;
        }
        if (evenHeroSpawned && enemies.transform.childCount <= 0)
        {
            gameOver.GameOverState("YOU WON!");
        }
        //if(allies.transform.childCount <= 0 && )

    }
    void SpawnEntity(string prefabName, string side)
    {
        GameObject entity = Resources.Load<GameObject>(prefabName);
        if (entity != null)
        {
            GameObject newEnt = Instantiate(entity);
            newEnt.transform.parent = (side == "ally") ? allies.transform : enemies.transform;
            RandomSpawn(newEnt, side);
        }
        else
        {
            Debug.LogError("Failed to load prefab: " + prefabName);
        }
        
    }
    //Random just off screen
    void RandomSpawn(GameObject entity, string side)
    {
        Vector3 position = new Vector3();
        float cameraX = camera.transform.position.x, cameraY = camera.transform.position.y;
        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;
        float offsetX = 1.5f; // Offset on the x-axis
        float offsetY = 4f; // Offset on the y-axis
        float minX = -25f; // Minimum x-coordinate
        float maxX = 25f; // Maximum x-coordinate
        float minY = -8f; // Minimum y-coordinate
        float maxY = 8f; // Maximum y-coordinate

        if (side == "ally")
        {
            float spawnX = Mathf.Clamp(cameraX - cameraWidth / 2f - offsetX, minX, maxX);
            float spawnY = Mathf.Clamp(UnityEngine.Random.Range(cameraY - cameraHeight / 2f, cameraY + cameraHeight / 2f) - offsetY, minY, maxY);
            position = new Vector3(spawnX, spawnY, 2f);
        }
        else if (side == "enemy")
        {
            float spawnX = Mathf.Clamp(cameraX + cameraWidth / 2f + offsetX, minX, maxX);
            float spawnY = Mathf.Clamp(UnityEngine.Random.Range(cameraY - cameraHeight / 2f, cameraY + cameraHeight / 2f) + offsetY, minY, maxY);
            position = new Vector3(spawnX, spawnY, 2f);
        }
        entity.transform.position = position;
    }
    public void UnitCountToString() {
        foreach (var kvp in unitCounts)
        {
            string unitName = kvp.Key;
            int count1 = kvp.Value.Item1;
            int count2 = kvp.Value.Item2;

            // Print the key and values
            Debug.Log($"{unitName}: ({count1}, {count2})");
        }
    }
}