using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string displayName;
    public float bulletSpacing, reloadTime, bulletSpeed, damage, boostForce = 320;
    public int magazineSize, magsLeft = 100;
    [SerializeField] int maxMags;
    [HideInInspector] public int currentAmmo;
    [SerializeField] GameObject bulletPrefab;
    [HideInInspector] public float cooldown, remainingBoostForce;
    [SerializeField] Sound gunshotSound;
    Sound instancedSound;

    [Space()]
    [SerializeField] bool automatic;
    [SerializeField] float cameraPullDistance;

    public float GetCameraPullDistance()
    {
        return cameraPullDistance;
    }

    public bool IsAutomatic()
    {
        return automatic;
    }

    public void Init()
    {
        instancedSound = Instantiate(gunshotSound);
        magsLeft = maxMags;
    }

    public void Shoot(Vector2 firePos, Vector3 aimAngle, Transform bulletParent = null)
    {
        if (instancedSound == null || !instancedSound.instantialized) Init();
        instancedSound.Play();
        currentAmmo -= 1;
        cooldown = bulletSpacing;

        SpawnBullet(firePos, aimAngle, bulletParent);
    }

    void SpawnBullet(Vector2 firePos, Vector3 aimAngle, Transform bulletParent = null)
    {
        var newBullet = Instantiate(bulletPrefab, firePos, Quaternion.identity);
        newBullet.transform.eulerAngles = aimAngle;
        newBullet.GetComponent<Bullet>().damage = damage;
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * bulletSpeed);
        newBullet.transform.SetParent(bulletParent.transform);
    }
}
