using System.Collections.Generic;
using UnityEngine;

public class EnemyTown : MonoBehaviour
{
    private float coinsPerTurn, woodPerTurn, ironPerTurn, foodPerTurn, stonePerTurn, oilPerTurn;
    private const float STARTGOLD = 10;
    public List<GameObject> territory;
    private GameInfo gameInfo;
    public string cityName;
    public static List<string> byNavnE = new() { "Tromsø", "Bodø", "Harstad", "Narvik", "Alta", "Hammerfest", "Mo i Rana", "Vadsø", "Kirkenes"};
    private readonly List<int> buildingCost = new(){150,150,150,150,150,150,150}; // i rekkefølge: farm, lumber, iron, gold, stone, hunter cabin, oil
    private GameObject currentProduction;
    private int turnsRemaining;
    private bool help = false;
    private const int turnCostTier1Buildings = 2, turnCostTier1Units = 2;
    private GameObject curTileForBuilding = null;
    public List<GameObject> standingArmy = new List<GameObject>();
    private CityManagement cityManagement;
    private MapMaker mapMaker;
    private List<List<string>> soldierInfo = new List<List<string>>()
    {
        //                  1. max count        2. desc     
        new List<string>    {"40",              "This units uses fast close range dagger attack to wound enemy units"},         // Dagger
        new List<string>    {"30",              "This units uses long range bow attack to wound enemy units"},                  // Bow
        new List<string>    {"50",              "This units uses slow close range sword attack to wound enemy units"},          // Sword
        new List<string>    {"15",              "This units uses magical attacks to wound enemies"},                            // Wizard
        new List<string>    {"25",              "This units is a proud knight using sword and shield"},                         // Knight
        new List<string>    {"10",              "This units uses her whip to punish her enemies"},                              // Catwoman
        new List<string>    {"20",              "This units uses magical attacks to wound enemies"},                            // Witch
        new List<string>    {"5",               "Me uses pointy stick. very fun, very much like"},                              // Orc
        new List<string>    {"15",              "'ching ching ching'"}                                                          // Skeleton
    };
    private readonly List<string> buildings = new() { "Farm", "GoldMine", "StoneMine", "CornMine", "Hunter", "LumberMill"};
    void Awake() {
        territory = new List<GameObject>();
    }
    void Start()
    {
        int rnd = UnityEngine.Random.Range(0,byNavnE.Count);
        cityName = byNavnE[rnd];
        byNavnE.RemoveAt(rnd);
        coinsPerTurn = STARTGOLD;
        gameInfo = GameObject.FindWithTag("GameInfo").GetComponent<GameInfo>();
        cityManagement = GameObject.FindGameObjectWithTag("TownUI").GetComponent<CityManagement>();
        mapMaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MapMaker>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateResourcesPerTurn();
    }
    public void Flip() {
        var hex = transform.gameObject;
        hex.AddComponent<Town>();
        var town = hex.GetComponent<Town>();
        town.territory = territory;
        hex.GetComponent<TileType>().eier = "friendly";
        foreach(GameObject t in territory) {
            foreach(Transform c in t.transform) {
                if(c.name == "BorderEnemy(Clone)") {
                    Destroy(c.gameObject);
                    var i = Instantiate(Resources.Load<GameObject>("BorderFriendly"), t.transform);
                    i.GetComponent<BorderOwnedTown>().Town = town.gameObject;
                }
            }
        }
        foreach(GameObject t in territory) {
            mapMaker.unFog(t, 2);
        }
        mapMaker.towns.Add(town.gameObject);
        mapMaker.townsE.Remove(gameObject);
        Destroy(this);
    }
    Transform GetResourceChild(GameObject parent) {
        foreach(Transform child in parent.transform) {
            if(child.CompareTag("Resource")) return child;
        }
        return null;
    }
    GameObject CanBuildSomething() {
        // Shuffle territory slik at den ikke henger seg opp
        List<GameObject> shuffledTerritory = new List<GameObject>(territory);
        ShuffleList(shuffledTerritory);

        foreach(GameObject t in shuffledTerritory) {
            foreach (Transform child in t.transform) {
                if(child.CompareTag("Resource") ) {
                    bool help = true;
                    foreach(string b in buildings) 
                        if(child.name.Equals(b)) help = false;
                    
                    if(!help) return null;
                    turnsRemaining = turnCostTier1Buildings;
                    return t;
                }
            }
            if(t.GetComponent<TileType>().type == "Forest") {
                foreach(Transform child in t.transform) 
                    if(child.name.Equals("LumberMill") || child.name.Equals("Hunter")) return null;
                
                turnsRemaining = turnCostTier1Buildings;
                return t;
            }
        }
        return null;
    }
    void ShuffleList<T>(List<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void armyTransaction(GameObject SelUnit) {
        List<string> tempList = new List<string>();
        for (int i=0; i < standingArmy.Count; i++)
        {   
            Debug.Log("i="+i);
            int soldierIndex = -1;
            switch(standingArmy[i].name) {
            case "Dagger":
                soldierIndex = 0; 
                break;
            case "Sword":
                soldierIndex = 2; 
                break;
            case "Bow":
                soldierIndex = 1;
                break;
            case "Wizard":
                soldierIndex = 3; 
                break;
            case "Knight":
                soldierIndex = 4; 
                break;
            case "Catwoman":
                soldierIndex = 5; 
                break;
            case "Witch":
                soldierIndex = 6; 
                break;
            case "Orc":
                soldierIndex = 7; 
                break;
            case "Skeleton":
                soldierIndex = 8; 
                break;
            }
            Debug.Log(standingArmy[i].name);
            if (soldierIndex != -1){
                string newUnitInfo = standingArmy[i].name + ";" + soldierInfo[soldierIndex][0] + ";" + soldierInfo[soldierIndex][0] + ";" + soldierInfo[soldierIndex][1];
                tempList.Add(newUnitInfo);
            }
            
        }
        SelUnit.GetComponent<HeroClass>().AddUnitsToArmy(tempList.ToArray());
        standingArmy = new List<GameObject>();
    }
    public void BuildBuilding(GameObject x) {
        print("Type. " + x.GetComponent<TileType>().ResourceType);
        switch(x.GetComponent<TileType>().ResourceType) {
            case "Iron" : {
                ChangeObjectProperties(GetResourceChild(x), "IronMine", new Vector2(0.09f, 0.09f));
                break;
            }
            case "Gold" : {
                ChangeObjectProperties(GetResourceChild(x), "GoldMine", new Vector2(0.09f, 0.09f));
                break;
            }
            case "Stone" : {
                ChangeObjectProperties(GetResourceChild(x), "StoneMine", new Vector2(0.09f, 0.09f));
                break;
            }
            case "Corn" : {
                ChangeObjectProperties(GetResourceChild(x), "Farm", new Vector2(0.4f, 0.4f));
                break;
            }
            case "Deer" : {
                ChangeObjectProperties(GetResourceChild(x), "Hunter", new Vector2(0.15f, 0.15f));
                break; 
            } 
            case "" : {
                if(x.GetComponent<TileType>().type == "Forest") {
                    bool help = true;
                    foreach(Transform child in x.transform) {
                        if(child.name == "LumberMill") {
                            help = false;
                        }
                    }
                    if(help) {
                        if(GetResourceChild(x))
                            Destroy(GetResourceChild(x).gameObject);
                        GameObject newObj = new GameObject("LumberMill");
                        newObj.transform.parent = x.transform;
                        newObj.AddComponent<SpriteRenderer>();
                        Sprite img = Resources.Load<GameObject>("LumberMill").GetComponent<SpriteRenderer>().sprite;
                        newObj.transform.position = x.transform.position;
                        newObj.transform.localScale = new Vector2(0.14f, 0.14f);
                        newObj.GetComponent<Renderer>().sortingOrder = 4;
                        newObj.GetComponent<SpriteRenderer>().sprite = img;
                        newObj.tag = "Resource";
                        break;
                    }
                }
                break;
                
            }
        }
        currentProduction = null;
        turnsRemaining = 0;
        help = false;
    }
    public void TrainUnit(string value) {
        int index = value.LastIndexOf('(');
        if (index != -1)   //Fjerner () biten av value
            value = value.Substring(0, index).Trim();
        
        List<Resource> resourceRequirements = new List<Resource>();
        switch(value) {
            case "Dagger":
                resourceRequirements = cityManagement.soldierCosts[0]; 
                break;
            case "Sword":
                resourceRequirements = cityManagement.soldierCosts[2]; 
                break;
            case "Bow":
                resourceRequirements = cityManagement.soldierCosts[1]; 
                break;
            case "Wizard":
                resourceRequirements = cityManagement.soldierCosts[3]; 
                break;
            case "Knight":
                resourceRequirements = cityManagement.soldierCosts[4]; 
                break;
            case "Catwoman":
                resourceRequirements = cityManagement.soldierCosts[5]; 
                break;
            case "Witch":
                resourceRequirements = cityManagement.soldierCosts[6]; 
                break;
            case "Orc":
                resourceRequirements = cityManagement.soldierCosts[7]; 
                break;
            case "Skeleton":
                resourceRequirements = cityManagement.soldierCosts[8]; 
                break;
            default:
                return; 
        }
        
        // Check if you have enough resources
        bool canTrainUnit = CheckResourceAvailability(resourceRequirements);
        
        if (canTrainUnit) {
        // Assign the unit directly without instantiation
        GameObject unitPrefab = null;
        switch (value) {
            case "Dagger":
                unitPrefab = Resources.Load<GameObject>("EnemyDagger");
                unitPrefab.name = "Dagger";
                break;
            case "Sword":
                unitPrefab = Resources.Load<GameObject>("SwordsmanEnemy");
                unitPrefab.name = "Sword";
                break;
            case "Bow":
                unitPrefab = Resources.Load<GameObject>("bowEnemy");
                unitPrefab.name = "Bow";
                break;
            case "Wizard":
                unitPrefab = Resources.Load<GameObject>("WizardEnemy");
                unitPrefab.name = "Wizard";
                break;
            case "Knight":
                unitPrefab = Resources.Load<GameObject>("EnemyKnight");
                unitPrefab.name = "Knight";
                break;
            case "Catwoman":
                unitPrefab = Resources.Load<GameObject>("CatwomanEnemy");
                unitPrefab.name = "Catwoman";
                break;
            case "Witch":
                unitPrefab = Resources.Load<GameObject>("WitchEnemy");
                unitPrefab.name = "Witch";
                break;
            case "Orc":
                unitPrefab = Resources.Load<GameObject>("OrcEnemy");
                unitPrefab.name = "Orc";
                break;
            case "Skeleton":
                unitPrefab = Resources.Load<GameObject>("ESkeleton");
                unitPrefab.name = "Skeleton";
                break;
        }
        currentProduction = unitPrefab;
        turnsRemaining = turnCostTier1Units;
            
        } else {
            Debug.Log("Insufficient resources to train " + value);
        }
    }


    void ChangeObjectProperties(Transform target, string resourcePath, Vector2 newScale) {
        target.name = resourcePath;
        target.tag = "Untagged";
        SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
        GameObject preFab = Resources.Load<GameObject>(resourcePath);
        if (preFab != null) {
            SpriteRenderer newSpriteRenderer = preFab.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && newSpriteRenderer != null) {
                spriteRenderer.sprite = newSpriteRenderer.sprite;
                target.localScale = new Vector3(newScale.x, newScale.y, 1f); 
            } else {
                Debug.LogWarning("SpriteRenderer: fant ikke");
            }
        }
    }
    public void StartOfTurn() {
        if (!currentProduction) {
            float r = UnityEngine.Random.value;
            bool buildBuilding = r  < 0.7f;
            if (buildBuilding) {
                curTileForBuilding = TryBuildBuilding(50); // Attempt building with 40 tries
                if (curTileForBuilding != null && gameInfo.GetTotalGoldE() >= buildingCost[0]) { 
                    currentProduction = curTileForBuilding;
                    gameInfo.IncreseTotalGoldE(buildingCost[0] * -1);
                } else {
                    // Ikke råd
                    ProduceRandomUnit();
                }
            } else {
                ProduceRandomUnit();
            }
        }
    }

    GameObject TryBuildBuilding(int maxTries) {
        GameObject tileForBuilding = CanBuildSomething();
        int retryCount = 0;
        while (tileForBuilding == null && retryCount < maxTries) {
            tileForBuilding = CanBuildSomething();
            retryCount++;
        }
        return tileForBuilding;
    }
    private void ProduceRandomUnit() {
        List<string> temp = new() {"Dagger", "Bow", "Sword", "Wizard"};
        while (temp.Count > 0) {
            int rIndex = UnityEngine.Random.Range(0, temp.Count);
            TrainUnit(temp[rIndex]);
            if (currentProduction) return;
            else temp.RemoveAt(rIndex);
        }
    }
    public void EndOfTurn() {
        gameInfo.IncreseTotalGoldE(coinsPerTurn);
        gameInfo.IncreseTotalIronE(ironPerTurn);
        gameInfo.IncreseTotalWoodE(woodPerTurn);
        gameInfo.IncreseTotalStoneE(stonePerTurn);
        gameInfo.IncreseTotalFoodE(foodPerTurn);
        turnsRemaining--;
        if (turnsRemaining <= 0) {
            if(currentProduction) {
                print(currentProduction.name);
                if(currentProduction.name == "newHex(Clone)")
                    BuildBuilding(currentProduction);
                else {
                    standingArmy.Add(currentProduction);
                    currentProduction = null;
                }
            }
        }
    }
    bool CheckResourceAvailability(List<Resource> resourceRequirements) {
        foreach (Resource resource in resourceRequirements) {
            switch (resource.type) {
                case "Gold":
                    if (resourceRequirements.Exists(r => r.type == "Gold") && gameInfo.GetTotalGoldE() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalGoldE(-resource.amount);
                    break;
                case "Iron":
                    if (resourceRequirements.Exists(r => r.type == "Iron") && gameInfo.GetTotalIronE() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalIronE(-resource.amount); 
                    break;
                case "Food":
                    if (resourceRequirements.Exists(r => r.type == "Food") && gameInfo.GetTotalFoodE() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalFoodE(-resource.amount); 
                    break;
                case "Wood":
                    if (resourceRequirements.Exists(r => r.type == "Wood") && gameInfo.GetTotalWoodE() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalWoodE(-resource.amount); 
                    break;
                case "Stone":
                    if (resourceRequirements.Exists(r => r.type == "Stone") && gameInfo.GetTotalStoneE() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalStoneE(-resource.amount); 
                    break;
            }
        }
        return true; 
    }
    public void CalculateResourcesPerTurn() {
        woodPerTurn = 0;
        stonePerTurn = 0;
        ironPerTurn = 0;
        oilPerTurn = 0;
        coinsPerTurn = 10; //Base income per city
        foodPerTurn = 0;

        foreach(GameObject t in territory) {
            switch(t.transform.GetChild(0).name) {
                case "LumberMill" : {
                    woodPerTurn += 5;
                    break;
                }
                case "StoneMine" : {
                    stonePerTurn += 5;
                    break;
                }
                case "IronMine" : {
                    ironPerTurn += 5;
                    break;
                }
                case "Farm" : {
                    foodPerTurn += 5;
                    break;
                }
                case "GoldMine" : {
                    coinsPerTurn += 5;
                    break;
                }
                case "Hunter" : {
                    foodPerTurn += 5;
                    break;
                }
            }
        }
    }
    public void addTerritory(GameObject x) { territory.Add(x); }
    public List<GameObject> GetTerritory() {return territory;}
}
