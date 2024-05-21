using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostBattle : MonoBehaviour
{
    public Canvas canvas;
    private BattleInitializer battleInitializer;
    public float GoldGain, ResourceGain, ItemDropRate;
    private GameInfo gameInfo;
    private MapMaker mapMaker;
    private InventoryManager inventoryManager;
    //Navn på alle equipments
    private readonly string[] equipment = new[]{"Leather Gloves", "Leather Chest", "Leather Gloves", "Iron Sword", "Steel Helm"};
    void Load() {
        gameInfo = GetComponent<GameInfo>();
        mapMaker = GameObject.FindWithTag("MainCamera").GetComponent<MapMaker>();
        battleInitializer = GameObject.FindWithTag("GameInfo").GetComponent<BattleInitializer>();
        FindObj("Leave").GetComponent<Button>().onClick.AddListener(Close);
        print("Attacker: " + battleInitializer.attackingHero + "\nDefender: " + battleInitializer.defendingHero);
        //Setter border og inn i territoire
        if(FindObj("Header").GetComponent<Text>().text == "Victory" && battleInitializer.reward) {
            battleInitializer.GetCurTile().eier = "friendly";
            //Forhindrer dobbel borders
            foreach(Transform t in battleInitializer.GetCurTile().gameObject.transform) {
                if(t.name == "BorderEnemy(Clone)" || t.name == "BorderFriendly(Clone)") {
                    Destroy(t.gameObject);
                }
            }
            //Angrip enemy i territoriet
            if(battleInitializer.defendingEnemyTown && battleInitializer.reward) {
                battleInitializer.defendingEnemyTown.Flip();
            }
            //Neutral tile
            else if(!battleInitializer.defendingHero) {
                var i = Instantiate(Resources.Load<GameObject>("BorderFriendly"), battleInitializer.GetCurTile().gameObject.transform);
                i.GetComponent<BorderOwnedTown>().Town = mapMaker.FindClosestTown(battleInitializer.GetCurTile().gameObject, "friendly");
                mapMaker.FindClosestTown(battleInitializer.GetCurTile().gameObject, "friendly").GetComponent<Town>().addTerritory(i.transform.parent.gameObject);
            }
            //PvP for enemy med autoresolve
            else if(battleInitializer.defendingHero && battleInitializer.attackingHero && battleInitializer.attackingHero.side == "enemy") {
                print("Etter PvP");
                if(battleInitializer.Won()) {
                    battleInitializer.defendingHero.army = new string[0];
                    battleInitializer.TakeArmyDamage(battleInitializer.attackingHero);
                } else {
                    battleInitializer.attackingHero.army = new string[0];
                    battleInitializer.TakeArmyDamage(battleInitializer.defendingHero);
                }
            }
            
        }   
        //Player
        float gold = AddRandom(GoldGain);
        FindObj("InfoPane").transform.Find("Gold").transform.Find("Text").GetComponent<Text>().text = gold+"";
        gameInfo.IncreseTotalGold(gold);
        Resource resource = FindResource();
        AddResource(resource);
        FindObj("InfoPane").transform.Find("Resource").transform.Find("Text").GetComponent<Text>().text = resource.amount+"";
        FindObj("InfoPane").transform.Find("Resource").GetComponent<Image>().sprite = resource.LoadResourceImage();
        AddItem();

        //Enemy
        gameInfo.IncreseTotalGoldE(gold);
        battleInitializer.reward = true;
        battleInitializer.Reset();
    }
    //void MoveChar(HeroClass)
    public void SetWinner(string x) { FindObj("Header").GetComponent<Text>().text = x; }
    public void Open() {
        canvas.enabled = true;
        Load();
        battleInitializer.canvas.enabled = false;
    }
    void AddItem() {
        float rnd = UnityEngine.Random.Range(0, 100);
        if(rnd <= ItemDropRate) {
            inventoryManager = GameObject.FindWithTag("InventoryUI").GetComponent<InventoryManager>();
            //Bare hvis du vinner får du item
            if(FindObj("Header").GetComponent<Text>().text == "Victory") {
                GameObject eq = GetRandomItem();
                FindObj("InfoPane").transform.Find("Item").transform.Find("ItemName").GetComponent<Text>().text = eq.name;
                FindObj("InfoPane").transform.Find("Item").GetComponent<Image>().sprite = eq.GetComponent<Image>().sprite;
                inventoryManager.AddItemImages(eq);
                FindObj("InfoPane").transform.Find("Item").GetComponent<Image>().color = new Color(1,1,1,1);
            } else {
                FindObj("InfoPane").transform.Find("Item").GetComponent<Image>().sprite = null;
                FindObj("InfoPane").transform.Find("Item").GetComponent<Image>().color = new Color(1,1,1,0);
                FindObj("InfoPane").transform.Find("Item").transform.Find("ItemName").GetComponent<Text>().text = "No Item";
            }
        } else {
            FindObj("InfoPane").transform.Find("Item").GetComponent<Image>().sprite = null;
            FindObj("InfoPane").transform.Find("Item").GetComponent<Image>().color = new Color(1,1,1,0);
            FindObj("InfoPane").transform.Find("Item").transform.Find("ItemName").GetComponent<Text>().text = "No Item";
        }
    }
    GameObject GetRandomItem() {
        return Resources.Load<GameObject>(equipment[UnityEngine.Random.Range(0,equipment.Length)]);
    }
    Resource FindResource() {
        Dictionary<string, List<string>> resourcesByTileType = new Dictionary<string, List<string>>
        {
            { "Grass", new List<string> { "food" } },
            { "Tundra", new List<string> { "iron", "stone" } },
            { "Forest", new List<string> { "wood" } }
        };
        if(battleInitializer.GetCurTile()) {
            string currentTileName = battleInitializer.GetCurTile().type;
            // Check if resources are available for the current tile type
            if (resourcesByTileType.ContainsKey(currentTileName)) {
                // Get the list of resources for the current tile type
                List<string> tileResources = resourcesByTileType[currentTileName];
                if (tileResources.Count > 0) {
                    int randomIndex = UnityEngine.Random.Range(0, tileResources.Count);
                    return new Resource(tileResources[randomIndex], AddRandom(ResourceGain));
                }
            }
        }
        return null;
    }
    void AddResource(Resource x) {
        switch(x.type) {
            case "food" : { 
                gameInfo.IncreseTotalFood(x.amount);
                gameInfo.IncreseTotalFoodE(x.amount);
                break;
            }
            case "wood" : { 
                gameInfo.IncreseTotalWood(x.amount);
                gameInfo.IncreseTotalWoodE(x.amount);
                break;
            }
            case "gold" : { 
                gameInfo.IncreseTotalGold(x.amount);
                gameInfo.IncreseTotalGoldE(x.amount);
                break;
            }
            case "iron" : { 
                gameInfo.IncreseTotalIron(x.amount);
                gameInfo.IncreseTotalIronE(x.amount);
                break;
            }
            case "stone" : { 
                gameInfo.IncreseTotalStone(x.amount);
                gameInfo.IncreseTotalStoneE(x.amount);
                break;
            }
            
        }
    }
    GameObject FindObj(string name) {
        foreach(Transform child in canvas.transform) {
            if(child.name == name) 
                return child.gameObject;
        }
        return null;
    }
    int AddRandom(float x) {
        float diff = 0.1f; 
        float randomFactor = UnityEngine.Random.Range(-diff, diff); 
        float result = x + x * randomFactor;
        return FindObj("Header").GetComponent<Text>().text == "Victory" ? (int)result : (int)(result * 0.1f); //10% på defeat
    }
    void Close() {
        canvas.enabled = false;
    }
}