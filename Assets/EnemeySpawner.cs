using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemy;         // Prefab musuh
    [SerializeField] private float interval = 1f;      // Jeda spawn
    [SerializeField] private int maxEnemies = 10;      // Batas maksimal musuh
    [SerializeField] private float spawnRange = 5f;    // Jarak spawn dari posisi spawner

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemy(interval, enemy));
    }

    private IEnumerator SpawnEnemy(float interval, GameObject enemyPrefab)
    {
        yield return new WaitForSeconds(interval);

        // Hapus referensi musuh yang sudah hancur
        spawnedEnemies.RemoveAll(e => e == null);

        // Jika jumlah musuh belum mencapai batas, spawn musuh baru
        if (spawnedEnemies.Count < maxEnemies)
        {
            Vector3 spawnPos = transform.position + new Vector3(
                Random.Range(-spawnRange, spawnRange),
                0,
                Random.Range(-spawnRange, spawnRange)
            );

            GameObject newEnemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            spawnedEnemies.Add(newEnemy);
        }

        // Lanjutkan coroutine
        StartCoroutine(SpawnEnemy(interval, enemyPrefab));
    }
}
