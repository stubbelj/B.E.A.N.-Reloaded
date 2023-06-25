using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tosser : BaseEnemy
{
    [SerializeField] Vector2 range;
    Bomber heldBomber;
    [SerializeField] Transform holdBomberPos;
    [SerializeField] float throwAngle = 45, throwSpeed, idleDist;
    [SerializeField] Animator anim;

    public bool HasBomber()
    {
        return heldBomber != null;
    }

    protected override void Update()
    {
        base.Update();

        if (heldBomber == null) {
            if (dist > idleDist) WalkTowardPlayer();
            else if (dist < idleDist) WalkAwayFromPlayer();
            else return;
        }

        if (dist > range.y) WalkTowardPlayer();
        else if (dist < range.x) WalkAwayFromPlayer();
        else {
            anim.SetBool("MOVE", false);
            ThrowBomber(); 
        }
    }

    protected override void WalkAwayFromPlayer()
    {
        anim.SetBool("MOVE", true);
        base.WalkAwayFromPlayer();
    }

    protected override void WalkTowardPlayer()
    {
        anim.SetBool("MOVE", true);
        base.WalkTowardPlayer();
    }

    protected override void Start()
    {
        base.Start();
        EnemyManager.i.AddTosser(transform);
    }
    public void PickupBomber(Bomber bomber)
    {
        anim.SetBool("CARRY", true);
        heldBomber = bomber;
        heldBomber.transform.parent = transform;
        heldBomber.transform.position = holdBomberPos.position;
        bomber.Pickup();
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        Gizmos.DrawWireSphere(transform.position, range.x);
        Gizmos.DrawWireSphere(transform.position, range.y);
        Gizmos.DrawWireSphere(transform.position, idleDist);
    }

    void ThrowBomber()
    {
        if (!LineOfSightToTarget(range.y)) return;

        anim.SetBool("CARRY", false);
        anim.SetTrigger("THROW");

        var offset = holdBomberPos.localPosition;

        Vector2 dir = target.position - (transform.position + offset);
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        heldBomber.transform.eulerAngles = new Vector3(0, 0, angle);

        Vector2 force = calcBallisticVelocityVector(heldBomber.transform.position, target.position, throwAngle);
        if (float.IsNaN(force.x) || float.IsNaN(force.y)) return;

        heldBomber.Throw();
        heldBomber.GetComponent<Rigidbody2D>().AddForce(force * throwSpeed); 
        heldBomber = null;
    }

}
