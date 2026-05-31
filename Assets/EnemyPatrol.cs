using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float health = 50f;
    public float speed = 3f;
    public float damage = 10f;
    public float pushForce = 0.5f;
    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;
    private float nextDamageTime = 0f; // KİLİT: Hız sabitleyici

    void Start() { targetPoint = pointB; }

    void Update()
    {
        Vector2 targetPos = new Vector2(targetPoint.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(targetPoint.position.x, 0)) < 0.2f)
            targetPoint = targetPoint == pointA ? pointB : pointA;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0) Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time > nextDamageTime)
        {
            MovePlayer player = collision.gameObject.GetComponent<MovePlayer>();
            Vector2 pushDir = (collision.transform.position - transform.position).normalized;
            pushDir.y = pushForce;
            player.TakeDamage(damage, pushDir);
            nextDamageTime = Time.time + 1f; // 1 saniye bekleme
        }
    }
}