using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public Text turnText;
    private int turn = 1;
    private enum BattleStages { START, PLAYERTURN, ENEMYTURN, WON, LOST };
    private BattleStages state;
    public Button endTurnButton;
    public MapMaker mapMaker;
    public TechManager techManager;
   
    void Start()
    {
        UpdateTurnUI();
        state = BattleStages.START;
        StartCoroutine(PlayerTurn());
    }
    
    IEnumerator PlayerTurn() {
        state = BattleStages.PLAYERTURN;
        CheckIfGameOver();
        techManager.StartOfTurn();
        //print("Player Turn");
        if(turn > 1) { //Teller ned på produksjonen til AI så ikke gratis turn
            foreach(GameObject town in mapMaker.GetTownsE()) {
                town.GetComponent<EnemyTown>().EndOfTurn();
            }
        }
        
        GameObject endTurnKnapp = GameObject.FindWithTag("EndTurnButton");
        endTurnKnapp.GetComponent<Image>().color = Color.gray;
        yield return new WaitForSeconds(2f);
        endTurnKnapp.GetComponent<Image>().color = Color.white;
        endTurnButton = GameObject.FindWithTag("EndTurnButton").GetComponent<Button>();
        if (endTurnButton) {
            endTurnButton.onClick.AddListener(() => StartCoroutine(EnemyTurn()));
            endTurnButton.interactable = true;
        }
    }
    IEnumerator EnemyTurn() {
        state = BattleStages.ENEMYTURN;
        CheckIfGameOver();
        //print("Enemy Turn");
        mapMaker.UnitGiver();
        yield return new WaitForSeconds(0.1f);
        endTurnButton.onClick.RemoveAllListeners();

        foreach(GameObject unitE in mapMaker.getEnemyUnits()) {
            unitE.GetComponent<aiActions>().ActionDoneOnTurn = false;
            unitE.GetComponent<aiActions>().visited = new List<GameObject>();
            //unitE.GetComponent<aiActions>().Debugger();
            unitE.GetComponent<UnitControl>().remainingMoves = unitE.GetComponent<UnitControl>().maxMoves;
            if (unitE.GetComponent<aiActions>().type == "settler") {
                unitE.GetComponent<aiActions>().lifetime += 1;
                foreach(GameObject unitEforS in mapMaker.getEnemyUnits()) {
                    unitE.GetComponent<aiActions>().AddSettleTargets(unitEforS.GetComponent<aiActions>().GetSettleTargets());
                }
            }
            Debug.Log(unitE.name + " is in " + unitE.GetComponent<aiActions>().currentState + " state");
            
        }

        EndTurn(); //Oppdaterer UI
        
        foreach(GameObject town in mapMaker.GetTownsE()) {
            town.GetComponent<EnemyTown>().StartOfTurn(); //Finner ut hva som skal produseres i den byen på starten av AI turn
        }
        
        StartOfTurn(); //End of turn for spilleren, Oppdaterer buildtimer ect.
        techManager.EndOfTurn();
        foreach(GameObject units in mapMaker.getFriendlyUnits()) {
            units.GetComponent<UnitControl>().remainingMoves = units.GetComponent<UnitControl>().maxMoves; //Finner ut hva som skal produseres i den byen på starten av AI turn
        }
        StartCoroutine(PlayerTurn());
        
    }
    void CheckIfGameOver() {
        if(turn > 1) {
            if(mapMaker.towns.Count == 0) {
            state = BattleStages.LOST;
            } else if(mapMaker.townsE.Count == 0) {
                state = BattleStages.WON;
            }
        }
        

        if(state == BattleStages.LOST) {
            print("LOST");
            PlayerPrefs.SetString("won","false");
            GameOver();
        } else if(state == BattleStages.WON) {
            print("WON");
            PlayerPrefs.SetString("won","true");
            GameOver();
        }
    }
    void GameOver() {
        foreach(Transform t in transform.parent) {
            if(t.name != "GameOver") t.gameObject.SetActive(false);
        }
        transform.parent.transform.Find("GameOver").GetComponent<Canvas>().enabled = true;
        transform.parent.transform.Find("GameOver").GetComponent<AudioListener>().enabled = true;
        transform.parent.transform.Find("GameOver").GetComponent<Camera>().enabled = true;
    }
    void UpdateTurnUI()
    {
        if(turnText)
            turnText.text = "Turn " + turn;
    }
    public void EndTurn()
    {
        
        turn++;
        UpdateTurnUI();
    }
    public void StartOfTurn() {
        foreach(GameObject town in mapMaker.GetTowns()) {
            town.GetComponent<Town>().EndOfTurn();
        }
    }
}
