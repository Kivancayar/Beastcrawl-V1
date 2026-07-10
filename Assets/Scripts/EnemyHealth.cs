using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Sınır Ayarları")]
    public float leftLimit = -15f;
    public float rightLimit = 15f;

    public float maxHealth = 100f;
    public float currentHealth;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isDead = false;

    [Header("Ödül Ayarları")]
    public GameObject coinPrefab; // Coin prefabını buraya atacağız

    public EnemySpawner spawner;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;

        // Referansı her zaman otomatik bulmaya zorlayalım
        if (spawner == null)
        {
            GameObject spawnerObj = GameObject.FindGameObjectWithTag("Spawner");
            if (spawnerObj != null)
            {
                spawner = spawnerObj.GetComponent<EnemySpawner>();
            }
            else
            {
                Debug.LogWarning("Sahne 'Spawner' tag'ine sahip bir obje bulamadı!");
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

    void Update()
    {
        // Sadece Düşmanlar için sınır kontrolü:
        if (!gameObject.CompareTag("Player"))
        {
           

            if (transform.position.x < leftLimit || transform.position.x > rightLimit)
            {
                if (!isDead) Die();
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Die fonksiyonuna girildi!"); // <-- Bunu mutlaka ekle!

        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
            Debug.Log("Instantiate kodu çalıştı, coin oluşturuldu!");
        }
        else
        {
            Debug.LogError("Coin Prefab atanmamış! Inspector'ı kontrol et.");
        }

        // Spawner'a haber ver ve düşmanı güçlendir
        if (spawner != null)
        {
            spawner.enemyHealth += 10f;      // Her öldüğünde +5 HP
            spawner.enemyDamage += 10f;      // +1 Damage
            spawner.enemySpeed += 1f;     // Biraz daha hızlı

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
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        for (float i = 1f; i >= 0; i -= 0.1f)
        {
            sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, i);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(gameObject);
    }
    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
    }
}