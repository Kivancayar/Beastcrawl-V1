using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovePlayer : MonoBehaviour
{
    [Header("Hareket ve Zıplama")]
    public float moveSpeed = 8f;
    public float acceleration = 15f;
    public float deceleration = 15f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;
    private bool isHitStopped = false;

    [Header("HİSSİYAT AYARLARI YENİ")]
    public float hitStopDuration = 0.08f;
    public float attackKnockback = 8f;

    [Header("Damage Popup")]
    public GameObject damageTextPrefab;

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
    public float maxHealth = 100f;
    public float playerHealth = 100f;
    public float playerMana = 100f;
    public float manaRegenSpeed = 10f;
    public Slider healthSlider;
    public Slider manaSlider;
    public RectTransform
        healthBarRect;

    private Rigidbody2D rb;
    private float originalGravity;
    public SpriteRenderer sr;
    public Color originalColor;
    [SerializeField] private Animator animator;

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
        originalColor = sr.color;

        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = playerHealth;
        }

        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
        }
    }

    void Update()
    {
        if (isHitStopped) return; // ESKİ HİTSTOP SİLİNDİ
        if (isDashing || isKnockback) return;

        if (healthSlider) healthSlider.value = playerHealth;
        if (manaSlider) manaSlider.value = playerMana;

        HandleMovement();
        HandleLaser();
        RegenerateMana();
        HandleM1Attack();
        if (Input.GetKeyDown(KeyCode.H))
        {
            IncreaseMaxHealth(20f);
        }
    }

    void HandleMovement()
    {
        if (!isCharging)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");

            animator.SetFloat("Speed", Mathf.Abs(moveInput));

            if (moveInput != 0) lastFacingDirection = moveInput;
            float targetSpeed = moveInput * moveSpeed;
            float currentSpeed = rb.linearVelocity.x;
            float smoothSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.2f);
            rb.linearVelocity = new Vector2(smoothSpeed, rb.linearVelocity.y);

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.localScale = new Vector3(mousePos.x > transform.position.x ? 1 : -1, 1, 1);

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            if (Input.GetButtonDown("Jump") && isGrounded) rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && playerMana >= dashManaCost)
                StartCoroutine(DashAction());
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    void HandleLaser()
    {
        if (isCharging && playerMana <= 0.1f)
        {
            StopLaser();
            return;
        }

        if (Input.GetKeyDown(KeyCode.F) && playerMana > 5f)
        {
            isCharging = true;
            isLaserFiring = false;
            currentChargeTimer = 0f;
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }

        if (isCharging && Input.GetKey(KeyCode.F))
        {
            if (!isLaserFiring)
            {
                currentChargeTimer += Time.deltaTime;
                if (currentChargeTimer >= 1f) isLaserFiring = true;
            }
            else
            {
                playerMana -= laserManaCostPerSecond * Time.deltaTime;
                playerMana = Mathf.Clamp(playerMana, 0, 100);

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f; // 2D İÇİN ŞART

                Vector2 dir = (mousePos - firePoint.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, laserRange, canavarLayerMask);

                if (laserLine) // BURASI EKSİKTİ
                {
                    laserLine.enabled = true;
                    laserLine.SetPosition(0, firePoint.position);
                    Vector3 endPos = hit.collider ? hit.point : firePoint.position + (Vector3)dir * laserRange;
                    laserLine.SetPosition(1, endPos);
                } // BURAYI KAPATTIM

                if (hit.collider)
                    hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(laserDamage * Time.deltaTime);
            }
        }
        else if (Input.GetKeyUp(KeyCode.F) && isCharging) // BURASI DA EKSİKTİ
        {
            StopLaser();
        }
    }

    void StopLaser()
    {
        isCharging = false;
        isLaserFiring = false;
        rb.gravityScale = originalGravity;
        if (laserLine) laserLine.enabled = false;
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.2f, rb.linearVelocity.y);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection)
    {
        if (isKnockback) return;
        playerHealth -= amount;
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
        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !isHitStopped)
        {
            nextAttackTime = Time.time + attackCooldown;
            Collider2D[] enemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in enemies)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    StartCoroutine(HitStopRoutine());
                    Vector2 knockDir = (enemy.transform.position - transform.position).normalized;
                    enemy.GetComponent<Rigidbody2D>()?.AddForce(knockDir * attackKnockback, ForceMode2D.Impulse);
                    enemyHealth.TakeDamage(attackDamage);

                    if (damageTextPrefab != null)
                    {
                        Vector3 spawnPos = enemy.transform.position + Vector3.up * 1.5f;
                        GameObject popup = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);
                        popup.GetComponent<DamagePopup>().SetDamage(attackDamage);
                        Destroy(popup, 1f);
                    }
                }
            }
        }
    }

    IEnumerator HitStopRoutine()
    {
        isHitStopped = true;
        yield return new WaitForSeconds(hitStopDuration);
        isHitStopped = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    public void IncreaseMaxHealth(float amount)
    {
        Debug.Log(maxHealth);
        maxHealth += amount;
        playerHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = playerHealth;
        }

        if (healthBarRect != null)
        {
            healthBarRect.SetSizeWithCurrentAnchors(
       RectTransform.Axis.Horizontal,
       healthBarRect.rect.width + 30f);
        }
    }
}