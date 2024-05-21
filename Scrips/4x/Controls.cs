using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using TMPro;

public class Controls : MonoBehaviour
{
    GameObject unit;
    UnitControl UnitCont;
    private GameObject curTile;
    //public Canvas TileInspect;
    public Canvas canvas;
    public GameObject tooltip;
    private CityManagement cityManagement;
    private BattleInitializer bI;
    public GameObject imageUI, timerUI;
    private AudioManager AudioMan;
    // Start is called before the first frame update
    void Start()
    {
        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        cityManagement = canvas.GetComponentInChildren<CityManagement>();
        bI = GameObject.FindWithTag("GameInfo").GetComponent<BattleInitializer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (unit != null){
            if (unit.GetComponent<UnitControl>().path.Count <= 1) {
                RaycastHit2D hit = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Default"));
                    if (hit) {
                        Vector2 sPos = unit.transform.position;

                        if (sPos.x >= GetComponent<PathGrid>().width || sPos.x < 0)
                            unit.GetComponent<UnitControl>().offset.x = Mathf.CeilToInt((sPos.x + 1 - GetComponent<PathGrid>().width)/GetComponent<PathGrid>().width) * GetComponent<PathGrid>().width;
                        else
                            unit.GetComponent<UnitControl>().offset.x = 0;

                        UnitCont.pathPrev = new List<Vector2>();
                        UnitCont.pathPrev = GetComponent<PathFinder>().GetPath(unit.transform.position, hit.collider.gameObject.transform.position, 3);
                        UnitCont.DrawPath();
                    }
            }
        }
        RaycastHit2D hitForHover = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Units"));
        RaycastHit2D hitForHoverTile = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Default"));
        if (hitForHover && hitForHover.collider.gameObject.GetComponent<UnitControl>().side != "enemy"){
            if (hitForHover.collider.gameObject.GetComponent<HeroClass>() != null) tooltip.GetComponent<TMP_Text>().text = "View Army [E]          Attack Current Tile [F]          Select Movement[Mouseclick]";
            if (hitForHover.collider.gameObject.GetComponent<Settler>() != null) tooltip.GetComponent<TMP_Text>().text = "Settle City [E]          Select Movement[Mouseclick]";
        }
        else if (hitForHover && hitForHover.collider.gameObject.GetComponent<UnitControl>().side == "enemy"){
            if (hitForHover.collider.gameObject.GetComponent<HeroClass>() != null) tooltip.GetComponent<TMP_Text>().text = "View Army [E]";
        }
        else if (hitForHoverTile && hitForHoverTile.collider.gameObject.transform.Find("Town(Clone)") && hitForHoverTile.collider.gameObject.transform.Find("BorderFriendly(Clone)")) {
             tooltip.GetComponent<TMP_Text>().text = "Open Townview[Mouseclick]";
        }
        else tooltip.GetComponent<TMP_Text>().text = "";

        if (Input.GetMouseButtonUp(0)){



            if (unit == null){
                AudioMan.PlayFX(AudioMan.ClickSound);
                RaycastHit2D hit = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Units"));
                if (hit && hit.collider.gameObject.GetComponent<UnitControl>().side != "enemy") {
                    unit = hit.collider.gameObject;
                    UnitCont = unit.GetComponent<UnitControl>();
                    UnitCont.DrawPath();
                }
                else {


                    //nye clickhandler hvis du ikke klikker på en unit


                    hit = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Default"));
                    if (hit) {
                        curTile = hit.collider.gameObject;
                        if(curTile && curTile.GetComponent<TileType>().type == "Town") {
                            
                            cityManagement.SetTown(curTile.GetComponent<Town>());
                            if(!curTile.GetComponent<Town>().GetCurrentProduction()) {
                                imageUI.GetComponent<SpriteRenderer>().sprite = null;
                                timerUI.GetComponent<Text>().text = "";
                            }
                            cityManagement.VisCityInfo();
                        } else if(curTile) {
                            //bI.StartBattle(curTile.GetComponent<TileType>());
                        }
                    }

                    //

                }
            }
            else{
                RaycastHit2D hit = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Units"));
                if (hit && hit.collider.gameObject.GetComponent<UnitControl>().side != "enemy"){
                    if (hit.collider.gameObject == unit) {
                        UnitCont.pathLine.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.pathLineLeft.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.pathLineRight.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        unit = null;
                        
                    }
                    else {
                        UnitCont.pathLine.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.pathLineLeft.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.pathLineRight.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);

                        
                        unit = hit.collider.gameObject;
                        UnitCont = unit.GetComponent<UnitControl>();
                        UnitCont.DrawPath();
                    } 

                } 
                else {
                   hit = Physics2D.Raycast(pos, pos, 0, LayerMask.GetMask("Default")); 
                   if (hit)
                   {
                        Vector2 sPos = unit.transform.position;

                        if (sPos.x >= GetComponent<PathGrid>().width || sPos.x < 0)
                            unit.GetComponent<UnitControl>().offset.x = Mathf.CeilToInt((sPos.x + 1 - GetComponent<PathGrid>().width)/GetComponent<PathGrid>().width) * GetComponent<PathGrid>().width;
                        else
                            unit.GetComponent<UnitControl>().offset.x = 0;

                        UnitCont.pathLine.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.pathLineLeft.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.pathLineRight.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
                        UnitCont.path = new List<Vector2>();
                        UnitCont.currNode = 0;
                        UnitCont.delay = .5f;
                        UnitCont.path = GetComponent<PathFinder>().GetPath(unit.transform.position, hit.collider.gameObject.transform.position, 3);
                        unit = null;
                   }
                }
            }
        }
    }
}
