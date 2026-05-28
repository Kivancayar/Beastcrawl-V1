using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public float speed = 3f;
    public Transform pointA;
    public Transform pointB;
    private Transform targetPoint;

    void Start() { targetPoint = pointB; }

    void Update()
    {
        // Havada yürümeyi engellemek için Y eksenini sabitliyoruz
        Vector2 targetPos = new Vector2(targetPoint.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        if (Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(targetPoint.position.x, 0)) < 0.2f)
            targetPoint = targetPoint == pointA ? pointB : pointA;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovePlayer player = collision.gameObject.GetComponent<MovePlayer>();
            // 1. İtme yönünü hesapla (Player'ın konumu - Düşmanın konumu)
            Vector2 pushDir = (collision.transform.position - transform.position).normalized;

            // 2. Karakteri biraz da havaya zıplatması için yukarı güç ekle
            pushDir.y = 0.5f;

            // 3. Hasarı ve hesapladığın bu yönü Player'a gönder
            player.TakeDamage(10f, pushDir);
        }
    }
}