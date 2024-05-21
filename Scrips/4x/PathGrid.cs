using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    private PathNode[,] grid;
    public int width = 0;
    public int height = 0;

    LayerMask defaultMask;


    public bool drawGrid = true;
    float delay = 0;

    // Start is called before the first frame update
    void Start()
    {
        defaultMask = LayerMask.GetMask("Default");
    }

    public void DrawGrid(){
        grid = new PathNode[width,height];

        for (int x=0; x<width; x++){
            for (int y=0; y<height; y++){
                Vector2 pos = new Vector2(x,y);
                if (y % 2 == 0){
                    pos.x += 0.5f;
                }
                pos.y *= 0.75f;

                grid[x,y] = new PathNode(pos);

                int open = 0;

                RaycastHit2D hit = Physics2D.Raycast(pos,pos,0,defaultMask);
                if (hit){
                    if (hit.collider.gameObject.GetComponent<TileType>().type == "Mountain"){
                        open = 10;
                    }
                    if (hit.collider.gameObject.GetComponent<TileType>().type == "Water"){
                        open = 7;
                    }
                    if (hit.collider.gameObject.GetComponent<TileType>().type == "Forest"){
                        open += 1;
                    }
                    if (hit.collider.gameObject.GetComponent<TileType>().type == "Tundra"){
                        open += 1;
                    }
                }

                grid[x,y].open = open;
                grid[x,y].oldOpen = open;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;

        if (drawGrid && delay < 0){
            for (int x=0; x<width; x++){
                for (int y=0; y<height; y++){
                    if (grid[x,y].open == 10){
                        Debug.DrawLine(grid[x,y].pos - new Vector2(.5f,.5f), grid[x,y].pos + new Vector2(.5f,.5f), Color.red, 0.5f);
                    }
                    else if (grid[x,y].open == 7){
                        Debug.DrawLine(grid[x,y].pos - new Vector2(.5f,.5f), grid[x,y].pos + new Vector2(.5f,.5f), Color.yellow, 0.5f);
                    }
                    else if (grid[x,y].open == 0){
                        Debug.DrawLine(grid[x,y].pos - new Vector2(.5f,.5f), grid[x,y].pos + new Vector2(.5f,.5f), Color.green, 0.5f);
                    }
                    else {
                        Debug.DrawLine(grid[x,y].pos - new Vector2(.5f,.5f), grid[x,y].pos + new Vector2(.5f,.5f), Color.cyan, 0.5f);
                    }
                }
            }
        }
    }

    public PathNode GetNode(Vector2 pos){
        pos.y = Mathf.RoundToInt(pos.y / 0.75f);
        if (pos.y % 2 == 0) pos.x -= .5f;

        if (pos.x >= width){
            while (pos.x >= width) pos.x -= width;
        }

        if (pos.x < 0){
            while (pos.x < 0) pos.x += width;
        }

        return grid[(int)pos.x, (int)pos.y];
    }

    public PathNode GetNodeInt (int x, int y){
        return grid[x,y];
    }
}
