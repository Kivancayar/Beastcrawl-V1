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
    private Rigidbody2D rb; // Fizik motoru için
    private float nextDamageTime = 0f;

    void Start()
    {
        targetPoint = pointB;
        rb = GetComponent<Rigidbody2D>(); // Objeden Rigidbody2D bileşenini al
    }

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < detectionRange)
        {
            // OYUNCUYU TAKİP ET (Sadece X ekseninde)
            float direction = (player.position.x > transform.position.x) ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
            // DEVRIYE GEZ (Sadece X ekseninde)
            float direction = (targetPoint.position.x > transform.position.x) ? 1 : -1;
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);

            if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.2f)
                targetPoint = targetPoint == pointA ? pointB : pointA;
        }
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