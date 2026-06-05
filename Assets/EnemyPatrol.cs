using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 5f;
    public Transform player;
    public Transform pointA;
    public Transform pointB;

    private Transform targetPoint;
    private Rigidbody2D rb;

    [Header("Zemin Ayarları")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    void Start()
    {
        targetPoint = pointB;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // TODO: Yarın burada Raycast ile daha hassas boşluk kontrolü yapılacak.
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.5f, groundLayer);

        // Yön hesapla
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction;

        if (distanceToPlayer < detectionRange)
        {
            // TODO: Düşmanın saldırı animasyonları ve saldırı bekleme (cooldown) mantığı buraya eklenecek.
            direction = (player.position.x > transform.position.x) ? 1 : -1;
        }
        else
        {
            direction = (targetPoint.position.x > transform.position.x) ? 1 : -1;
            if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.2f)
                targetPoint = targetPoint == pointA ? pointB : pointA;
        }

        // Hareketi uygula
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
            // TODO: Havada zıplama mekaniği veya boşluk algılama mantığı buraya entegre edilecek.
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}