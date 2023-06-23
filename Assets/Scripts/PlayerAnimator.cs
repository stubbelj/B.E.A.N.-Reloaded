using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] Animator backArm, cockpit, frontArm;
    [SerializeField] float punchComboBreakTime = 1;
    [SerializeField] Vector2 AimArmLimits = new Vector2();
    float punchComboCooldown;

    public bool testWalking, punch;
    int punchStep;

    private void Update()
    {
        punchComboCooldown -= Time.deltaTime;

        Walk(testWalking);

        if (punch) {
            punch = false;
            Punch();
        }
    }

    public Vector3 AimFrontArm()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10);
        Vector2 dir = mousePosition - transform.position;
        float angle = Vector2.SignedAngle(Vector2.right, dir);

        angle = Mathf.Clamp(angle, AimArmLimits.x, AimArmLimits.y);
        Vector3 eulerAngles = new Vector3(0, 0, angle);
        frontArm.transform.eulerAngles = eulerAngles;

        return eulerAngles;
    }

    public void Punch()
    {
        if (punchComboCooldown <= 0 || punchStep > 4) punchStep = 1;

        frontArm.transform.eulerAngles = Vector3.zero;

        if (punchStep == 1) frontArm.SetTrigger("PUNCH1");
        if (punchStep == 2) backArm.SetTrigger("PUNCH2");
        if (punchStep == 3) frontArm.SetTrigger("PUNCH3");
        if (punchStep == 4) backArm.SetTrigger("PUNCH4");

        punchComboCooldown = punchComboBreakTime;
        punchStep += 1;
    }

    public void Walk(bool walking)
    {
        cockpit.SetBool("WALK", walking);
    }
}
