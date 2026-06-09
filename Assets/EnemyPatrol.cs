using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float jumpForce = 8f;
    public float detectionRange = 5f;
    public float attackRange = 0.6f;
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Efektler")]
    public GameObject jumpDustPrefab;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float lastJumpTime;
    private float checkTimer; // Performans için zamanlayıcı

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // 1. Performans Optimizasyonu: Kontrolleri her karede değil, 0.1 saniyede bir yap
        checkTimer += Time.fixedDeltaTime;
        if (checkTimer >= 0.1f)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
            checkTimer = 0;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = (distanceToPlayer < detectionRange) ? (player.position.x > transform.position.x ? 1 : -1) : 0;

        if (distanceToPlayer < attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (isGrounded)
        {
            // Zıplama mantığı
            if (IsLedgeAhead(direction) && Time.time > lastJumpTime + 1f)
            {
                if (jumpDustPrefab != null) Instantiate(jumpDustPrefab, groundCheck.position, Quaternion.identity);
                rb.linearVelocity = new Vector2(direction * speed * 1.5f, jumpForce);
                lastJumpTime = Time.time;
            }
            else
            {
                rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            }
        }
    }

    bool IsLedgeAhead(float dir)
    {
        if (dir == 0) return false;
        // Raycast biraz ağır bir işlemdir, gerekirse mesafe veya maskeyi optimize edebilirsin
        return !Physics2D.Raycast(transform.position + new Vector3(dir * 0.1f, 0, 0), Vector2.down, 1.5f, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // GetComponent yerine direkt referans kullanmak daha hızlıdır ama şimdilik bu kalsın
            var playerScript = collision.gameObject.GetComponent<MovePlayer>();
            if (playerScript != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                playerScript.TakeDamage(10f, knockbackDir);
            }
        }
    }
}