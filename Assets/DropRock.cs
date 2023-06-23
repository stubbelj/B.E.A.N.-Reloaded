using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRock : MonoBehaviour
{
    [SerializeField] GameObject enemy, crackedRock;
    [SerializeField] int groundLayer;
    [SerializeField] float stunTimeOnSpawn = 2;
    EnemySpawner spawner;

    public void Init(GameObject enemyPrefab, EnemySpawner spawner)
    {
        enemy = enemyPrefab;
        this.spawner = spawner;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != groundLayer) return;

        if (enemy) {
            enemy = Instantiate(enemy, transform.position, Quaternion.identity);
            enemy.GetComponent<BaseEnemy>().Stun(stunTimeOnSpawn);
            if (spawner) spawner.AddSpawnedEnemy(enemy);
        }
        if (crackedRock) Instantiate(crackedRock, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
