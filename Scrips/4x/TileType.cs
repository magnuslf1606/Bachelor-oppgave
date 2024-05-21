using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : MonoBehaviour
{
    public String eier;
    public String ResourceType;
    public GameObject bFriendly, bEnemy;
    public Sprite[] TerrainType;
    public String type;
    public int grow = 1;
    public int baseGrow = 1;
    public int islandSize = 1;

    public int width = 60;

    public float testvalue = 2;

    public MapMaker cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwapType(String typeTo)
    {
        type = typeTo;
        switch(typeTo){
            case "Water":
                GetComponent<SpriteRenderer>().sprite = TerrainType[0];
                break;
            case "Grass":
                GetComponent<SpriteRenderer>().sprite = TerrainType[1];
                break;
            case "Forest":
                GetComponent<SpriteRenderer>().sprite = TerrainType[2];
                break;
            case "Tundra":
                GetComponent<SpriteRenderer>().sprite = TerrainType[3];
                break;
            case "Mountain":
                GetComponent<SpriteRenderer>().sprite = TerrainType[4];
                break;            
            case "Ocean":
                GetComponent<SpriteRenderer>().sprite = TerrainType[5];
                break;
        }
    }
    public void Grow(float mountainDensity,float grassDensity,float forestDensity,float tundraDensity){
        List<Vector2> coords = new List<Vector2>
        {
            new Vector2(transform.position.x + 1, transform.position.y),
            new Vector2(transform.position.x - 1, transform.position.y),
            new Vector2(transform.position.x - .5f, transform.position.y + .75f),
            new Vector2(transform.position.x + .5f, transform.position.y + .75f),
            new Vector2(transform.position.x - .5f, transform.position.y - .75f),
            new Vector2(transform.position.x + .5f, transform.position.y - .75f)
        };

        for (int n = 0; n < islandSize; n++){
            int m = Mathf.RoundToInt(UnityEngine.Random.value * 5);

            if (coords[m].x < 0 ){
                Vector2 newCoord = new Vector2(coords[m].x + width, coords[m].y);
                coords[m] = newCoord;
            }
            else if (coords[m].x >= width){
                Vector2 newCoord = new Vector2(coords[m].x - width, coords[m].y);
                coords[m] = newCoord;
            }

            RaycastHit2D hit = Physics2D.Raycast(coords[m], coords[m], 0, LayerMask.GetMask("Default"));
            if (hit.collider != null){
                TileType newTile = hit.collider.gameObject.GetComponent<TileType>();

                if (newTile.type == "Water"){

                    float centerized = (baseGrow-grow)/(mountainDensity + grassDensity + forestDensity + tundraDensity);

                    float rollTerrain = centerized + UnityEngine.Random.value * ((mountainDensity + grassDensity + forestDensity + tundraDensity)/grow);

                    if (rollTerrain >= 0 && rollTerrain < mountainDensity){newTile.SwapType("Mountain");}
                    else if (rollTerrain >= mountainDensity && rollTerrain < mountainDensity  + tundraDensity){newTile.SwapType("Tundra");}
                    else if (rollTerrain >= mountainDensity + tundraDensity && rollTerrain < mountainDensity  + tundraDensity + forestDensity){newTile.SwapType("Forest");}
                    else if (rollTerrain >= mountainDensity + tundraDensity + forestDensity && rollTerrain < mountainDensity + grassDensity + forestDensity + tundraDensity + centerized){newTile.SwapType("Grass");}
                    else {
                        newTile.SwapType("Ocean");
                        Debug.Log("out of bounds");
                        }
                    
                    cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MapMaker>();
                    cam.GenerateResource(hit.collider.gameObject, newTile, newTile.type);

                    if (grow - 1 > 0){
                        newTile.grow = grow - 1;
                        newTile.baseGrow = baseGrow;
                        newTile.islandSize = islandSize;
                        newTile.width = width;

                        newTile.Grow(mountainDensity,grassDensity,forestDensity,tundraDensity);
                    }
                    
                }
                
            }
        }
    
    }

    public void SetBorder(string side, GameObject townOwned) {
        if(side == "friendly") {
            GameObject borderF = Instantiate(bFriendly, transform) as GameObject;
            borderF.GetComponent<BorderOwnedTown>().Town = townOwned;
        } else if(side == "enemy"){
            GameObject borderF = Instantiate(bEnemy, transform) as GameObject;
            borderF.GetComponent<BorderOwnedTown>().Town = townOwned;
        }
    }
    public void SetEier(string x, GameObject y) { 
        eier = x;
        SetBorder(x,y);
        }
    public String GetEier() { return eier;}

}
//
