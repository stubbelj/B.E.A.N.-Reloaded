using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    [SerializeField] bool canPerfectReload = true;
    [SerializeField] float cameraPullDistance;

    [Header("Upgrade options")]
    [SerializeField] List<UpgradeOption.config> upgrades = new List<UpgradeOption.config>();

    public float GetCameraPullDistance()
    {
        return cameraPullDistance;
    }

    public bool CanPerfectReload()
    {
        return canPerfectReload;
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
        newBullet.GetComponent<Bullet>().gunName = displayName;
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * bulletSpeed);
        newBullet.transform.SetParent(bulletParent.transform);
    }

    public void ConfigureOption(UpgradeOption option, int i)
    {
        if (upgrades.Count <= i) return;

        var config = upgrades[i];
        option.upgradeTitle.text = config.title;
        option.subtitle.text = config.subtitle;
        option.cost.text = config.cost.ToString();
    }

    public void ChoseOption(int i)
    {
        upgrades[i].Chose();
    }
}
