using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float jumpForce = 8f;
    public float detectionRange = 5f;
    public float attackRange = 0.6f;
    public float attackCooldown = 1.5f;
    public float damage = 10f;

    [Header("Referanslar")]
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public ParticleSystem jumpEffect;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        FindPlayer();
    }

    void FixedUpdate()
    {
        if (this == null || rb == null) return;

        if (player == null)
        {
            FindPlayer();
            if (player == null)
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }
        }

        // 1. Zemin Kontrolü
        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = (distanceToPlayer < detectionRange) ? (player.position.x > transform.position.x ? 1 : -1) : 0;

        // 2. TEMEL ZIPLAMA MANTIĞI
        if (isGrounded && direction != 0)
        {
            // Önünde bir engel varsa zıpla
            RaycastHit2D wallAhead = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0.2f), new Vector2(direction, 0), 0.7f, groundLayer);

            if (wallAhead.collider != null)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (jumpEffect != null) jumpEffect.Play();
            }
        }

        // 3. Hareket
        if (distanceToPlayer < attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            MovePlayer playerScript = collision.gameObject.GetComponent<MovePlayer>();
            if (playerScript != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                knockbackDir.y = 0.5f;
                playerScript.TakeDamage(damage, knockbackDir);
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void FindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
}