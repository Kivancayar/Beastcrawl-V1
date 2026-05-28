using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start() { currentHealth = maxHealth; }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("DÜŞMAN HASAR ALDI! Kalan Can: " + (int)currentHealth);

        if (currentHealth <= 0)
        {
            Debug.Log("DÜŞMAN ÖLDÜ!");
            Destroy(gameObject);
        }
    }
}