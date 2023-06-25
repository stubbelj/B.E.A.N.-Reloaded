using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float maxHealth;
    float health;
    [HideInInspector] public bool isDead { get { return health <= 0; } }

    [Space()]
    [SerializeField] KeyCode reload = KeyCode.R;
    [SerializeField] float boostForceMult = 0.8f;

    [SerializeField] Gun currentGun;
    [SerializeField] Transform bulletFirePoint;

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
    
    bool isReloading = false;
    float reloadDur = 1.3f, reloadTimer = 0f;
    
    GameManager gameManager => GameObject.Find("gameManager").GetComponent<GameManager>();

    [SerializeField] GameObject deathPrefab;

    public void AddAmmo(int magAmount, int bulletAmount)
    {
        currentGun.magsLeft += magAmount;
        currentGun.currentAmmo = Mathf.Min(currentGun.magazineSize, currentGun.currentAmmo + bulletAmount);

        if (magAmount > 0) pSound.ammoPickup.Play();
        else pSound.magRefillPickup.Play();
    }

    public void Hit(float Damage)
    {
        if(isDead) return;
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
        var corpse = Instantiate(deathPrefab, transform.position, Quaternion.identity);
        PlayerCorpse corpseScript = corpse.GetComponent<PlayerCorpse>();
        corpseScript.Init(gameObject.GetComponent<Rigidbody2D>().velocity);
        foreach(Transform child in transform){
            child.gameObject.SetActive(false);
        }
        gameObject.GetComponent<Rigidbody2D>().simulated = false;
        CameraController camera = FindObjectOfType<CameraController>();
        camera.player = corpse.transform.Find("Bean").transform;

        //Make the corpse face left or right appropriately
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
        float yAngle = mousePos.x < transform.position.x ? 180 : 0;
        corpse.transform.eulerAngles = new Vector3(transform.eulerAngles.x, yAngle, 0);

        gameManager.GameOver();
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
        GetNewGun(currentGun);
    }

    public float GetCameraPullDistance()
    {
        return currentGun.GetCameraPullDistance();
    }

    private void Update()
    {
        if(gameManager.isPaused() || isDead){ return; }
        DoCooldowns();
        anim.AimFrontArm();

        if (Input.GetMouseButtonDown(0) ) Melee();

        if (Input.GetKeyDown(reload)) ReloadInteract();
        if (Input.GetMouseButtonDown(1) && isReloading) AttemptPerfectReload();
        if (Input.GetMouseButtonUp(1) && currentGun.currentAmmo <= 0 && !isReloading) StartReload();

        if (Input.GetMouseButtonUp(1)) needToRelease = false;

        if (!isReloading) {
            if (currentGun.IsAutomatic() && Input.GetMouseButton(1)) FireCurrentGun();
            if (!currentGun.IsAutomatic() && Input.GetMouseButtonDown(1)) FireCurrentGun();
        }

        if (Input.GetKeyDown(KeyCode.Minus) && !isDead){
            health = 0f;
            Die();
        }

        if (pMove.isOnGround) {
            if (slamming) LandSlam();
            currentGun.remainingBoostForce = currentGun.boostForce;
        }

        if(isReloading){
            reloadTimer += Time.deltaTime;
            if(reloadTimer >= reloadDur) {
                reloadTimer = reloadDur;
                FinishReload();
            }
        }
    }

    public void GetNewGun(Gun newGun)
    {
        newGun.Init();
        currentGun = newGun;
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
        currentGun.currentAmmo = currentGun.magazineSize;
    }

    void StartReload()
    {
        if (currentGun.magsLeft <= 0 || GetCurAmmo() == currentGun.magazineSize) return;

        attempingPerfectReload = true;
        currentGun.magsLeft -= 1;
        reloadTimer = 0f;
        isReloading = true;
    }

    void DoCooldowns()
    {
        currentGun.cooldown -= Time.deltaTime;
    }

    public int GetMagsLeft()
    {
        return currentGun.magsLeft;
    }
    public int GetCurAmmo()
    {
        return currentGun.currentAmmo;
    }

    public int GetMagCapacity()
    {
        return currentGun.magazineSize;
    }

    void FireCurrentGun()
    {
        if (needToRelease || currentGun.cooldown > 0 || currentGun.currentAmmo <= 0) return;
        
        var aimAngle = anim.AimFrontArm();
        if (Mathf.Abs(aimAngle.z + 90) < 8f) Boost();
        currentGun.Shoot(bulletFirePoint.position, aimAngle, bulletParent);    
    }

    void Boost()
    {
        pMove.BoostUp(currentGun.remainingBoostForce);
        currentGun.remainingBoostForce *= boostForceMult;
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
