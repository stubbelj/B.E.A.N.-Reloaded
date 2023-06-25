using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantKnight : BaseEnemy
{
    [SerializeField] float range, attackDamage, attackResetTime;
    float attackCooldown;
    [SerializeField] HitBox attackHB;
    [SerializeField] Animator anim;
    [SerializeField] string AttackTrigger = "ATTACK";
    bool attacking;

    protected override void Update()
    {
        base.Update();

        if (stunTime > 0) return;

        if (dist > range && !attacking) WalkTowardPlayer();
        else Stop();

        if (attackCooldown <= 0) Attack();
    }

    public override void EndAttack()
    {
        base.EndAttack();
        attacking = false;
    }

    public override void StartAttck()
    {
        base.StartAttck();
        attackHB.StartHitting();
    }

    void Attack()
    {
        anim.SetTrigger(AttackTrigger);
        attackHB.Setup(attackDamage, null, 0);
    }

    protected override void Cooldowns()
    {
        base.Cooldowns();
        attackCooldown -= Time.deltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        Gizmos.DrawWireSphere(transform.position, range);
    }
}
