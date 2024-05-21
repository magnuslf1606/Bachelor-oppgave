using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float speed;
    private Rigidbody2D myRigid2D;
    private Vector3 scaleLeft, scaleRight;
    private Animator animator;
    public LayerMask wallLayer;
    //public GameObject sled; // Legg til en referanse til sleden
    public float collisionRadius = 0.2f;
    private CollisionScipt collisionScipt;
    private bool isOnSled;

    void Start()
    {
        collisionScipt = GameObject.FindGameObjectWithTag("CollisionScript").GetComponent<CollisionScipt>();
        myRigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = Time.deltaTime * 90;
        scaleLeft = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        scaleRight = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
       // offset = sled.transform.position - transform.position;
    }

    void Update()
    {
        MoveCharacter();
    }

    void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical);
        movement.Normalize();

        myRigid2D.velocity = movement * speed;

        if (HasWallCollision(movement))
            myRigid2D.velocity = Vector2.zero;

        if (horizontal > 0.01f)
        {
            transform.localScale = scaleLeft;
            animator.SetBool("run", true);
        }
        else if (horizontal < -0.01f)
        {
            transform.localScale = scaleRight;
            animator.SetBool("run", true);
        }
        else if (vertical > 0.01f || vertical < -0.01f)
        {
            animator.SetBool("run", true);
        }
        else
        {
            animator.SetBool("run", false);
        }

    }

    bool HasWallCollision(Vector2 movement)
    {
        Vector2 raycastDirection = movement.normalized;
        float raycastLength = 0.1f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, raycastLength, wallLayer);

        return hit.collider != null;
    }
}



