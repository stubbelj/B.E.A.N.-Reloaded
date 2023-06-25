using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth;
    float health;
    [HideInInspector] public bool dead { get { return health <= 0; } }

    [Space()]
    [SerializeField] KeyCode reload = KeyCode.R;
    [SerializeField] float boostForceMult = 0.8f;

    [Header("SMG")]
    [SerializeField] float SMGResetTime;
    [SerializeField] float SMGReloadTime, SMGBulletSpeed, SMGDamage, SMGBoostForce = 320;
    [SerializeField] int SMGMagazineSize, SMGMagsLeft = 100;
    int SMGcurAmmo;
    [SerializeField] GameObject SMGBulletPrefab;
    float SMGcooldown, remainingBoostForce;

    [Header("Punch")]
    [SerializeField] HitBox punchHB;
    [SerializeField] float punchDamage, punchKBStrength, punchStunTime, punch3Damage, punch3KB, punch3StunTime;
    [SerializeField] float punchStepForce = 50, stepTime = 0.1f;

    [Header("Gound Slam")]
    [SerializeField] float groundSlamSpeed = 30;
    [SerializeField] HitBox slamHB;
    [SerializeField] float slamDamage, slamKB, slamStunTime;
    bool slamming;

    [Header("Reloading")]
    [SerializeField] float perfectReloadStart;
    [SerializeField] float perfectReloadEnd;
    [HideInInspector] public bool attempingPerfectReload;

    bool aimingSniper, needToRelease, punching;
    PlayerAnimator anim => GetComponent<PlayerAnimator>();
    PlayerController pMove => GetComponent<PlayerController>();
    PlayerSound pSound => GetComponent<PlayerSound>();
    Transform bulletParent => FindObjectOfType<GameManager>().transform;
    Transform bulletSpawnLoc => gameObject.transform.Find("frontArm").Find("bulletSpawnLocation");
    
    bool isReloading = false;
    float reloadDur = 1.3f, reloadTimer = 0f;
    

    public void AddAmmo(int magAmount, int bulletAmount)
    {
        SMGMagsLeft += magAmount;
        SMGcurAmmo = Mathf.Min(SMGMagazineSize, SMGcurAmmo + bulletAmount);

        if (magAmount > 0) pSound.ammoPickup.Play();
        else pSound.magRefillPickup.Play();
    }

    public void Hit(float Damage)
    {
        if (pMove.isDashing) return;

        CameraShake.i.Shake(0.3f, 0.2f);

        health -= Damage;
        if (health <= 0) Die();
    }
    
    public void EndAttack()
    {
        if (!slamHB.hitting) return;
        
        anim.slamming = false;
        slamHB.EndHitting();
    }

    void Die()
    {
    }

    public float GetHealthPercent()
    {
        return health / maxHealth;
    }

    public void EndPunch()
    {
        if (anim.GetPunchStep() <= 3) CameraShake.i.Shake(0.05f, 0.1f);

        punching = false;
        punchHB.EndHitting();
    }

    public void Punch3StartHit()
    {
        pSound.strongPunch.Play();
        punchHB.StartHitting();
    }

    private void Start()
    {
        health = maxHealth;
        StartReload();
    }

    private void Update()
    {
        DoCooldowns();
        anim.AimFrontArm();

        if (Input.GetMouseButtonDown(0) ) Melee();

        if (Input.GetKeyDown(reload)) ReloadInteract();
        if (Input.GetMouseButtonDown(1) && isReloading) AttemptPerfectReload();
        if (Input.GetMouseButtonUp(1) && SMGcurAmmo <= 0 && !isReloading) StartReload();

        if (Input.GetMouseButtonUp(1)) needToRelease = false;
        if (Input.GetMouseButton(1) && !isReloading) FireCurrentGun();

        if (pMove.isOnGround) {
            if (slamming) LandSlam();
            remainingBoostForce = SMGBoostForce;
        }

        if(isReloading){
            reloadTimer += Time.deltaTime;
            if(reloadTimer >= reloadDur) {
                reloadTimer = reloadDur;
                FinishReload();
            }
        }
    }

    void LandSlam()
    {
        slamming = false;
        slamHB.StartHitting(slamDamage, transform, slamKB, slamStunTime);

        anim.LandSlam();
        pSound.slamLand.Play();
        CameraShake.i.Shake(0.3f, 0.2f);
    }

    void ReloadInteract()
    {
        if (isReloading) AttemptPerfectReload();
        else StartReload();
    }

    public bool AttemptPerfectReload(){
        if(!isReloading) return false; 
        if(GetReloadProgress() > perfectReloadStart && GetReloadProgress() < perfectReloadEnd){
            reloadTimer = reloadDur;
            FinishReload();
            return true;
        } else {
            attempingPerfectReload = false;
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

        attempingPerfectReload = true;
        SMGMagsLeft -= 1;
        reloadTimer = 0f;
        isReloading = true;
    }

    void DoCooldowns()
    {
        SMGcooldown -= Time.deltaTime;
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

    void FireCurrentGun()
    {
        if (needToRelease || aimingSniper || SMGcooldown > 0 || SMGcurAmmo <= 0) return;

        pSound.SMGFire.Play();
        SMGcurAmmo -= 1;
        var aimAngle = anim.AimFrontArm();
        SMGcooldown = SMGResetTime;

        if (Mathf.Abs(aimAngle.z + 90) < 8f) Boost(); 

        var newBullet = Instantiate(SMGBulletPrefab, bulletSpawnLoc.position, Quaternion.identity);
        newBullet.transform.eulerAngles = aimAngle;
        newBullet.GetComponent<Bullet>().damage = SMGDamage;
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * SMGBulletSpeed);
        newBullet.transform.SetParent(bulletParent.transform);
    }

    void Boost()
    {
        pMove.BoostUp(remainingBoostForce);
        remainingBoostForce *= boostForceMult;
    }

    void Melee()
    {
        if (aimingSniper || anim.slamming) return;

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

        if (Input.GetMouseButton(1)) needToRelease = true;
        punching = true;
        anim.Punch();
        pMove.Step(transform.eulerAngles.y == 0 ? punchStepForce : -punchStepForce, stepTime);

        bool final = anim.GetPunchStep() == 4;
        if (final) punchHB.Setup(punch3Damage, transform, punch3KB, punch3StunTime, pSound.punchHit);
        else punchHB.StartHitting(punchDamage, transform, punchKBStrength, punchStunTime, pSound.punchHit);
        if (!final) pSound.weakPunch.Play();
        else pSound.punch3WindUp.Play();
    }

    void GroundSlam()
    {
        slamming = true;
        pMove.GroundSlam(groundSlamSpeed);
    }
}
