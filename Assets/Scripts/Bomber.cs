using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : BaseEnemy
{
    [SerializeField] float explodeRange;
    [SerializeField] string explodeTrigger = "EXPLODE";
    [SerializeField] Animator anim;
    [SerializeField] HitBox explodeHitBox;
    [SerializeField] float explodeDamage;
    [SerializeField] Sound explodeSound;
    bool exploding;

    protected override void Start()
    {
        base.Start();
        explodeSound = Instantiate(explodeSound);
    }

    public void Suicide()
    {
        Die();
    }

    protected override void Die(bool playerKill = true)
    {
        base.Die(!exploding);
    }

    protected override void Update()
    {
        base.Update(); 
        
        if (dist > explodeRange && !exploding) {
            WalkTowardPlayer();
            return;
        }
        else Stop();

        if (!exploding) StartExplode();
    }

    void StartExplode()
    {
        exploding = true;

        anim.SetTrigger(explodeTrigger);
        explodeHitBox.Setup(explodeDamage, null, 0);
    }

    public void Explode()
    {
        explodeSound.Play();
        explodeHitBox.StartHitting();
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        Gizmos.DrawWireSphere(transform.position, explodeRange);
    }
}
