using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Settler : MonoBehaviour
{

    public GameObject CurrentTile;
    private bool avalible, isHovering;
    public KeyCode interactionKey = KeyCode.E;
    public string side;
    private AudioManager AudioMan;

    // Start is called before the first frame update
    void Start()
    {
        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentTile = null;
        if (transform.parent.Find("BorderFriendly(Clone)")){
            CurrentTile = transform.parent.Find("BorderFriendly(Clone)").GetComponent<BorderOwnedTown>().Town;
        }
        else if (transform.parent.Find("BorderEnemy(Clone)")){
            CurrentTile = transform.parent.Find("BorderEnemy(Clone)").gameObject;
        }


        if (CurrentTile != null) {
            //Debug.Log("Unavalible Tile");
            avalible = false;
        }
        else{
            avalible = true;
        }

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Units"));
        if (hit.collider != null && hit.collider.CompareTag("Hero") && side == "friendly")
        {
            if (hit.collider.gameObject == transform.gameObject) isHovering = true;
        }
        else
        {
            isHovering = false;
        }

        if (isHovering && Input.GetKeyDown(interactionKey))
        {
            Settle();
        }
    }

    public void Settle(){
        bool g = false;
        if (avalible){
            foreach (GameObject Tile in GameObject.FindWithTag("MainCamera").GetComponent<MapMaker>().Naboer(transform.parent.gameObject)){
                if (Tile.transform.Find("BorderFriendly(Clone)")) g = true;
            }
            if (!g) GameObject.FindWithTag("MainCamera").GetComponent<MapMaker>().SettleTown(transform.parent.gameObject, transform.gameObject);
        }
    }
}
