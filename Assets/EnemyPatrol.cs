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
    public GameObject jumpDustPrefab; // Buraya toz efekti prefabını sürükle

    [Header("Hasar Ayarları")]
    public float damage = 10f;
    public float knockbackForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float lastJumpTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = (distanceToPlayer < detectionRange) ? (player.position.x > transform.position.x ? 1 : -1) : 0;

        if (distanceToPlayer < attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            if (isGrounded)
            {
                if (IsLedgeAhead(direction) && Time.time > lastJumpTime + 1f)
                {
                    // TOZ EFEKTİ: Zıplama anında toz bulutunu oluştur
                    if (jumpDustPrefab != null)
                    {
                        Instantiate(jumpDustPrefab, groundCheck.position, Quaternion.identity);
                    }

                    rb.linearVelocity = new Vector2(direction * speed * 1.5f, jumpForce);
                    lastJumpTime = Time.time;
                }
                else
                {
                    rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
                }
            }
        }
    }

    bool IsLedgeAhead(float dir)
    {
        if (dir == 0) return false;
        return !Physics2D.Raycast(transform.position + new Vector3(dir * 0.1f, 0, 0), Vector2.down, 1.5f, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovePlayer playerScript = collision.gameObject.GetComponent<MovePlayer>();
            if (playerScript != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                playerScript.TakeDamage(damage, knockbackDir);
            }
        }
    }
}