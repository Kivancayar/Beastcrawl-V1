using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 2f;

    // Her spawn'da kullanılacak enemy değerleri
    public float enemyHealth = 20f;
    public float enemyDamage = 10f;
    public float enemySpeed = 2f;

    void Start()
    {
        // Başlangıçta düşman doğurmasını istemiyorsan burayı boş bırak.
        // Eğer ilk girişte düşman olsun istiyorsan buraya StartSpawnRoutine(); yazabilirsin.
    }

    public void StartSpawnRoutine()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);

        // Yeni düşmanı yarat
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // EnemyHealth referansını al
        EnemyHealth healthScript = newEnemy.GetComponent<EnemyHealth>();
        if (healthScript != null)
        {
            healthScript.spawner = this;
            healthScript.maxHealth = enemyHealth;
            healthScript.currentHealth = enemyHealth;
        }

        // EnemyPatrol referansını al
        EnemyPatrol patrol = newEnemy.GetComponent<EnemyPatrol>();
        if (patrol != null)
        {
            patrol.damage = enemyDamage;
            patrol.speed = enemySpeed;
        }
    }
}