using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] bool playerFollow;
    Transform player => FindObjectOfType<PlayerController>().transform;
    float oldPlayerPos;
    [Space()]

    [SerializeField] Vector2 waitRange = new Vector2(1, 3), forceRange = new Vector2(-3, 3), xLimits;
    float waitCooldown;
    [SerializeField] List<GameObject> enemyPrefabs = new List<GameObject>(), spawnedEnemies = new List<GameObject>();
    [SerializeField] GameObject dropRock;

    private void Start()
    {
        oldPlayerPos = player.position.x;
    }

    private void Update()
    {
        if (playerFollow) {
            transform.position += Vector3.right * (player.position.x - oldPlayerPos);
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, xLimits.x, xLimits.y), transform.position.y, transform.position.z);

            oldPlayerPos = player.position.x;
        }

        waitCooldown -= Time.deltaTime;
        if (waitCooldown > 0) return;

        SpawnEnemy();
    }

    public void AddSpawnedEnemy(GameObject enemy)
    {
        spawnedEnemies.Add(enemy);
    }

    void SpawnEnemy()
    {
        for (int i = 0; i < spawnedEnemies.Count; i++) {
            if (spawnedEnemies[i] == null) spawnedEnemies.RemoveAt(i);
        }
        if (spawnedEnemies.Count >= GameManager.i.maxEnemies) return;

        var newDropRock = Instantiate(dropRock, transform.position, Quaternion.identity);
        newDropRock.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(forceRange.x, forceRange.y), 0));
        newDropRock.GetComponent<DropRock>().Init(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], this);

        waitCooldown = Random.Range(waitRange.x, waitRange.y);
    }
}
