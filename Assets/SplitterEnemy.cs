using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterEnemy : BaseEnemy
{
    [Space()]
    [SerializeField] float range = 1;
    [SerializeField] GameObject nextSplitter;
    [SerializeField] Vector2 spawnOffset;
    [SerializeField] int spawnCount;

    [Header("attack")]
    [SerializeField] float attackResetTime;
    float attackCooldown;
    [SerializeField] float attackDamage;

    [Header("Anims")]
    [SerializeField] Animator anim;
    [SerializeField] string attackTrigger = "ATTACK";

    protected override void Start()
    {
        walkSpeed *= Random.Range(0.9f, 1.1f);
        base.Start();
    }

    protected override void Cooldowns()
    {
        base.Cooldowns();
        attackCooldown -= Time.deltaTime;
    }

    protected override void Update()
    {
        base.Update();

        if (stunTime > 0) return;

        if (dist > range) {
            WalkTowardPlayer();
            return;
        }
        else Stop();
        if (attackCooldown <= 0) Attack();
    }

    void Attack()
    {
        attackCooldown = attackResetTime;
        anim.SetTrigger(attackTrigger);
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

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        Gizmos.DrawWireSphere(transform.position, range);
    }
}
