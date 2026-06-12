using UnityEngine;
using System.Collections; // Bunu eklemeyi unutma!

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 2f;

    void Start()
    {
        // Start() içinden SpawnEnemy() komutunu SİL. 
        // Böylece oyun başlayınca otomatik doğurmaz.
    }

    public void StartSpawnRoutine()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);
        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    }
}