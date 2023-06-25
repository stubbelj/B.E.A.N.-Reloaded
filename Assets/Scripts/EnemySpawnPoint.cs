using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [SerializeField]bool show;

    [Header("Conditions")]
    [SerializeField] bool spawnIfXGreater;
    [SerializeField] bool spawnIfYLess, spawnIfYGreater;

    [Header("Enemy")]
    [SerializeField] GameObject dropRockPrefab;
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] float dropForce = 60;
    Transform spawnPoint => transform.GetChild(0);
    Transform player => FindObjectOfType<PlayerController>().transform;

    private void Update()
    {
        if (spawnIfXGreater && player.position.x > transform.position.x) SpawnEnemy();
        if (spawnIfYGreater && player.position.y > transform.position.y) SpawnEnemy();
        if (spawnIfYLess && player.position.y < transform.position.y) SpawnEnemy();
    }
    private void OnDrawGizmos()
    {
        if (!show) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
    }

    void SpawnEnemy()
    {
        var newDropRock = Instantiate(dropRockPrefab, spawnPoint.position, Quaternion.identity);
        newDropRock.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -dropForce));
        newDropRock.GetComponent<DropRock>().Init(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]);
        Destroy(gameObject);
    }

}
