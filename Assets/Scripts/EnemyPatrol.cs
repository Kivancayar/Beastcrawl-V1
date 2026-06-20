using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    // Değişkenler...
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

        //Oyun başlar başlamaz efekti durdur ve temizle
        if (jumpEffect != null)
        {
            jumpEffect.Stop();
            jumpEffect.Clear();
        }
    }

    void FixedUpdate()
    {
        // ... (Zıplama ve hareket mantığı)
        if (this == null || rb == null) return;

        if (player == null)
        {
            FindPlayer();
            if (player == null) return;
        }

        if (groundCheck != null)
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction = (distanceToPlayer < detectionRange) ? (player.position.x > transform.position.x ? 1 : -1) : 0;

        if (isGrounded && direction != 0)
        {
            RaycastHit2D wallAhead = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, 0.2f), new Vector2(direction, 0), 0.7f, groundLayer);
            Vector2 checkPos = (Vector2)transform.position + new Vector2(direction * 0.5f, -0.5f);
            RaycastHit2D groundAhead = Physics2D.Raycast(checkPos, Vector2.down, 1f, groundLayer);

            if (wallAhead.collider != null || groundAhead.collider == null)
            {
                // Sadece zıpladığımız o an tetiklenmesi için:
                // Zaten havadaysak tekrar zıplamasın diye bir kontrol ekliyoruz
                if (isGrounded)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

                    if (jumpEffect != null)
                    {
                        jumpEffect.Stop(); // Önceki olası takılmaları temizle
                        jumpEffect.Clear(); // Efekti sıfırla
                        jumpEffect.Play(); // Tek bir patlama başlat
                    }
                }
            }
        }

        if (distanceToPlayer < attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
    }

    // FONKSİYONLAR BURADA (FixedUpdate'in dışında)

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

    private void FindPlayer() // Burası private olabilir
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
}