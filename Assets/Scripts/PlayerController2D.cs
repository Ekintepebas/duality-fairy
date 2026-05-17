using UnityEngine;
using UnityEngine.UI;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 12f;

    [Header("Flight Mechanics")]
    public int level = 1;
    public float cooldownTime = 3f;
    public bool isFlying = false;
    public float presentFlyingTime;

    private float flyTime = 0f;
    private float cooldownTimer = 0f;
    private float normalGravity;

    [Header("Ground Check")]
    [Tooltip("Inspector'dan Zemin Katmanını seçmeyi UNUTMAYIN!")]
    public LayerMask groundLayer;
    private float coyoteTime = 0.15f;
    private float coyoteCounter;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.2f;

    [Header("Wind")]
    [HideInInspector]
    public Vector2 windForce;

    [Header("UI")]
    public Slider flightSlider;

    [Header("Görev Sistemi")]
    public int hedefKelebekSayisi = 3;
    public int yakalananKelebekSayisi = 0;

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
        normalGravity = rb.gravityScale;
    }

    void Start()
    {
        if (groundLayer.value == 0)
        {
            Debug.LogError("DİKKAT: 'Ground Layer' seçilmemiş! Inspector'dan Ground Layer'ı ayarlayın.");
        }
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        // 1. ZEMİN KONTROLÜ
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

        // 2. ZIPLAMA
        if (Input.GetKeyDown(KeyCode.Space) && coyoteCounter > 0f && !isFlying)
        {
            Jump();
        }

        // 3. YÖN DEĞİŞTİRME
        if (moveInput > 0 && !facingRight) Flip();
        else if (moveInput < 0 && facingRight) Flip();

        // 4. UÇMA KONTROLÜ
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        switch (level)
        {
            case 1: flyTime = 0f; break;
            case 2: flyTime = 5f; break;
            case 3: flyTime = 10f; break;
            default: flyTime = 9999f; break;
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
                StopFlying();
            }
        }

        // 5. ANIMATOR
        bool yururken = moveInput != 0 && isGrounded && !isFlying;

        if (animator != null)
        {
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isWalking", yururken);
        }

        UpdateFlightSlider();
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
            rb.gravityScale = normalGravity;
            rb.linearVelocity = new Vector2(
                moveInput * moveSpeed + windForce.x,
                rb.linearVelocity.y + windForce.y
            );
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        coyoteCounter = 0f;
        isGrounded = false;
    }

    void StopFlying()
    {
        isFlying = false;
        rb.gravityScale = normalGravity;
        cooldownTimer = cooldownTime;
    }

    public void KelebekYakala()
    {
        yakalananKelebekSayisi++;
        Debug.Log("Kelebek Yakalandı! Toplam: " + yakalananKelebekSayisi + " / " + hedefKelebekSayisi);

        if (yakalananKelebekSayisi >= hedefKelebekSayisi)
        {
            Debug.Log("GÖREV TAMAMLANDI! Tüm kelebekleri topladın!");
        }
    }

    void UpdateFlightSlider()
    {
        if (flightSlider != null && level >= 2)
        {
            flightSlider.maxValue = flyTime;

            if (isFlying)
            {
                flightSlider.value = flyTime - presentFlyingTime;
            }
            else if (cooldownTimer > 0f)
            {
                float cooldownProgress = 1f - (cooldownTimer / cooldownTime);
                flightSlider.value = cooldownProgress * flyTime;
            }
            else
            {
                flightSlider.value = flyTime;
            }
        }
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
        Vector3 boxCenter = col.bounds.center + Vector3.down * (col.bounds.extents.y + 0.04f);
        Gizmos.DrawWireCube(boxCenter, boxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckDistance);
    }
}