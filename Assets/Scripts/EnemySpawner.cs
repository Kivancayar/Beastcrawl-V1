using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 2f;

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

        // Yeni düşmanın içindeki Spawner referansını, bu spawner olarak ata
        EnemyHealth enemyHealth = newEnemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.spawner = this;
        }
    }
}