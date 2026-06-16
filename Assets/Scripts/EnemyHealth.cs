using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    private float maxHealth = 100f;
    private float currentHealth;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isDead = false;

    // Artık Inspector'dan sürüklemene gerek yok, kod kendisi bulacak.
    public EnemySpawner spawner;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        
    }

    void Update()
    {
        // Sadece Düşmanlar için sınır kontrolü:
        // Eğer bu objenin Tag'i 'Player' değilse, sınır dışı olunca sil.
        if (!gameObject.CompareTag("Player"))
        {
            if (transform.position.x < -15 || transform.position.x > 15)
            {
                if (!isDead) Die();
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Düşman ölürken Spawner'a haber ver (2 saniye sonra yeni doğur)
        if (spawner != null)
        {
            spawner.StartSpawnRoutine();
        }

        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FlashEffect()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = originalColor;
    }

    IEnumerator FadeAndDestroy()
    {
        // 1. Çarpışmayı Kapat (Ölürken oyuncuya takılmasın)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 2. Şeffaflaştır ve yok et
        for (float i = 1f; i >= 0; i -= 0.1f)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, i);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(gameObject);
    }
}