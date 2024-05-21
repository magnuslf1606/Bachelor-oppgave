using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Town : MonoBehaviour
{
    private float coinsPerTurn, woodPerTurn, ironPerTurn, foodPerTurn, stonePerTurn, oilPerTurn;
    private CityManagement cityManagement;
    public string cityName; //ID
    private const float STARTGOLD = 10;
    public List<GameObject> territory;
    private GameInfo gameInfo;
    private GameObject farmPF, goldPF, ironPF, stonePF, oilPF, woodPF, deerPF;
    public static List<string> byNavn = new List<string>{"Tromsø", "Bodø", "Harstad", "Narvik", "Alta", "Hammerfest", "Mo i Rana", "Vadsø", "Kirkenes"};
    private GameObject currentProduction;
    private int turnsRemaining;
    private GameObject imagePath, turnPath;
    private Dropdown trainDD;
    public List<GameObject> standingArmy = new List<GameObject>();
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
    public string whatToBuild;
    private GameObject techTree;
    // Start is called before the first frame update
    void Awake() {
        territory = new List<GameObject>();
        farmPF = Resources.Load<GameObject>("Farm");
        deerPF = Resources.Load<GameObject>("Hunter");
        goldPF = Resources.Load<GameObject>("GoldMine");
        ironPF = Resources.Load<GameObject>("IronMine");
        stonePF = Resources.Load<GameObject>("StoneMine"); 
        //oilPF = Resources.Load<GameObject>("OilRig");
        woodPF = Resources.Load<GameObject>("LumberMill");
    }
    void Start()
    {
        int rnd = UnityEngine.Random.Range(0,byNavn.Count);
        cityName = byNavn[rnd];
        byNavn.RemoveAt(rnd);
        gameInfo = GameObject.FindWithTag("GameInfo").GetComponent<GameInfo>();
        coinsPerTurn = STARTGOLD;
        techTree = GameObject.FindWithTag("TechTreeUI");
        mapMaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MapMaker>();
        Debug.Log(soldierInfo.Count);
    }
    
    // Update is called once per frame
    void Update()
    {
       CalculateResourcesPerTurn();
    }

    public void UpdateQueue() {
        if(imagePath) 
            imagePath.GetComponent<SpriteRenderer>().sprite = null;
        if(turnPath) 
            turnPath.GetComponent<Text>().text = "";
            //print("Ingen prouksjon i denne byen");
        if(currentProduction) {
            if(imagePath) {
                if(currentProduction.name == "newHex(Clone)") { //Buildings
                    imagePath.GetComponent<SpriteRenderer>().sprite = GetSpirte().sprite;
                    //imagePath.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(35f,35f);
                } else { //Units
                    imagePath.GetComponent<SpriteRenderer>().sprite = currentProduction.GetComponent<SpriteRenderer>().sprite;
                    imagePath.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(200f,200f);
                }
                
                imagePath.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
                imagePath.GetComponent<SpriteRenderer>().sortingOrder = 13;
            }
            if(turnPath) {
                turnPath.GetComponent<Text>().text = turnsRemaining + " turns left";
            }
        } 
        if(trainDD)
            UpdateTrainDropdown();
    }
    public void TrainUnit(string value) {
        int index = value.LastIndexOf('(');
        if (index != -1)   //Fjerner () biten av value
            value = value.Substring(0, index).Trim();
        int soldierIndex;
        switch(value) {
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
            default:
                return; 
        }
        
        // Check if you have enough resources
        if(!cityManagement)
                    cityManagement = GameObject.FindWithTag("TownUI").GetComponent<CityManagement>();
        bool canTrainUnit = CheckResourceAvailability(cityManagement.soldierCosts[soldierIndex]);
        
        if (canTrainUnit) {
            GameObject temp = new GameObject(value);
            temp.AddComponent<SpriteRenderer>();
            switch (value) {
                case "Dagger":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("DaggermanAlly").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Sword":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("SwordsmanAlly").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Bow":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("AllyBow").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Wizard":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("Wizard").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Knight":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("Knight").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Catwoman":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("Catwoman").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Witch":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("Witch").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Orc":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("OrcMale").GetComponent<SpriteRenderer>().sprite;
                    break;
                case "Skeleton":
                    temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<GameObject>("ESkeleton").GetComponent<SpriteRenderer>().sprite;
                    break;
            }
            currentProduction = temp;
            
        } else {
            Debug.Log("Insufficient resources to train " + value);
        }
        
        UpdateQueue();
    }
    SpriteRenderer GetSpirte() {
        SpriteRenderer curr = null;
        if(currentProduction) {
            switch(whatToBuild) {
                case "Farm": { 
                    curr = Resources.Load<GameObject>("Farm").GetComponent<SpriteRenderer>();
                    imagePath.GetComponent<RectTransform>().anchoredPosition = new Vector2(-827.74f, -198.7f);
                    imagePath.transform.localScale = new Vector2(60, 60);
                    break;
                }
                case "Iron Mine":  { 
                    curr = Resources.Load<GameObject>("IronMine").GetComponent<SpriteRenderer>(); 
                    imagePath.GetComponent<RectTransform>().anchoredPosition = new Vector2(-827.74f, -198.7f);
                    imagePath.transform.localScale = new Vector2(15, 15);
                    break;
                }
                case "Gold Mine":  { 
                    curr = Resources.Load<GameObject>("GoldMine").GetComponent<SpriteRenderer>();
                    imagePath.GetComponent<RectTransform>().anchoredPosition = new Vector2(-827.74f, -198.7f);
                    imagePath.transform.localScale = new Vector2(15, 15);
                    break;
                }
                case "Stone Mine": { 
                    curr = Resources.Load<GameObject>("StoneMine").GetComponent<SpriteRenderer>();
                    imagePath.GetComponent<RectTransform>().anchoredPosition = new Vector2(-827.74f, -198.7f);
                    imagePath.transform.localScale = new Vector2(15, 15);
                    break;
                }
                case "Hunter Cabin": { 
                    curr = Resources.Load<GameObject>("Hunter").GetComponent<SpriteRenderer>();
                    imagePath.GetComponent<RectTransform>().anchoredPosition = new Vector2(-812.5f, -184.6f);
                    imagePath.transform.localScale = new Vector2(25, 25);
                    break;
                }
                case "Lumber Mill": { 
                    curr = Resources.Load<GameObject>("LumberMill").GetComponent<SpriteRenderer>();
                    imagePath.GetComponent<RectTransform>().anchoredPosition = new Vector2(-827.9f, -190.3f);
                    imagePath.transform.localScale = new Vector2(23, 23);
                    break;
                }
            }
        } 
        if(curr) return curr;
        return null;
    }
    public bool CheckIfInTerritory(GameObject x) {
        foreach(GameObject t in territory) {
            if(t.Equals(x)) return true;
        }
        return false;
    }
    Transform GetResourceChild(GameObject parent) {
        foreach(Transform child in parent.transform) {
            if(child.CompareTag("Resource")) return child;
        }
        return null;
    }
    //Tile som parameter
    public void BuildBuilding(GameObject x) {
        switch(whatToBuild) {
            case "Iron Mine" : {
                ChangeObjectProperties(GetResourceChild(x), "IronMine", new Vector2(0.09f, 0.09f));
                break;
            }
            case "Gold Mine" : {
                ChangeObjectProperties(GetResourceChild(x), "GoldMine", new Vector2(0.09f, 0.09f));
                break;
            }
            case "Stone Mine" : {
                ChangeObjectProperties(GetResourceChild(x), "StoneMine", new Vector2(0.09f, 0.09f));
                break;
            }
            case "Farm" : {
                ChangeObjectProperties(GetResourceChild(x), "Farm", new Vector2(0.4f, 0.4f));
                break;
            }
            case "Hunter Cabin" : {
                ChangeObjectProperties(GetResourceChild(x), "Hunter", new Vector2(0.15f, 0.15f));
                break;
            }
            case "Lumber Mill" : {
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
        currentProduction = null;
        imagePath.GetComponent<SpriteRenderer>().sprite = null;
        turnPath.GetComponent<Text>().text = "";
        turnsRemaining = 0;
        UpdateQueue();
    }
    
    //Setter riktig spirte og navn på objecktet og en scale (på ressuser som blir gjort om til produksjon)
    void ChangeObjectProperties(Transform target, string resourcePath, Vector2 newScale) {
        target.name = resourcePath;
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
    public void EndOfTurn() {
        var x = techTree.transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");

        if(x.transform.Find("GoldIncrease").transform.Find("Time").GetComponent<Text>().text == "")
            gameInfo.IncreseTotalGold(x.transform.Find("GoldIncrease").GetComponent<ResourceIncrease>().CalculateTotalIncrease(coinsPerTurn));
        
        if(x.transform.Find("StrategicResourceIncrease").transform.Find("Time").GetComponent<Text>().text == "") {
            gameInfo.IncreseTotalIron(x.transform.Find("StrategicResourceIncrease").GetComponent<ResourceIncrease>().CalculateTotalIncrease(ironPerTurn));
            gameInfo.IncreseTotalWood(x.transform.Find("StrategicResourceIncrease").GetComponent<ResourceIncrease>().CalculateTotalIncrease(woodPerTurn));
            gameInfo.IncreseTotalStone(x.transform.Find("StrategicResourceIncrease").GetComponent<ResourceIncrease>().CalculateTotalIncrease(stonePerTurn));
        } 
        if(x.transform.Find("FoodIncrease").transform.Find("Time").GetComponent<Text>().text == "") 
            gameInfo.IncreseTotalFood(x.transform.Find("FoodIncrease").GetComponent<ResourceIncrease>().CalculateTotalIncrease(foodPerTurn));
        
        gameInfo.IncreseTotalGold(coinsPerTurn);
        gameInfo.IncreseTotalIron(ironPerTurn);
        gameInfo.IncreseTotalWood(woodPerTurn);
        gameInfo.IncreseTotalStone(stonePerTurn);
        gameInfo.IncreseTotalFood(foodPerTurn);
        
        turnsRemaining--;
        if (turnsRemaining <= 0) {
            if(currentProduction) {
                if(currentProduction.name == "newHex(Clone)")
                    BuildBuilding(currentProduction);
                else {
                    standingArmy.Add(currentProduction);
                    currentProduction = null;
                }
            }
        }
        UpdateQueue();
    }
    public void Flip() {
        var hex = transform.gameObject;
        hex.AddComponent<EnemyTown>();
        var town = hex.GetComponent<EnemyTown>();
        town.territory = territory;
        hex.GetComponent<TileType>().eier = "enemy";
        foreach(GameObject t in territory) {
            foreach(Transform c in t.transform) {
                if(c.name == "BorderFriendly(Clone)") {
                    Destroy(c.gameObject);
                    var i = Instantiate(Resources.Load<GameObject>("BorderEnemy"), t.transform);
                    i.GetComponent<BorderOwnedTown>().Town = town.gameObject;
                }
            }
        }
        
        mapMaker.townsE.Add(town.gameObject);
        mapMaker.towns.Remove(gameObject);
        Destroy(this);
    }
    void UpdateTrainDropdown()
    {
        // Create a dictionary to store unit names and their counts
        Dictionary<string, int> unitCounts = new Dictionary<string, int>();

        // Count the occurrences of each unit in the standingArmy list
        foreach (GameObject unit in standingArmy)
        {
            string unitName = unit.name;
            if (unitCounts.ContainsKey(unitName))
                unitCounts[unitName]++;
            else
                unitCounts[unitName] = 1;
        }

        // Update existing Dropdown options with unit names and their counts
        foreach (Dropdown.OptionData option in trainDD.options)
        {
            string unitName = option.text.Split(' ')[0]; // Extract unit name from the option text
            if (unitCounts.ContainsKey(unitName))
            {
                int count = unitCounts[unitName];
                option.text = $"{unitName} ({count})"; // Update the option text with count
            }
            else
            {
                option.text = unitName; // If the unit is not found in standingArmy, revert to just the unit name
            }
        }
        trainDD.value = 0;
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
                Debug.Log(newUnitInfo);
                tempList.Add(newUnitInfo);
            }
            
        }
        Debug.Log(tempList.Count);
        SelUnit.GetComponent<HeroClass>().AddUnitsToArmy(tempList.ToArray());
        standingArmy = new List<GameObject>();
        UpdateTrainDropdown();
    }

    bool CheckResourceAvailability(List<Resource> resourceRequirements) {
        foreach (Resource resource in resourceRequirements) {
            switch (resource.type) {
                case "Gold":
                    if (resourceRequirements.Exists(r => r.type == "Gold") && gameInfo.GetTotalGold() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalGold(-resource.amount);
                    break;
                case "Iron":
                    if (resourceRequirements.Exists(r => r.type == "Iron") && gameInfo.GetTotalIron() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalIron(-resource.amount); 
                    break;
                case "Food":
                    if (resourceRequirements.Exists(r => r.type == "Food") && gameInfo.GetTotalFood() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalFood(-resource.amount); 
                    break;
                case "Wood":
                    if (resourceRequirements.Exists(r => r.type == "Wood") && gameInfo.GetTotalWood() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalWood(-resource.amount); 
                    break;
                case "Stone":
                    if (resourceRequirements.Exists(r => r.type == "Stone") && gameInfo.GetTotalStone() < resource.amount)
                        return false; 
                    else
                        gameInfo.IncreseTotalStone(-resource.amount); 
                    break;
            }
        }
        return true; 
    }
    public void CalculateResourcesPerTurn() {
        woodPerTurn = 0;
        stonePerTurn = 0;
        ironPerTurn = 0;
        foodPerTurn = 0;
        coinsPerTurn = 10; //Base income per city
        foodPerTurn = 0;

        foreach(GameObject t in territory) {
            foreach(Transform child in t.transform) {
                if(child.CompareTag("Resource")) {
                    switch(child.name) {
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
            
        }
    }
    public void addTerritory(GameObject x) { territory.Add(x); }
    public List<GameObject> GetTerritory() {return territory;}
    public string getCityName() { return cityName;}
    public float GetCoinsPerTurn() => coinsPerTurn;
    public float GetWoodPerTurn() => woodPerTurn;
    public float GetIronPerTurn() => ironPerTurn;
    public float GetFoodPerTurn() => foodPerTurn;
    public float GetStonePerTurn() => stonePerTurn;
    public float GetOilPerTurn() => oilPerTurn;
    public GameObject FarmPF => farmPF;
    public GameObject GoldPF => goldPF;
    public GameObject IronPF => ironPF;
    public GameObject StonePF => stonePF;
    public GameObject OilPF => oilPF;
    public GameObject WoodPF => woodPF;
    public GameObject DeerPF => deerPF;
    public void SetCurrentProduction(GameObject x) { currentProduction = x;}
    public GameObject GetCurrentProduction() { return currentProduction;}
    public void SetTurnsRemaining(int x) { turnsRemaining = x;}
    public void SetImagePath(GameObject x) { imagePath = x; }
    public void SetTurnPath(GameObject x) { turnPath = x; }
    public void SetTrainDD(Dropdown x) { trainDD = x; }
}
