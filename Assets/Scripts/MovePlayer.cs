using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovePlayer : MonoBehaviour
{
    [Header("Hareket ve Zıplama")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("Lazer Ayarlari")]
    public bool isCharging = false;
    private bool isLaserFiring = false;
    private Vector3 lockedDirection;
    private float currentChargeTimer = 0f;
    private float laserManaCostPerSecond = 25f;
    private LineRenderer laserLine;

    [SerializeField] private float yanaItmeGucu = 15f;
    [SerializeField] private float yukariItmeGucu = 4f;

    [Header("Dash Ayarları")]
    public float dashForce = 20f;
    public float dashTime = 0.2f;
    public float dashCooldown = 1f;
    public float dashManaCost = 20f;
    private bool canDash = true;
    public bool isDashing;
    private bool isKnockback;
    private float lastFacingDirection = 1f;

    [Header("Can ve Mana")]
    public float playerHealth = 100f;
    public float playerMana = 100f;
    public float manaRegenSpeed = 10f;
    public Slider healthSlider;
    public Slider manaSlider;

    [Header("Lazer ve Nişan Ayarları")]
    public Transform firePoint;
    public LayerMask canavarLayerMask;
    public float laserRange = 15f;
    public float laserDamage = 20f;

    private Rigidbody2D rb;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // STABİLİTE: Hareketin akıcı olması için
        originalGravity = rb.gravityScale;

        if (firePoint != null)
        {
            laserLine = firePoint.GetComponent<LineRenderer>();
            if (laserLine != null) laserLine.enabled = false;
        }
    }

    void Update()
    {
        if (isDashing || isKnockback) return; // Knockback anında kontrolü kısıtla

        // UI Güncelleme (Performans için sadece değiştiğinde de yapılabilir)
        if (healthSlider != null) healthSlider.value = playerHealth;
        if (manaSlider != null) manaSlider.value = playerMana;

        HandleMovement();
        HandleLaser();
        RegenerateMana();
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
            if (Input.GetButtonDown("Jump") && isGrounded)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

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
            isCharging = true;
            isLaserFiring = false;
            currentChargeTimer = 0f;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lockedDirection = (mousePos - firePoint.position).normalized;
            rb.gravityScale = 0f;
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
                RaycastHit2D hit = Physics2D.CircleCast(firePoint.position, 1f, lockedDirection, laserRange, canavarLayerMask);
                Vector2 endPoint = (Vector2)firePoint.position + ((Vector2)lockedDirection * laserRange);

                if (hit.collider != null)
                {
                    endPoint = hit.point;
                    hit.collider.GetComponent<EnemyHealth>()?.TakeDamage(laserDamage * Time.deltaTime);
                }
                if (laserLine) { laserLine.enabled = true; laserLine.SetPosition(0, firePoint.position); laserLine.SetPosition(1, endPoint); }
            }
        }
        else if (Input.GetKeyUp(KeyCode.F) && isCharging) StopLaserAndRelease();
    }

    private void RegenerateMana()
    {
        if (playerMana < 100) playerMana = Mathf.Min(100, playerMana + manaRegenSpeed * Time.deltaTime);
    }

    private void StopLaserAndRelease()
    {
        isCharging = false;
        isLaserFiring = false;
        rb.gravityScale = originalGravity;
        if (laserLine) laserLine.enabled = false;
    }

    private IEnumerator DashAction()
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
        playerHealth -= amount;

        // KİLİT: Kinematic kullanmak yerine Velocity'yi sıfırlayıp kuvvet uyguluyoruz.
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(knockbackDirection.x * yanaItmeGucu, knockbackDirection.y * yukariItmeGucu), ForceMode2D.Impulse);

        StartCoroutine(KnockbackRoutine());
        if (playerHealth <= 0) gameObject.SetActive(false);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(0.2f);
        isKnockback = false;
    }
}