using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class HeroClass : MonoBehaviour
{
    public GameObject uiObject,CurrentTile; // Reference to the UI object you want to make visible
    public KeyCode interactionKey = KeyCode.E, battleKey = KeyCode.F; // Define the key to trigger interaction
    public string heroName = "PlayerName"; // Name of the hero
    public string side;
    private AudioManager AudioMan;

    public Sprite daggerS, bowS, swordS, wizardS, knightS, catwomanS, witchS, orcS, skeletonS;

    private string[] _army; // Backing field for the army array
    public string[] army // Property for the army array
    {
        get { return _army; }
        set
        {
            _army = value;
            OnArmyChanged(); // Trigger function when army array is changed
        }
    }
    private BattleInitializer battleInitializer;
    public GameObject nameDisplay; // Reference to the NameDisplay GameObject
    public GameObject unitGrid; // Reference to the UnitGrid GameObject
    public GameObject unitTemplate; // Reference to the template GameObject for units
    public GameObject DeployButton, DeployButtonText;
    public float maxCellSizeX = 65f; // Maximum cell size for GridLayoutGroup
    private MapMaker mapMaker;
    public Town currentTown;

    private bool isHovering = false;
    
    void Start()
    {
        // Initialize the army array with the test elements and additional random values
        army = new string[] { 
            "Dagger;40;40;This units uses fast close range dagger attack to wound enemy units"
        };

        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        // Call OnArmyChanged to initialize the UI based on the initial army array
        OnArmyChanged();
        battleInitializer = GameObject.FindWithTag("GameInfo").GetComponent<BattleInitializer>();
        DeployButton.GetComponent<Button>().onClick.AddListener(ButtonClickHandle);
        mapMaker = GameObject.FindWithTag("MainCamera").GetComponent<MapMaker>();
    }

    void Update()
    {
        // Check if the mouse pointer is hovering over an object in the "Units" layer with the "Hero" tag
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Units"));
        if (hit.collider != null && hit.collider.CompareTag("Hero"))
        {
            if (hit.collider.gameObject == transform.gameObject) isHovering = true;
        }
        else
        {
            isHovering = false;
        }

        // Toggle UI visibility when hovering and pressing the interaction key
        if (this.isHovering && Input.GetKeyDown(interactionKey))
        {
            this.ToggleUIVisibility();
        }
        
        if (isHovering && Input.GetKeyDown(battleKey)) {
            List<GameObject> temp = mapMaker.Naboer(transform.parent.gameObject);
            foreach(GameObject g in temp) {
                print("TILE: " + g.name);
                foreach(Transform t in g.transform) print(t.name);
                //Neutral tile
                if(g.transform.Find("BorderFriendly(Clone)") && !transform.parent.Find("BorderFriendly(Clone)")) {
                    print("Neutral tile");
                    battleInitializer.attackingHero = this;
                    battleInitializer.reward = true;
                    battleInitializer.Open(transform.parent.GetComponent<TileType>());
                    break;
                } 
                //Angrip enemy i territoriet
                else if(g.transform.Find("MainChar(Clone)") && g.transform.Find("BorderEnemy(Clone)")) {
                    
                    print("Angrip enemy i territoriet");
                    battleInitializer.defendingEnemyTown = g.transform.Find("BorderEnemy(Clone)").GetComponent<BorderOwnedTown>().Town.GetComponent<EnemyTown>();
                    battleInitializer.attackingHero = this;
                    battleInitializer.defendingHero = g.transform.Find("MainChar(Clone)").GetComponent<HeroClass>();
                    battleInitializer.reward = false;
                    battleInitializer.Open(transform.parent.GetComponent<TileType>());
                    break;
                }
                //Angrip enemy utenfor noen sitt territoriet
                else if(g.transform.Find("MainChar(Clone)")) {
                    print("Angrip enemy utenfor territoriet");
                    battleInitializer.attackingHero = this;
                    battleInitializer.defendingHero = g.transform.Find("MainChar(Clone)").GetComponent<HeroClass>();
                    battleInitializer.reward = false;
                    battleInitializer.Open(transform.parent.GetComponent<TileType>());
                    break;
                    
                } 
                //Angrip enemy i ditt territoriet
                else if(g.transform.Find("MainChar(Clone)") && transform.parent.Find("BorderFriendly(Clone)")) {
                    print("Angrip enemy i ditt territoriet");
                    battleInitializer.attackingHero = this;
                    battleInitializer.defendingHero = g.transform.Find("MainChar(Clone)").GetComponent<HeroClass>();
                    battleInitializer.defendingAllyTown = transform.parent.Find("BorderFriendly(Clone)").GetComponent<BorderOwnedTown>().Town.GetComponent<Town>();
                    battleInitializer.reward = false;
                    battleInitializer.Open(transform.parent.GetComponent<TileType>());
                    break;
                }
                //Angriper enemy territoriet uten en helt på tilen, hvis border ved fiende
                else if(transform.parent.transform.Find("BorderEnemy(Clone)")) {
                    print("Angriper enemy territoriet uten en helt på tilen");
                    foreach(GameObject n in mapMaker.Naboer(transform.parent.gameObject)) {
                        if(n.transform.Find("BorderFriendly(Clone)")) {
                           
                            battleInitializer.attackingHero = this;
                            battleInitializer.defendingEnemyTown = transform.parent.transform.Find("BorderEnemy(Clone)").GetComponent<BorderOwnedTown>().Town.GetComponent<EnemyTown>();
                            battleInitializer.Open(transform.parent.GetComponent<TileType>());
                            battleInitializer.reward = true;
                            break;
                        }
                    }
                    break;
                }
            }
        }

        CurrentTile = null;
        if (transform.parent.Find("BorderFriendly(Clone)")){
            CurrentTile = transform.parent.Find("BorderFriendly(Clone)").GetComponent<BorderOwnedTown>().Town;
        }


        if (CurrentTile != null && side == "friendly") {
            DeployButton.SetActive(true);
            
            DeployButtonText.GetComponent<TMP_Text>().text = "Deploy units from " + CurrentTile.GetComponent<Town>().cityName;
        }
        else{
            DeployButton.SetActive(false);
        }
    }

    void ToggleUIVisibility()
    {
        bool currentState = uiObject.activeSelf;
        this.uiObject.SetActive(!currentState);
        AudioMan.PlayFX(AudioMan.ArmySound);
        
        if (!currentState && nameDisplay != null) // If the UI is being toggled to be active and the NameDisplay GameObject is assigned
        {
            // Get the TextMeshPro component attached to the NameDisplay GameObject
            TMP_Text textComponent = nameDisplay.GetComponent<TMP_Text>();
            if (textComponent != null)
            {
                // Set the text of the TextMeshPro component to heroName
                textComponent.text = heroName;
            }
            else
            {
                Debug.LogWarning("No TextMeshPro component found on assigned GameObject named 'NameDisplay'.");
            }
        }
    }

    // Function to trigger when army array is changed
    public void OnArmyChanged()
    {
        // Clear all children of the UnitGrid GameObject
        foreach (Transform child in unitGrid.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < army.Length; i++)
        {
            // Split the string by the ";" character
            string[] values = army[i].Split(';');
            
            if (values[1] == "0"){
                List<string> someList = new List<string>(army);
                someList.RemoveAt(i);
                army = someList.ToArray();
            }
        }

        // Iterate through each element in the army array
        for (int i = 0; i < army.Length; i++)
        {
            // Split the string by the ";" character
            string[] values = army[i].Split(';');


            // Clone the template GameObject
            GameObject unit = Instantiate(unitTemplate, unitGrid.transform);

            // Access the child GameObjects of the cloned GameObject
            GameObject nameLabel = unit.transform.Find("NameLabel").gameObject;
            GameObject countLabel = unit.transform.Find("UnitCount&Limit").gameObject;
            GameObject descLabel = unit.transform.Find("UnitDesc").gameObject;

            // Set the text of the TMP_Text components with the corresponding values from the army array
            nameLabel.GetComponent<TMP_Text>().text = values[0]; // Unit name
            countLabel.GetComponent<TMP_Text>().text = values[1] + "/" + values[2]; // Unit count/limit
            descLabel.GetComponent<TMP_Text>().text = values[3]; // Unit description

            switch(values[0]) {
            case "Dagger":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = daggerS;
                break;
            case "Sword":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = swordS;
                break;
            case "Bow":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = bowS;
                break;
            case "Wizard":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = wizardS;
                break;
            case "Knight":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = knightS;
                break;
            case "Catwoman":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = catwomanS;
                break;
            case "Witch":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = witchS;
                break;
            case "Orc":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = orcS;
                break;
            case "Skeleton":
                unit.transform.Find("UnitImage").GetComponent<Image>().sprite = skeletonS;
                break;
            }
        }

        // Calculate the total width of the UnitGrid's RectTransform
        float totalWidth = unitGrid.GetComponent<RectTransform>().rect.width - 10f;

        // Calculate the average width based on the number of child GameObjects
        float averageWidth = totalWidth / army.Length;

        // Calculate the new cell size X value with a maximum of 65
        float newCellSizeX = Mathf.Min(averageWidth, 65f);

        // Get the GridLayoutGroup component attached to the UnitGrid GameObject
        GridLayoutGroup gridLayout = unitGrid.GetComponent<GridLayoutGroup>();

        // Set the new cell size X value
        gridLayout.cellSize = new Vector2(newCellSizeX, gridLayout.cellSize.y);
    }

    public void AddUnitsToArmy(string[] unitsInfo){
        print("Adding units: ");
        foreach (string unitInfo in unitsInfo){
            print("" + unitInfo);
            string[] values = unitInfo.Split(';');

            string unitName = values[0];
            int countToAdd = int.Parse(values[1]);
            int maxCount = int.Parse(values[2]);


            bool unitExists = false;

            for (int i = 0; i < army.Length; i++){

                string[] armyValues = army[i].Split(';');
                if (armyValues[0] == unitName){

                    unitExists = true;

                    int currentCount = int.Parse(armyValues[1]);

                    int newCount = Mathf.Min(currentCount + countToAdd, maxCount);

                    int remainingCount = (currentCount + countToAdd > maxCount) ? (currentCount + countToAdd - maxCount) : 0;

                    army[i] = unitName + ";" + newCount + ";" + maxCount + ";" + values[3];

                    if (remainingCount > 0){
                        string newUnitInfo = unitName + ";" + remainingCount + ";" + maxCount + ";" + values[3];
                        List<string> tempList = new List<string>(army);
                        tempList.Add(newUnitInfo);
                        army = tempList.ToArray();
                    }

                    break;
                }


            } 

            if (!unitExists){
                List<string> tempList = new List<string>(army);
                tempList.Add(unitInfo);
                army = tempList.ToArray();
            }
        }
        OnArmyChanged();
    }

    void ButtonClickHandle(){
        CurrentTile.GetComponent<Town>().armyTransaction(gameObject);
        AudioMan.PlayFX(AudioMan.ClickSound);
    }

}
