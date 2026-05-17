
using System;
using UnityEngine;
using static UnityEngine.Rigidbody2D;

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

    [Header("Wind")]
    public bool isInWindZone;
    public float windStrength = 3f;

    [HideInInspector]
    public Vector2 windForce;

    [Header("Flight")]
    public bool canFly = true;

    public KeyCode flyKey = KeyCode.P;

    public float flyDuration = 5f;
    public float flyCooldown = 3f;

    public float glideMoveSpeed = 5f;
    public float glideVerticalSpeed = 4f;
    public float glideFallSpeed = -1.5f;

    private bool isFlying;

    private float flyTimer;
    private float cooldownTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        flyTimer = flyDuration;
    }

    void Update()
    {
        //Debug.Log(isGrounded);
        // Input
        moveInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        HandleFlight();
        // Ground check

        //Debug.Log(groundCheckRadius);
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    private void HandleFlight()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(flyKey)
            && canFly
            && cooldownTimer <= 0
            && flyTimer > 0)
        {
            isFlying = true;
        }
        /**
        if (Input.GetKeyUp(flyKey))
        {
            StopFlying();
        }
        */
        if (isFlying)
        {
            flyTimer -= Time.deltaTime;

            if (flyTimer <= 0)
            {
                StopFlying();
            }
        }

        if (isGrounded && !isFlying)
        {
            flyTimer = flyDuration;
        }

    }

    private void StopFlying()
    {
        void StopFlying()
        {
            isFlying = false;

            cooldownTimer = flyCooldown;
        }
    }

    void FixedUpdate()
    {
        if (isFlying)
        {
            GlideMovement();
        }
        else
        {
            Move();
        }
    }

    private void GlideMovement()
    {
        void GlideMovement()
        {
            rb.gravityScale = 0f;

            float verticalVelocity;

            if (verticalInput > 0)
            {
                verticalVelocity = glideVerticalSpeed;
            }
            else if (verticalInput < 0)
            {
                verticalVelocity = -glideVerticalSpeed;
            }
            else
            {
                verticalVelocity = glideFallSpeed;
            }

            rb.linearVelocity = new Vector2(
                moveInput * glideMoveSpeed,
                verticalVelocity
            );
        }
    }

    void Move()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        rb.linearVelocity += windForce; // * Time.fixedDeltaTime;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}