using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed;

    [Header("Salto")]
    private bool canDoubleJump;
    public float jumpForce;

    [Header("Componentes")]
    public Rigidbody2D theRB;

    [Header("Animator")]
    private Animator anim;
    private SpriteRenderer theSR;

    [Header("Grounded")]
    private bool isGrounded;
    public Transform groundCheckpoint;
    public LayerMask whatIsGround;

    // Escalas para squash & stretch
    private Vector3 scaleNormal = new Vector3(1f, 1f, 1f);
    private Vector3 scaleSquash = new Vector3(1.3f, 0.6f, 1f);
    private Vector3 scaleStretch = new Vector3(0.9f, 1.1f, 1f);
    private Vector3 scaleCrouchBounce = new Vector3(1.2f, 0.9f, 1f); // Escala para el efecto bouncing

    // Control de squash
    private bool didSquash = false;
    private bool wasGrounded = false;
    private float squashTimer = 0f;
    private float squashDuration = 0.04f;

    // Interpolación de escala
    private Vector3 targetScale;
    public float scaleLerpSpeed = 12f; // Velocidad de interpolación

    // Control de agachado/bouncing
    private bool isCrouching = false;
    private bool isCrouchBouncing = false;
    private float crouchBounceTimer = 0f;
    private float crouchBounceDuration = 0.12f; // Duración del efecto bouncing

    void Start()
    {
        anim = GetComponent<Animator>();
        theSR = GetComponent<SpriteRenderer>();
        targetScale = scaleNormal;
        transform.localScale = scaleNormal;
    }

    void Update()
    {
        // Movimiento
        theRB.linearVelocity = new Vector2(moveSpeed * Input.GetAxisRaw("Horizontal"), theRB.linearVelocity.y);

        // Verifica si el jugador esta en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheckpoint.position, .2f, whatIsGround);

        // Detecta aterrizaje
        if (!wasGrounded && isGrounded)
        {
            // Squash al aterrizar
            targetScale = scaleSquash;
            squashTimer = squashDuration;
        }

        if (isGrounded)
        {
            canDoubleJump = true;
            didSquash = false;
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                theRB.linearVelocity = new Vector2(theRB.linearVelocity.x, jumpForce);
                // Squash al inicio del salto
                targetScale = scaleSquash;
                squashTimer = squashDuration;
                didSquash = true;
            }
            else
            {
                if (canDoubleJump)
                {
                    theRB.linearVelocity = new Vector2(theRB.linearVelocity.x, jumpForce);
                    canDoubleJump = false;
                }
            }
        }

        if (theRB.linearVelocityX < 0)
        {
            theSR.flipX = true;
        }
        else if (theRB.linearVelocityX > 0)
        {
            theSR.flipX = false;
        }

        // ------------------- Bouncing al agacharse -------------------
        if (isGrounded && (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)))
        {
            isCrouching = true;
            isCrouchBouncing = true;
            crouchBounceTimer = crouchBounceDuration;
            targetScale = scaleCrouchBounce;
        }

        if (isCrouchBouncing)
        {
            crouchBounceTimer -= Time.deltaTime;
            if (crouchBounceTimer <= 0)
            {
                isCrouchBouncing = false;
                targetScale = scaleNormal;
            }
        }
        else
        {
            // Squash & Stretch con interpolación (solo si no está haciendo bouncing de agachado)
            if (squashTimer > 0)
            {
                squashTimer -= Time.deltaTime;
                if (squashTimer <= 0)
                {
                    if (!isGrounded && Mathf.Abs(theRB.linearVelocity.y) > 0.1f)
                    {
                        targetScale = scaleStretch;
                    }
                    else
                    {
                        targetScale = scaleNormal;
                    }
                }
            }
            else if (!isGrounded && Mathf.Abs(theRB.linearVelocity.y) > 0.1f)
            {
                targetScale = scaleStretch;
            }
            else if (isGrounded)
            {
                targetScale = scaleNormal;
            }
        }

        // Si se suelta la tecla de agacharse, deja de estar agachado
        if (!(Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
        {
            isCrouching = false;
        }
        // ----------------------------------------------------------

        // Interpolación suave de la escala
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);

        // Actualiza el estado anterior
        wasGrounded = isGrounded;

        // cambia los valores del contexto del animator
        anim.SetFloat("moveSpeed", Mathf.Abs(theRB.linearVelocityX));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);
    }
}