using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CamControl : MonoBehaviour
{   

    public float speed = 6;
    public float scrollspeed = 10;
    Camera cam;

    public float height = 35;
    public int width = 60;

    float moveDelay = 0;

    bool mouseMove = false;
    Vector3 mousePos = new Vector3(0,0,0);

    Vector3 desPos = new Vector3(0,0,0);
    Vector3 oldMouse = new Vector3(0,0,0);
    private Vector3 moveto;
    private int moveToSize;
    private bool shouldMove = false;

    public BoxCollider2D barrierLeft, barrierRight;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        height = GetComponent<MapMaker>().mapSize.y * 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        float hori = Input.GetAxisRaw("Horizontal");
        float vert = Input.GetAxisRaw("Vertical");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (shouldMove)
        {
            transform.position = Vector2.Lerp(transform.position, moveto, Time.deltaTime);
            transform.position += new Vector3(0,0,-9);
            transform.gameObject.GetComponent<Camera>().orthographicSize = math.lerp(cam.orthographicSize, moveToSize, Time.deltaTime);
            if (transform.position.x - moveto.x <= .3f && transform.position.x - moveto.x >= -.3f){
                if (transform.position.y - moveto.y <= .3f && transform.position.y - moveto.y >= -.3f) shouldMove = false;
            } 
            //Debug.Log(transform.position);
        }


        if (Input.mousePosition != oldMouse){
            oldMouse = Input.mousePosition;
            desPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)){
            mouseMove = true;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2)){
            mouseMove = false;
        }

        Vector3 pos = transform.position;


        if (mouseMove){
            Vector3 deltaPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - mousePos;
            pos -= deltaPos * Time.deltaTime * speed;
        }

        
        if (scroll < 0 && cam.orthographicSize < (height/2) +1 ){
            cam.orthographicSize -= scroll * scrollspeed;
            speed -= scroll * scrollspeed;

            Vector3 deltaPos = desPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos += deltaPos;
        }
        else if (scroll > 0 && cam.orthographicSize > 2){
            cam.orthographicSize -= scroll * scrollspeed;
            speed -= scroll * scrollspeed;

            Vector3 deltaPos = desPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos += deltaPos;
        }


        if (hori != 0 || vert != 0){
            oldMouse = new Vector3(-1,-1,-1);
            pos.x += hori * Time.deltaTime * speed;
            pos.y += vert * Time.deltaTime * speed;
        }

        float minY = cam.orthographicSize -1;
        float maxY = height - minY;

        if (pos.y < minY) pos.y = minY;
        if (pos.y > maxY) pos.y = maxY;

        if (transform.position != pos){
            transform.position = pos;
            if (moveDelay <= 0) MoveTiles();
        }

        if (moveDelay > 0){
            moveDelay -= Time.deltaTime;
        }

        
    }

    void MoveTiles(){
        Collider2D[] targets = new Collider2D[1000];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("Default"));

        int count = barrierLeft.OverlapCollider(filter, targets);

        for (int n = 0; n < count; n++){
            Vector3 pos = targets[n].transform.position;
            pos.x += width;
            targets[n].transform.position = pos;
        }

        targets = new Collider2D[1000];

        count = barrierRight.OverlapCollider(filter, targets);

        for (int n = 0; n < count; n++){
            Vector3 pos = targets[n].transform.position;
            pos.x -= width;
            targets[n].transform.position = pos;
        }

        moveDelay = 0.01f;
    }

    public void moveTo(Vector3 pos, int size){
        moveto = pos;
        moveToSize = size;
        shouldMove = true;
    }

}
