using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerCombat : MonoBehaviour
{
    [SerializeField] KeyCode melee = KeyCode.E;
    public bool TESTGROUNDED;

    [Header("SMG")]
    [SerializeField] float SMGResetTime;
    [SerializeField] float SMGReloadTime, SMGBulletSpeed;
    [SerializeField] int SMGMagazineSize;
    [SerializeField] GameObject SMGBulletPrefab;
    float SMGcooldown;

    bool aimingSniper;
    PlayerAnimator anim => GetComponent<PlayerAnimator>();

    private void Update()
    {
        DoCooldowns();

        if (Input.GetKeyDown(melee)) Melee();

        if (Input.GetMouseButton(0)) FireGun1();

        if (Input.GetMouseButtonDown(1)) StartAimingSniper();
        if (Input.GetMouseButtonUp(1)) FireSniper();
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

    void FireGun1()
    {
        if (aimingSniper || SMGcooldown > 0) return;

        SMGcooldown = SMGResetTime;

        var newBullet = Instantiate(SMGBulletPrefab, transform.position, Quaternion.identity);
        newBullet.transform.eulerAngles = FaceMouse(newBullet.transform);
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
        anim.Punch();
    }

    void GroundSlam()
    {
        print("ground slam!");
    }
}
