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
        if (rb != null) originalGravity = rb.gravityScale;

        if (firePoint != null)
        {
            laserLine = firePoint.GetComponent<LineRenderer>();
            if (laserLine != null)
            {
                laserLine.enabled = false;
                laserLine.SetPosition(0, firePoint.position);
                laserLine.SetPosition(1, firePoint.position);
            }
        }
    }

    void Update()
    {
        if (isDashing) return;

        if (healthSlider != null) healthSlider.value = playerHealth;
        if (manaSlider != null) manaSlider.value = playerMana;

        if (!isCharging)
        {
            if (!isKnockback)
            {
                float moveInput = Input.GetAxisRaw("Horizontal");
                if (moveInput != 0)
                {
                    lastFacingDirection = moveInput;
                    rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
                    transform.localScale = new Vector3(moveInput > 0 ? 1 : -1, 1, 1);
                }
                else
                {
                    rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                }

                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                if (mousePos.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
                else transform.localScale = new Vector3(-1, 1, 1);
            }

            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
            if (Input.GetButtonDown("Jump") && isGrounded && !isKnockback)
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && playerMana >= dashManaCost && !isKnockback)
                StartCoroutine(DashAction());

            if (playerMana < 100)
            {
                playerMana += manaRegenSpeed * Time.deltaTime;
                if (playerMana > 100) playerMana = 100;
            }

            if (!isKnockback && firePoint != null)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0f;
                Vector2 lookDir = (mousePos - firePoint.position).normalized;
                float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
                if (transform.localScale.x < 0) angle += 180f;
                firePoint.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            if (firePoint != null)
            {
                float angle = Mathf.Atan2(lockedDirection.y, lockedDirection.x) * Mathf.Rad2Deg;
                if (transform.localScale.x < 0) angle += 180f;
                firePoint.rotation = Quaternion.Euler(0, 0, angle);
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && !isKnockback && !isDashing && playerMana > 5f)
        {
            isCharging = true;
            isLaserFiring = false;
            currentChargeTimer = 0f;
            if (laserLine != null) laserLine.enabled = false;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            if (mousePosition.x > transform.position.x) transform.localScale = new Vector3(1, 1, 1);
            else transform.localScale = new Vector3(-1, 1, 1);
            lockedDirection = (mousePosition - firePoint.position).normalized;
        }

        if (Input.GetKey(KeyCode.F) && isCharging)
        {
            if (!isLaserFiring)
            {
                currentChargeTimer += Time.deltaTime;
                if (currentChargeTimer >= 2f) isLaserFiring = true;
            }
            else
            {
                if (playerMana > 0)
                {
                    playerMana -= laserManaCostPerSecond * Time.deltaTime;
                    Vector2 laserEndPoint = (Vector2)firePoint.position + ((Vector2)lockedDirection * laserRange);
                    RaycastHit2D hit = Physics2D.CircleCast(firePoint.position, 1f, lockedDirection, laserRange, canavarLayerMask);
                    if (hit.collider != null)
                    {
                        laserEndPoint = hit.point;
                        EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                        if (enemy != null) enemy.TakeDamage(laserDamage * Time.deltaTime);
                    }
                    if (laserLine != null) { laserLine.enabled = true; laserLine.SetPosition(0, firePoint.position); laserLine.SetPosition(1, laserEndPoint); }
                }
                else StopLaserAndRelease();
            }
        }

        if (Input.GetKeyUp(KeyCode.F) && isCharging) StopLaserAndRelease();
    }

    private void StopLaserAndRelease()
    {
        isCharging = false;
        isLaserFiring = false;
        rb.gravityScale = originalGravity;
        if (laserLine != null) laserLine.enabled = false;
    }

    private IEnumerator DashAction()
    {
        canDash = false;
        isDashing = true;
        playerMana -= dashManaCost;
        float originalGrav = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(lastFacingDirection * dashForce, 0f);

        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGrav;
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection)
    {
        // KORUMA KALKANI: Eğer zaten geri savruluyorsak, yeni hasar alma.
        if (isKnockback) return;

        playerHealth -= amount;
        StartCoroutine(KnockbackRoutine());

        rb.linearVelocity = Vector2.zero;
        Vector2 finalForce = new Vector2(knockbackDirection.x * yanaItmeGucu, knockbackDirection.y * yukariItmeGucu);
        rb.AddForce(finalForce, ForceMode2D.Impulse);

        if (playerHealth <= 0) { playerHealth = 0; gameObject.SetActive(false); }
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnockback = true;
        yield return new WaitForSeconds(0.2f); // Hasar arası bekleme süresi
        isKnockback = false;
    }
}