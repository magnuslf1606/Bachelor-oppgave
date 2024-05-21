using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{   
    public PathGrid grid;
    private bool isPossiple = false;
    private bool prev = false;
    List<PathNode> openNodes = new List<PathNode>();
    List<PathNode> closedNodes = new List<PathNode>();

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<PathGrid>();
    }

    public List<Vector2> GetPath(Vector2 startPos, Vector2 endPos, int unitType){
        List<Vector2> pathVect = new List<Vector2>();

        PathNode startNode = grid.GetNode(startPos);
        PathNode endNode = grid.GetNode(endPos);

        if (startNode == null || endNode == null){
            Debug.Log("Invalid pos");
            pathVect.Add(startPos);
            return pathVect;
        }

        openNodes = new List<PathNode>{ startNode };
        closedNodes = new List<PathNode>();

        for (int x=0; x<grid.width; x++){
            for (int y=0; y<grid.height; y++){
                 PathNode pathNode = grid.GetNodeInt(x,y);
                 pathNode.gCost = 99999999;
                 pathNode.CalcFcost();
                 pathNode.cameFrom = null;
            }
        }


        startNode.gCost = 0;
        startNode.hCost = CalcDist(startNode.pos, endNode.pos);
        startNode.CalcFcost();
        
        while (openNodes.Count > 0){
            PathNode currNode = GetLowerstF();
            if (currNode == endNode){
                return CalcPath(currNode);
            }

            openNodes.Remove(currNode);
            closedNodes.Add(currNode);

            foreach (PathNode neighbour in GetNeighbours(currNode)){
                if (closedNodes.Contains(neighbour)) continue;
                if (neighbour.open > unitType){
                    closedNodes.Add(neighbour);
                    continue;
                }
                else{
                    float newGCost = currNode.gCost + 1;

                    if (newGCost < neighbour.gCost){
                        neighbour.cameFrom = currNode;
                        neighbour.gCost = newGCost;
                        neighbour.hCost = CalcDist(neighbour.pos, endNode.pos);
                        neighbour.CalcFcost();

                        if (!openNodes.Contains(neighbour)) openNodes.Add(neighbour);
                    }
                }
            }

        }

        Debug.Log("No path found");
        isPossiple = false;
        pathVect.Add(startPos);
        return pathVect;
    }

    private float CalcDist(Vector2 a, Vector2 b){
        float xDist = Mathf.Abs(a.x - b.x);
        float yDist = Mathf.Abs(a.y - b.y);

        float xAlt = Mathf.Abs(a.x + grid.width - b.x);
        if (xAlt < xDist) xDist = xAlt;

        xAlt = Mathf.Abs(a.x - grid.width - b.x);
        if (xAlt < xDist) xDist = xAlt;

        return xDist + yDist;
    }

    private PathNode GetLowerstF(){
        PathNode lowestF = openNodes[0];
        for (int n = 0; n < openNodes.Count; n++){
            if (openNodes[n].fCost < lowestF.fCost){
                lowestF = openNodes[n];
            }
        }
        return lowestF;
    }

    private List<PathNode> GetNeighbours(PathNode currNode){
        List<PathNode> neighbourList = new List<PathNode>();

        if (currNode.pos.x -1 >= 0) 
            neighbourList.Add(grid.GetNode(currNode.pos - new Vector2(1,0)));
        else {
            neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(grid.width - 1,0)));
            }

        if (currNode.pos.x +1 <= grid.width) 
            neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(1,0)));
        else  {
            neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(1-grid.width,0)));
            }
        if (Mathf.RoundToInt((currNode.pos.y + .75f) / .75f) < grid.height){
            if (currNode.pos.x -.5f >= 0) 
                neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(-.5f,0.75f)));
            else {
                neighbourList.Add(grid.GetNode(currNode.pos - new Vector2(grid.width - .5f,0.75f)));
                Debug.Log("1");
                }
            if (currNode.pos.x +.5f < grid.width) 
                neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(.5f,0.75f)));
            else {
                neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(.5f-grid.width,0.75f)));
                Debug.Log("2");
                }
        }

        if (currNode.pos.y - .75f >= 0){
            if (currNode.pos.x -.5f >= 0) 
                neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(-.5f,-0.75f)));
            else {
                neighbourList.Add(grid.GetNode(currNode.pos - new Vector2(grid.width - .5f,-0.75f)));
                Debug.Log("3");
                }
            if (currNode.pos.x +.5f < grid.width) 
                neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(.5f,-0.75f)));
            else {
                neighbourList.Add(grid.GetNode(currNode.pos + new Vector2(.5f-grid.width,-.75f)));
                Debug.Log("4");
                }
        }

        return neighbourList;
    }

    private List<Vector2> CalcPath(PathNode endNode){
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currNode = endNode;
        while (currNode.cameFrom != null){
            path.Add(currNode.cameFrom);
            currNode = currNode.cameFrom;
        }
        path.Reverse();

        List<Vector2> pathVect = new List<Vector2>();

        for (int n = 0; n < path.Count; n++) pathVect.Add(path[n].pos);

        return pathVect;
    }

    public bool checkPath(Vector2 start, Vector2 end, int unittyperrr){
        isPossiple = true;
        GetPath(start,end,unittyperrr);
        return isPossiple;
    }
}
