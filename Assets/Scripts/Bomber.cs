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
    EnemyManager eMan;

    [Header("Throwing")]
    [SerializeField] float maxTosserSearchDist = 8;
    [SerializeField] float tosserPickupDistance = 1, throwExplodeRange;
    Transform targetTosser;
    bool held, beingThrown;

    protected override void Start()
    {
        base.Start();
        explodeSound = Instantiate(explodeSound);
        eMan = EnemyManager.i;
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

        if (beingThrown && dist < throwExplodeRange) {
            StartExplode();
            return;
        }
        if (exploding || held || beingThrown) return;

        if (GoToTosser()) return;    
        if (dist > explodeRange) {
            WalkTowardPlayer();
            return;
        }
        else Stop();

        StartExplode();
    }

    public void Throw()
    {
        GetComponent<Rigidbody2D>().isKinematic = false;
        beingThrown = true;
        held = false;
    }

    public void Pickup()
    {
        held = true;
        Stop();
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    bool GoToTosser()
    {
        targetTosser = eMan.GetClosestTosser(transform.position, maxTosserSearchDist);
        if (targetTosser == null) return false;

        WalkToTosser();
        if (Vector2.Distance(transform.position, targetTosser.position) < tosserPickupDistance) {
            targetTosser.GetComponent<Tosser>().PickupBomber(this);
        }

        return true;
    }

    void WalkToTosser()
    {
        Vector2 dir = targetTosser.position - transform.position;
        var targetSpeed = new Vector2(dir.x > 0 ? walkSpeed : -walkSpeed, rb.velocity.y);
        rb.velocity = Vector2.Lerp(rb.velocity, targetSpeed, 0.25f);
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explodeRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, maxTosserSearchDist);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tosserPickupDistance);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, throwExplodeRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6) {
            beingThrown = false;
        }
    }
}
