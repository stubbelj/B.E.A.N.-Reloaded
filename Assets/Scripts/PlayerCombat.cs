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
    bool invincible;

    [Space()]
    [SerializeField] KeyCode reload = KeyCode.R;
    [SerializeField] float boostForceMult = 0.8f;

    [SerializeField] List<Gun> allguns = new List<Gun>();
    [SerializeField] Gun currentGun;
    [SerializeField] Transform bulletFirePoint;

    [Header("Punch")]
    [SerializeField] HitBox punchHB;
    [SerializeField] float punchDamage, punchKBStrength, punchStunTime, punch3Damage, punch3KB, punch3StunTime;
    [SerializeField] float punchStepForce = 50, stepTime = 0.1f;
    bool punching;

    [Header("Gound Slam")]
    [SerializeField] float groundSlamSpeed = 30;
    [SerializeField] HitBox slamHB;
    [SerializeField] float slamDamage, slamKB, slamStunTime;
    bool slamming;

    [Header("Reloading")]
    [SerializeField] float perfectReloadStart;
    [SerializeField] float perfectReloadEnd;
    [HideInInspector] public bool attempingPerfectReload;
    bool isReloading = false, needToRelease;
    float reloadDur = 1.3f, reloadTimer = 0f;

    [Header("tutorialStrings")]
    [SerializeField] string normalPunchString = "punch";
    [SerializeField] string bigPunchString = "bigPunch", slamString = "groundSlam";

    [Space()]
    [SerializeField] GameObject deathPrefab;

    PlayerAnimator anim => GetComponent<PlayerAnimator>();
    PlayerController pMove => GetComponent<PlayerController>();
    PlayerSound pSound => GetComponent<PlayerSound>();
    Transform bulletParent => FindObjectOfType<GameManager>().transform;
    GameManager gameManager => GameObject.Find("gameManager").GetComponent<GameManager>();

    public Sound split;

    public void HealPercent(float percent)
    {
        health += maxHealth * (percent/100);
        health = Mathf.Min(health, maxHealth);
    }

    public void SetInvincible()
    {
        invincible = true;
    }

    public void SetVincible()
    {
        invincible = false;
    }

    public void FullHeal()
    {
        health = maxHealth;
    }
    public void IncreaseMaxHealth(float amount)
    {
        health += amount;
        maxHealth += amount;
    }

    public void AddAmmo(int magAmount, int bulletAmount)
    {
        currentGun.AddMags(magAmount);
        currentGun.AddAmmo(bulletAmount);

        if (magAmount > 0) pSound.ammoPickup.Play();
        else pSound.magRefillPickup.Play();
    }

    public Gun GetCurrentGun()
    {
        return currentGun;
    }
    public void Hit(float Damage)
    {
        if(isDead || invincible) return;
        if (pMove.isDashing) return;

        pSound.playerHurt.Play();

        CameraShake.i.Shake(0.3f, 0.2f);

        anim.OnPlayerHit();

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
        split.Play();
        
        pSound.playerDeath.Play();
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
        FinishReload();

        if (FindObjectOfType<LevelGenerator>() != null) {
            string gunName = PlayerPrefs.GetString("gun");
            foreach (var gun in allguns) if (string.Equals(gun.displayName, gunName)) currentGun = gun;
            //currentGun = allguns[Random.Range(0, allguns.Count)];
        }
        else PlayerPrefs.SetString("gun", currentGun.displayName);
        GetNewGun(currentGun);
        split = Instantiate(split);
    }

    public float GetCameraPullDistance()
    {
        return currentGun.GetCameraPullDistance();
    }

    private void Update()
    {
        if(gameManager.isPaused() || isDead){ return; }


        pSound.heartBeat.PercentVolume(1 - GetHealthPercent());

        DoCooldowns();
        anim.AimFrontArm();

        if (Input.GetMouseButtonDown(0) ) Melee();

        if (Input.GetKeyDown(reload)) ReloadInteract();
        if (Input.GetMouseButtonDown(1) && isReloading) AttemptPerfectReload();
        if (Input.GetMouseButtonUp(1) && currentGun.GetCurrentAmmo() <= 0 && !isReloading) StartReload();

        if (Input.GetMouseButtonUp(1)) needToRelease = false;

        if (!isReloading) {
            if (currentGun.IsAutomatic() && Input.GetMouseButton(1)) FireCurrentGun();
            if (!currentGun.IsAutomatic() && Input.GetMouseButtonDown(1)) FireCurrentGun();
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

    public void GetNewGun(Gun newGun, bool spawnOldGun = true)
    {
        if (spawnOldGun && currentGun != null && !string.Equals(newGun.displayName, currentGun.displayName)) GameManager.i.SpawnGunPickup(currentGun, transform.position);
        newGun.Init();
        currentGun = newGun;
        currentGun.OnBulletReady.AddListener(PlayBulletReadySound);
    }

    void PlayBulletReadySound()
    {
        pSound.bulletReady.Play();
    }

    void LandSlam()
    {
        slamming = false;
        slamHB.attackType = slamString;
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
            pSound.perfectReload.Play();
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
        pSound.reload.Play();
        isReloading = false;
        currentGun.Reload();
    }

    void StartReload()
    {
        if (currentGun.GetMagsLeft() <= 0 || currentGun.FullMag()) return;

        attempingPerfectReload = currentGun.CanPerfectReload();
        reloadTimer = 0f;
        isReloading = true;
    }

    void DoCooldowns()
    {
        bool ready = currentGun.cooldown <= 0;
        currentGun.cooldown -= Time.deltaTime;
        if (!ready && currentGun.cooldown <= 0 && currentGun.GetBulletSpacing() > 0.5f) PlayBulletReadySound();
    }
    public string GetGunName()
    {
        return currentGun.displayName;
    }

    public int GetMagsLeft()
    {
        return currentGun.GetMagsLeft() ;
    }
    public int GetCurAmmo()
    {
        return currentGun.GetCurrentAmmo();
    }

    public int GetMagCapacity()
    {
        return currentGun.GetMagSize();
    }

    void FireCurrentGun()
    {
        if (needToRelease || currentGun.cooldown > 0 || currentGun.GetCurrentAmmo() <= 0) return;
        
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
        if (anim.slamming) return;

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
        if (final) {
            punchHB.Setup(punch3Damage, transform, punch3KB, punch3StunTime, pSound.punchHit);
            pSound.punch3WindUp.Play();
            punchHB.attackType = bigPunchString;
        }
        else {
            punchHB.StartHitting(punchDamage, transform, punchKBStrength, punchStunTime, pSound.punchHit);
            pSound.weakPunch.Play();
            punchHB.attackType = normalPunchString;
        }
    }

    void GroundSlam()
    {
        slamming = true;
        pMove.GroundSlam(groundSlamSpeed);
    }
}
