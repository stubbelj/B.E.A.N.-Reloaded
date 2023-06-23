using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.UIElements;

public class RangedEnemy : BaseEnemy
{
    [Space()]
    [SerializeField] Vector2 range;

    [SerializeField] float damage, bulletAngle = 45, bulletSpeed, bulletSpacing = 0.1f;
    float bulletCooldown;
    [SerializeField] float reloadTime;
    float reloadCooldown;
    [SerializeField] int magazineSize = 3;
    int leftInMag;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Vector3 bulletSpawnOffset;
    [SerializeField] Color rootedColor;


    [Header("Rooting")]
    [SerializeField] float rootTime = 1;
    float rootCooldown;
    bool rooted;

    protected override void Start()
    {
        base.Start();
        rootCooldown = rootTime;
        leftInMag = magazineSize;
        range *= Random.Range(0.9f, 1.1f);
    }

    protected override void Update()
    {
        base.Update();

        srend.color = rooted ? rootedColor : Color.white;

        if (stunTime > 0) return;

        if (dist > range.y) {
            UprootAndChase();
            return;
        }
        else if (dist < range.x) {
            UprootAndLeave();
            return;
        }
        else Stop();

        if (bulletCooldown <= 0 && reloadCooldown <= 0) Shoot();
    }

    void UprootAndChase()
    {
        Uproot();
        if (rooted) return;

        WalkTowardPlayer();
    }

    void UprootAndLeave()
    {
        Uproot();
        if (rooted) return;

        WalkAwayFromPlayer();
    }

    void Uproot()
    {
        if (rooted) {
            rootCooldown -= Time.deltaTime;
            if (rootCooldown <= 0) {
                rooted = false;
                rootCooldown = rootTime;
            }
        }
    }

    protected override void Cooldowns()
    {
        base.Cooldowns();
        bulletCooldown -= Time.deltaTime;
        reloadCooldown -= Time.deltaTime;
    }

    void Shoot()
    {
        if (!rooted) {
            Root();
            return;
        }

        if (leftInMag <= 0) {
            Reload();
            return;
        }
        if (!LineOfSightToTarget(range.y)) return;

        var offset = bulletSpawnOffset;
        if (srend.flipX) offset.x *= -1;
        var newBullet = Instantiate(bulletPrefab, transform.position + offset, Quaternion.identity);
        
        Vector2 dir = target.position - (transform.position + offset);
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        newBullet.transform.eulerAngles = new Vector3(0, 0, angle);

        newBullet.GetComponent<Bullet>().damage = damage; 
        bulletCooldown = bulletSpacing;
        leftInMag -= 1;

        Vector2 force = calcBallisticVelocityVector(newBullet.transform.position, target.position, bulletAngle);
        if (float.IsNaN(force.x) || float.IsNaN(force.y)) {
            Destroy(newBullet);
        }
        else newBullet.GetComponent<Rigidbody2D>().AddForce(force * bulletSpeed);
    }

    void Root()
    {
        rootCooldown -= Time.deltaTime;
        if (rootCooldown > 0) return;

        rooted = true;
        rootCooldown = rootTime;
    }

    Vector3 calcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }

    void Reload()
    {
        reloadCooldown = reloadTime;
        leftInMag = magazineSize;
    }

    private void OnDrawGizmosSelected()
    {
        if (!debug) return;

        Gizmos.DrawWireSphere(transform.position, range.x);
        Gizmos.DrawWireSphere(transform.position, range.y);
    }
}
