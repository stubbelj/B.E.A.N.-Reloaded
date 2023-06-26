using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string displayName;
    [SerializeField] float bulletSpacing, reloadTime, bulletSpeed, damage;
    public float boostForce = 320;
    float _bulletSpacing, _reloadTime, _bulletSpeed, _damage;
    [SerializeField] int magazineSize, magsLeft = 100;
    int _magazineSize;
    [SerializeField] int maxMags;
    [HideInInspector] int currentAmmo;
    [SerializeField] GameObject bulletPrefab;
    [HideInInspector] public float cooldown, remainingBoostForce;
    [SerializeField] Sound gunshotSound;
    Sound instancedSound;
    public Sprite image;

    [Space()]
    [SerializeField] bool automatic;
    [SerializeField] bool canPerfectReload = true;
    [SerializeField] float cameraPullDistance;

    [Header("Upgrade options")]
    [SerializeField] List<UpgradeOption.config> upgrades = new List<UpgradeOption.config>();
    List<UpgradeOption.config> _upgrades = new List<UpgradeOption.config>();

    public void Reload()
    {
        magsLeft -= 1;
        currentAmmo = _magazineSize;
    }

    public bool FullMag()
    {
        return currentAmmo == _magazineSize;
    }

    public int GetMagSize()
    {
        return _magazineSize;
    }

    public int GetMagsLeft()
    {
        return magsLeft;
    }

    public void AddMags(int mags)
    {
        magsLeft += mags;
        magsLeft = Mathf.Min(maxMags, magsLeft);
    }

    public void AddAmmo(int bullets)
    {
        currentAmmo += bullets;
        currentAmmo = Mathf.Min(currentAmmo, _magazineSize);
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

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
        _upgrades = new List<UpgradeOption.config>();
        foreach (var u in upgrades) {
            _upgrades.Add(new UpgradeOption.config(u));
        }

        _damage = damage;
        _bulletSpacing = bulletSpacing;
        _reloadTime = reloadTime;
        _bulletSpeed = bulletSpeed;
        _magazineSize = magazineSize;
    }

    public void Shoot(Vector2 firePos, Vector3 aimAngle, Transform bulletParent = null)
    {
        if (instancedSound == null || !instancedSound.instantialized) Init();
        instancedSound.Play();
        currentAmmo -= 1;
        cooldown = _bulletSpacing;

        SpawnBullet(firePos, aimAngle, bulletParent);
    }

    void SpawnBullet(Vector2 firePos, Vector3 aimAngle, Transform bulletParent = null)
    {
        var newBullet = Instantiate(bulletPrefab, firePos, Quaternion.identity);
        newBullet.transform.eulerAngles = aimAngle;
        newBullet.GetComponent<Bullet>().damage = _damage;
        newBullet.GetComponent<Bullet>().gunName = displayName;
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * _bulletSpeed);
        newBullet.transform.SetParent(bulletParent.transform);
    }

    public void ConfigureOption(UpgradeOption option, int i)
    {
        if (_upgrades.Count <= i) return;

        var config = _upgrades[i];
        option.upgradeTitle.text = config.title;
        option.subtitle.text = config.subtitle;
        option.cost.text = config.cost.ToString();

        option.currentStats = _upgrades[i];
    }

    public void ChoseOption(int i)
    {
        var config = _upgrades[i];
        _upgrades[i].Chose();

        switch (config.type) {
            case UpgradeOption.config.effect.DAMAGE:
                _damage += config.amount;
                break;
            case UpgradeOption.config.effect.FIRE_RATE:
                _bulletSpacing -= config.amount;
                break;
            case UpgradeOption.config.effect.CLIP_SIZE:
                _magazineSize += Mathf.RoundToInt(config.amount);
                currentAmmo = _magazineSize;
                break;
            default: break;
        }
    }
}
