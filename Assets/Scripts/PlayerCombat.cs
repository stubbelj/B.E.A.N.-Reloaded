using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] KeyCode melee = KeyCode.E, reload = KeyCode.R;
    public bool TESTGROUNDED;

    [Header("SMG")]
    [SerializeField] float SMGResetTime;
    [SerializeField] float SMGReloadTime, SMGBulletSpeed;
    [SerializeField] int SMGMagazineSize;
    int SMGmagLeft;
    [SerializeField] GameObject SMGBulletPrefab;
    float SMGcooldown;

    bool aimingSniper, needToRelease;
    PlayerAnimator anim => GetComponent<PlayerAnimator>();

    public void PunchImpact()
    {
        anim.SpawnPunchFX();
    }

    private void Start()
    {
        Reload();
    }

    private void Update()
    {
        DoCooldowns();

        if (Input.GetKeyDown(melee)) Melee();

        if (Input.GetKeyDown(reload)) Reload();
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
        newBullet.GetComponent<Rigidbody2D>().AddForce(newBullet.transform.right * SMGBulletSpeed);
    }

    Vector3 FaceMouse(Transform transform)
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward*10);
        Vector2 dir = mousePosition - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, dir);
        return new Vector3(0, 0, angle);
    }

    void Melee()
    {
        if (aimingSniper) return;

        if (TESTGROUNDED) Punch();
        else GroundSlam();
    }

    void Punch()
    {
        if (Input.GetMouseButton(0)) needToRelease = true;
        anim.Punch();
    }

    void GroundSlam()
    {
        print("ground slam!");
    }
}
