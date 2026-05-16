using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    public float flyTime = 3;
    public float presentFlyingTime;
    public bool isFlying = false;
    public int level;
    private float normalGravity;
    public float cooldownTime = 3f;
    private float cooldownTimer = 0f;

    [Header("Ground Check")]
    public Transform groundCheck; // z4 branch'inden gelen değişkenler
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private float coyoteTime = 0.15f;
    private float coyoteCounter;
    [Header("Wall Check")]
    public float wallCheckDistance = 0.2f;

    [Header("Wind")]
    [HideInInspector]
    public Vector2 windForce;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator animator;

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    // yürüme animasyonu
    private Animator walking;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        normalGravity = rb.gravityScale;
        level = 1;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // z4 branch'indeki OverlapCircle zemin kontrolü
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // Flip (Yön değiştirme)
        if (moveInput == 1 && !facingRight)
        {
            Flip();
        }
        else if (moveInput == -1 && facingRight)
        {
            Flip();
        }

        // Fly (Uçma Mekaniği)
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (level == 1)
        {
            flyTime = 0f;
        }
        else if (level == 2)
        {
            flyTime = 5f;
        }
        else if (level == 3)
        {
            flyTime = 10f;
        }
        else
        {
            flyTime = Mathf.Infinity;
        }

        if (Input.GetKey(KeyCode.P) && !isFlying && cooldownTimer <= 0f && level >= 2)
        {
            isFlying = true;
            presentFlyingTime = 0f;
        }

        if (isFlying)
        {
            presentFlyingTime += Time.deltaTime;

            if (presentFlyingTime >= flyTime)
            {
                isFlying = false;
                rb.gravityScale = normalGravity;
                cooldownTimer = cooldownTime;
            }
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (isFlying)
        {
            float verticalInput = Input.GetAxisRaw("Vertical");

            rb.gravityScale = 0;

            rb.linearVelocity = new Vector2(
                moveInput * moveSpeed + windForce.x,
                verticalInput * moveSpeed + windForce.y
            );
        }
        else
        {
            Vector2 finalWind = windForce;

            rb.linearVelocity = new Vector2(
                moveInput * moveSpeed + finalWind.x,
                rb.linearVelocity.y + finalWind.y
            );
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteCounter = 0f;
        isGrounded = false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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
}