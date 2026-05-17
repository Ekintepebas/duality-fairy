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
    private float verticalInput; // Yukarı-Aşağı uçuş kontrolü için
    private bool isGrounded;
    private bool facingRight = true; 

    private Animator walking; 

    [Header("Kelebek Görevi & Uçma")]
    public int yakalananKelebekSayisi = 0;
    public int hedefKelebekSayisi = 3;
    public bool canFly = false;           
    public bool isFlying = false;          

    [Header("Uçuş Süreleri (8 Saniye)")]
    public float maxFlightTime = 8f;       // İstediğin gibi 8 saniye yaptık!
    public float flightTimer;              
    public float cooldownTime = 3f;        
    public float cooldownTimer = 0f;      
    private float originalGravity;         

    [Header("Kuş Avı (Aşama 2)")]
    public int yakalananKusSayisi = 0;
    public int hedefKusSayisi = 3;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        walking = GetComponent<Animator>();
        originalGravity = rb.gravityScale; 
        flightTimer = maxFlightTime;       
    }

    void Update()
    {
        // Sağ-Sol girdisi (A-D veya Sağ-Sol Ok tuşları)
        moveInput = Input.GetAxisRaw("Horizontal");
        // Yukarı-Aşağı girdisi (W-S veya Yukarı-Aşağı Ok tuşları)
        verticalInput = Input.GetAxisRaw("Vertical");

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Zıplama (Sadece uçmuyorken)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isFlying)
        {
            Jump();
        }

        if (moveInput == 1 && !facingRight) Flip();
        else if (moveInput == -1 && facingRight) Flip();

        // UÇMA DÖNGÜSÜ
        if (canFly)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            // F tuşuna basılı tutuluyorsa, uçuş süresi varsa ve cooldown bittiyse uç
            if (Input.GetKey(KeyCode.F) && flightTimer > 0 && cooldownTimer <= 0)
            {
                isFlying = true;
                flightTimer -= Time.deltaTime;

                if (flightTimer <= 0)
                {
                    cooldownTimer = cooldownTime;
                    isFlying = false;
                }
            }
            else
            {
                isFlying = false;

                // Yere inince uçuş süresi tamamen yenilenir
                if (isGrounded && cooldownTimer <= 0)
                {
                    flightTimer = maxFlightTime;
                }
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
            rb.gravityScale = 0; // Uçarken yerçekimini tamamen sıfırla
            // 8 saniyelik serbest uçuş: Hem yatay hem dikey ok tuşlarıyla kontrol
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, verticalInput * moveSpeed);
        }
        else
        {
            rb.gravityScale = originalGravity; // Uçmuyor iken yerçekimi normal
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    // Kelebeklerin tetiklediği fonksiyon
    public void KelebekYakala()
    {
        yakalananKelebekSayisi++;
        Debug.Log("Kelebek Yakalandı! Sayı: " + yakalananKelebekSayisi);

        if (yakalananKelebekSayisi >= hedefKelebekSayisi)
        {
            canFly = true; 
            Debug.Log("1. AŞAMA BİTTİ! Havada 'F' tuşuna basılı tutarak Ok Tuşlarıyla 8 saniye serbest uçabilirsin!");
        }
    }

    // Kuşların tetikleyeceği fonksiyon (Aşama 2)
    public void KusYakalandi()
    {
        yakalananKusSayisi++;
        Debug.Log("Kuş Yakalandı! Toplam Kuş: " + yakalananKusSayisi);

        if (yakalananKusSayisi >= hedefKusSayisi)
        {
            Debug.Log("2. AŞAMA BİTTİ! 3 Kuş yakalandı! Karakter artık ALEV TOPU atma gücü kazandı!");
            // İleride buraya alev topu açma kodunu ekleyeceğiz.
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }
}