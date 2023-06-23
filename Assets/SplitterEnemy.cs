using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterEnemy : BaseEnemy
{
    [SerializeField] float range = 1;
    [SerializeField] GameObject nextSplitter;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] int spawnCount;

    protected override void Start()
    {
        walkSpeed *= Random.Range(0.9f, 1.1f);
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (stunTime > 0) return;

        if (dist > range) WalkTowardPlayer();
        else Stop();
    }

    protected override void Die()
    {
        if (nextSplitter) SpawnChild(); 
        base.Die();
    }

    void SpawnChild()
    {
        for (int i = 0; i < spawnCount; i++) {
            var offset = spawnOffset * i;
            offset.y = spawnOffset.y;
            Instantiate(nextSplitter, transform.position + (Vector3)offset, transform.rotation);
        }
    }
}
