using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator backArm, cockpit, frontArm;

    [Header("Punch")]
    [SerializeField] float punchComboBreakTime = 1;
    [SerializeField] GameObject dashFX;
    [SerializeField] Vector2 fXOffsetHorizontal, FXOffsetVertical, AimArmLimits = new Vector2();
    float punchComboCooldown;
    [Space()]
    [SerializeField] float walkThreshold, jumpThreshold = 0.1f;

    PlayerController pMove =>GetComponent<PlayerController>();
    Rigidbody2D rb => GetComponent<Rigidbody2D>();
    int punchStep;
    public bool slamming;

    public void LandSlam()
    {
        cockpit.SetTrigger("SLAM");
        slamming = true;
    }

    public int GetPunchStep()
    {
        return punchStep;
    }

    private void Update()
    {
        punchComboCooldown -= Time.deltaTime;
        Walk(Mathf.Abs(rb.velocity.x) > walkThreshold);
        Jump(Mathf.Abs(rb.velocity.y) > jumpThreshold || !pMove.isOnGround);
    }

    public Vector3 AimFrontArm()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
        if (mousePosition.x < transform.position.x) mousePosition.x = transform.position.x + Mathf.Abs(transform.position.x - mousePosition.x);
        Vector2 dir = mousePosition - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, dir);

        angle = Mathf.Clamp(angle, AimArmLimits.x, AimArmLimits.y);
        Vector3 eulerAngles = new Vector3(0, 0, angle);
        frontArm.transform.localEulerAngles = eulerAngles;

        if (transform.eulerAngles.y != 0) eulerAngles.y += 180;
        return eulerAngles;
    }

    public void SetDash()
    {
        cockpit.SetTrigger("DASH");
        Instantiate(dashFX, transform.position, transform.rotation);
    }

    public void ShowArms()
    {
        frontArm.gameObject.SetActive(true);
        backArm.gameObject.SetActive(true);
    }

    public void Punch()
    {
        if (punchComboCooldown <= 0 || punchStep > 3) punchStep = 1;

        frontArm.transform.localEulerAngles = Vector3.zero;
        frontArm.gameObject.SetActive(false);
        backArm.gameObject.SetActive(false);

        if (punchStep == 1) cockpit.SetTrigger("PUNCH1");
        if (punchStep == 2) cockpit.SetTrigger("PUNCH2");
        if (punchStep == 3) cockpit.SetTrigger("PUNCH3");

        punchComboCooldown = punchComboBreakTime;
        punchStep += 1;
    }

    void Jump(bool jumping)
    {
        cockpit.SetBool("JUMP", jumping);
    }

    void Walk(bool walking)
    {
        cockpit.SetBool("WALK", walking);
    }
}
