using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    public EnemyHealth enemyHealth;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (enemyHealth == null) return;

        transform.localScale = new Vector3(
            originalScale.x * enemyHealth.GetHealthPercent(),
            originalScale.y,
            originalScale.z
        );
    }
}