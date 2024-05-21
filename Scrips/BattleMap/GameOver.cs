using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
 
public class GameOver : MonoBehaviour
{

    public Text xp, winner;
    public Player player;
    private CameraSwitch camswitch;
    public Transform enties;
    private Transform allies, enemies;
    public GameData gameData;
    private DataAccessor dataAccessor;
    public EntitySpawner entitySpawner;
    private BattleInitializer battleInitializer;
    private PostBattle postBattle;
    void Start()
    {
        camswitch = GameObject.FindWithTag("GameInfo").GetComponent<CameraSwitch>();
        allies = enties.transform.Find("Allies").transform;
        enemies = enties.transform.Find("Enemies").transform;
        dataAccessor = GameObject.FindGameObjectWithTag("DataAccessor").GetComponent<DataAccessor>();
        battleInitializer = GameObject.FindGameObjectWithTag("GameInfo").GetComponent<BattleInitializer>();
        postBattle = GameObject.FindGameObjectWithTag("GameInfo").GetComponent<PostBattle>();
    }

    private void SetScore(float xp)
    {
        this.xp.text = xp.ToString() + " EXP";
    }
    private void SetWinState(string winner) {
        this.winner.text = winner;
    }
    public void BackTo4x() {
        string mal = "dagger:0,0 sword:0,0 bow:0,0 wizard:0,0 knight:0,0 catwoman:0,0 witch:0,0 orc:0,0 skeleton:0,0";
        string[] inn = dataAccessor.ParseData(mal);
        
        //Find allies på slagmarken
        foreach(Transform t in allies) {
            for (int i = 0; i < inn.Length; i++) {
                if (ConvertName(t.name).Equals(inn[i]) ) 
                    inn[i+1] = int.Parse(inn[i+1]) + 1 + "";
            }
        }
        //Find enemies på slagmarken
        foreach(Transform t in enemies) {
            for (int i = 0; i < inn.Length; i++) {
                if (ConvertName(t.name).Equals(inn[i]) ) 
                    inn[i+2] = int.Parse(inn[i+2]) + 1 + "";
            }
        }

        //Tropper som ikke en spawned enda

        //Allies
        var keys = new List<string>(entitySpawner.unitCounts.Keys);
        for (int i = 0; i < keys.Count; i++) {
            string unitName = keys[i];
            Tuple<int, int> counts = entitySpawner.unitCounts[unitName];
            int allyCount = counts.Item1;
            for (int j = 0; j < inn.Length; j++) {
                if(inn[j].Equals(unitName)) {
                    inn[j+1] = int.Parse(inn[j+1]) + allyCount + ""; 
                }
            }
        }

        //Enemies
        keys = new List<string>(entitySpawner.unitCounts.Keys);
        for (int i = 0; i < keys.Count; i++) {
            string unitName = keys[i];
            Tuple<int, int> counts = entitySpawner.unitCounts[unitName];
            int enemyCount = counts.Item2;
            for (int j = 0; j < inn.Length; j++) {
                if(inn[j].Equals(unitName)) {
                    inn[j+2] = int.Parse(inn[j+2]) + enemyCount + ""; 
                }
            }
        }
        
        //Lager string til HeroClass med remaning tropper
        string[] temp = new string[battleInitializer.attackingHero.army.Length];
        for (int i = 0; i < battleInitializer.attackingHero.army.Length; i++) {
            for (int j = 0; j < inn.Length; j++) {
                string[] a = battleInitializer.attackingHero.army[i].Split(";");
                if (a[0].ToLower().Equals(inn[j]))
                    temp[i] = $"{a[0]};{inn[j+1]};{a[2]};{a[3]}";
            }
        }
        //Enemy string
        string[] tempE = new string[0];
        if(battleInitializer.defendingHero) {
            tempE = new string[battleInitializer.defendingHero.army.Length];
            for (int i = 0; i < battleInitializer.defendingHero.army.Length; i++) {
                for (int j = 0; j < inn.Length; j++) {
                    string[] a = battleInitializer.defendingHero.army[i].Split(";");
                    if (a[0].ToLower().Equals(inn[j]))
                        tempE[i] = $"{a[0]};{inn[j+2]};{a[2]};{a[3]}";
                }
            }
        }
        
        
        battleInitializer.attackingHero.army = temp;
        if(battleInitializer.defendingHero)  battleInitializer.defendingHero.army = tempE;
        Time.timeScale = 1f;
        camswitch.swap();
        postBattle.SetWinner(enemies.transform.childCount > 0 ? "Defeat" : "Victory");
        postBattle.Open();
        
    }
    public void GameOverState(string winner) {
        gameObject.SetActive(true);
        SetScore(player.GetXp());
        SetWinState(winner);
        FreezeGame();
    }
    void FreezeGame() {
        Time.timeScale = 0f; // Setter tiden til 0 for å fryse spillet.
    }
    string ConvertName(string x) {
        switch (x) {
            case "DaggermanAlly(Clone)": return "dagger";
            case "EnemyDagger(Clone)": return "dagger";
            case "Swordsmanally(Clone)": return "sword";
            case "SwordsmanEnemy(Clone)": return "sword";
            case "AllyBow(Clone)": return "bow";
            case "bowEnemy(Clone)": return "bow";
            case "Wizard(Clone)": return "wizard";
            case "WizardEnemy(Clone)": return "wizard";
            case "Knight(Clone)": return "knight";
            case "EnemyKnight(Clone)": return "knight";
            case "Catwoman(Clone)": return "catwoman";
            case "CatwomanEnemy(Clone)": return "catwoman";
            case "Witch(Clone)": return "witch";
            case "WitchEnemy(Clone)": return "witch";
            case "OrcMale(Clone)": return "orc";
            case "OrcMaleEnemy(Clone)": return "orc";
            case "Skeleton(Clone)": return "skeleton";
            case "ESkeleton(Clone)": return "skeleton";
            default: return "";
        }
    }
}
