using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aiActions : MonoBehaviour
{
    private List<GameObject> ExpansionTargets, SettleTargets, EnemyTargets, EnemyHeroTargets, ExploreTargets;
    public UnitControl unitControl;
    public enum AIState { Explore, Expand, Moving, Exterminate, Idle, GetUnits }
    public AIState currentState;
    public BattleInitializer battleInitializer;

    private GameObject Camera;
    private PathFinder pathFinder;
    private PathGrid pathGrid;
    private MapMaker mapMaker;
    public GameObject targetTile, targetHero;
    private int numScans = 0;
    public string type;
    public bool ActionDoneOnTurn = true;
    public bool HasAttacked = false;

    private bool lookingForGetUnit = false;
    public int lifetime = 0;
    private int GoodSettle = 3;
    private bool prepearingToFight = false;
    private bool prepearingToFightPlayer = false;
    private bool prepearingToFightTown = false;
    
    public List<GameObject> visited;
    // Start is called before the first frame update
    void Start()
    {
        Camera = GameObject.FindWithTag("MainCamera");
        battleInitializer = GameObject.FindWithTag("GameInfo").GetComponent<BattleInitializer>();
        pathFinder = Camera.GetComponent<PathFinder>();
        pathGrid = Camera.GetComponent<PathGrid>();
        mapMaker = Camera.GetComponent<MapMaker>();
        ExpansionTargets = new List<GameObject>();
        SettleTargets = new List<GameObject>();
        EnemyTargets = new List<GameObject>();
        EnemyHeroTargets = new List<GameObject>();
        ExploreTargets = new List<GameObject>();
        currentState = AIState.Idle;
        visited = new List<GameObject>();
        if (GetComponent<HeroClass>() != null) type = "hero";
        if (GetComponent<Settler>() != null) type = "settler";
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentState == AIState.Moving && targetHero != null && targetTile != targetHero.transform.parent.gameObject){
            MoveTo(targetHero.transform.parent.gameObject);
        }

        if (currentState == AIState.Moving && transform.parent.gameObject == targetTile && lookingForGetUnit == true){
            GameObject town = targetTile.transform.Find("BorderEnemy(Clone)").GetComponent<BorderOwnedTown>().Town;
            town.GetComponent<EnemyTown>().armyTransaction(transform.gameObject);
            lookingForGetUnit = false;
            currentState = AIState.Idle;
        } 

        if (currentState == AIState.Moving && transform.parent.gameObject == targetTile && prepearingToFight == true){
            //Autoresolve neutral tile
            if (battleInitializer.AutoResolveEnemy(GetComponent<HeroClass>())) {
                var i = Instantiate(Resources.Load<GameObject>("BorderEnemy"), battleInitializer.GetCurTile().gameObject.transform);
                i.GetComponent<BorderOwnedTown>().Town = mapMaker.FindClosestTown(battleInitializer.GetCurTile().gameObject, "enemy");
                mapMaker.FindClosestTown(battleInitializer.GetCurTile().gameObject, "enemy").GetComponent<EnemyTown>().addTerritory(i.transform.parent.gameObject);
            }

            //Autoresolve friendly tile
            

            prepearingToFight = false;
            currentState = AIState.Idle;
        } 

        if (currentState == AIState.Moving && transform.parent.gameObject == targetTile && prepearingToFightPlayer == true){
            if (targetHero.transform.parent.Find("BorderFriendly(Clone)") == null) {
                battleInitializer.Reset();
                battleInitializer.defendingHero = targetHero.GetComponent<HeroClass>();
                if (battleInitializer.AutoResolveEnemy(GetComponent<HeroClass>())) {
                    print("AI won");
                }
                else print("AI lost");

                prepearingToFightPlayer = false;
                currentState = AIState.Idle;
            }
            else {
                battleInitializer.Reset();
                battleInitializer.defendingAllyTown = targetHero.transform.parent.Find("BorderFriendly(Clone)").GetComponent<BorderOwnedTown>().Town.GetComponent<Town>();
                battleInitializer.defendingHero = targetHero.GetComponent<HeroClass>();
                if (battleInitializer.AutoResolveEnemy(GetComponent<HeroClass>())) battleInitializer.defendingAllyTown.Flip();

                prepearingToFightPlayer = false;
                currentState = AIState.Idle;
            }
        }

        if (currentState == AIState.Moving && transform.parent.gameObject == targetTile && prepearingToFightTown == true){
            battleInitializer.Reset();
            battleInitializer.defendingAllyTown = targetTile.transform.Find("BorderFriendly(Clone)").GetComponent<BorderOwnedTown>().Town.GetComponent<Town>();
            if (battleInitializer.AutoResolveEnemy(GetComponent<HeroClass>())) battleInitializer.defendingAllyTown.Flip();

            prepearingToFightTown = false;
            currentState = AIState.Idle;
        }

        if (currentState == AIState.Moving && transform.parent.gameObject == targetTile) currentState = AIState.Idle;

        // settler actions

        if (type == "settler" && !ActionDoneOnTurn){
            ActionDoneOnTurn = true;
            ExploreTargets = new List<GameObject>();
            visited = new List<GameObject>();
            numScans = 0;
            Scan(unitControl.maxMoves*2,transform.parent.gameObject);
            if (currentState == AIState.Idle){
                if (SettleTargets.Count > 0 && SettleTargets.Contains(transform.parent.gameObject) == true){
                    Debug.Log("time to settle");
                    Settle();
                }
                else if (SettleTargets.Count > 0 && SettleTargets.Contains(transform.parent.gameObject) == false){
                    Debug.Log("moving to settle tile");
                    targetTile = SettleTargets[UnityEngine.Random.Range(0, SettleTargets.Count-1)];
                    MoveTo(targetTile);
                }
                else{
                    currentState = AIState.Explore;
                    ExploreMode();
                }
            }
        }

        // hero actions

        if (type == "hero" && !ActionDoneOnTurn){
            ActionDoneOnTurn = true;
            EnemyTargets = new List<GameObject>();
            ExpansionTargets = new List<GameObject>();
            ExploreTargets = new List<GameObject>();
            numScans = 0;
            visited = new List<GameObject>();
            Scan(unitControl.maxMoves*2,transform.parent.gameObject);
            if (currentState == AIState.Idle){
                int randomAction = CalcChance();
                print(randomAction + " ER AI RANDOM INTEN");
                if (randomAction == 0){
                    print("GET UNITS");
                    currentState = AIState.GetUnits;
                    List<GameObject> enemytiles = mapMaker.getEnemyTerritory();
                    List<GameObject> acceptedTiles = new List<GameObject>();
                    foreach (GameObject tile in enemytiles){
                        GameObject t = tile.transform.Find("BorderEnemy(Clone)").GetComponent<BorderOwnedTown>().Town;
                        if (t.GetComponent<EnemyTown>().standingArmy.Count > 0){
                            acceptedTiles.Add(tile);
                        }
                    }
                    GameObject tileAtRandom = acceptedTiles[UnityEngine.Random.Range(0, enemytiles.Count -1)];
                    MoveTo(tileAtRandom);
                    lookingForGetUnit = true;
                }
                else if (randomAction == 1){
                    print("EXPAND!!");
                    currentState = AIState.Expand;
                    MoveTo(ExpansionTargets[UnityEngine.Random.Range(0, ExpansionTargets.Count -1 )]);
                    prepearingToFight = true;
                }
                else if (randomAction == 2){
                    print("EXPLORE!!");
                    currentState = AIState.Explore;
                    ExploreMode();
                }
                else if (randomAction == 3){
                    print("Exterminate >:(");
                    List<GameObject> templist = new List<GameObject>(EnemyHeroTargets);
                    for (int i=0; i<EnemyTargets.Count; i++){
                        templist.Add(EnemyTargets[i]);
                    }
                    print(EnemyTargets.Count + " enemy targets");
                    print(EnemyHeroTargets.Count + " enemy hero targets");
                    print(templist.Count + " combined targets");
                    currentState = AIState.Exterminate;
                    int roll = UnityEngine.Random.Range(0,templist.Count-1);

                    if (EnemyTargets.Contains(templist[roll])) AttackMode(templist[roll], 0);
                    else AttackMode(templist[roll], 1);
                }
            }
        }

        if (GetComponent<UnitControl>().path.Count == 0) currentState = AIState.Idle;
    }


    void MoveTo(GameObject moveToTile){
        currentState = AIState.Moving;
        targetTile = moveToTile;

        Vector2 sPos = transform.position;

        if (sPos.x >= pathGrid.width || sPos.x < 0)
            GetComponent<UnitControl>().offset.x = Mathf.CeilToInt((sPos.x + 1 - pathGrid.width)/pathGrid.width) * pathGrid.width;
        else
            GetComponent<UnitControl>().offset.x = 0;


        if (pathFinder.checkPath(transform.parent.gameObject.transform.position, moveToTile.transform.position, 3)) {
            Debug.Log("Moving to "+moveToTile);
            unitControl.path = pathFinder.GetPath(transform.parent.gameObject.transform.position, moveToTile.transform.position, 3);
        }
        else {
            currentState = AIState.Idle;
            Debug.Log("Invalid Path");
        }
    }

    public void Scan(int range, GameObject tile){
        if (lifetime == 10) GoodSettle = 2;
        numScans++;
        if (range == 0 && tile.GetComponent<TileType>().type != "Water" && tile.GetComponent<TileType>().type != "Mountain") ExploreTargets.Add(tile);
        if (tile.transform.Find("MainChar(Clone)") != null) {
            if (tile.transform.Find("MainChar(Clone)").GetComponent<HeroClass>().side == "friendly") EnemyHeroTargets.Add(tile.transform.Find("MainChar(Clone)").gameObject);
        }
        int NaboWithResource = 0;
        bool isNearTown = false;
        bool isNearPlayersTown = false;
        foreach (GameObject Nabo in mapMaker.Naboer(tile)){
            foreach (Transform child in Nabo.transform){
                if (child.tag == "Resource"){
                    NaboWithResource++;
                }
            }
            

            if (Nabo.transform.Find("BorderEnemy(Clone)") != null) isNearTown = true;
            if (Nabo.transform.Find("BorderFriendly(Clone)") != null) isNearPlayersTown = true;


            if (!visited.Contains(Nabo)){
                visited.Add(Nabo);
                if (range >= 1) Scan(range-1, Nabo);
            }
            
        }
        if (!ExpansionTargets.Contains(tile) && !tile.transform.Find("BorderEnemy(Clone)") && !tile.transform.Find("BorderFriendly(Clone)") && isNearTown) ExpansionTargets.Add(tile);


        if (NaboWithResource >= GoodSettle && !isNearPlayersTown && !isNearTown && type == "settler"){
            if (!SettleTargets.Contains(tile) && !tile.transform.Find("BorderEnemy(Clone)") && !tile.transform.Find("BorderFriendly(Clone)") && tile.GetComponent<TileType>().type != "Water" && tile.GetComponent<TileType>().type != "Mountain") {
                SettleTargets.Add(tile);
            }
        }
        if (tile.transform.Find("BorderFriendly(Clone)") != null && !EnemyTargets.Contains(tile) && type == "hero" && isNearTown) EnemyTargets.Add(tile);
    }

    public void Debugger(){
        Debug.Log("ENEMYTARGETS: " + EnemyTargets.Count);
        Debug.Log("SETTLETARGETS: " + SettleTargets.Count);
        Debug.Log("EXPANSIONTARGETS: " + ExpansionTargets.Count);
        print("SCAN COUNT: " + numScans);
    }
    void ExploreMode(){
        Debug.Log("ExploreMode");
        currentState = AIState.Explore;
        if (ExploreTargets.Count > 0) {
            targetTile = ExploreTargets[UnityEngine.Random.Range(0, ExploreTargets.Count-1)];
            if (targetTile != null) {
                Debug.Log(targetTile + "nå");
                MoveTo(targetTile);
                currentState = AIState.Moving;
            }
            else ExploreMode();
        }
        else {
            Debug.Log("No Explore targets");
            currentState = AIState.Idle;
        }
        
    }

    void Settle(){
        GetComponent<Settler>().Settle();
    }

    public void AddSettleTargets(List<GameObject> addList){
        
        for (int i = 0; i < addList.Count; i++){
            if (!SettleTargets.Contains(addList[i])) {
                SettleTargets.Add(addList[i]);
            }
        }

    }

    public List<GameObject> GetSettleTargets(){
        return SettleTargets;
    }

    private int CalcChance(){
        List<int> chance = new List<int>();
        
        
        for (int i = 0; i < ExpansionTargets.Count; i++){
            //sjanse for expand (50% av mulige targets)
            if (i%2 == 0) chance.Add(1);
            //sjanse på explore (ca samme som expand for å gjøre AI'en minde effektiv)
            else chance.Add(2);
        } 

        for (int i = 0; i < EnemyTargets.Count; i++){
            chance.Add(3);
        }
        for (int i = 0; i < EnemyHeroTargets.Count; i++){
            chance.Add(3);
        }

        //sjanse for getUnits (en sjanse per unit i byene)
        foreach (GameObject town in mapMaker.GetTownsE()) {
            for (int i = 0; i < town.GetComponent<EnemyTown>().standingArmy.Count; i++) chance.Add(0);
        }
        print("4");

        string output = "These are the avalible numbers: ";

        foreach (int i in chance) output = output + i + " ,";

        print(output);
        
        return chance[UnityEngine.Random.Range(0, chance.Count-1)];

    }

    private void AttackMode(GameObject target, int type) {
        // tile target
        if (type == 0) {
            prepearingToFightTown = true;
            MoveTo(target);
        }

        // hero target
        else if (type == 1) {
            targetHero = target;
            prepearingToFightPlayer = true;
            MoveTo(target.transform.parent.gameObject);
        }
    }
}
