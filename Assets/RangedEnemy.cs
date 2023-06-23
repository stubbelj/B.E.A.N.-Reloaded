using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RangedEnemy : BaseEnemy
{
    [Space()]
    [SerializeField] float range;

    [SerializeField] float damage, bulletSpeed, bulletSpacing = 0.1f;
    float bulletCooldown;
    [SerializeField] float reloadTime;
    float reloadCooldown;
    [SerializeField] int magazineSize = 3;
    int leftInMag;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector3 bulletSpawnOffset;

    protected override void Start()
    {
        base.Start();
        leftInMag = magazineSize;
        range *= Random.Range(0.9f, 1.1f);
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

        if (bulletCooldown <= 0 && reloadCooldown <= 0) Shoot();
    }

    protected override void Cooldowns()
    {
        base.Cooldowns();
        bulletCooldown -= Time.deltaTime;
        reloadCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (leftInMag <= 0) {
            Reload();
            return;
        }
        if (!LineOfSightToTarget(range)) return;

        var offset = bulletSpawnOffset;
        if (srend.flipX) offset.x *= -1;
        var newBullet = Instantiate(bulletPrefab, transform.position + offset, Quaternion.identity);
        
        Vector2 dir = target.position - (transform.position + offset);
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        newBullet.transform.eulerAngles = new Vector3(0, 0, angle);

        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * bulletSpeed);
        newBullet.GetComponent<Bullet>().damage = damage; 
        bulletCooldown = bulletSpacing;
        leftInMag -= 1;
    }

    void Reload()
    {
        reloadCooldown = reloadTime;
        leftInMag = magazineSize;
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        Gizmos.DrawWireSphere(transform.position, range);
    }
}
