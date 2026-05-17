using UnityEngine;
using UnityEngine.UI;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    public float flyTime;
    public float presentFlyingTime;
    public bool isFlying = false;
    public int level = 2;
    private float normalGravity;
    public float cooldownTime = 3f;
    private float cooldownTimer = 0f;


    [Header("UI")]
    public Slider abilityBar;

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
        normalGravity = rb.gravityScale;
        level = 2;
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
        if (moveInput == 1 & !facingRight)
        {
            Flip();
        }

        else if (moveInput == -1 & facingRight)
        {
            Flip();
        }

        //Fly
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
        if (Input.GetKeyDown(KeyCode.P) && !isFlying && cooldownTimer <= 0f && level >= 2)
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
        UpdateUIBars();
    }

    void UpdateUIBars()
    {
        if (abilityBar == null) return;

        if (level == 1)
        {
            abilityBar.value = 0f;
            return;
        }

        if (isFlying)
        {
            if (level >= 4)
            {
                abilityBar.value = 1f;
            }
            else
            {
                abilityBar.value = 1f - (presentFlyingTime / flyTime);
            }
        }
        else if (cooldownTimer > 0f)
        {
            abilityBar.value = 1f - (cooldownTimer / cooldownTime);
        }
        else
        {
            abilityBar.value = 1f;
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