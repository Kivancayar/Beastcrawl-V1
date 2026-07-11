using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour
{
    [Header("Hareket ve Zıplama")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("M1 Attack")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public float attackDamage = 20f;
    public LayerMask enemyLayer;

    public float attackCooldown = 0.4f;
    private float nextAttackTime = 0f;

    [Header("Dash ve Knockback")]
    public float dashForce = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float dashManaCost = 20f;
    private bool canDash = true;
    public bool isDashing;
    private bool isKnockback;
    private float lastFacingDirection = 1f;
    [SerializeField] private float yanaItmeGucu = 15f;
    [SerializeField] private float yukariItmeGucu = 4f;

    [Header("Lazer ve Yetenekler")]
    public bool isCharging = false;
    private bool isLaserFiring = false;
    private float currentChargeTimer = 0f;
    public float laserManaCostPerSecond = 25f;
    public float laserDamage = 20f;
    public float laserRange = 15f;
    public Transform firePoint;
    public LayerMask canavarLayerMask;
    private LineRenderer laserLine;

    [Header("Can ve Mana")]
    public float maxHealth = 100f; // MAX CAN
    public float playerHealth = 100f; // ŞU ANKİ CAN
    public float playerMana = 100f;
    public float manaRegenSpeed = 10f;
    public Slider healthSlider;
    public Slider manaSlider;

    private Rigidbody2D rb;
    private float originalGravity;
    public SpriteRenderer sr;
    public Color originalColor; // Karakterin normal rengini saklıyacaz

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        if (firePoint != null)
        {
            laserLine = firePoint.GetComponent<LineRenderer>();
            if (laserLine != null) laserLine.enabled = false;
        }
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color; // Başlangıç rengini kaydet
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = playerHealth;
        }
    }

    void Update()
    {
        if (isDashing || isKnockback) return;

        if (healthSlider)
        {
            healthSlider.value = playerHealth; // sadece value değişsin
        }
        if (manaSlider) manaSlider.value = playerMana;

        HandleMovement();
        HandleLaser();
        RegenerateMana();
        HandleM1Attack();
    }

    void HandleMovement()
    {
        if (!isCharging)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            if (moveInput != 0) lastFacingDirection = moveInput;
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = new Vector3(mousePos.x > transform.position.x ? 1 : -1, 1, 1);

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            if (Input.GetButtonDown("Jump") && isGrounded) rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && playerMana >= dashManaCost)
                StartCoroutine(DashAction());
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleLaser()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerMana > 5f)
        {
            isCharging = true; isLaserFiring = false; currentChargeTimer = 0f;
            rb.gravityScale = 0f; rb.linearVelocity = Vector2.zero;
        }

        if (isCharging && Input.GetKey(KeyCode.F))
        {
            if (!isLaserFiring)
            {
                currentChargeTimer += Time.deltaTime;
                if (currentChargeTimer >= 1f) isLaserFiring = true;
            }
            else if (playerMana > 0)
            {
                playerMana -= laserManaCostPerSecond * Time.deltaTime;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 dir = (mousePos - firePoint.position).normalized;
                RaycastHit2D hit = Physics2D.CircleCast(firePoint.position, 1f, dir, laserRange, canavarLayerMask);

                if (laserLine)
                {
                    laserLine.enabled = true;
                    laserLine.SetPosition(0, firePoint.position);
                    laserLine.SetPosition(1, hit.collider ? hit.point : (Vector2)firePoint.position + dir * laserRange);
                }
                if (hit.collider) hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(laserDamage * Time.deltaTime);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F) && isCharging)
        {
            isCharging = false; rb.gravityScale = originalGravity;
            if (laserLine) laserLine.enabled = false;
        }
    }

    void RegenerateMana()
    {
        if (playerMana < 100) playerMana = Mathf.Min(100, playerMana + manaRegenSpeed * Time.deltaTime);
    }

    IEnumerator DashAction()
    {
        canDash = false; isDashing = true; playerMana -= dashManaCost;
        float oldGrav = rb.gravityScale; rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(lastFacingDirection * dashForce, 0f);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = oldGrav; isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection)
    {
        if (isKnockback) return;
        playerHealth -= amount; // Burası böyle kalacak

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDirection.x * yanaItmeGucu, knockbackDirection.y * yukariItmeGucu), ForceMode2D.Impulse);
        StartCoroutine(KnockbackRoutine());
        if (playerHealth <= 0) gameObject.SetActive(false);
    }

    IEnumerator KnockbackRoutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(0.2f);
        isKnockback = false;
    }
    void HandleM1Attack()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            Debug.Log("M1 Attack!");

            Collider2D[] enemies = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayer
            );

            foreach (Collider2D enemy in enemies)
            {
                enemy.GetComponent<EnemyHealth>()?.TakeDamage(attackDamage);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}