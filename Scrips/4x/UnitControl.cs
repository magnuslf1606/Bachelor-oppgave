using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class UnitControl : MonoBehaviour
{
    public List<Vector2> path = new List<Vector2>();
    public int currNode = 0;
    public int remainingMoves;
    public int maxMoves, viewRadius;
    public float delay = 0.25f;

    public Vector2 offset = new Vector2(0,0);

    PathGrid grid;
    public SpriteShapeController pathLine;
    public SpriteShapeController pathLineLeft;
    public SpriteShapeController pathLineRight;
    public List<Vector2> pathPrev = new List<Vector2>();
    private MapMaker mapMaker;
    public string side;
    private AudioManager AudioMan;

    // Start is called before the first frame update
    void Start()
    {
        AudioMan = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();
        grid = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PathGrid>();
        mapMaker = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MapMaker>();
        if (GetComponent<HeroClass>() != null) side = GetComponent<HeroClass>().side;
        if (GetComponent<Settler>() !=null) side = GetComponent<Settler>().side;

        if (side == "friendly") Destroy(GetComponent<aiActions>());
    }

    // Update is called once per frame
    void Update()
    {
        if (path.Count > 0 && remainingMoves > 0){
            if (delay >= 0.5f){
                RaycastHit2D hit = Physics2D.Raycast(path[currNode] + offset, path[currNode] + offset, 0, LayerMask.GetMask("Default"));
                if (hit){
                    MoveTo(hit.collider.gameObject);
                    if (side=="friendly") mapMaker.unFog(hit.collider.gameObject, viewRadius);
                    if (currNode != 1) remainingMoves -= 1;
                } 
                else {
                    delay = 0;
                    Debug.Log("No tile found");

                    if (currNode >=1 ){
                        if (path[currNode - 1].x - path[currNode].x > 0)
                            offset.x += grid.width;
                        if (path[currNode - 1].x - path[currNode].x < 0)
                            offset.x -= grid.width;
                    }

                    hit = Physics2D.Raycast(path[currNode] + offset, path[currNode] + offset, 0, LayerMask.GetMask("Default"));
                if (hit) MoveTo(hit.collider.gameObject);
                }
            }
            else{
                delay += Time.deltaTime;
            }
        }

    }

    void MoveTo(GameObject hex){
        if (side=="friendly") AudioMan.PlayFX(AudioMan.MoveSound);
        transform.position = hex.transform.position;
        transform.parent = hex.transform;

        delay = 0;

        currNode += 1;
        if (currNode >= path.Count){
            path = new List<Vector2>();
            currNode = 0;
            //delay time
            delay = .5f;
        }
    }

    public void DrawPath(){
        if (pathPrev.Count > 1)
        {
            
            pathLineLeft.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
            pathLineRight.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
            
            Spline current = null;

            Spline pathSpline = pathLine.spline;
            pathSpline.Clear();
            Vector2 pos = new Vector2(0,0);
            pathSpline.InsertPointAt(pathSpline.GetPointCount(),pos);
            pathSpline.SetHeight(0,.1f);

            Spline pathSplineLeft = pathLineLeft.spline;
            pathSplineLeft.Clear();

            Spline pathSplineRight = pathLineRight.spline;
            pathSplineRight.Clear();

            current = pathSpline;

            for (int n = 1; n < pathPrev.Count; n++){
                if (remainingMoves+1 > n){
                    Vector2 dir = pathPrev[n] - pathPrev[n-1];
                    if (pathPrev[n].x+1 == pathPrev[n-1].x - grid.width || pathPrev[n].x-1 == pathPrev[n-1].x - grid.width || pathPrev[n].x == pathPrev[n-1].x - grid.width){
                        pos += dir + new Vector2(grid.width,0);
                        //current = pathSplineLeft;
                        //pathLineLeft.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,255);
                    }
                    else if (pathPrev[n].x-1 == pathPrev[n-1].x + grid.width || pathPrev[n].x+1 == pathPrev[n-1].x + grid.width || pathPrev[n].x == pathPrev[n-1].x + grid.width){
                        pos += dir - new Vector2(grid.width,0);
                        //current = pathSplineRight;
                        //pathLineRight.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,255);
                    }
                    else {
                        pos += dir;
                        //current = pathSpline;
                    }

                    current.InsertPointAt(current.GetPointCount(),pos);
                    current.SetHeight(current.GetPointCount()-1,.1f);
                    current.SetTangentMode(current.GetPointCount()-1, ShapeTangentMode.Continuous);
                    current.SetTangentMode(current.GetPointCount()-1, ShapeTangentMode.Continuous);
                    if (current.GetPointCount()-2 > 0){
                        current.SetRightTangent(n-1, current.GetPosition(current.GetPointCount()-1)-current.GetPosition(current.GetPointCount()-2));
                        current.SetLeftTangent(current.GetPointCount()-1, current.GetPosition(current.GetPointCount()-2)-current.GetPosition(current.GetPointCount()-1));
                    } 
                }
                
            }

            pathLine.BakeCollider();
            //pathLineLeft.BakeCollider();
            //pathLineRight.BakeCollider();
            pathLine.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,255);
            if (remainingMoves == 0) pathLine.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
        }
        else{
            pathLine.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
            pathLineLeft.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
            pathLineRight.GetComponent<SpriteShapeRenderer>().color = new Color32(255,255,255,0);
        }
    }

}
