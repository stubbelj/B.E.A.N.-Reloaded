using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    Transform player => FindObjectOfType<PlayerController>().transform;

    private void Update()
    {
        bool xGreater = player.position.x > transform.position.x || !spawnIfXGreater;
        bool yGreater = player.position.y > transform.position.y || !spawnIfYGreater;
        bool yLess = player.position.y < transform.position.y || !spawnIfYLess;

        if (xGreater && yGreater && yLess) SpawnEnemy();
    }
    private void OnDrawGizmos()
    {
        if (!show) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        Gizmos.color = Color.green;
        foreach (Transform child in transform) Gizmos.DrawWireSphere(child.position, 0.2f);
    }

    void SpawnEnemy()
    {
        foreach (Transform child in transform) {
            var newDropRock = Instantiate(dropRockPrefab, child.position, Quaternion.identity);
            newDropRock.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -dropForce));
            newDropRock.GetComponent<DropRock>().Init(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)]);
            Destroy(gameObject);
        }
    }

}
