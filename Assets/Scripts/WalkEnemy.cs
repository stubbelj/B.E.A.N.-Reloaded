using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkEnemy : BaseEnemy
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

        if (dist > range && !attacking) {
            WalkTowardPlayer();
            anim.SetBool("MOVE", true);
        }
        else Stop();

        if (dist < range && attackCooldown <= 0) Attack();
    }

    protected override void Stop()
    {
        base.Stop();
        anim.SetBool("MOVE", false);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        flipToFacePlayer = true;
        attacking = false;
        attackHB.EndHitting();
    }

    public override void StartAttck()
    {
        base.StartAttck();
        attackHB.StartHitting();
    }

    void Attack()
    {
        attacking = true;
        flipToFacePlayer = false;
        attackCooldown = attackResetTime;
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
