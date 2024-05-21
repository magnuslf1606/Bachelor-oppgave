using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleInitializer : MonoBehaviour
{
    public Canvas canvas;
    public Button start, leave, autoresolve;
    public GameData GameData;
    private TileType curTile;
    public HeroClass attackingHero, defendingHero;
    public DataAccessor dataAccessor;
    public GameObject slider;
    private float fillAmount;
    public PostBattle postBattle;
    public CameraSwitch camswitch;
    private string ArmyToFight = "dagger:0,0 sword:0,0 bow:0,0 wizard:0,0 knight:0,0 catwoman:0,0 witch:0,0 orc:0,0 skeleton:0,0";
    public EnemyTown defendingEnemyTown;
    public Town defendingAllyTown;
    public List<HeroClass> additionalArmies;
    private MapMaker mapMaker;
    public bool reward;
    void Start()
    {
        mapMaker = GameObject.FindWithTag("MainCamera").GetComponent<MapMaker>();
        start.onClick.AddListener(StartBattleButton);
        leave.onClick.AddListener(Leave);
        autoresolve.onClick.AddListener(AutoResolve);
    }
    
    public void StartBattleButton() {
        //postBattle.Open();
        camswitch.swap();
    }
    string[] ArmyToArray(string x) {
        string[] temp = dataAccessor.ParseData(x);
        string[] ut = new string[temp.Length/3];
        for(int i = 0; i < temp.Length - 1; i+=3)
            ut[i/3] = $"{temp[i]};{temp[i+1]};{temp[i+2]}";
        return ut;
    }
    public void Leave() {
        canvas.enabled = false;
        
    }
    public void Open(TileType x) {
        curTile = x;
        canvas.enabled = true;
        additionalArmies = new List<HeroClass>();
        ArmyToFight = GetArmyFromNeutralTile();
        SetArmies();
        CalculateAutoResolve();
    }
    void AutoResolve() {
        if(attackingHero && defendingHero) {
            if(Won()) {
                defendingHero.army = new string[0];
                TakeArmyDamage(attackingHero);
            } else {
                attackingHero.army = new string[0];
                TakeArmyDamage(defendingHero);
            }
            Leave();
            postBattle.SetWinner(Won() ? "Victory" : "Defeat");
            postBattle.Open();
        } else {
            //For neutral tiles, hvor bare helten tar dmg
            TakeArmyDamage(attackingHero);
            Leave();
            postBattle.SetWinner(Won() ? "Victory" : "Defeat");
            postBattle.Open();
        }
    }
    public bool AutoResolveEnemy(HeroClass enemy) {
        curTile = enemy.gameObject.transform.parent.GetComponent<TileType>();
        attackingHero = enemy;

        //Attacking hero
        if(defendingHero) {
            SetArmies();
            CalculateAutoResolve();
            if(Won()) {
                defendingHero.army = new string[0];
                TakeArmyDamage(attackingHero);
            } else {
                attackingHero.army = new string[0];
                TakeArmyDamage(defendingHero);
            }
        } 
        //Attacking city
        else if(defendingAllyTown) {
            SetArmies();
            CalculateAutoResolve();
            if(Won()) {
                if(defendingHero) {
                    defendingHero.army = new string[0];
                    TakeArmyDamage(attackingHero);
                }
                defendingAllyTown.Flip();
            } else {
                if(attackingHero) {
                    attackingHero.army = new string[0];
                    TakeArmyDamage(defendingHero);
                }  
            }
        }
        //Neutral tile
        else {
            
            ArmyToFight = GetArmyFromNeutralTile();
            SetArmies();
            CalculateAutoResolve();
            TakeArmyDamage(enemy);
        }
        
        return Won();
    }
    void SetArmies() {
        string mal = "dagger:0,0 sword:0,0 bow:0,0 wizard:0,0 knight:0,0 catwoman:0,0 witch:0,0 orc:0,0 skeleton:0,0";
        string[] start = dataAccessor.ParseData(mal);     
        string ut = "";
        print("ARMY A: ");   
        foreach(string s in attackingHero.army) print(s);
        
        print("ARMY D: ");   
        foreach(string s in ArmyToArray(ArmyToFight)) print(s);
        //Setter army til start stringen
        for(int i = 0; i < start.Length; i++) {
            //For attacking
            for(int j = 0; j < attackingHero.army.Length; j++) {
                string[] temp = attackingHero.army[j].Split(";");
                if(temp[0].ToLower() == start[i]) 
                    start[i+1] = int.Parse(start[i+1]) + int.Parse(temp[1]) + ""; 
                
            } 
            int minLimit = 10;
            if(!defendingHero && !defendingEnemyTown) {
                //For defender, (AutoResolve only)
                for(int j = 0; j < ArmyToArray(ArmyToFight).Length; j++) {
                    string[] temp = ArmyToArray(ArmyToFight)[j].Split(";");
                    if(temp[0].ToLower() == start[i]) 
                        start[i+2] = Math.Max(int.Parse(start[i+2]) + int.Parse(temp[1]), minLimit) + ""; 
                    
                } 
            } if(defendingHero) {
                //For defender, (Faktisk hær)
                for(int j = 0; j < defendingHero.army.Length; j++) {
                    string[] temp = defendingHero.army[j].Split(";");
                    if(temp[0].ToLower() == start[i]) 
                        start[i+2] = int.Parse(start[i+2]) + int.Parse(temp[1]) + ""; 
                
                } 
            }
            
        }

        if(defendingEnemyTown) {            
            //Adde arme'er fra forsvarer i sitt territoriet til kampen (for friendly)
            foreach(GameObject t in defendingEnemyTown.GetTerritory()) {
                if(t.transform.Find("MainChar(Clone)")) {
                    if(t.transform.Find("MainChar(Clone)").GetComponent<HeroClass>().side != attackingHero.side) {
                        additionalArmies.Add(t.transform.Find("MainChar(Clone)").GetComponent<HeroClass>());
                    }
                }
            }
        }
        else if (defendingAllyTown) {
            //Adde arme'er fra forsvarer i sitt territoriet til kampen (for enemy)
            foreach(GameObject t in defendingAllyTown.GetTerritory()) {
                if(t.transform.Find("MainChar(Clone)")) {
                    if(t.transform.Find("MainChar(Clone)").GetComponent<HeroClass>().side != attackingHero.side) {
                        additionalArmies.Add(t.transform.Find("MainChar(Clone)").GetComponent<HeroClass>());
                    }
                }
            }
        }
        if(defendingEnemyTown || defendingAllyTown) {
            print("Extra armies, Before allies : " + additionalArmies.Count);
            //Hvis det er noen angripende snille hærer rundt main attacker -> blir addet
            foreach(GameObject t in mapMaker.Naboer(attackingHero.transform.parent.gameObject)) {
                if(t.transform.Find("MainChar(Clone)") && t.transform.Find("BorderEnemy(Clone)")) {
                    if(!t.transform.Find("MainChar(Clone)").Equals(attackingHero) && t.transform.Find("MainChar(Clone)").GetComponent<HeroClass>().side == attackingHero.side) {
                        additionalArmies.Add(t.transform.Find("MainChar(Clone)").GetComponent<HeroClass>());
                    }
                }
            }
            print("Extra armies: " + additionalArmies.Count);
        }

        //Hvis det er ekstra armies blir det lagt til å battle utifra hvilken side der tilhører
        for (int i = 0; i < additionalArmies.Count; i++) {
            string[] a = additionalArmies[i].army;
            for(int j = 0; j < a.Length; j++) {
                for(int k = 0; k < start.Length; k++) {
                    if(a[j].Split(";")[0].ToLower() == start[k]) {
                        if(additionalArmies[i].side == "friendly") 
                            start[k+1] = int.Parse(start[k+1]) + int.Parse(a[j].Split(";")[1]) + ""; 
                        else 
                            start[k+2] = int.Parse(start[k+2]) + int.Parse(a[j].Split(";")[1]) + ""; 
                    }
                }
            }
        }

        //Hvis det ikke finnes en (garrison)
        if(defendingEnemyTown) {
            for(int i = 0; i < start.Length; i++) {
                if(start[i] == "bow")
                    start[i+2] = int.Parse(start[i+2]) + (defendingEnemyTown.GetTerritory().Count * 10) + "";
            }      
        }
        //Hvis det ikke finnes en (garrison)
        if(defendingAllyTown) {
            for(int i = 0; i < start.Length; i++) {
                if(start[i] == "bow")
                    start[i+1] = int.Parse(start[i+1]) + (defendingEnemyTown.GetTerritory().Count * 10) + "";
            }      
        }

        //Gjør om til standar-string fra army format
        for(int i = 0; i < start.Length; i++) {
            if(i % 3 == 0) 
                ut += $"{start[i]}:";
            else if(i % 3 == 1) 
                ut += $"{start[i]},";
            else if(i % 3 == 2) {
                if(i == start.Length - 1) 
                    ut += $"{start[i]}";
                else
                    ut += $"{start[i]} ";               
            }
        }
        //Resetter for neste battle
        GameData.setData($"biom:{char.ToLower(curTile.type[0])}{curTile.type.Substring(1)} enemyHero:even {ut}");  
        print(GameData.GetData());
    }
    public void Reset() {
        defendingHero = null;
        attackingHero = null;
        defendingAllyTown = null;
        defendingEnemyTown = null;
        additionalArmies.Clear();
    }
    
    string GetArmyFromNeutralTile() {
        string ut = "";
        for(int i = 0; i < attackingHero.army.Length; i++) {
            for(int j = 0; j < attackingHero.army[i].Split(";").Length; j++) {
                string[] curr = attackingHero.army[i].Split(";");
                if(j % 4 == 0) 
                    ut += curr[0]+":";
                else if(j % 4 == 1) 
                    ut += Math.Round(int.Parse(curr[1]) * 0.3f)+",";
                else if(j % 4 == 2) {
                    if(i == attackingHero.army.Length - 1) 
                    ut += $"{curr[2]}";
                else
                    ut += $"{curr[2]} ";               
                } 
            }
        }
        return ut;
    }
    void CalculateAutoResolve() {
        float totalPowerAlly = 0, totalPowerEnemy = 0;
        // Calculate total power of forces
        var arr = dataAccessor.ParseData(GameData.GetData());
        for(int i = 4; i < arr.Length; i++) {
            if(i % 3 == 2) {
                totalPowerAlly += GetValueOfPrefab(ConvertName(arr[i-1], "ally")) * int.Parse(arr[i]); 
            } else if(i % 3 == 0) {
                totalPowerEnemy += GetValueOfPrefab(ConvertName(arr[i-2], "enemy")) * int.Parse(arr[i]);
            }
        }
        if(totalPowerEnemy == 0) {
            Debug.LogError("Total power of enemy is zero. Cannot calculate ratio.");
            return;
        }
        float ratio = totalPowerAlly / totalPowerEnemy;

        // Use a logarithmic function to map the ratio to a fill amount
        fillAmount = Mathf.Clamp01(ratio / (1 + ratio));
        float maxWidth = slider.transform.parent.GetComponent<RectTransform>().rect.width;
        float fillWidth = fillAmount * maxWidth;
        float rightEdge = maxWidth - fillWidth;

        RectTransform rectTransform = slider.GetComponent<RectTransform>();
        // Set the position and size of the filled area
        rectTransform.offsetMax = new Vector2(-rightEdge, rectTransform.offsetMax.y);
    }
    public void TakeArmyDamage(HeroClass hero) {
        string[] army = hero.army;
        string[] temp = new string[army.Length];
        float percentLoss = (1-fillAmount) * 100;

        for (int i = 0; i < army.Length; i++) {
            string a = army[i];
            string[] tab = a.Split(";");
            temp[i] = $"{tab[0]};{Math.Round(int.Parse(tab[1]) - int.Parse(tab[1]) * percentLoss / 100)};{tab[2]};{army[i].Split(";")[3]}";
        }
        hero.army = temp;
    }
    public bool Won() { return fillAmount > .5f; }
    float GetValueOfPrefab(string name) {
        var obj = Resources.Load<GameObject>(name);
        if(obj.GetComponent<RangedAlly>()) 
            return (obj.GetComponent<RangedAlly>().attackDmg / obj.GetComponent<RangedAlly>().attackSpeed) + obj.GetComponent<RangedAlly>().hp;
        if(obj.GetComponent<MeleeAlly>()) 
            return obj.GetComponent<MeleeAlly>().attackDmg + obj.GetComponent<MeleeAlly>().health;
        if(obj.GetComponent<RangedEnemy>()) 
            return (obj.GetComponent<RangedEnemy>().attackDmg / obj.GetComponent<RangedEnemy>().attackSpeed) + obj.GetComponent<RangedEnemy>().hp;
        if(obj.GetComponent<MeleeEnemy>()) 
            return obj.GetComponent<MeleeEnemy>().attackDmg + obj.GetComponent<MeleeEnemy>().health;
        return 0f;
    }
    string ConvertName(string x, string side) {
        switch (x.ToLower()) {
            case "dagger":
                return side == "ally" ? "DaggermanAlly" : "EnemyDagger";
            case "sword":
                return side == "ally" ? "SwordsmanAlly" : "SwordsmanEnemy";
            case "bow":
                return side == "ally" ? "AllyBow" : "bowEnemy";
            case "wizard":
                return side == "ally" ? "Wizard" : "WizardEnemy";
            case "knight":
                return side == "ally" ? "Knight" : "EnemyKnight";
            case "catwoman":
                return side == "ally" ? "Catwoman" : "CatwomanEnemy";
            case "witch":
                return side == "ally" ? "Witch" : "WitchEnemy";
            case "orc":
                return side == "ally" ? "OrcMale" : "OrcMaleEnemy";
            case "skeleton":
                return side == "ally" ? "Skeleton" : "ESkeleton";
            default:
                return "";
        }
    }
    public TileType GetCurTile() { return curTile;}
    public void SetCurTile(TileType x) { curTile = x;}   
}
