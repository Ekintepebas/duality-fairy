using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.2f;

    [Header("Wind")]
    [HideInInspector]
    public Vector2 windForce;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true; 

    // yürüme animasyonu
    private Animator walking; 


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        walking = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        //Flip
        if (moveInput==1 &!facingRight)
        {
            Flip();
        }  

        else if (moveInput==-1 & facingRight)
        {
            Flip();
        } 

        //Walking
        /*
        if (moveInput != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }     
        */
    }





    void FixedUpdate()
    {
        Move();
    }

    bool IsBlockedByWall(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            direction,
            wallCheckDistance,
            groundLayer
        );

        return hit.collider != null;
    }

    void Move()
    {
        Vector2 finalWind = windForce;

        if (windForce.x < 0 && IsBlockedByWall(Vector2.left))
        {
            finalWind.x = 0;
        }
        else if (windForce.x > 0 && IsBlockedByWall(Vector2.right))
        {
            finalWind.x = 0;
        }

        rb.linearVelocity = new Vector2(
            moveInput * moveSpeed + finalWind.x,
            rb.linearVelocity.y + finalWind.y
        );
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
    }

    // sağa sola döndür
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}