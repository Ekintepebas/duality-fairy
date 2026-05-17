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

    private Rigidbody2D rb;
    private float moveInput;
    private float verticalInput;
    private bool isGrounded;
    private bool facingRight = true; 

    [Header("Görev Sistemi")]
    public int yakalananKelebekSayisi = 0;
    public int hedefKelebekSayisi = 3;

    [Header("Uçuş Sistemi (7sn Uçuş, 3sn Dinlenme)")]
    public bool isFlying = false;
    public float maxFlightTime = 7f;       
    public float flightTimer;              
    public float cooldownTime = 3f;        
    public float cooldownTimer = 0f;       
    private float originalGravity;

    // EKSİK OLAN RÜZGAR BAĞLANTILARI BURAYA EKLENDİ
    [Header("Wind (Rüzgar Ayarı)")]
    public bool isInWindZone;
    [HideInInspector]
    public Vector2 windForce;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        flightTimer = maxFlightTime;       
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isFlying)
        {
            Jump();
        }

        if (moveInput == 1 && !facingRight) Flip();
        else if (moveInput == -1 && facingRight) Flip();

        // --- UÇUŞ DÖNGÜSÜ ---
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            isFlying = false;
        }
        else 
        {
            if (Input.GetKey(KeyCode.F) && flightTimer > 0)
            {
                isFlying = true;
                flightTimer -= Time.deltaTime; 

                if (flightTimer <= 0)
                {
                    cooldownTimer = cooldownTime;
                    isFlying = false;
                    flightTimer = maxFlightTime; 
                }
            }
            else
            {
                isFlying = false;
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
            rb.gravityScale = 0; 
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, verticalInput * moveSpeed);
        }
        else
        {
            rb.gravityScale = originalGravity; 
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        // RÜZGARIN GÜCÜNÜ HAREKETE DAHİL ET
        rb.linearVelocity += windForce;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void KelebekYakala()
    {
        yakalananKelebekSayisi++;
        Debug.Log("Kelebek Yakalandı! Toplam: " + yakalananKelebekSayisi);

        if (yakalananKelebekSayisi >= hedefKelebekSayisi)
        {
            Debug.Log("GÖREV TAMAMLANDI! Parkurdaki 3 kelebeği de topladın!");
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}