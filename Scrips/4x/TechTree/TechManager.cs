using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechManager : MonoBehaviour
{
    public Button techButton, exitButton;
    public Canvas techCanvas;
    public List<GameObject> military, economic;
    private int militaryIndex = 0, economicIndex = 0;
    public Image prodImage;
    public Text turnsLeft;
    private GameObject cur;
    private string typeInProduction;
    private GameObject endOfTurn;
    private AudioManager AudioMan;
    // Start is called before the first frame update
    void Awake() {
        endOfTurn = GameObject.FindWithTag("EndTurnButton");
    }
    void Start()
    {
        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        techButton.onClick.AddListener(OnTechButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
        ChangeColorOnAvailable(military[militaryIndex]);
        ChangeColorOnAvailable(economic[economicIndex]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartOfTurn() {
        if(!cur) {
            foreach(Transform t in endOfTurn.transform.parent) {
                if(t.name == "Description") {
                    foreach(Transform c in t) {
                        if(c.name == "Status") {
                            c.GetComponent<Text>().text = "Choose research";
                        }
                    }
                }
            }
        }
    }
    public void EndOfTurn() {
        if(cur) {
            Text timerText = cur.transform.GetChild(4).GetComponent<Text>();
            string timer = timerText.text;
            if (timer.Length > 0) {
                int lastDigit = int.Parse(timer);
                //Ikke ferdig
                if (lastDigit > 1) {
                    lastDigit--;
                    timerText.text = lastDigit+"";
                    turnsLeft.text = "Turns left: " + timerText.text;
                } else { //Ferdig
                    prodImage.sprite = null;
                    Color c = Color.white;
                    c.a = 0;
                    prodImage.color = c;
                    turnsLeft.text = "Turns left: ";
                    MarkComplete(cur);
                }
            }
        }
    }

    void MarkComplete(GameObject x) {
        x.transform.GetChild(6).GetComponent<Image>().enabled = true;
        x.transform.GetChild(5).GetComponent<Image>().enabled = false;
        x.transform.GetChild(4).GetComponent<Text>().text = "";
        if(typeInProduction == "economic") {
            economicIndex++;
            if(economicIndex < economic.Count)
                ChangeColorOnAvailable(economic[economicIndex]);
        } else {
            militaryIndex++;
            if(militaryIndex < military.Count)
                ChangeColorOnAvailable(military[militaryIndex]);
        }
        cur = null;
    }
    void ChangeColorOnAvailable(GameObject x) {
        Color c = new(1, 1, 1)
        {
            a = 0.5450f
        };
        x.transform.GetChild(0).GetComponent<Image>().color = c;
    }
    public void AddToProductionEconomic(GameObject x) {
        if(economic.Count > 0 && economic[economicIndex] == x) {
            if(x.transform.GetChild(4).GetComponent<Text>().text != "") {
                cur = x;
                turnsLeft.text = "Turns left: " + x.transform.GetChild(4).GetComponent<Text>().text;
                prodImage.sprite = x.transform.GetChild(1).GetComponent<Image>().sprite;
                prodImage.color = Color.white;
                typeInProduction = "economic";
            }
        }
        foreach(Transform t in endOfTurn.transform.parent) {
                if(t.name == "Description") {
                    foreach(Transform c in t) {
                        if(c.name == "Status") {
                            c.GetComponent<Text>().text = "End turn";
                        }
                    }
                }
            }
    }
    public void AddToProductionMilitary(GameObject x) {
        if(military.Count > 0 && military[militaryIndex] == x) {
            if(x.transform.GetChild(4).GetComponent<Text>().text != "") {
                cur = x;
                turnsLeft.text = "Turns left: " + x.transform.GetChild(4).GetComponent<Text>().text;
                prodImage.sprite = x.transform.GetChild(1).GetComponent<Image>().sprite;
                prodImage.color = Color.white;
                typeInProduction = "military";
                
            }
        }
        foreach(Transform t in endOfTurn.transform.parent) {
                if(t.name == "Description") {
                    foreach(Transform c in t) {
                        if(c.name == "Status") {
                            c.GetComponent<Text>().text = "End turn";
                        }
                    }
                }
            }
    }
    void OnTechButtonClick() {
        techCanvas.GetComponent<Canvas>().enabled = true;
        AudioMan.PlayFX(AudioMan.TechSound);
    }
    void OnExitButtonClick() {
        techCanvas.GetComponent<Canvas>().enabled = false;
        AudioMan.PlayFX(AudioMan.ExitSound);
    }
}
