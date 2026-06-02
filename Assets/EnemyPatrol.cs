using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float damage = 10f;
    public float pushForce = 0.5f;
    public float detectionRange = 5f;
    public Transform player;
    public Transform pointA;
    public Transform pointB;

    private Transform targetPoint;
    private Rigidbody2D rb;
    private float nextDamageTime = 0f;

    void Start()
    {
        targetPoint = pointB;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        float direction;

        if (distanceToPlayer < detectionRange)
        {
            // OYUNCUYU TAKİP ET
            direction = (player.position.x > transform.position.x) ? 1 : -1;
        }
        else
        {
            // DEVRIYE GEZ
            direction = (targetPoint.position.x > transform.position.x) ? 1 : -1;

            if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.2f)
                targetPoint = targetPoint == pointA ? pointB : pointA;
        }

        // Hareketi uygula
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

        // YÖNÜ ÇEVİR (Sprite Flip)
        if (direction > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    // Görüş alanını sahnede görmek için
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > nextDamageTime)
        {
            MovePlayer playerScript = collision.gameObject.GetComponent<MovePlayer>();
            Vector2 pushDir = (collision.transform.position - transform.position).normalized;
            pushDir.y = pushForce;
            playerScript.TakeDamage(damage, pushDir);
            nextDamageTime = Time.time + 1f;
        }
    }
}