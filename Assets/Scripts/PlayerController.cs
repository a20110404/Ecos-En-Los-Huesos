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

    [Header("Dash")]
    public float dashSpeed = 50f;
    public float dashTime = 0.8f;
    private float dashCounter;
    private bool isDashing;

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

    // Interpolaci�n de escala
    private Vector3 targetScale;
    public float scaleLerpSpeed = 12f; // Velocidad de interpolaci�n

    // Control de agachado/bouncing
    private bool isCrouching = false;
    private bool isCrouchBouncing = false;
    private float crouchBounceTimer = 0f;
    private float crouchBounceDuration = 0.12f; // Duraci�n del efecto bouncing

    // Objeto para inventario
    GameObject inventario_com;
    private bool inventoryVisible = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        theSR = GetComponent<SpriteRenderer>();
        targetScale = scaleNormal;
        transform.localScale = scaleNormal;
        // inventario
        inventario_com = GameObject.FindGameObjectWithTag("inventario-com");
        inventario_com.SetActive(false);
    }

    void Update()
    {
        // Verifica si el jugador esta en el suelo
        isGrounded = Physics2D.OverlapCircle(groundCheckpoint.position, .2f, whatIsGround);

        // ------------------ DASH ------------------
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetAxisRaw("Horizontal") != 0 && !isDashing)
        {
            isDashing = true;
            dashCounter = dashTime;
            theRB.linearVelocity = new Vector2(Input.GetAxisRaw("Horizontal") * dashSpeed, theRB.linearVelocity.y);
        }

        if (isDashing)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter <= 0)
            {
                isDashing = false;
            }
        }

        // Movimiento solo si no est� haciendo dash
        if (!isDashing)
        {
            theRB.linearVelocity = new Vector2(moveSpeed * Input.GetAxisRaw("Horizontal"), theRB.linearVelocity.y);
        }

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

        if (didSquash)
        {
            // Si ya se hizo squash, no hacer squash de nuevo hasta que termine el timer
            squashTimer = 0f;
            didSquash = false;
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
            // Squash & Stretch con interpolaci�n (solo si no est� haciendo bouncing de agachado)
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

        // Interpolaci�n suave de la escala
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleLerpSpeed);

        // Actualiza el estado anterior
        wasGrounded = isGrounded;

        // cambia los valores del contexto del animator
        anim.SetFloat("moveSpeed", Mathf.Abs(theRB.linearVelocityX));
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isCrouching", isCrouching);

        // Control para el inventario
        if (Input.GetKeyUp(KeyCode.I))
        {
            inventoryVisible = !inventoryVisible;
            inventario_com.SetActive(inventoryVisible);

            if (inventoryVisible)
            {
                GameObject.FindGameObjectWithTag("general_events")
                    .GetComponent<InventoryController>()
                    .showInventory();
            }
        }
    }
}