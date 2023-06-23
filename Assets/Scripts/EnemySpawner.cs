using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Vector2 waitRange = new Vector2(1, 3);
    float waitCooldown;
    [SerializeField] int maxEnemies = 5;
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>(), spawnedEnemies = new List<GameObject>();

    private void Update()
    {
        waitCooldown -= Time.deltaTime;
        if (waitCooldown > 0) return;

        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++) {
            if (spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
        }
        if (spawnedEnemies.Count >= maxEnemies) return;

        var newEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
        spawnedEnemies.Add(newEnemy);

        waitCooldown = Random.Range(waitRange.x, waitRange.y);
    }
}
