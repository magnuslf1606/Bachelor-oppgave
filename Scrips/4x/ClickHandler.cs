using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    private GameObject curTile;
    public Canvas canvas;
    private CityManagement cityManagement;
    private BattleInitializer bI;
     
    void Start() {
        cityManagement = canvas.GetComponentInChildren<CityManagement>();
        bI = GameObject.FindWithTag("GameInfo").GetComponent<BattleInitializer>();
    }
    void Update()
    {
        //curTile = GetTile();
        //if(curTile && curTile.GetComponent<TileType>().type == "Town") {

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        cityManagement.VisCityInfo();
        //        cityManagement.SetTown(curTile.GetComponent<Town>());
        //        print(curTile.GetComponent<Town>().getCityName());
        //    }

        //} else if(curTile) {
        //    bI.StartBattle(curTile.GetComponent<TileType>());
        //}
       
    }
    public GameObject GetTile() {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                return clickedObject;
            }
        }
        return null;
    }
}
