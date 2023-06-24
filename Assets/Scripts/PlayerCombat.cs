using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth;
    float health;
    [HideInInspector] public bool dead { get { return health <= 0; } }

    [Space()]
    [SerializeField] KeyCode melee = KeyCode.E, altMelee = KeyCode.LeftAlt, reload = KeyCode.R;

    [Header("SMG")]
    [SerializeField] float SMGResetTime;
    [SerializeField] float SMGReloadTime, SMGBulletSpeed, SMGDamage;
    [SerializeField] int SMGMagazineSize, SMGMagsLeft = 100;
    int SMGcurAmmo;
    [SerializeField] GameObject SMGBulletPrefab;
    float SMGcooldown;

    [Header("Punch")]
    [SerializeField] HitBox punchHB;
    [SerializeField] float punchDamage, punchKBStrength, punchStunTime, punch3Damage, punch3KB, punch3StunTime;
    [SerializeField] float punchStepForce = 50, stepTime = 0.1f;

    bool aimingSniper, needToRelease, punching;
    PlayerAnimator anim => GetComponent<PlayerAnimator>();
    PlayerController pMove => GetComponent<PlayerController>();
    PlayerSound pSound => GetComponent<PlayerSound>();
    GameObject bulletParent => GameObject.Find("PlayerBulletParent");
    
    private bool isReloading = false;
    private float reloadDur = 1.3f;
    private float reloadTimer = 0f;
    [Header("Reloading")]
    [SerializeField] float perfectReloadStart;
    [SerializeField] float perfectReloadEnd;

    public void AddAmmo(int amount)
    {
        SMGMagsLeft += amount;
        pSound.AmmoPickup.Play();
    }

    public void Hit(float Damage)
    {
        if (pMove.isDashing) return;

        CameraShake.i.Shake(0.3f, 0.2f);

        health -= Damage;
        if (health <= 0) Die();
    }

    void Die()
    {
        print("Player has died");
    }

    public float GetHealthPercent()
    {
        return health / maxHealth;
    }

    public void EndPunch()
    {
        if (anim.GetPunchStep() > 3) CameraShake.i.Shake(0.2f, 0.2f);
        else CameraShake.i.Shake(0.05f, 0.1f);

        punching = false;
        punchHB.EndHitting();
    }

    private void Start()
    {
        health = maxHealth;
        StartReload();
    }

    private void Update()
    {
        DoCooldowns();

        if (Input.GetKeyDown(melee) || Input.GetKeyDown(altMelee)) Melee();

        if (Input.GetKeyDown(reload)){
            if(!isReloading){
                StartReload();
            } else {
                print("Attempting perfect reload");
                AttemptPerfectReload();
            }
        }
        if (Input.GetMouseButtonUp(0) && SMGcurAmmo <= 0 && !isReloading) StartReload();
        if (Input.GetMouseButtonUp(0)) needToRelease = false;
        if (Input.GetMouseButton(0) && !isReloading) FireGun1();

        if (Input.GetMouseButtonDown(1)) StartAimingSniper();
        if (Input.GetMouseButtonUp(1)) FireSniper();

        if(isReloading){
            reloadTimer += Time.deltaTime;
            if(reloadTimer >= reloadDur) {
                reloadTimer = reloadDur;
                FinishReload();
            }
        }
    }

    public bool AttemptPerfectReload(){
        if(!isReloading) { print("not reloading"); return false; }
        if(GetReloadProgress() > perfectReloadStart && GetReloadProgress() < perfectReloadEnd){
            reloadTimer = reloadDur;
            print("perf reload successful");
            FinishReload();
            return true;
        } else {
            print("perf reload failed");
            return false;
        }
    }

    public float GetReloadProgress(){
        if(!isReloading) { return -1f; }
        
        return reloadTimer / reloadDur;
    }

    void FinishReload()
    {
        isReloading = false;
        SMGcurAmmo = SMGMagazineSize;
    }

    void StartReload()
    {
        if (SMGMagsLeft <= 0 || GetCurAmmo() == SMGMagazineSize) return;
        SMGMagsLeft -= 1;
        reloadTimer = 0f;
        isReloading = true;
    }

    void DoCooldowns()
    {
        SMGcooldown -= Time.deltaTime;
    }

    void StartAimingSniper()
    {
        aimingSniper = true;
    }

    void FireSniper()
    {
        if (!aimingSniper) return;
    }

    public int GetMagsLeft()
    {
        return SMGMagsLeft;
    }
    public int GetCurAmmo()
    {
        return SMGcurAmmo;
    }

    public int GetMagCapacity()
    {
        return SMGMagazineSize;
    }

    void FireGun1()
    {
        if (needToRelease || aimingSniper || SMGcooldown > 0 || SMGcurAmmo <= 0) return;

        pSound.SMGFire.Play();
        SMGcurAmmo -= 1;
        var aimAngle = anim.AimFrontArm();
        SMGcooldown = SMGResetTime;

        var newBullet = Instantiate(SMGBulletPrefab, transform.position, Quaternion.identity);
        newBullet.transform.eulerAngles = aimAngle;
        newBullet.GetComponent<Bullet>().damage = SMGDamage;
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * SMGBulletSpeed);
        newBullet.transform.SetParent(bulletParent.transform);
    }

    void Melee()
    {
        if (aimingSniper) return;

        if (pMove.isOnGround) Punch();
        else GroundSlam();
    }

    public bool IsPunching()
    {
        return punching;
    }

    void Punch()
    {
        if (punching) return;

        if (Input.GetMouseButton(0)) needToRelease = true;
        punching = true;
        anim.Punch();
        pMove.Step(transform.eulerAngles.y == 0 ? punchStepForce : -punchStepForce, stepTime);

        bool final = anim.GetPunchStep() == 4;
        punchHB.StartHitting(final ? punch3Damage : punchDamage, transform, final ? punch3KB : punchKBStrength, final ? punch3StunTime: punchStunTime, pSound.punchHit);
        if (final) pSound.strongPunch.Play();
        else pSound.weakPunch.Play();
    }

    void GroundSlam()
    {
        print("ground slam!");
    }
}
