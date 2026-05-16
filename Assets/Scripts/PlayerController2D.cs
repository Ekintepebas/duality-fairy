using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    private float coyoteTime = 0.15f;
    private float coyoteCounter;

    private Rigidbody2D rb;
    private Collider2D col;
    private Animator animator;

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // Zemin kontrolü
        Vector2 boxSize = new Vector2(col.bounds.size.x * 0.7f, 0.08f);
        RaycastHit2D hit = Physics2D.BoxCast(
            col.bounds.center,
            boxSize,
            0f,
            Vector2.down,
            col.bounds.extents.y + 0.08f,
            groundLayer
        );

        bool zeminTemasi = hit.collider != null;

        if (zeminTemasi)
        {
            coyoteCounter = coyoteTime;
            isGrounded = true;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
            if (coyoteCounter < 0f)
                isGrounded = false;
        }

        // Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && coyoteCounter > 0f)
            Jump();

        // Yön değiştirme
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // Animator — velocity değil moveInput kullan, flicker olmaz
        bool yururken = moveInput != 0 && isGrounded;

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWalking", yururken);
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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
        if (col == null) return;
        Gizmos.color = Color.yellow;
        Vector2 boxSize = new Vector2(col.bounds.size.x * 0.7f, 0.08f);
        Vector3 boxCenter = col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.08f);
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
