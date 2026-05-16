using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Ground Check")]
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Collider2D col; // Karakterin kendi collider'ı
    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true; 

    private Animator animator; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>(); // Collider'ı otomatik bulur
    }

   void Update()
    {
        // 1. Girdi Alma
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. KUSURSUZ ZEMİN KONTROLÜ (Daraltılmış Sensör)
        // Karakterin genişliğini 0.5 ile çarparak yarı yarıya daralttık. 
        // Böylece karakter yürürken zemindeki pürüzlere veya çizgilere takılmaz, hep "Yerde (True)" kalır.
        Vector2 boxSize = new Vector2(col.bounds.size.x * 0.5f, col.bounds.size.y);
        RaycastHit2D hit = Physics2D.BoxCast(col.bounds.center, boxSize, 0f, Vector2.down, 0.1f, groundLayer);
        isGrounded = hit.collider != null;

        // 3. Zıplama
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        // 4. Yön Dönme (Flip)
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // 5. ANIMASYONLAR
        animator.SetBool("isGrounded", isGrounded);
        
        if (moveInput != 0 && isGrounded)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }     
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
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}