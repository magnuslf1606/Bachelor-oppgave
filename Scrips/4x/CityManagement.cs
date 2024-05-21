using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityManagement : MonoBehaviour
{
    public Button cityNameButton;
    public Button escapeButton, trainButton;
    private Town town;
    public Text townName, townGold, townWood, townFood, townIron, townStone, townOil;
    public Dropdown buildingDD, unitDD;
    private GameObject draggedBuilding;
    private SpriteRenderer draggedSpriteRenderer;
    private AudioManager AudioMan;
    public readonly List<int> buildingCost = new List<int>(){150,150,150,150,150,150,150}; // i rekkefølge: farm, lumber, iron, gold, stone, hunter cabin, oil
    public List<List<Resource>> soldierCosts = new List<List<Resource>>()
    {
        new List<Resource> { new Resource("Food", 10), new Resource("Iron", 15) },     // Dagger
        new List<Resource> { new Resource("Food", 10), new Resource("Wood", 15) },      // Bow
        new List<Resource> { new Resource("Food", 10), new Resource("Iron", 15) },      // Sword
        new List<Resource> { new Resource("Food", 20), new Resource("Gold", 35) },     // Wizard
        new List<Resource> { new Resource("Food", 10), new Resource("Stone", 20) },     // Knight
        new List<Resource> { new Resource("Food", 25), new Resource("Gold", 150) },     // Catwoman
        new List<Resource> { new Resource("Gold", 75), new Resource("Wood", 30) },     // Witch
        new List<Resource> { new Resource("Food", 50), new Resource("Stone", 50) },    // Orc
        new List<Resource> { new Resource("Iron", 50), new Resource("Wood", 50) },     // Skeleton
    };
    private GameInfo gameInfo;
    private const int turnCostTier1Buildings = 2, turnCostTier1Units = 2;
    public GameObject imagePathProduction, turnPath;
    private GameObject techTree;
    //Script som viser den selecterte byen
    void Start()
    {
        gameInfo = GameObject.FindWithTag("GameInfo").GetComponent<GameInfo>();
        escapeButton.onClick.AddListener(HideCityInfo);
        trainButton.onClick.AddListener(UnitDDOnChange);
        buildingDD.onValueChanged.AddListener(OnBuildingDDValueChanged);
        techTree = GameObject.FindWithTag("TechTreeUI");
        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        HideCityInfo();
        
    }
    void Update() {
        //evt fikse for ytelse senere
        if(town) {
            townName.text = town.GetComponent<Town>().getCityName();
            townGold.text = town.GetComponent<Town>().GetCoinsPerTurn()+ "";
            townWood.text = town.GetComponent<Town>().GetWoodPerTurn() + "";
            townFood.text = town.GetComponent<Town>().GetFoodPerTurn() + "";
            townIron.text = town.GetComponent<Town>().GetIronPerTurn() + "";
            townStone.text = town.GetComponent<Town>().GetStonePerTurn() + "";
            townOil.text = town.GetComponent<Town>().GetOilPerTurn() + "";
        } 
        //Mouse event
        BuildingPlacment();
        
    }

    void UnitDDOnChange() {
        town.SetTrainDD(unitDD);
        town.TrainUnit(unitDD.options[unitDD.value].text);
        town.SetTurnsRemaining(turnCostTier1Units);
        town.SetImagePath(imagePathProduction);
        town.SetTurnPath(turnPath);
        town.UpdateQueue();
    }
    void BuildingPlacment() {
        if(draggedBuilding != null) {
            Vector3 mousePosition = GetComponent<RectTransform>().anchoredPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; 

            // Update the position of the dragged sprite to follow the cursor
            draggedBuilding.transform.position = mousePosition;
            draggedBuilding.GetComponent<SpriteRenderer>().sortingOrder = 13;
            var x = techTree.transform.Find("Scroll View").transform.Find("Viewport").transform.Find("Content");
            
            if(Input.GetMouseButtonDown(0)) {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                print("HIT: " + hit.collider.gameObject.transform.Find("").name);
                if (town.CheckIfInTerritory(hit.collider.gameObject)) { 
                    //For forest tiles
                    if(gameInfo.GetTotalGold() >= buildingCost[1] && hit.collider.gameObject.GetComponent<TileType>().type == "Forest" && TypeMatchOnTile_Building(hit.collider.gameObject.GetComponent<TileType>().type)) {
                        town.whatToBuild = buildingDD.options[buildingDD.value].text + "";
                        town.SetCurrentProduction(hit.collider.gameObject);
                        //Build Tech
                        if(x.transform.Find("TurnReduction").transform.Find("Time").GetComponent<Text>().text == "")
                            town.SetTurnsRemaining(x.transform.Find("TurnReduction").GetComponent<BuildDecrease>().SetBuildTimer(turnCostTier1Buildings));
                        else
                            town.SetTurnsRemaining(turnCostTier1Buildings);
                        
                        town.SetImagePath(imagePathProduction);
                        town.SetTurnPath(turnPath);
                        town.UpdateQueue();
                        gameInfo.IncreseTotalGold(buildingCost[1] * -1);
                        Destroy(draggedBuilding);
                        buildingDD.value = 0;
                    } else {
                        Transform tra = null;
                        foreach(Transform t in hit.collider.gameObject.transform) {
                            if(t.CompareTag("Resource")) tra = t;
                        }
                        if(gameInfo.GetTotalGold() >= buildingCost[buildingDD.value] && TypeMatchOnTile_Building(tra.name)) {
                            
                            //Build Tech
                            if(x.transform.Find("TurnReduction").transform.Find("Time").GetComponent<Text>().text == "")
                                town.SetTurnsRemaining(x.transform.Find("TurnReduction").GetComponent<BuildDecrease>().SetBuildTimer(turnCostTier1Buildings));
                            else
                                town.SetTurnsRemaining(turnCostTier1Buildings);

                            town.whatToBuild = buildingDD.options[buildingDD.value].text + "";
                            print("What to build: " + buildingDD.options[buildingDD.value].text + "");
                            town.SetCurrentProduction(hit.collider.gameObject);
                            town.SetImagePath(imagePathProduction);
                            town.SetTurnPath(turnPath);
                            town.UpdateQueue();
                            gameInfo.IncreseTotalGold(getCorrectBuildingCost(hit.collider.gameObject.transform.GetChild(0).name) * -1);
                            Destroy(draggedBuilding);
                            buildingDD.value = 0;
                        } 
                    }
                }    
            }
            if(Input.GetMouseButtonDown(1)) {
                Destroy(draggedBuilding);
            }
        }
    }
    
    int getCorrectBuildingCost(string x) {
        switch(x) {
            case "Corn(Clone)" : {return buildingCost[0];}
            case "Iron(Clone)" : {return buildingCost[2];}
            case "Gold(Clone)" : {return buildingCost[3];}
            case "Stone(Clone)" : {return buildingCost[4];}
            case "Deer(Clone)" : {return buildingCost[5];}
            //case "Oil(Clone)" : {return buildingCost[6];}
        }
        return -1;
    }
    bool TypeMatchOnTile_Building(string x) {
        int selectedIndex = buildingDD.value;
        switch (x) {
            case "Corn(Clone)":
                return buildingDD.options[selectedIndex].text == "Farm";
            case "Forest":
                return buildingDD.options[selectedIndex].text == "Lumber Mill";
            case "Iron(Clone)":
                return buildingDD.options[selectedIndex].text == "Iron Mine";
            case "Gold(Clone)":
                return buildingDD.options[selectedIndex].text == "Gold Mine";
            case "Stone(Clone)":
                return buildingDD.options[selectedIndex].text == "Stone Mine";
            case "Deer(Clone)":
                return buildingDD.options[selectedIndex].text == "Hunter Cabin";
            default:
            return false;
        }
    }    

    void OnBuildingDDValueChanged(int index)
    {
        switch(buildingDD.options[index].text) { //Valgene fra dropdown meny
            case "Farm" : {
                DragBuilding(town.FarmPF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.4f, 0.4f);
                break;
            }
            case "Lumber Mill" : {
                DragBuilding(town.WoodPF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.14f, 0.14f);
                break;
            }
            case "Iron Mine" : {
                DragBuilding(town.IronPF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.09f, 0.09f);
                break;
            }
            case "Gold Mine" : {
                DragBuilding(town.GoldPF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.09f, 0.09f);
                break;
            }
            case "Stone Mine" : {
                DragBuilding(town.StonePF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.09f, 0.09f);
                break;
            }
            case "Hunter Cabin" : {
                DragBuilding(town.DeerPF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.15f, 0.15f);
                break;
            }/*
            case "Oil" : {
                DragBuilding(town.FarmPF.GetComponent<SpriteRenderer>().sprite);
                draggedBuilding.transform.localScale = new Vector2(0.4f, 0.4f);
                break;
            }*/
        }
    }

    void DragBuilding(Sprite originalSprite) {
        // Create a new instance of the building sprite and attach it to the cursor
        draggedBuilding = new GameObject("DraggedBuilding");
        draggedSpriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
        draggedSpriteRenderer.sprite = originalSprite;
    } 
    public void HideCityInfo() {
        if(town)
            HighLightTerretory(false);
        GetComponent<Canvas>().enabled = false;
        Destroy(draggedBuilding);
        AudioMan.PlayFX(AudioMan.ExitSound);
        imagePathProduction.GetComponent<SpriteRenderer>().enabled = false;
        
    }
    public void VisCityInfo() {
        
        buildingDD.value = 0;
        if(town) {
            town.UpdateQueue();
            HighLightTerretory(true);
        }
        GetComponent<Canvas>().enabled = true;
        imagePathProduction.GetComponent<SpriteRenderer>().enabled = true;
    }
    public void SetTown(Town x) { town = x; }
    public void HighLightTerretory(bool x) {
        foreach(GameObject t in town.territory) {
            Transform border = t.transform.Find("BorderFriendly(Clone)");
            if(border) {
                if(x)
                    border.GetComponent<SpriteRenderer>().color = new Color(0.2078432f, 0.4470589f, 1, 0.5019608f);
                else
                    border.GetComponent<SpriteRenderer>().color = new Color(0.3764706f, 0.7176471f, 0.7011231f, 0.5019608f);
            }
        }
    }
}