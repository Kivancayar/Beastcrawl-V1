using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float speed = 2f;
    public float jumpForce = 8f;
    public float spawnJumpDelay = 1f; // Oyun başı zıplama gecikmesi

    [Header("AI Ayarları")]
    public float detectionRange = 5f;
    public float attackRange = 0.6f;
    public float attackCooldown = 1f;
    public float damage = 10f;

    [Header("Referanslar")]
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public ParticleSystem jumpEffect;
    public AudioSource jumpSound;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float nextAttackTime = 0f;
    private float gameStartTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameStartTime = Time.time;
        FindPlayer();

        if (jumpEffect == null)
        {
            Transform particleTransform = transform.Find("JumpParticle");
            if (particleTransform != null)
            {
                jumpEffect = particleTransform.GetComponent<ParticleSystem>();
            }
        }

        if (jumpEffect != null)
        {
            jumpEffect.Stop();
            jumpEffect.Clear();
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        // Zeminde mi kontrolü
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = 0f;

        if (distanceToPlayer < detectionRange)
            direction = (player.position.x > transform.position.x) ? 1f : -1f;

        // Zıplama Mantığı - SADECE engel/boşluk varsa VE spawn gecikmesi bittiyse
        if (isGrounded && direction != 0f && Time.time - gameStartTime > spawnJumpDelay)
        {
            RaycastHit2D wallAhead = Physics2D.Raycast((Vector2)transform.position + Vector2.up * 0.2f, Vector2.right * direction, 0.7f, groundLayer);
            Vector2 checkPos = (Vector2)transform.position + new Vector2(direction * 0.5f, -0.5f);
            RaycastHit2D groundAhead = Physics2D.Raycast(checkPos, Vector2.down, 1f, groundLayer);

            if (wallAhead.collider != null || groundAhead.collider == null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                if (jumpEffect != null)
                {
                    jumpEffect.Stop();
                    jumpEffect.Clear();
                    jumpEffect.Play();
                }

                if (jumpSound != null)
                {
                    jumpSound.Play();
                }
            }
        }

        // Hareket Mantığı
        if (distanceToPlayer < attackRange)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
        else if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovePlayer playerScript = collision.gameObject.GetComponent<MovePlayer>();

            if (playerScript != null && Time.time >= nextAttackTime)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerScript.TakeDamage(damage, knockbackDir);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
}