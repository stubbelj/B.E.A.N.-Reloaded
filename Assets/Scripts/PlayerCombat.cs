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
    [SerializeField] int SMGMagazineSize;
    int SMGmagLeft;
    [SerializeField] GameObject SMGBulletPrefab;
    float SMGcooldown;

    [Header("Punch")]
    [SerializeField] HitBox punchHB;
    [SerializeField] float punchDamage, punchKBStrength, punchStunTime, punch3Damage, punch3KB, punch3StunTime;
    [SerializeField] float punchStepForce = 50, stepTime = 0.1f;

    bool aimingSniper, needToRelease, punching;
    PlayerAnimator anim => GetComponent<PlayerAnimator>();
    PlayerController pMove => GetComponent<PlayerController>();

    

    public void Hit(float Damage)
    {
        if (pMove.isDashing) return;

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
        punching = false;
        punchHB.EndHitting();
    }

    private void Start()
    {
        health = maxHealth;
        Reload();
    }

    private void Update()
    {
        DoCooldowns();

        if (Input.GetKeyDown(melee) || Input.GetKeyDown(altMelee)) Melee();

        if (Input.GetKeyDown(reload)) Reload();
        if (Input.GetMouseButtonDown(0) && SMGmagLeft <= 0) Reload();
        if (Input.GetMouseButtonUp(0)) needToRelease = false;
        if (Input.GetMouseButton(0)) FireGun1();

        if (Input.GetMouseButtonDown(1)) StartAimingSniper();
        if (Input.GetMouseButtonUp(1)) FireSniper();
    }

    void Reload()
    {
        if (Input.GetMouseButton(0)) needToRelease = true;
        SMGmagLeft = SMGMagazineSize;
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

    public int GetMagLeft()
    {
        return SMGmagLeft;
    }

    public int GetMagCapacity()
    {
        return SMGMagazineSize;
    }

    void FireGun1()
    {
        if (needToRelease || aimingSniper || SMGcooldown > 0 || SMGmagLeft <= 0) return;

        SMGmagLeft -= 1;
        var aimAngle = anim.AimFrontArm();
        SMGcooldown = SMGResetTime;

        var newBullet = Instantiate(SMGBulletPrefab, transform.position, Quaternion.identity);
        newBullet.transform.eulerAngles = aimAngle;
        newBullet.GetComponent<Bullet>().damage = SMGDamage;
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * SMGBulletSpeed);
    }

    void Melee()
    {
        if (aimingSniper) return;

        if (pMove.isOnGround) Punch();
        else GroundSlam();
    }

    void Punch()
    {
        if (punching) return;

        if (Input.GetMouseButton(0)) needToRelease = true;
        punching = true;
        anim.Punch();
        pMove.Step(transform.eulerAngles.y == 0 ? punchStepForce : -punchStepForce, stepTime);

        punchHB.StartHitting(punchDamage, transform, punchKBStrength, punchStunTime);
    }

    void GroundSlam()
    {
        print("ground slam!");
    }
}
