using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    private float maxHealth = 100f;
    private float currentHealth;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log("DÜŞMAN HASAR ALDI! Kalan Can: " + (int)currentHealth);

        
        StartCoroutine(FlashEffect());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("DÜŞMAN ÖLDÜ!");

        // Görsel efekt: Yavaşça şeffaflaş ve yok ol
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
        // Düşmanı hareketten kes
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // Yavaşça şeffaflaştır
        for (float i = 1f; i >= 0; i -= 0.1f)
        {
            sr.color = new Color(1, 1, 1, i);
            yield return new WaitForSeconds(0.05f);
        }

        Destroy(gameObject);
    }
}