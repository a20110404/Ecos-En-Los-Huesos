using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    // ─────────── Ajustes en Inspector ───────────
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 18f;
    [SerializeField] private bool allowDoubleJump = true;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckpoint;
    [SerializeField] private LayerMask  whatIsGround;
    [SerializeField] private float      groundCheckRadius = .2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed    = 50f;
    [SerializeField] private float dashDuration = .8f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = .3f;

    [Header("Muerte por caída")]
    [SerializeField] private float fallDeathThreshold = -10f;

    [Header("Escalas (Squash & Stretch)")]
    [SerializeField] private Vector3 scaleNormal       = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 scaleSquash       = new Vector3(1.3f, .6f, 1f);
    [SerializeField] private Vector3 scaleStretch      = new Vector3(.9f, 1.1f, 1f);
    [SerializeField] private Vector3 scaleCrouchBounce = new Vector3(1.2f, .9f, 1f);
    [SerializeField] private float   scaleLerpSpeed    = 12f;

    [Header("Timers (segundos)")]
    [SerializeField] private float squashDuration       = .04f;
    [SerializeField] private float crouchBounceDuration = .12f;

    // ─────────── Componentes cacheados ───────────
    private Rigidbody2D    rb;
    private Animator       anim;
    private SpriteRenderer sr;

    // ─────────── Estado interno ───────────
    private bool  isGrounded, wasGrounded;
    private bool  isDashing, isKnockback;
    private bool  canDoubleJump, isCrouching, isCrouchBouncing, didSquash;
    private float horizontalInput, dashTimer, squashTimer, crouchBounceTimer;
    private Vector3 targetScale;

    // ─────────── Unity events ───────────
    private void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr   = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        targetScale       = scaleNormal;
        transform.localScale = scaleNormal;
    }

    private void Update()
    {
        if (isKnockback) return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        CheckGrounded();
        HandleDash();          // incluye movimiento
        HandleJump();
        HandleCrouchBounce();
        HandleSquashStretch();

        transform.localScale = Vector3.Lerp(transform.localScale,
                                            targetScale,
                                            Time.deltaTime * scaleLerpSpeed);

        wasGrounded = isGrounded;

        UpdateAnimator();
        CheckForFallDeath();
    }

    // ─────────────────────────────────────────────
    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckpoint.position,
                                             groundCheckRadius,
                                             whatIsGround);
        if (isGrounded && allowDoubleJump) canDoubleJump = true;
    }

    private void HandleHorizontalMovement()
    {
        if (isDashing) return;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        if (horizontalInput < 0)      sr.flipX = true;
        else if (horizontalInput > 0) sr.flipX = false;
    }

    private void HandleJump()
    {
        if (!Input.GetButtonDown("Jump")) return;

        if (isGrounded)
        {
            Jump();
            StartSquash();
        }
        else if (canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }

    private void Jump() =>
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && horizontalInput != 0 && !isDashing)
        {
            isDashing   = true;
            dashTimer   = dashDuration;
            rb.linearVelocity = new Vector2(horizontalInput * dashSpeed, rb.linearVelocity.y);
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) isDashing = false;
        }

        if (!isDashing) HandleHorizontalMovement();
    }

    private void HandleCrouchBounce()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            isCrouching       = true;
            isCrouchBouncing  = true;
            crouchBounceTimer = crouchBounceDuration;
            targetScale       = scaleCrouchBounce;
        }

        if (!(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
            isCrouching = false;

        if (isCrouchBouncing)
        {
            crouchBounceTimer -= Time.deltaTime;
            if (crouchBounceTimer <= 0f)
            {
                isCrouchBouncing = false;
                targetScale      = scaleNormal;
            }
        }
    }

    private void HandleSquashStretch()
    {
        if (!wasGrounded && isGrounded) StartSquash();

        if (squashTimer > 0)
        {
            squashTimer -= Time.deltaTime;
            if (squashTimer <= 0) ChooseTargetScaleAfterSquash();
        }
        else if (!isGrounded && Mathf.Abs(rb.linearVelocity.y) > 0.1f)
        {
            targetScale = scaleStretch;
        }
        else if (isGrounded && !isCrouchBouncing)
        {
            targetScale = scaleNormal;
        }
    }

    private void StartSquash()
    {
        if (didSquash) return;
        targetScale  = scaleSquash;
        squashTimer  = squashDuration;
        didSquash    = true;
    }

    private void ChooseTargetScaleAfterSquash()
    {
        didSquash   = false;
        targetScale = (!isGrounded && Mathf.Abs(rb.linearVelocity.y) > 0.1f)
                      ? scaleStretch
                      : scaleNormal;
    }

    // ─────────── Knockback ───────────
    public void ApplyKnockback(Vector2 force) =>
        StartCoroutine(ProcessKnockback(force));

    private IEnumerator ProcessKnockback(Vector2 force)
    {
        isKnockback   = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    // ─────────── Animator & muerte ───────────
    private void UpdateAnimator()
    {
        anim.SetFloat("moveSpeed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool ("isGrounded", isGrounded);
        anim.SetBool ("isCrouching", isCrouching);
    }

    private void CheckForFallDeath()
    {
        if (transform.position.y < fallDeathThreshold)
        {
            RespawnController.Instance.StartCoroutine(
                RespawnController.Instance.RespawnPlayer(gameObject));
        }
    }
}
