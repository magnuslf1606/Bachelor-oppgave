using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MapMaker : MonoBehaviour
{
    public Vector2 mapSize = new Vector2 (60, 25);
    public GameObject hex;
    public GameObject hexes;
    public GameObject Deer, Iron, Stone, Gold, Oil, Corn, Cave, Town;
    private GameObject[] GrassR, ForestR, TundraR, MountainR, WaterR;
    public int LandMasses = 15;
    public int cluster = 1;
    public int EnemyTownsSpawned = 3;
    private GameObject EsettlePoint;
    public Vector2 grow = new Vector2(4,7);
    public int islandSize = 3;
    public int ResourceDensity;
    public float grassDensity, forestDensity, tundraDensity, mountainDensity;
    CamControl cam;
    public List<GameObject> friendlyTerritory, enemyTerritory, towns, townsE, units, unitsE;
    private GameObject movetopos;
    private int resetSettler, resetSettlerE, resetHero, resetHeroE;
    private AudioManager AudioMan;
    // Start is called before the first frame update
    void Awake() {
        mapSize = new Vector2 (PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        EnemyTownsSpawned = PlayerPrefs.GetInt("eCount");
    }
    void Start()
    {
        resetHero = 0;
        resetHeroE = 0;
        resetSettler = 0;
        resetSettlerE = 0;
        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        cam = GetComponent<CamControl>();
        friendlyTerritory = new List<GameObject>();
        enemyTerritory = new List<GameObject>();
        towns = new List<GameObject>();
        townsE = new List<GameObject>();
        units = new List<GameObject>();
        unitsE = new List<GameObject>();
        

        Generate();

        transform.gameObject.GetComponent<CamControl>().moveTo(movetopos.transform.position,3);

    }
    public void SetStartLoaction(string side) {
        if(hexes.transform.childCount > 0) {
            GameObject childObject;
            List<GameObject> arr;

            do {
                // Find a random child tile
                int index = UnityEngine.Random.Range(0, hexes.transform.childCount);
                childObject = hexes.transform.GetChild(index).gameObject;

                // Get the tile type of the child tile
                TileType tileType = childObject.GetComponent<TileType>();

                // Get the neighbors of the child tile
                arr = Naboer(childObject);
                
                bool isValidTile = true;

                // Check if the current tile and its neighbors are not water or mountain
                foreach (GameObject neighbor in arr) {
                    string type = neighbor.GetComponent<TileType>().type;
                    if (type == "Water" || type == "Mountain") {
                        isValidTile = false;
                        break;
                    }
                }
                // Check if the array size is 6
                if (arr.Count != 6) {
                    isValidTile = false;
                }

                if(side == "enemy") {
                    float distance = Vector3.Distance(childObject.transform.position, towns[0].transform.position);
                    float preferdRange = (int) (Math.Min(mapSize.x, mapSize.y) / EnemyTownsSpawned);
                    if(distance <= preferdRange) {
                        isValidTile = false;
                    }
                    
                    for (int i = 0; i < townsE.Count; i++) {
                        float dista = Vector3.Distance(childObject.transform.position, townsE[i].transform.position);
                        if(dista <= preferdRange) {
                            isValidTile = false;
                        }
                    }
                    /*
                    print("KOMMET HIT");
                    isValidTile = true;
                    childObject = EsettlePoint;*/
                }
                if (tileType && tileType.type != "Water" && tileType.type != "Mountain" && isValidTile) {
                    break; // Exit the loop if a valid tile is found
                }
            } while (true);

            DestroyAllChildren(childObject);
            Instantiate(Town, childObject.transform);

            childObject.GetComponent<TileType>().SetEier(side,childObject);
            
            if(side == "friendly") {
                movetopos = childObject;
                childObject.GetComponent<TileType>().type = "Town";
                towns.Add(childObject);
                childObject.AddComponent<Town>();
                friendlyTerritory.Add(childObject);
                unFog(childObject, 3);
            }
                
            if(side == "enemy") {
                townsE.Add(childObject);
                childObject.GetComponent<TileType>().type = "Town";
                childObject.AddComponent<EnemyTown>();
                enemyTerritory.Add(childObject);
            }
                
            // Set the owner of neighboring tiles to "friendly"
            foreach(GameObject neighbor in Naboer(childObject)) {
                neighbor.GetComponent<TileType>().SetEier(side,childObject);
                if (side == "friendly") friendlyTerritory.Add(neighbor);
                else enemyTerritory.Add(neighbor);
            }
        }
    }
    
    void Generate()
    {
        SetLists();

        foreach(Transform child in hexes.transform) Destroy(child.gameObject);

        transform.position = new Vector3(mapSize.x / 2, mapSize.y * 0.75f / 2, -10);
        cam.height = mapSize.y * 0.75f;
        cam.width = (int)mapSize.x;
        cam.barrierLeft.offset = new Vector2(-(mapSize.x / 2) - 51,0);
        cam.barrierRight.offset = new Vector2((mapSize.x / 2) + 50,5f);

        for (int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                Vector2 pos = new Vector2(x, y * 0.75f);

                if (y % 2 == 0) pos.x += 0.5f;
                hex.GetComponent<TileType>().SetEier("neutral",null); 
                Instantiate(hex, pos, Quaternion.identity, hexes.transform);
            }
        }

        float ratio = mapSize.y/mapSize.x;
        int yUnits = 1;
        int mass = Mathf.CeilToInt((float)LandMasses / cluster);
        int xUnits = mass;
        Vector2 oldDiff = new Vector2((float)yUnits/ xUnits, yUnits);

        bool sol = false;
        int attempts = 0;

        while (!sol && attempts < 100){
            attempts += 1;

            yUnits += 1;
            xUnits = Mathf.CeilToInt((float)mass / yUnits);
            float newRatio = (float)yUnits/xUnits;

            if (Mathf.Abs(ratio - newRatio) <= Mathf.Abs(ratio - oldDiff.x)){
                oldDiff = new Vector2(newRatio, yUnits);}
            else{
                yUnits = (int)oldDiff.y;
                xUnits = Mathf.CeilToInt(mass / (float)yUnits);
                sol = true;
            }
        }

        int xBase = Mathf.RoundToInt(mapSize.x / xUnits);
        int yBase = Mathf.RoundToInt(mapSize.y / yUnits);

        int xLimit = Mathf.FloorToInt(xBase / 5f);
        int yLimit = Mathf.FloorToInt(yBase / 5f);

        if (cluster == LandMasses){
            xLimit = xLimit / 2;
        }

        for (int x = 0; x < xUnits; x++){
            Vector2 xLimits = new Vector2(x * xBase + xLimit, xBase - 2 * xLimit);
        
            for (int y = 0; y < yUnits; y++){
                Vector2 yLimits = new Vector2(y * yBase + yLimit, yBase - 2 * yLimit);


                int massTemp = cluster;
                attempts = 0;

                while(massTemp > 0 && attempts < LandMasses * 2){
                    attempts += 1;

                    Vector2 pos = new Vector2(Mathf.Round(xLimits.x + UnityEngine.Random.value * xLimits.y), Mathf.Round(yLimits.x + UnityEngine.Random.value * yLimits.y));

                    if (pos.y % 2 == 0) pos.x += 0.5f;

                    pos.y *= 0.75f;

                    RaycastHit2D hit = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Default"));
                    if (hit.collider != null){
                        TileType newTile = hit.collider.gameObject.GetComponent<TileType>();

                        //Debug.Log("type = " + newTile.type);
                        if (newTile.type == "Water"){
                            

                            int rollGrow = Mathf.RoundToInt(grow.x + UnityEngine.Random.value * (grow.y - grow.x));

                            newTile.grow = rollGrow;
                            newTile.baseGrow = rollGrow;
                            newTile.islandSize = islandSize;
                            newTile.width = (int)mapSize.x;

                            newTile.SwapType("Mountain");
                            
                            newTile.Grow(mountainDensity,grassDensity,forestDensity,tundraDensity);

                            massTemp -= 1;

                            /*
                            int attm = 0;
                            while(attm < 100){
                                attm++;
                                print(attm);
                                pos = new Vector2(Mathf.Round(xLimits.x + UnityEngine.Random.value * xLimits.y), Mathf.Round(yLimits.x + UnityEngine.Random.value * yLimits.y));
                                RaycastHit2D hitLand = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Default"));
                                
                                if (hitLand.collider != null){
                                    TileType newTile2 = hitLand.collider.gameObject.GetComponent<TileType>();

                                    //Debug.Log("type = " + newTile.type);
                                    if (newTile2.type != "Water" && newTile2.type != "Mountain"){
                                        EsettlePoint = hitLand.collider.gameObject;
                                        print("Fant cluster elns");
                                        SetStartLoaction("enemy");
                                        break;
                                    }
                                    
                                    
                                    
                                }
                            }*/
                        }
                        
                        
                    }
                }
            }
        }

        SetStartLoaction("friendly");
        for (int i = 0; i < EnemyTownsSpawned; i++) SetStartLoaction("enemy"); 
        SetTilesToCloesestCity();
        Invoke("DrawGrid", Time.deltaTime);
    }
    public GameObject FindClosestTown(GameObject tile, string side) {
        GameObject closestTown = null;
        float closestDistance = float.MaxValue;
        if(side == "friendly") {
            foreach(GameObject town in towns) { // Assuming towns is a list of GameObjects representing towns
                // Calculate the distance between the tile and the town
                float distance = Vector3.Distance(tile.transform.position, town.transform.position);

                // Update the closest town if a closer one is found
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestTown = town;
                }
            }
        } else {
            foreach(GameObject town in townsE) { // Assuming towns is a list of GameObjects representing towns
                // Calculate the distance between the tile and the town
                float distance = Vector3.Distance(tile.transform.position, town.transform.position);

                // Update the closest town if a closer one is found
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestTown = town;
                }
            }
        }
        
        return closestTown;
    }
    void DestroyAllChildren(GameObject parent)
    {
        parent.tag = "Untagged";
        if (parent != null)
        {
            foreach (Transform child in parent.transform)
                if (child.name != "FOG") Destroy(child.gameObject);
        }
        
    }

    public void unFog(GameObject root, int radius){
        bool unfogged = false;
        foreach (GameObject Nabo in Naboer(root)){
            foreach (Transform child in root.transform)
                if (child.name == "FOG"){
                    Destroy(child.gameObject);
                    unfogged = true;
                }
            if (radius >= 1) unFog(Nabo, radius-1);
        }
        if (unfogged) AudioMan.PlayFX(AudioMan.FogClearingSound);
    }

    public void GenerateResource(GameObject Tile, TileType gO, String t){

        switch(gO.type){
            case "Water":
                int rngWa = Mathf.RoundToInt(UnityEngine.Random.value * (WaterR.Length * ResourceDensity));
                if (rngWa >= 0 && rngWa <= WaterR.Length -1){
                    GameObject roll = WaterR[rngWa];
                    Instantiate(roll, Tile.transform.position, Quaternion.identity, Tile.transform);
                    gO.ResourceType = roll.name;
                }
                break;
            case "Grass":
                int rngGr = Mathf.RoundToInt(UnityEngine.Random.value * (GrassR.Length * ResourceDensity));
                if (rngGr >= 0 && rngGr <= GrassR.Length -1){
                    GameObject roll = GrassR[rngGr];
                    Instantiate(roll, Tile.transform.position, Quaternion.identity, Tile.transform);
                    gO.ResourceType = roll.name;
                }
                break;
            case "Forest":
                int rngFo = Mathf.RoundToInt(UnityEngine.Random.value * (ForestR.Length * ResourceDensity));
                if (rngFo >= 0 && rngFo <= ForestR.Length -1){
                    GameObject roll = ForestR[rngFo];
                    Instantiate(roll, Tile.transform.position, Quaternion.identity, Tile.transform);
                    gO.ResourceType = roll.name;
                }
                break;
            case "Tundra":
                int rngTu = Mathf.RoundToInt(UnityEngine.Random.value * (TundraR.Length * ResourceDensity));
                if (rngTu >= 0 && rngTu <= TundraR.Length -1){
                    GameObject roll = TundraR[rngTu];
                    Instantiate(roll, Tile.transform.position, Quaternion.identity, Tile.transform);
                    gO.ResourceType = roll.name;
                }
                break;
            case "Mountain":
                int rngMo = Mathf.RoundToInt(UnityEngine.Random.value * (MountainR.Length * ResourceDensity * 2));
                if (rngMo >= 0 && rngMo <= MountainR.Length -1){
                    GameObject roll = MountainR[rngMo];
                    Instantiate(roll, Tile.transform.position, Quaternion.identity, Tile.transform);
                    gO.ResourceType = roll.name;
                }
                break;            
            case "Ocean":
                break;
        }
    }
    void SetTilesToCloesestCity() {
        foreach(GameObject tile in friendlyTerritory) { // Assuming tiles is a list of GameObjects representing tiles
            // Find the closest town to the current tile
            GameObject closestTown = FindClosestTown(tile, "friendly");
            // Add the tile to the territory of the closest town
            if (closestTown != null) {
                Town townScript = closestTown.GetComponent<Town>();
                if (townScript == null) {
                    Debug.Log("Town script not found on closest town: " + closestTown.name);
                } else {
                    townScript.addTerritory(tile);
                }
            } else {
                Debug.LogWarning("No town found for tile: " + tile.name);
            }
        }
        foreach(GameObject tile in enemyTerritory) { // Assuming tiles is a list of GameObjects representing tiles
            // Find the closest town to the current tile
            GameObject closestTown = FindClosestTown(tile, "enemy");
            // Add the tile to the territory of the closest town
            if (closestTown != null) {
                EnemyTown townScript = closestTown.GetComponent<EnemyTown>();
                if (townScript == null) {
                    Debug.Log("Town script not found on closest town: " + closestTown.name);
                } else {
                    townScript.addTerritory(tile);
                }
            } else {
                Debug.LogWarning("No town found for tile: " + tile.name);
            }
        }
    }

    public void SettleTown(GameObject tile, GameObject settler){
        Instantiate(Town, tile.transform);
        string side = settler.GetComponent<Settler>().side;
        tile.GetComponent<TileType>().SetEier(side,tile);
        AudioMan.PlayFX(AudioMan.SettleSound);
        foreach (Transform child in tile.transform) {
            if (child.gameObject.tag == "Resource") Destroy(child.gameObject);
        }
            
        if(side == "friendly") {
            tile.GetComponent<TileType>().type = "Town";
            towns.Add(tile);
            tile.AddComponent<Town>();
            tile.GetComponent<Town>().addTerritory(tile);
            friendlyTerritory.Add(tile);
            units.Remove(settler);
            unFog(tile, 3);
            
        }

        if(side == "enemy") {
            townsE.Add(tile);
            tile.GetComponent<TileType>().type = "Town";
            tile.AddComponent<EnemyTown>();
            tile.GetComponent<EnemyTown>().addTerritory(tile);
            enemyTerritory.Add(tile);
            unitsE.Remove(settler);
        }
            
        // Set the owner of neighboring tiles to "friendly"
            foreach(GameObject neighbor in Naboer(tile)) {
                neighbor.GetComponent<TileType>().SetEier(side,tile);
                if (side == "friendly") {
                    friendlyTerritory.Add(neighbor);
                    tile.GetComponent<Town>().addTerritory(neighbor);
                }
                else{
                    enemyTerritory.Add(neighbor);
                    tile.GetComponent<EnemyTown>().addTerritory(neighbor);
                } 
            }

        Destroy(settler);
    }
    public void SetLists(){
        ForestR = new GameObject[]{Deer};
        GrassR = new GameObject[]{Stone, Corn};
        TundraR = new GameObject[]{Iron, Gold, Deer};
        MountainR = new GameObject[]{Cave};
        WaterR = new GameObject[]{Oil};
    }

    internal void GenerateResource()
    {
        throw new NotImplementedException();
    }

    public List<GameObject> Naboer(GameObject naboRot){
        List<GameObject> naboListe = new List<GameObject>{};
        List<Vector2> coords = new List<Vector2>
        {
            new Vector2(naboRot.transform.position.x + 1, naboRot.transform.position.y),
            new Vector2(naboRot.transform.position.x - 1, naboRot.transform.position.y),
            new Vector2(naboRot.transform.position.x - .5f, naboRot.transform.position.y + .75f),
            new Vector2(naboRot.transform.position.x + .5f, naboRot.transform.position.y + .75f),
            new Vector2(naboRot.transform.position.x - .5f, naboRot.transform.position.y - .75f),
            new Vector2(naboRot.transform.position.x + .5f, naboRot.transform.position.y - .75f)
        };

        for (int n = 0; n < 6; n++){
            RaycastHit2D hit = Physics2D.Raycast(coords[n], coords[n], 0, LayerMask.GetMask("Default"));
            if (hit.collider != null){
                naboListe.Add(hit.collider.gameObject);
            }
        }
        return naboListe;
    }
    public List<GameObject> getFriendlyTerritory() { return friendlyTerritory; }
    public void setFriendlyTerritory(List<GameObject> friendlyTerritory) { this.friendlyTerritory = friendlyTerritory; }
    public List<GameObject> getEnemyTerritory() { return enemyTerritory; }
    public void setEnemyTerritory(List<GameObject> enemyTerritory) { this.enemyTerritory = enemyTerritory; }
    public List<GameObject> getFriendlyUnits() { return units; }
    public void setFriendlyUnits(List<GameObject> units) { this.units = units; }
    public List<GameObject> getEnemyUnits() { return unitsE; }
    public void setEnemyUnits(List<GameObject> unitsE) { this.unitsE = unitsE; }

    void DrawGrid(){

        int attempts = 0;
        int placed = 0;

        while (placed < 1 && attempts < 100){
            attempts +=1;

            Vector2 pos = new Vector2(Mathf.RoundToInt(UnityEngine.Random.value * (mapSize.x -1)), Mathf.RoundToInt(UnityEngine.Random.value * (mapSize.y -1)));
            RaycastHit2D hit = Physics2D.Raycast(pos,pos,0,LayerMask.GetMask("Default"));
            if (hit){
                if (hit.collider.gameObject.GetComponent<TileType>().type != "Mountain" && hit.collider.gameObject.GetComponent<TileType>().type != "Water"){
                    int random = UnityEngine.Random.Range(0, Naboer(towns[0]).Count-1);
                    GameObject heroF = (GameObject)Instantiate(Resources.Load("MainChar"),Naboer(towns[0])[random].transform.position, Quaternion.identity, Naboer(towns[0])[random].transform);
                    GameObject settlerF = (GameObject)Instantiate(Resources.Load("MainSettler"),Naboer(towns[0])[random+1].transform.position, Quaternion.identity, Naboer(towns[0])[random+1].transform);
                    for (int i= 0; i < townsE.Count; i++){
                        GameObject heroE = (GameObject)Instantiate(Resources.Load("MainChar"),Naboer(townsE[i])[random].transform.position, Quaternion.identity, Naboer(townsE[i])[random].transform);
                        GameObject settlerE = (GameObject)Instantiate(Resources.Load("MainSettler"),Naboer(townsE[i])[random+1].transform.position, Quaternion.identity, Naboer(townsE[i])[random+1].transform);
                        heroE.GetComponent<HeroClass>().side = "enemy";
                        settlerE.GetComponent<Settler>().side = "enemy";
                        unitsE.Add(heroE);
                        unitsE.Add(settlerE);
                    }
                    units.Add(heroF);
                    units.Add(settlerF);
                    placed += 1;
                }
            }
        }

        GetComponent<PathGrid>().width = (int)mapSize.x;
        GetComponent<PathGrid>().height = (int)mapSize.y;
        GetComponent<PathGrid>().DrawGrid();
    }
    public List<GameObject> GetTowns() { return towns; }
    public List<GameObject> GetTownsE() { return townsE; }

    public void UnitGiver(){
        int tiles = 0;
        int Etiles = 0;

        foreach(GameObject town in towns) {
            tiles += town.GetComponent<Town>().territory.Count;
        }

        foreach(GameObject town in townsE) {
            Etiles += town.GetComponent<EnemyTown>().territory.Count;
        }

        if (resetHero < (int)Math.Floor((double)tiles/50)) {
            resetHero += 1;
            int randomTown = UnityEngine.Random.Range(0, towns.Count-1);
            int random = UnityEngine.Random.Range(0, Naboer(towns[0]).Count-1);
            GameObject heroF = (GameObject)Instantiate(Resources.Load("MainChar"),Naboer(towns[randomTown])[random].transform.position, Quaternion.identity, Naboer(towns[randomTown])[random].transform);
            units.Add(heroF);
        }

        if (resetHeroE < (int)Math.Floor((double)Etiles/50)) {
            resetHeroE += 1;
            int randomTown = UnityEngine.Random.Range(0, townsE.Count-1);
            int random = UnityEngine.Random.Range(0, Naboer(townsE[0]).Count-1);
            GameObject heroE = (GameObject)Instantiate(Resources.Load("MainChar"),Naboer(townsE[randomTown])[random].transform.position, Quaternion.identity, Naboer(townsE[randomTown])[random].transform);
            heroE.GetComponent<HeroClass>().side = "enemy";
            unitsE.Add(heroE);
        }

        if (resetSettler < (int)Math.Floor((double)tiles/20)) {
            resetSettler += 1;
            int randomTown = UnityEngine.Random.Range(0, towns.Count-1);
            int random = UnityEngine.Random.Range(0, Naboer(towns[0]).Count-1);
            GameObject settlerF = (GameObject)Instantiate(Resources.Load("MainSettler"),Naboer(towns[randomTown])[random].transform.position, Quaternion.identity, Naboer(towns[randomTown])[random].transform);
            units.Add(settlerF);
        }

        if (resetSettlerE < (int)Math.Floor((double)Etiles/20)) {
            resetSettlerE += 1;
            int randomTown = UnityEngine.Random.Range(0, townsE.Count-1);
            int random = UnityEngine.Random.Range(0, Naboer(townsE[0]).Count-1);
            GameObject settlerE = (GameObject)Instantiate(Resources.Load("MainSettler"),Naboer(townsE[randomTown])[random].transform.position, Quaternion.identity, Naboer(townsE[randomTown])[random].transform);
            settlerE.GetComponent<Settler>().side = "enemy";
            unitsE.Add(settlerE);
        }
    }
}